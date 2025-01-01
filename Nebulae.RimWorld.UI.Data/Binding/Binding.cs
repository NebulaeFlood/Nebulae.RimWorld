using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示对象指定成员间的绑定关系
    /// </summary>
    /// <typeparam name="T">被绑定成员的类型</typeparam>
    public class Binding<T> : BindingBase<T, T>
    {
        private T _cachedSourceValue;
        private T _cachedTargetValue;
        private readonly EqualityComparer<T> _comparer;


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="Binding{T}"/> 的新实例
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="target">绑定目标</param>
        /// <param name="sourcePath">绑定源的成员</param>
        /// <param name="targetPath">绑定目标的成员</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        internal Binding(
            object source,
            object target,
            object sourcePath,
            object targetPath,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            : base(source, target, sourcePath, targetPath, mode, flags)
        {
            _cachedSourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(source);
            _cachedTargetValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(target);
            _comparer = EqualityComparer<T>.Default;
        }

        #endregion


        /// <inheritdoc/>
        public override void Synchronize()
        {
            if (!IsBindingValid) { return; }

            T sourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(Source.Target);
            T targetValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(Target.Target);

            if (!_comparer.Equals(sourceValue, _cachedSourceValue))
            {
                _cachedSourceValue = sourceValue;
                _cachedTargetValue = sourceValue;

                if (TargetMemberInfo.IsStatic)
                {
                    TargetMemberInfo.StaticMemberSetter.Invoke(sourceValue);
                }
                else
                {
                    TargetMemberInfo.MemberSetter.Invoke(Target.Target, sourceValue);
                }
            }
            else if (Mode is BindingMode.TwoWay && !_comparer.Equals(targetValue, _cachedTargetValue))
            {
                _cachedSourceValue = targetValue;
                _cachedTargetValue = targetValue;

                if (SourceMemberInfo.IsStatic)
                {
                    SourceMemberInfo.StaticMemberSetter.Invoke(targetValue);
                }
                else
                {
                    SourceMemberInfo.MemberSetter.Invoke(Source.Target, targetValue);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsBindingValid) { return; }

            _cachedSourceValue = (T)e.NewValue;
            if (_comparer.Equals(_cachedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = _cachedSourceValue;
            if (TargetMemberInfo.IsStatic)
            {
                TargetMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                TargetMemberInfo.MemberSetter.Invoke(Target.Target, _cachedSourceValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsBindingValid) { return; }

            _cachedSourceValue = (T)e.NewValue;
            if (_comparer.Equals(_cachedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = _cachedSourceValue;
            if (SourceMemberInfo.IsStatic)
            {
                SourceMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                SourceMemberInfo.MemberSetter.Invoke(Source.Target, _cachedSourceValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBindingValid) { return; }

            _cachedSourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(sender);

            if (_comparer.Equals(_cachedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = _cachedSourceValue;
            if (TargetMemberInfo.IsStatic)
            {
                TargetMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                TargetMemberInfo.MemberSetter.Invoke(Target.Target, _cachedSourceValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBindingValid || Mode is BindingMode.OneWay) { return; }

            _cachedSourceValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(sender);
            if (_comparer.Equals(_cachedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = _cachedSourceValue;
            if (SourceMemberInfo.IsStatic)
            {
                SourceMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                SourceMemberInfo.MemberSetter.Invoke(Source.Target, _cachedSourceValue);
            }
        }
    }

    /// <summary>
    /// 表示对象指定成员间的绑定关系
    /// </summary>
    /// <typeparam name="TSource">绑定源成员值的类型</typeparam>
    /// <typeparam name="TTarget">绑定目标成员值的类型</typeparam>
    public class Binding<TSource, TTarget> : BindingBase<TSource, TTarget>
    {
        private TSource _cachedSourceValue;
        private TTarget _cachedTargetValue;
        private readonly EqualityComparer<TSource> _sourceComparer;
        private readonly EqualityComparer<TTarget> _targetComparer;
        private readonly IValueConverter<TSource, TTarget> _valueConverter;


        //------------------------------------------------------
        //
        //  Constructor
        //
        //------------------------------------------------------

        #region Constructor

        /// <summary>
        /// 初始化 <see cref="Binding{TSource, TTarget}"/> 的新实例
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="target">绑定目标</param>
        /// <param name="sourcePath">绑定源的成员</param>
        /// <param name="targetPath">绑定目标的成员</param>
        /// <param name="valueConverter">綁定源和目标的成员值的转换器</param>
        /// <param name="mode">绑定关系的类型</param>
        /// <param name="flags">搜索成员的方式</param>
        internal Binding(
            object source,
            object target,
            object sourcePath,
            object targetPath,
            IValueConverter<TSource, TTarget> valueConverter,
            BindingMode mode,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            : base(source, target, sourcePath, targetPath, mode, flags)
        {
            _cachedSourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(source);
            _cachedTargetValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(target);
            _sourceComparer = EqualityComparer<TSource>.Default;
            _targetComparer = EqualityComparer<TTarget>.Default;
            _valueConverter = valueConverter;
        }

        #endregion


        /// <inheritdoc/>
        public override void Synchronize()
        {
            if (!IsBindingValid) { return; }

            TSource sourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(Source.Target);
            TTarget targetValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(Target.Target);

            if (!_sourceComparer.Equals(sourceValue, _cachedSourceValue))
            {
                _cachedSourceValue = sourceValue;
                _cachedTargetValue = _valueConverter.Convert(sourceValue);

                if (TargetMemberInfo.IsStatic)
                {
                    TargetMemberInfo.StaticMemberSetter.Invoke(_cachedTargetValue);
                }
                else
                {
                    TargetMemberInfo.MemberSetter.Invoke(Target.Target, _cachedTargetValue);
                }
            }
            else if (Mode is BindingMode.TwoWay && !_targetComparer.Equals(targetValue, _cachedTargetValue))
            {
                _cachedSourceValue = _valueConverter.ConvertBack(targetValue);
                _cachedTargetValue = targetValue;

                if (SourceMemberInfo.IsStatic)
                {
                    SourceMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
                }
                else
                {
                    SourceMemberInfo.MemberSetter.Invoke(Source.Target, _cachedSourceValue);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsBindingValid) { return; }

            _cachedSourceValue = (TSource)e.NewValue;
            TTarget convertedSourceValue = _valueConverter.Convert(_cachedSourceValue);
            if (_targetComparer.Equals(convertedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = convertedSourceValue;
            if (TargetMemberInfo.IsStatic)
            {
                TargetMemberInfo.StaticMemberSetter.Invoke(_cachedTargetValue);
            }
            else
            {
                TargetMemberInfo.MemberSetter.Invoke(Target.Target, _cachedTargetValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsBindingValid || Mode is BindingMode.OneWay) { return; }

            _cachedTargetValue = (TTarget)e.NewValue;
            TSource convertedTargetValue = _valueConverter.ConvertBack(_cachedTargetValue);
            if (_sourceComparer.Equals(convertedTargetValue, _cachedSourceValue))
            {
                return;
            }

            _cachedSourceValue = _valueConverter.ConvertBack(_cachedTargetValue);
            if (SourceMemberInfo.IsStatic)
            {
                SourceMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                SourceMemberInfo.MemberSetter.Invoke(Source.Target, _cachedSourceValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBindingValid || e.PropertyName != SourceMemberInfo.MemberName) { return; }

            _cachedSourceValue = SourceMemberInfo.IsStatic
                ? SourceMemberInfo.StaticMemberGetter.Invoke()
                : SourceMemberInfo.MemberGetter.Invoke(sender);

            TTarget convertedSourceValue = _valueConverter.Convert(_cachedSourceValue);
            if (_targetComparer.Equals(convertedSourceValue, _cachedTargetValue))
            {
                return;
            }

            _cachedTargetValue = convertedSourceValue;
            if (TargetMemberInfo.IsStatic)
            {
                TargetMemberInfo.StaticMemberSetter.Invoke(_cachedTargetValue);
            }
            else
            {
                TargetMemberInfo.MemberSetter.Invoke(Target.Target, _cachedTargetValue);
            }
        }

        /// <inheritdoc/>
        protected override void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsBindingValid || Mode is BindingMode.OneWay || e.PropertyName != SourceMemberInfo.MemberName) { return; }

            _cachedTargetValue = TargetMemberInfo.IsStatic
                ? TargetMemberInfo.StaticMemberGetter.Invoke()
                : TargetMemberInfo.MemberGetter.Invoke(sender);

            TSource convertedTargetValue = _valueConverter.ConvertBack(_cachedTargetValue);
            if (_sourceComparer.Equals(convertedTargetValue, _cachedSourceValue))
            {
                return;
            }

            _cachedSourceValue = _valueConverter.ConvertBack(_cachedTargetValue);
            if (SourceMemberInfo.IsStatic)
            {
                SourceMemberInfo.StaticMemberSetter.Invoke(_cachedSourceValue);
            }
            else
            {
                SourceMemberInfo.MemberSetter.Invoke(Source.Target, _cachedSourceValue);
            }
        }
    }
}
