using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Controls.Converters;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Data.Bindings;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static UnityEngine.GraphicsBuffer;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.Basic.TextBlock;

namespace Nebulae.RimWorld.UI.Automation.Diagnostics.Views
{
    internal sealed class DebugMemberView : Control
    {
        public static readonly DebugMemberView View = new DebugMemberView();


        public override IEnumerable<Control> LogicalChildren
        {
            get
            {
                yield return _content;
            }
        }


        private DebugMemberView()
        {
            var rectDebugPanel = new Border
            {
                Background = BrushUtility.DarkGrey,
                BorderBrush = BrushUtility.Grey,
                BorderThickness = 1f,
                Content = new StackPanel().Set(_debugControlRect, _debugDesiredRect, _debugRenderRect, _debugRegionRect, _debugVisibleRect)
            };

            var memberDebugPanel = new Border
            {
                Background = BrushUtility.DarkGrey,
                BorderBrush = BrushUtility.Grey,
                BorderThickness = 1f,
                Content = new ScrollViewer { Content = _textBlock }
            };

            _content = new Grid().DefineRows(Grid.Auto, 24f, Grid.Remain).Set(rectDebugPanel, null, memberDebugPanel);
            _content.Parent = this;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void Bind(Control control)
        {
            Clear();

            _debugControlRect.State = control.DebugControlRect ? ToggleState.On : ToggleState.Off;
            _debugDesiredRect.State = control.DebugDesiredRect ? ToggleState.On : ToggleState.Off;
            _debugRenderRect.State = control.DebugRenderRect ? ToggleState.On : ToggleState.Off;
            _debugRegionRect.State = control.DebugRegionRect ? ToggleState.On : ToggleState.Off;
            _debugVisibleRect.State = control.DebugVisibleRect ? ToggleState.On : ToggleState.Off;

            _debugControlRectBinding = Binding.Create(_debugControlRect, ToggleButton.StateProperty, control, nameof(DebugControlRect), BindingMode.OneWay);
            _debugDesiredRectBinding = Binding.Create(_debugDesiredRect, ToggleButton.StateProperty, control, nameof(DebugDesiredRect), BindingMode.OneWay);
            _debugRenderRectBinding = Binding.Create(_debugRenderRect, ToggleButton.StateProperty, control, nameof(DebugRenderRect), BindingMode.OneWay);
            _debugRegionRectBinding = Binding.Create(_debugRegionRect, ToggleButton.StateProperty, control, nameof(DebugRenderRect), BindingMode.OneWay);
            _debugVisibleRectBinding = Binding.Create(_debugVisibleRect, ToggleButton.StateProperty, control, nameof(DebugVisibleRect), BindingMode.OneWay);

            if (!TryCreateDebugMembers(control, out _debugMembers))
            {
                return;
            }

            _anyDebugMember = true;
            _stringBuilder.Capacity = 400;
        }

        public void Clear()
        {
            if (_debugControlRectBinding != null)
            {
                _debugControlRectBinding.Unbind();
                _debugDesiredRectBinding.Unbind();
                _debugRenderRectBinding.Unbind();
                _debugRegionRectBinding.Unbind();
                _debugVisibleRectBinding.Unbind();

                _debugControlRectBinding = null;
                _debugDesiredRectBinding = null;
                _debugRenderRectBinding = null;
                _debugRegionRectBinding = null;
                _debugVisibleRectBinding = null;
            }

            if (!_anyDebugMember)
            {
                return;
            }

            Array.Clear(_debugMembers, 0, _debugMembers.Length);
            _stringBuilder.Clear();

            _stringBuilder.Capacity = 0;
            _textBlock.Text = string.Empty;

            _anyDebugMember = false;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        protected override Rect ArrangeCore(Rect availableRect)
        {
            return _content.Arrange(availableRect);
        }

        protected override Size MeasureCore(Size availableSize)
        {
            return _content.Measure(availableSize);
        }

        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            return _content.Segment(visiableRect);
        }

        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            return _content.HitTest(hitPoint);
        }

        protected override void DrawCore(ControlState states)
        {
            if (_anyDebugMember && Event.current.type is EventType.Repaint)
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(_debugMembers[0].Value);

                for (int i = 1; i < _debugMembers.Length; i++)
                {
                    _stringBuilder.Append('\n');
                    _stringBuilder.Append(_debugMembers[i].Value);
                }

                _textBlock.Text = _stringBuilder.ToString();
            }

            _content.Draw();
        }

        #endregion


        private static bool TryCreateDebugMembers(Control target, out DebugMember[] debugMembers)
        {
            var members = target.Type.GetMembers();
            var count = members.Length;

            List<DebugMember> cache = new List<DebugMember>(count);

            for (int i = 0; i < count; i++)
            {
                if (DebugMember.TryCreate(target, members[i], out var debugMember))
                {
                    cache.Add(debugMember);
                }
            }

            if (cache.Count > 0)
            {
                cache.Sort(DebugMember.Comparison);
                debugMembers = cache.ToArray();
                return true;
            }
            else
            {
                debugMembers = null;
                return false;
            }
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly StringBuilder _stringBuilder = new StringBuilder(0);

        private DebugMember[] _debugMembers;
        private bool _anyDebugMember;

        private readonly Grid _content;
        private readonly TextBlock _textBlock = new TextBlock { Margin = new Thickness(4f, 4f, 0f, 0f) };

        private readonly CheckBox _debugControlRect = new CheckBox { Text = "Show Control Rect" };
        private readonly CheckBox _debugDesiredRect = new CheckBox { Text = "Show Desired Rect" };
        private readonly CheckBox _debugRenderRect = new CheckBox { Text = "Show Render Rect" };
        private readonly CheckBox _debugRegionRect = new CheckBox { Text = "Show Region Rect" };
        private readonly CheckBox _debugVisibleRect = new CheckBox { Text = "Show Visible Rect" };

        private Binding _debugControlRectBinding;
        private Binding _debugDesiredRectBinding;
        private Binding _debugRenderRectBinding;
        private Binding _debugRegionRectBinding;
        private Binding _debugVisibleRectBinding;

        #endregion
    }
}
