using Nebulae.RimWorld.UI.Data.Binding.Converters;
using Nebulae.RimWorld.UI.Data.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示对象指定成员间的绑定关系的基类，定义了其共同行为
    /// </summary>
    public abstract class BindingBase : IEquatable<BindingBase>
    {
        private static readonly Dictionary<ConverterKey, IValueConverter> _createdConverters =
            new Dictionary<ConverterKey, IValueConverter>();


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

        private bool _isBinding;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        /// <summary>
        /// 绑定成员间的值转换器
        /// </summary>
        protected readonly IValueConverter Converter;

        /// <summary>
        /// 绑定关系的类型
        /// </summary>
        protected readonly BindingMode Mode;

        /// <summary>
        /// 绑定成员间是否需要使用 <see cref="Converter"/>
        /// </summary>
        /// <remarks>若为 <see langword="false"/>，<see cref="Converter"/> = <see langword="null"/>。</remarks>
        protected readonly bool ShouldConvert;

        /// <summary>
        /// 绑定源成员的信息
        /// </summary>
        protected internal readonly BindingMember SourceMember;

        /// <summary>
        /// 绑定目标成员的信息
        /// </summary>
        protected internal readonly BindingMember TargetMember;

        #endregion


        /// <summary>
        /// 绑定关系是否可用
        /// </summary>
        public bool IsBinding
        {
            get => _isBinding;
            internal set => _isBinding = value;
        }


#if DEBUG
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        ~BindingBase()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A {Mode} Binding from [{SourceMember.GetHashCode()}]{_sourceType}.{_sourcePath} to [{TargetMember.GetHashCode()}]{_targetType}.{_targetPath} has been collected.");
        }
#endif


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
            #region Source Member

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
                _sourceType = sourceProperty.OwnerType;

                SourceMember = new BindingMember(source, sourceProperty);
            }

            #endregion

            #region Validate Source Member

            if (!SourceMember.IsReadable)
            {
                throw new InvalidOperationException($"Source member {_sourceType}.{_sourcePath} must be readable.");
            }
            else if (mode is BindingMode.TwoWay && !SourceMember.IsWritable)
            {
                throw new InvalidOperationException($"Source member {_sourceType}.{_sourcePath} must be writable for a TowWay binding.");
            }

            #endregion

            #region Target Member

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
                _targetType = targetProperty.OwnerType;

                TargetMember = new BindingMember(target, targetProperty);
            }

            #endregion

            #region Validate Target Member

            if (!TargetMember.IsWritable)
            {
                throw new InvalidOperationException($"Subscriber member {_targetType}.{_targetPath} must be writable.");
            }
            else if (mode is BindingMode.TwoWay && !TargetMember.IsReadable)
            {
                throw new InvalidOperationException($"Subscriber member {_targetType}.{_targetPath} must be readable for a TowWay binding.");
            }

            #endregion

            #region Check Repetition

            _hashCode = SourceMember.GetHashCode() ^ TargetMember.GetHashCode();
            _isBinding = true;

            if (!BindingManager.GlobalBindings.Add(this))
            {
                throw new InvalidOperationException($"Binding from {_sourceType}.{_sourcePath} to {_targetType}.{_targetPath} has already exist.");
            }

            #endregion

            Mode = mode;
            Converter = converter
                ?? CreateDefaultConverter(SourceMember.MemberType, TargetMember.MemberType);
            ShouldConvert = !(Converter is null);

            StartBinding(source, target);
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

            if (SourceMember.BindingTarget is DependencyObject dependencySource)
            {
                dependencySource.DependencyPropertyChanged -= OnDependencySourceChanged;
            }
            else if (SourceMember.BindingTarget is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged -= OnNotifiableSourceChanged;
            }

            if (Mode is BindingMode.TwoWay)
            {
                if (TargetMember.BindingTarget is DependencyObject dependencyTarget)
                {
                    dependencyTarget.DependencyPropertyChanged -= OnDependencyTargetChanged;
                }
                else if (TargetMember.BindingTarget is INotifyPropertyChanged notifiableTarget)
                {
                    notifiableTarget.PropertyChanged -= OnNotifiableTargetChanged;
                }
            }

            _isBinding = false;

            SourceMember.Invalid();
            TargetMember.Invalid();
            BindingManager.GlobalBindings.Remove(this);
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
        /// <param name="e">事件数据</param>
        protected abstract void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="DependencyObject"/> 绑定目标成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定目标</param>
        /// <param name="e">事件数据</param>
        protected abstract void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="INotifyPropertyChanged"/> 绑定源成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定源</param>
        /// <param name="e">事件数据</param>
        protected abstract void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e);

        /// <summary>
        /// 当 <see cref="INotifyPropertyChanged"/> 绑定目标成员变化执行的操作
        /// </summary>
        /// <param name="sender">绑定目标</param>
        /// <param name="e">事件数据</param>
        protected abstract void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e);

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

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
                    throw new InvalidCastException($"Default binding converter can not cast type: {sourceType} to type: {targetType}.");
                }

                _createdConverters.Add(key, converter);
            }

            return converter;
        }

        private void StartBinding(object source, object target)
        {
            if (source is DependencyObject dependencySource)
            {
                dependencySource.DependencyPropertyChanged += OnDependencySourceChanged;
            }
            else if (source is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged += OnNotifiableSourceChanged;
            }

            if (Mode is BindingMode.TwoWay)
            {
                if (target is DependencyObject dependencyTarget)
                {
                    dependencyTarget.DependencyPropertyChanged += OnDependencyTargetChanged;
                }
                else if (target is INotifyPropertyChanged notifiableTarget)
                {
                    notifiableTarget.PropertyChanged += OnNotifiableTargetChanged;
                }
            }
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
