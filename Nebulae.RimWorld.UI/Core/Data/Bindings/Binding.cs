using Nebulae.RimWorld.UI.Core.Data.Bindings.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Nebulae.RimWorld.UI.Core.Data.Bindings
{
    /// <summary>
    /// 标准的绑定关系
    /// </summary>
    /// <remarks>自动更新仅对依赖属性和能够触发 <see cref="INotifyPropertyChanged.PropertyChanged"/> 事件的属性生效，拥有对绑定源和目标的强引用，需要被手动取消。</remarks>
    public sealed class Binding : BindingBase, IEquatable<Binding>
    {
        /// <summary>
        /// 默认的成员搜索方式
        /// </summary>
        public const BindingFlags DefaultFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


#if DEBUG
        /// <summary>
        /// <see cref="Binding"/> 的析构函数
        /// </summary>
        ~Binding()
        {
            System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A {Mode} binding of type:Nebulae.RimWorld.UI.Core.Data.Bindings.Binding from <{_source.GetHashCode()}>{_source} to <{_target.GetHashCode()}>{_target} has been collected.");
        }
#endif

        private Binding(BindingMember source, BindingMember target, BindingMode mode, IValueConverter converter)
            : base(mode, converter ?? GetDefaultConverter(source.Type, target.Type))
        {
            _source = source;
            _target = target;

            if (!Bindings.Add(this))
            {
                throw new InvalidOperationException($"Binding type:{typeof(Binding)} already exists between {source} and {target}.");
            }

            _sourceValueCache = source.Value;
            _targetValueCache = target.Value;
        }


        //------------------------------------------------------
        //
        //  Public Static Methods
        //
        //------------------------------------------------------

        #region Public Static Methods

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员名称</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员名称</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(object source, string sourcePath, object target, string targetPath, BindingMode mode, BindingFlags flags = DefaultFlags)
        {
            return Create(source, sourcePath, target, targetPath, mode, null, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员名称</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员名称</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="converter">绑定成员间的值转换器</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(object source, string sourcePath, object target, string targetPath, BindingMode mode, IValueConverter converter, BindingFlags flags = DefaultFlags)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var sourceMember = new BindingMember(source, FindMember(source, sourcePath, flags));
            var targetMember = new BindingMember(target, FindMember(target, targetPath, flags));

            return StartBinding(new Binding(sourceMember, targetMember, mode, converter),
                source, target, mode);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员名称</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员名称</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(object source, string sourcePath, DependencyObject target, DependencyProperty targetPath, BindingMode mode, BindingFlags flags = DefaultFlags)
        {
            return Create(source, sourcePath, target, targetPath, mode, null, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的成员名称</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的依赖属性标识</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="converter">绑定成员间的值转换器</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(object source, string sourcePath, DependencyObject target, DependencyProperty targetPath, BindingMode mode, IValueConverter converter, BindingFlags flags = DefaultFlags)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (targetPath is null)
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var sourceMember = new BindingMember(source, FindMember(source, sourcePath, flags));
            var targetMember = new BindingMember(target, targetPath);

            return StartBinding(new Binding(sourceMember, targetMember, mode, converter),
                source, target, mode);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的依赖属性标识</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(DependencyObject source, DependencyProperty sourcePath, object target, string targetPath, BindingMode mode, BindingFlags flags = DefaultFlags)
        {
            return Create(source, sourcePath, target, targetPath, mode, null, flags);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的依赖属性标识</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="converter">绑定成员间的值转换器</param>
        /// <param name="flags">搜索成员的方式</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(DependencyObject source, DependencyProperty sourcePath, object target, string targetPath, BindingMode mode, IValueConverter converter, BindingFlags flags = DefaultFlags)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourcePath is null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var sourceMember = new BindingMember(source, sourcePath);
            var targetMember = new BindingMember(target, FindMember(target, targetPath, flags));

            return StartBinding(new Binding(sourceMember, targetMember, mode, converter),
                source, target, mode);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的依赖属性标识</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的依赖属性标识</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(DependencyObject source, DependencyProperty sourcePath, DependencyObject target, DependencyProperty targetPath, BindingMode mode)
        {
            return Create(source, sourcePath, target, targetPath, mode, null);
        }

        /// <summary>
        /// 创建一个绑定关系
        /// </summary>
        /// <param name="source">绑定源</param>
        /// <param name="sourcePath">绑定源的依赖属性标识</param>
        /// <param name="target">绑定目标</param>
        /// <param name="targetPath">绑定目标的成员路径</param>
        /// <param name="mode">绑定成员之间的关系</param>
        /// <param name="converter">绑定目标的依赖属性标识</param>
        /// <returns>创建的 <see cref="Binding"/> 实例。</returns>
        /// <remarks>
        /// 对于静态成员，对应的 <paramref name="source"/> 或 <paramref name="target"/> 传入声明该成员的 <see cref="Type"/>。
        /// </remarks>
        public static Binding Create(DependencyObject source, DependencyProperty sourcePath, DependencyObject target, DependencyProperty targetPath, BindingMode mode, IValueConverter converter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourcePath is null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (targetPath is null)
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var sourceMember = new BindingMember(source, sourcePath);
            var targetMember = new BindingMember(target, targetPath);

            return StartBinding(new Binding(sourceMember, targetMember, mode, converter),
                source, target, mode);
        }

        /// <summary>
        /// 获取与指定对象相关联的所有 <see cref="Binding"/> 的绑定关系
        /// </summary>
        /// <param name="obj">要获取绑定关系的对象</param>
        /// <returns>与指定对象相关联的所有绑定关系。</returns>
        public static IEnumerable<Binding> GetAll(object obj)
        {
            if (obj is null)
            {
                return Enumerable.Empty<Binding>();
            }

            bool IsRelated(Binding binding)
            {
                return ReferenceEquals(binding._source.Target, obj)
                    || ReferenceEquals(binding._target.Target, obj);
            }

            return Bindings.Where(IsRelated);
        }

        /// <summary>
        /// 解除与指定的对象相关联的 <see cref="Binding"/> 的绑定关系
        /// </summary>
        /// <param name="obj">要解除绑定关系的对象</param>
        public static void Unbind(object obj)
        {
            if (obj is null)
            {
                return;
            }

            bool UnbindIfRelated(Binding binding)
            {
                if (ReferenceEquals(binding._source.Target, obj)
                    || ReferenceEquals(binding._target.Target, obj))
                {
                    binding.Unbind();

                    return true;
                }

                return false;
            }

            _globalUnbingind = true;

            Bindings.RemoveWhere(UnbindIfRelated);

            _globalUnbingind = false;

            Bindings.TrimExcess();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 判断当前 <see cref="Binding"/> 是否与指定的对象相等
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>如果相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public override bool Equals(object obj)
        {
            if (obj is Binding other)
            {
                return _source.Equals(other._source)
                    && _target.Equals(other._target);
            }

            return false;
        }

        /// <summary>
        /// 判断当前 <see cref="Binding"/> 是否与指定 <see cref="Binding"/> 等效
        /// </summary>
        /// <param name="other">要判断的 <see cref="Binding"/></param>
        /// <returns>若二者相等，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Equals(Binding other)
        {
            return _source.Equals(other._source)
                && _target.Equals(other._target);
        }

        /// <summary>
        /// 获取当前 <see cref="Binding"/> 的哈希代码
        /// </summary>
        /// <returns>当前 <see cref="Binding"/> 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return IsBinding
                ? _source.GetHashCode() ^ _target.GetHashCode()
                : _source.GetHashCode() ^ _target.GetHashCode() ^ 0b10101010;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected internal override bool IsSource(object obj, string memberName)
        {
            return ReferenceEquals(_source.Target, obj) && memberName.Equals(_source.Name);
        }

        /// <inheritdoc/>
        protected internal override bool IsTarget(object obj, string memberName)
        {
            return ReferenceEquals(_target.Target, obj) && memberName.Equals(_target.Name);
        }

        /// <inheritdoc/>
        protected override void OnSynchronize()
        {
            object value = _source.Value;

            if (!value.Equals(_sourceValueCache))
            {
                _sourceValueCache = value;
                _targetValueCache = RequiresConversion
                    ? Converter.Convert(value, _target.Type, CurrentCulture)
                    : value;

                _target.Value = _targetValueCache;
            }
            else if (Mode is BindingMode.TwoWay)
            {
                value = _target.Value;

                if (!value.Equals(_targetValueCache))
                {
                    _targetValueCache = value;
                    _sourceValueCache = RequiresConversion
                        ? Converter.ConvertBack(value, _source.Type, CurrentCulture)
                        : value;

                    _source.Value = _sourceValueCache;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnUnbind()
        {
            if (!_globalUnbingind)
            {
                Bindings.Remove(this);
            }

            if (_source.Target is DependencyObject dependencySource)
            {
                dependencySource.PropertyBindings.Remove(this);
            }
            else if (_source.Target is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged -= OnNotifiableSourceChanged;
            }

            if (Mode is BindingMode.OneWay)
            {
                return;
            }

            if (_target.Target is DependencyObject dependencyTarget)
            {
                dependencyTarget.PropertyBindings.Remove(this);
            }
            else if (_target.Target is INotifyPropertyChanged notifiableTarget)
            {
                notifiableTarget.PropertyChanged -= OnNotifiableTargetChanged;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static MemberInfo FindMember(object obj, string memberName, BindingFlags flags)
        {
            var type = obj as Type ?? obj.GetType();

            return (type.GetMember(memberName, MemberTypes.Field | MemberTypes.Property, flags)
                ?? throw new MissingMemberException(type.FullName, memberName))[0];
        }

        private static Binding StartBinding(Binding binding, object source, object target, BindingMode mode)
        {
            if (source is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged += binding.OnNotifiableSourceChanged;
            }

            if (mode is BindingMode.OneWay)
            {
                return binding;
            }

            if (target is INotifyPropertyChanged notifiableTarget)
            {
                notifiableTarget.PropertyChanged += binding.OnNotifiableTargetChanged;
            }

            return binding;
        }

        private static Binding StartBinding(Binding binding, object source, DependencyObject target, BindingMode mode)
        {
            if (source is INotifyPropertyChanged notifiableSource)
            {
                notifiableSource.PropertyChanged += binding.OnNotifiableSourceChanged;
            }

            if (mode is BindingMode.OneWay)
            {
                return binding;
            }

            target.PropertyBindings.Add(binding);

            return binding;
        }

        private static Binding StartBinding(Binding binding, DependencyObject source, object target, BindingMode mode)
        {
            source.PropertyBindings.Add(binding);

            if (mode is BindingMode.OneWay)
            {
                return binding;
            }

            if (target is INotifyPropertyChanged notifiableTarget)
            {
                notifiableTarget.PropertyChanged += binding.OnNotifiableTargetChanged;
            }

            return binding;
        }

        private static Binding StartBinding(Binding binding, DependencyObject source, DependencyObject target, BindingMode mode)
        {
            source.PropertyBindings.Add(binding);

            if (mode is BindingMode.OneWay)
            {
                return binding;
            }

            target.PropertyBindings.Add(binding);

            return binding;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            _sourceValueCache = e.NewValue;

            object convertedValue = RequiresConversion
                ? Converter.Convert(_sourceValueCache, _target.Type, CurrentCulture)
                : _sourceValueCache;

            if (ReferenceEquals(convertedValue, DependencyProperty.UnsetValue) 
                || (_targetValueCache?.Equals(convertedValue) ?? convertedValue is null))
            {
                return;
            }

            _targetValueCache = convertedValue;
            _target.Value = _targetValueCache;
        }

        internal void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            _targetValueCache = e.NewValue;

            object convertedValue = RequiresConversion
                ? Converter.ConvertBack(_targetValueCache, _target.Type, CurrentCulture)
                : _targetValueCache;

            if (ReferenceEquals(convertedValue, DependencyProperty.UnsetValue)
                || (_sourceValueCache?.Equals(convertedValue) ?? convertedValue is null))
            {
                return;
            }

            _sourceValueCache = convertedValue;
            _source.Value = _sourceValueCache;
        }

        internal void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsSource(sender, e.PropertyName))
            {
                return;
            }

            _sourceValueCache = _source.Value;

            object convertedValue = RequiresConversion
                ? Converter.Convert(_sourceValueCache, _target.Type, CurrentCulture)
                : _sourceValueCache;

            if (ReferenceEquals(convertedValue, DependencyProperty.UnsetValue)
                || (_targetValueCache?.Equals(convertedValue) ?? convertedValue is null))
            {
                return;
            }

            _targetValueCache = convertedValue;
            _target.Value = _targetValueCache;
        }

        internal void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsTarget(sender, e.PropertyName))
            {
                return;
            }

            _targetValueCache = _target.Value;

            object convertedValue = RequiresConversion
                ? Converter.ConvertBack(_targetValueCache, _target.Type, CurrentCulture)
                : _targetValueCache;

            if (ReferenceEquals(convertedValue, DependencyProperty.UnsetValue)
                || (_sourceValueCache?.Equals(convertedValue) ?? convertedValue is null))
            {
                return;
            }

            _sourceValueCache = convertedValue;
            _source.Value = _sourceValueCache;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly HashSet<Binding> Bindings = new HashSet<Binding>();
        private static readonly CultureInfo CurrentCulture = CultureInfo.InvariantCulture;

        private static bool _globalUnbingind;

        private readonly BindingMember _source;
        private readonly BindingMember _target;

        private object _sourceValueCache;
        private object _targetValueCache;

        #endregion
    }
}
