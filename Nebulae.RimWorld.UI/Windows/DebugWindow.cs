using LudeonTK;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using TextBlock = Nebulae.RimWorld.UI.Controls.TextBlock;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 用于显示控件信息的窗口
    /// </summary>
    public sealed class DebugWindow : ControlWindow
    {
        private static readonly DebugWindow _instance;
        private static readonly OptionPanel _optionPanel;


        static DebugWindow()
        {
            _optionPanel = new OptionPanel();
            _instance = new DebugWindow { Content = _optionPanel };

            Windows.Remove(_instance);
        }

        private DebugWindow()
        {
            draggable = true;
            doCloseButton = false;
            drawShadow = false;
            resizeable = true;

            layer = WindowLayer.Super;

            InitialWidth = 300f;
            IsDebugWindow = true;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Method

        /// <summary>
        /// 显示所有的除了 <see cref="DebugWindow"/> 以外的 <see cref="ControlWindow"/> 的调试按钮
        /// </summary>
        [DebugAction("Nubulae's UI Control", "Show Debug Buttons", allowedGameStates = AllowedGameStates.Entry, displayPriority = 1)]
        public static void DrawDebugButtons()
        {
            foreach (var window in Windows)
            {
                window.IsDebugWindow = false;
        }
        }

        /// <summary>
        /// 隐藏所有的除了 <see cref="DebugWindow"/> 以外的 <see cref="ControlWindow"/> 的调试按钮
        /// </summary>
        [DebugAction("Nubulae's UI Control", "Hide Debug Buttons", allowedGameStates = AllowedGameStates.Entry, displayPriority = 0)]
        public static void HideDebugButtons()
        {
            foreach (var window in Windows)
            {
                window.IsDebugWindow = true;
        }
        }

        /// <inheritdoc/>
        public override void PostClose()
        {
            base.PostClose();
            _optionPanel.Reset();
        }

        /// <summary>
        /// 显示控件树信息
        /// </summary>
        /// <param name="root">根控件</param>
        public static void ShowWindow(Control root)
        {
            if (_instance.IsOpen)
            {
                _instance.Close();
            }

            _optionPanel.SetOptionContent(root, GenerateTreeInfo(root));
            _instance.Show();
        }

        #endregion


        /// <inheritdoc/>
        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(0f, 0f, InitialWidth, Verse.UI.screenHeight);
        }

        private static Expander GenerateTreeInfo(Control source)
        {
            var node = new Expander
            {
                Text = source.ToString(),
                Tooltip =
                    GenerateInfoLine("Type", source.Type) +
                    GenerateInfoLine("Name", source.Name) +
                    GenerateInfoLine("RenderRect", source.RenderRect) +
                    GenerateInfoLine("DesiredRect", source.DesiredRect) +
                    GenerateInfoLine("ContentRect", source.ContentRect) +
                    GenerateInfoLine("Visibility", source.Visibility) +
                    GenerateInfoLine("IsArrangeValid", source.IsArrangeValid) +
                    GenerateInfoLine("IsMeasureValid", source.IsMeasureValid) +
                    GenerateInfoLine("IsSegmentValid", source.IsSegmentValid) +
                    GenerateInfoLine("ShowTooltip", source.ShowTooltip) +
                    GenerateInfo("Tooltip", source.Tooltip.text)
            };
            node.Clicked += _optionPanel.SetInfo;
            var children = source.EnumerateLogicalChildren();

            if (children.Any())
            {
                var panel = new VirtualizingStackPanel { ChildMaxHeight = float.PositiveInfinity };

                foreach (var child in source.EnumerateLogicalChildren())
                {
                    panel.Append(GenerateTreeInfo(child));
                }

                node.Content = panel;
            }

            return node;
        }

        private static string GenerateInfo(string key, object value)
        {
            return $"<color=yellow>{key}</color>:\n{value}";
        }

        private static string GenerateInfoLine(string key, object value)
        {
            return $"<color=yellow>{key}</color>:\n{value}\n\n";
        }


        private class OptionPanel : Control
        {
            private static readonly Color _backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            private static readonly Color _backgroundBorderColor = new Color(0.6f, 0.6f, 0.6f);

            private readonly TextBlock _infoBox;
            private readonly ScrollViewer _infoViewer;
            private readonly ScrollViewer _treeViewer;

            private bool _initialized = false;
            private bool _sourceInLayoutTree;
            private ControlWindow _sourceWindow;


            public OptionPanel()
            {
                _infoBox = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                _infoViewer = new ScrollViewer
                {
                    Margin = 4f,
                    Content = _infoBox
                };
                _infoViewer.SetParent(this);

                _treeViewer = new ScrollViewer { Margin = 4f };
                _treeViewer.SetParent(this);
            }


            //------------------------------------------------------
            //
            //  Public Methods
            //
            //------------------------------------------------------

            #region Public Methods

            public void Reset()
            {
                if (!_initialized)
                {
                    return;
                }

                _infoBox.Text = string.Empty;
                _initialized = false;

                if (!_sourceInLayoutTree)
                {
                    return;
                }

                _sourceInLayoutTree = false;
                _sourceWindow.DebugDrawContentRect = false;
                _sourceWindow.DebugDrawDesiredRect = false;
                _sourceWindow.DebugDrawHitTestRect = false;
                _sourceWindow.DebugDrawRenderRect = false;
                _sourceWindow = null;
            }

            public void SetInfo(Expander sender, Control content)
            {
                _infoBox.Text = sender.Tooltip.text;
            }

            public void SetOptionContent(Control source, Control content)
            {
                if (source is null)
                {
                    Reset();
                    return;
                }

                _initialized = true;

                if (source.Owner is ControlWindow window)
                {
                    _sourceWindow = window;
                    _sourceInLayoutTree = true;
                }
                else
                {
                    _sourceInLayoutTree = false;
                }

                _treeViewer.Content = content;
                _treeViewer.InvalidateMeasure();
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
                _infoViewer.Arrange(new Rect(
                    availableRect.x,
                    availableRect.y + 120f,
                    availableRect.width,
                    300f));

                _treeViewer.Arrange(new Rect(
                    availableRect.x,
                    availableRect.y + 428f,
                    availableRect.width,
                    availableRect.height - 428f));

                return availableRect;
            }

            protected override void DrawCore()
            {
                if (_initialized)
                {
                    Rect checkBoxRect = new Rect(RenderRect.x, RenderRect.y, RenderRect.width, 24f);

                    if (_sourceInLayoutTree)
                    {
                        var anchor = Text.Anchor;
                        Text.Anchor = TextAnchor.MiddleLeft;

                        DrawCheckBox(checkBoxRect, "Hide Dbug Buttons", ref _sourceWindow.IsDebugWindow);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show HitBox Rect", ref _sourceWindow.DebugDrawHitTestRect);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show Layout Rect", ref _sourceWindow.DebugDrawDesiredRect);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show Render Rect", ref _sourceWindow.DebugDrawRenderRect);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show Visible Rect", ref _sourceWindow.DebugDrawContentRect);

                        Text.Anchor = anchor;
                    }
                    else
                    {
                        DrawEntry(checkBoxRect, "Hide Dbug Buttons");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show HitBox Rect");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show Layout Rect");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show Render Rect");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show Visible Rect");
                    }

                    UIUtility.DrawRectangle(_infoViewer.DesiredRect, _backgroundColor, _backgroundBorderColor);
                    _infoViewer.Draw();

                    UIUtility.DrawRectangle(_treeViewer.DesiredRect, _backgroundColor, _backgroundBorderColor);
                    _treeViewer.Draw();
                }
                else
                {
                    var anchor = Text.Anchor;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    GUI.Label(RenderRect, "Please select source to get info.");
                    Text.Anchor = anchor;
                }
            }

            protected internal override IEnumerable<Control> EnumerateLogicalChildren()
            {
                yield return _infoViewer;
                yield return _treeViewer;
            }

            protected override Size MeasureCore(Size availableSize)
            {
                _infoViewer.Measure(new Size(availableSize.Width, 300f));
                _treeViewer.Measure(new Size(availableSize.Width, availableSize.Height - 428f));
                return availableSize;
            }

            protected override Rect SegmentCore(Rect visiableRect)
            {
                Rect contentRect = visiableRect.IntersectWith(RenderRect);
                _infoViewer.Segment(contentRect);
                _treeViewer.Segment(contentRect);
                return contentRect;
            }

            #endregion


            //------------------------------------------------------
            //
            //  Private Methods
            //
            //------------------------------------------------------

            #region Private Methods

            private static void DrawCheckBox(Rect renderRect, string text, ref bool result)
            {
                GUI.Label(renderRect, text);
                Widgets.Checkbox(
                    renderRect.xMax - 24f,
                    renderRect.y,
                    ref result,
                    disabled: false,
                    texChecked: Widgets.CheckboxOnTex,
                    texUnchecked: Widgets.CheckboxOffTex);
            }

            private static void DrawEntry(Rect renderRect, string text)
            {
                var anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                GUI.Label(renderRect, text);
                Text.Anchor = TextAnchor.MiddleRight;
                GUI.Label(renderRect, "Invalid");
                Text.Anchor = anchor;
            }

            #endregion
        }
    }
}
