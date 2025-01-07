using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 表示对象指定成员间的绑定关系
    /// </summary>
    public class Binding : BindingBase
    {
        private static readonly CultureInfo _currentCulture = CultureInfo.InvariantCulture;

        private object _sourceValueCache;
        private object _targetValueCache;


        internal Binding(
            object source,
            object sourcePath,
            object target,
            object targetPath,
            IValueConverter converter,
            BindingMode mode) : base(source, sourcePath, target, targetPath, converter, mode)
        {
        }


        /// <inheritdoc/>
        public override void Synchronize()
        {
            if (!IsBinding)
            {
                return;
            }

            if (!IsBindingValid)
            {
                Unbind();
            }

            object value = SourceMember.Value;
            if (!value.Equals(_sourceValueCache))
            {
                _sourceValueCache = value;
                _targetValueCache = ShouldConvert
                    ? Converter.Convert(value, _currentCulture)
                    : value;

                TargetMember.Value = _targetValueCache;
            }
            else if (Mode is BindingMode.TwoWay)
            {
                value = TargetMember.Value;

                if (!value.Equals(_targetValueCache))
                {
                    _targetValueCache = value;
                    _sourceValueCache = ShouldConvert
                        ? Converter.ConvertBack(value, _currentCulture)
                        : value;

                    SourceMember.Value = _sourceValueCache;
                }
            }
        }


        //------------------------------------------------------
        //
        //  Protected Method
        //
        //------------------------------------------------------

        #region Protected Method

        /// <inheritdoc/>
        protected override void OnDependencySourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            _sourceValueCache = e.NewValue;
            _targetValueCache = ShouldConvert
                ? Converter.Convert(_sourceValueCache, _currentCulture)
                : _sourceValueCache;

            TargetMember.Value = _targetValueCache;
        }

        /// <inheritdoc/>
        protected override void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            _targetValueCache = e.NewValue;
            _sourceValueCache = ShouldConvert
                ? Converter.ConvertBack(_targetValueCache, _currentCulture)
                : _targetValueCache;

            SourceMember.Value = _sourceValueCache;
        }

        /// <inheritdoc/>
        protected override void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            _sourceValueCache = SourceMember.Value;
            _targetValueCache = ShouldConvert
                ? Converter.Convert(_sourceValueCache, _currentCulture)
                : _sourceValueCache;

            TargetMember.Value = _targetValueCache;
        }

        /// <inheritdoc/>
        protected override void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e)
        {
            _targetValueCache = TargetMember.Value;
            _sourceValueCache = ShouldConvert
                ? Converter.ConvertBack(_targetValueCache, _currentCulture)
                : _targetValueCache;

            SourceMember.Value = _sourceValueCache;
        }

        #endregion
    }
}
