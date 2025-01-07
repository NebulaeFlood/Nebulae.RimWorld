using System;
using System.ComponentModel;
using System.Reflection;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Networking.Types;
using System.Collections.Generic;
using Verse;
using Nebulae.RimWorld.UI.Data.Binding.Converters;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示对象指定成员间的绑定关系的基类，定义了其共同行为
    /// </summary>
    public abstract class BindingBase : IEquatable<BindingBase>
    {
        private static readonly Dictionary<ConverterKey, IValueConverter> _createdConverters = new Dictionary<ConverterKey, IValueConverter>();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly int _hashCode;

        private readonly string _sourcePath;
        private readonly Type _sourceType;

        private readonly string _targetPath;
        private readonly Type _targetType;

        private readonly PropertyMetadata _sourcePropertyData;
        private readonly PropertyMetadata _targetPropertyData;

        private bool _isBinding;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        /// <summary>
        /// 成员间的转换器
        /// </summary>
        protected readonly IValueConverter Converter;

        /// <summary>
        /// 绑定关系的类型
        /// </summary>
        protected readonly BindingMode Mode;

        /// <summary>
        /// 成员间是否需要使用 <see cref="Converter"/>
        /// </summary>
        /// <remarks>若为 <see langword="false"/>，<see cref="Converter"/> = <see langword="null"/>。</remarks>
        protected readonly bool ShouldConvert;

        /// <summary>
        /// 绑定源成员的信息
        /// </summary>
        protected readonly BindingMember SourceMember;

        /// <summary>
        /// 绑定目标成员的信息
        /// </summary>
        protected readonly BindingMember TargetMember;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 绑定关系是否存在
        /// </summary>
        public bool IsBinding => _isBinding;

        /// <summary>
        /// 绑定对象是否支持绑定
        /// </summary>
        public bool IsBindingValid => SourceMember.IsAlive && TargetMember.IsAlive;

        #endregion


        /// <summary>
        /// 为 <see cref="BindingBase"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员</param>
        /// <param name="converter">成员间的值转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <exception cref="InvalidOperationException">当绑定的成员不满足绑定关系的条件时发生。</exception>
        protected BindingBase(
            object source,
            object sourcePath,
            object target,
            object targetPath,
            IValueConverter converter,
            BindingMode mode)
        {
            if (sourcePath is MemberInfo sourceMember)
            {
                _sourcePath = sourceMember.Name;
                _sourceType = sourceMember.DeclaringType;

                SourceMember = new BindingMember(source, sourceMember);
            }
            else
            {
                DependencyProperty sourceProperty = (DependencyProperty)sourcePath;

                _sourcePath = sourceProperty.Name;
                _sourcePropertyData = sourceProperty.GetMetadata((DependencyObject)source);
                _sourceType = sourceProperty.OwnerType;

                SourceMember = new BindingMember(source, sourceProperty);
            }

            _hashCode = SourceMember.GetHashCode() ^ TargetMember.GetHashCode();

            if (BindingManager.IsBinding(this))
            {
                throw new InvalidOperationException($"Binding from {_sourceType}.{_sourcePath} to {_targetType}.{_targetPath} has already exist.");
            }

            if (!SourceMember.IsReadable)
            {
                throw new InvalidOperationException($"Source member {_sourceType}.{_sourcePath} must be readable.");
            }
            else if (mode is BindingMode.TwoWay && !SourceMember.IsWritable)
            {
                throw new InvalidOperationException($"Source member {_sourceType}.{_sourcePath} must be writable for a TowWay binding.");
            }

            if (targetPath is MemberInfo targetMember)
            {
                _targetPath = targetMember.Name;
                _targetType = targetMember.DeclaringType;

                TargetMember = new BindingMember(target, targetMember);
            }
            else
            {
                DependencyProperty targetProperty = (DependencyProperty)targetPath;

                _targetPath = targetProperty.Name;
                _targetPropertyData = targetProperty.GetMetadata((DependencyObject)target);
                _targetType = targetProperty.OwnerType;

                TargetMember = new BindingMember(target, targetProperty);
            }

            if (!TargetMember.IsWritable)
            {
                throw new InvalidOperationException($"Target member {_targetType}.{_targetPath} must be writable.");
            }
            else if (mode is BindingMode.TwoWay && !TargetMember.IsReadable)
            {
                throw new InvalidOperationException($"Target member {_targetType}.{_targetPath} must be readable for a TowWay binding.");
            }

            Mode = mode;
            Converter = converter
                ?? CreateDefaultConverter(SourceMember.MemberType, TargetMember.MemberType);
            ShouldConvert = !(Converter is null);

            StartBinding(source, sourcePath, target, targetPath);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is BindingBase other
                    && _isBinding == other._isBinding
                    && SourceMember.Equals(other.SourceMember)
                    && TargetMember.Equals(other.TargetMember));
        }

        /// <inheritdoc/>
        public bool Equals(BindingBase other)
        {
            return ReferenceEquals(this, other)
                || (_isBinding == other._isBinding
                    && SourceMember.Equals(other.SourceMember)
                    && TargetMember.Equals(other.TargetMember));
        }

        /// <inheritdoc/>
        public override int GetHashCode() => _hashCode;

        /// <summary>
        /// 取消对象成员间的绑定关系
        /// </summary>
        public void Unbind()
        {
            if (!_isBinding)
            {
                return;
            }

            if (_sourcePropertyData != null)
            {
                _sourcePropertyData.PropertyChanged -= OnDependencySourceChanged;
            }

            if (_targetPropertyData != null)
            {
                _targetPropertyData.PropertyChanged -= OnDependencyTargetChanged;
            }

            if (Mode is BindingMode.TwoWay)
            {
                if (SourceMember.AssociatedObject is INotifyPropertyChanged notifiableSource)
                {
                    notifiableSource.PropertyChanged -= OnNotifiableSourceChanged;
                }

                if (TargetMember.AssociatedObject is INotifyPropertyChanged notifiableTarget)
                {
                    notifiableTarget.PropertyChanged -= OnNotifiableTargetChanged;
                }
            }

            _isBinding = false;
        }

        /// <summary>
        /// 强制以当前绑定模式同步绑定对象的值
        /// </summary>
        public abstract void Synchronize();

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 当 <see cref="DependencyObject"/> 绑定源成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定源</param>
        /// <param name="e">变化信息</param>
        protected abstract void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="DependencyObject"/> 绑定目标成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定目标</param>
        /// <param name="e">变化信息</param>
        protected abstract void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="INotifyPropertyChanged"/> 绑定源成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定源</param>
        /// <param name="e">变化信息</param>
        protected abstract void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="INotifyPropertyChanged"/> 绑定目标成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定目标</param>
        /// <param name="e">变化信息</param>
        protected abstract void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e);

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void StartBinding(
            object source,
            object sourcePath,
            object target,
            object targetPath)
        {
            if (source is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged += OnNotifiableSourceChanged;
            }
            else if (sourcePath is DependencyProperty)
            {
                _sourcePropertyData.PropertyChanged += OnDependencySourceChanged;
            }

            if (Mode is BindingMode.TwoWay)
            {
                if (target is INotifyPropertyChanged notifiableTarget)
                {
                    notifiableTarget.PropertyChanged += OnNotifiableTargetChanged;
                }
                else if (targetPath is DependencyProperty)
                {
                    _targetPropertyData.PropertyChanged += OnDependencyTargetChanged;
                }
            }

            _isBinding = true;
        }

        private static IValueConverter CreateDefaultConverter(Type sourceType, Type targetType)
        {
            if (sourceType == targetType)
            {
                return null;
            }

            ConverterKey key = new ConverterKey(sourceType, targetType);
            if (!_createdConverters.TryGetValue(key, out IValueConverter converter))
            {
                if (SystemConvertUtility.CanConvert(sourceType, targetType))
                {
                    converter = new SystemConverter(sourceType, targetType);
                }
                else
                {
                    throw new InvalidCastException($"Default binding converter can not cast type {sourceType} to type {targetType}.");
                }

                _createdConverters.Add(key, converter);
            }

            return converter;
        }

        #endregion


        private readonly struct ConverterKey : IEquatable<ConverterKey>
        {
            private readonly int _hashCode;
            private readonly Type _sourceType;
            private readonly Type _targetType;

            internal ConverterKey(Type sourceType, Type targetType)
            {
                _hashCode = sourceType.GetHashCode() ^ targetType.GetHashCode();
                _sourceType = sourceType;
                _targetType = targetType;
            }

            public override bool Equals(object obj)
            {
                return obj is ConverterKey other
                    && _sourceType == other._sourceType
                    && _targetType == other._targetType;
            }

            public bool Equals(ConverterKey other)
            {
                return _sourceType == other._sourceType
                    && _targetType == other._targetType;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}
