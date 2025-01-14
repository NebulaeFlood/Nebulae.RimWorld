using Nebulae.RimWorld.UI.Data.Binding.Converters;
using System.ComponentModel;
using System.Globalization;

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
            OnSourceChanged(e.NewValue);
        }

        /// <inheritdoc/>
        protected override void OnDependencyTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OnTargetChanged(e.NewValue);
        }

        /// <inheritdoc/>
        protected override void OnNotifiableSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSourceChanged(SourceMember.Value);
        }

        /// <inheritdoc/>
        protected override void OnNotifiableTargetChanged(object sender, PropertyChangedEventArgs e)
        {
            OnTargetChanged(TargetMember.Value);
        }

        #endregion


        private void OnSourceChanged(object value)
        {
            _sourceValueCache = value;

            object convertedValue = ShouldConvert
                ? Converter.Convert(value, _currentCulture)
                : _sourceValueCache;

            if (convertedValue?.IsUnsetValue() ?? false)
            {
                return;
            }

            if (!convertedValue?.Equals(_targetValueCache) ?? true)
            {
                _targetValueCache = convertedValue;

                TargetMember.Value = convertedValue;
            }
        }

        private void OnTargetChanged(object value)
        {
            _targetValueCache = value;

            object convertedValue = ShouldConvert
                ? Converter.ConvertBack(value, _currentCulture)
                : value;

            if (convertedValue?.IsUnsetValue() ?? false)
            {
                return;
            }

            if (!convertedValue?.Equals(_sourceValueCache) ?? true)
            {
                _sourceValueCache = convertedValue;

                SourceMember.Value = convertedValue;
            }
        }
    }
}
