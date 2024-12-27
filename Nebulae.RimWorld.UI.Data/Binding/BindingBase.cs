using System;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示对象指定成员间的绑定关系的基类，定义了其共同行为
    /// </summary>
    /// <typeparam name="TSource">绑定源成员值的类型</typeparam>
    /// <typeparam name="TTarget">绑定目标成员值的类型</typeparam>
    public abstract class BindingBase<TSource, TTarget> : IBinding, IEquatable<BindingBase<TSource, TTarget>>
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly int _hashCode;
        private readonly Type _sourceType;
        private readonly PropertyMetadata _sourcePropertyData;
        private readonly string _sourcePath;
        private readonly Type _targetType;
        private readonly PropertyMetadata _targetPropertyData;
        private readonly string _targetPath;

        private bool _isBinding;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        /// <summary>
        /// 绑定源
        /// </summary>
        protected readonly WeakReference Source;

        /// <summary>
        /// 绑定源成员的信息
        /// </summary>
        protected readonly BindingMemberInfo<TSource> SourceMemberInfo;

        /// <summary>
        /// 绑定目标
        /// </summary>
        protected readonly WeakReference Target;

        /// <summary>
        /// 绑定目标成员的信息
        /// </summary>
        protected readonly BindingMemberInfo<TTarget> TargetMemberInfo;

        /// <summary>
        /// 绑定关系的类型
        /// </summary>
        protected readonly BindingMode Mode;

        #endregion


        //------------------------------------------------------
        //
        //  Public Property
        //
        //------------------------------------------------------

        #region Public Property

        /// <inheritdoc/>
        public bool IsBindingValid
        {
            get
            {
                if (!_isBinding) { return false; }
                if ((!SourceMemberInfo.IsStatic && !Source.IsAlive)
                    || (!TargetMemberInfo.IsStatic && !Target.IsAlive))
                {
                    Unbind();
                    return false;
                }
                return true;
            }
        }

        /// <inheritdoc/>
        public Type SourceType => _sourceType;

        /// <inheritdoc/>
        public Type TargetType => _targetType;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 为 <see cref="BindingBase{TSource, TTarget}"/> 派生类实现基本初始化
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="target">绑定目标</param>
        /// <param name="sourcePath">绑定源的成员</param>
        /// <param name="targetPath">绑定目标的成员</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        protected BindingBase(
            object source,
            object target,
            object sourcePath,
            object targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            _sourceType = typeof(TSource);
            _sourcePath = sourcePath.ToString();
            _targetType = typeof(TTarget);
            _targetPath = targetPath.ToString();
            _hashCode = _sourceType.GetHashCode() ^ _sourcePath.GetHashCode() ^ _targetType.GetHashCode() ^ _targetPath.GetHashCode();

            if (BindingManager.IsBinding(this))
            {
                throw new InvalidOperationException($"Binding from {_sourceType}.{sourcePath} to {_targetType}.{targetPath} has already exist.");
            }


            if (source is DependencyObject dependencySource && sourcePath is DependencyProperty sourceProperty)
            {
                sourceProperty.TryGetMetadata(_sourceType, out PropertyMetadata metadata);
                _sourcePropertyData = metadata;
                _sourcePropertyData.PropertyChanged += OnDependencySourceChanged;

                SourceMemberInfo = new BindingMemberInfo<TSource>(dependencySource, sourceProperty, _targetType);
            }
            else
            {
                SourceMemberInfo = new BindingMemberInfo<TSource>(_sourceType, _sourcePath, flags);
            }

            if (!SourceMemberInfo.IsReadable())
            {
                throw new InvalidOperationException($"Source property {_sourceType}.{sourcePath} must have get method.");
            }
            else if (mode is BindingMode.TwoWay && !SourceMemberInfo.IsWritable())
            {
                throw new InvalidOperationException($"Source property {_sourceType}.{sourcePath} must have set method for a TowWay binding.");
            }


            if (target is DependencyObject dependencyTarget && targetPath is DependencyProperty targetProperty)
            {
                targetProperty.TryGetMetadata(_targetType, out PropertyMetadata metadata);
                _targetPropertyData = metadata;
                _targetPropertyData.PropertyChanged += OnDependencyTargetChanged;

                TargetMemberInfo = new BindingMemberInfo<TTarget>(dependencyTarget, targetProperty, _targetType);
            }
            else
            {
                TargetMemberInfo = new BindingMemberInfo<TTarget>(_targetType, _targetPath, flags);
            }

            if (!TargetMemberInfo.IsWritable())
            {
                throw new InvalidOperationException($"Target property {_targetType}.{targetPath} must have set method.");
            }
            else if (mode is BindingMode.TwoWay && !TargetMemberInfo.IsReadable())
            {
                throw new InvalidOperationException($"Target property {_targetType}.{targetPath} must have get method for a TwoWay binding.");
            }

            if (source != null)
            {
                Source = new WeakReference(source);
            }
            if (target != null)

            {
                Target = new WeakReference(target);
            }

            Mode = mode;

            _isBinding = true;
        }

        #endregion


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BindingBase<TSource, TTarget> other
                && _sourceType == other._sourceType && _targetType == other._targetType
                && _sourcePath == other._sourcePath && _targetPath == other._targetPath;
        }

        /// <inheritdoc/>
        public bool Equals(BindingBase<TSource, TTarget> other)
        {
            return _sourceType == other._sourceType && _targetType == other._targetType
                && _sourcePath == other._sourcePath && _targetPath == other._targetPath;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => _hashCode;

        /// <inheritdoc/>
        public void Unbind()
        {
            _isBinding = false;

            if (_sourcePropertyData != null)
            {
                _sourcePropertyData.PropertyChanged -= OnDependencySourceChanged;
            }
            if (_targetPropertyData != null)
            {
                _targetPropertyData.PropertyChanged -= OnDependencyTargetChanged;
            }
        }

        /// <inheritdoc/>
        public abstract void Synchronize();


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
    }
}
