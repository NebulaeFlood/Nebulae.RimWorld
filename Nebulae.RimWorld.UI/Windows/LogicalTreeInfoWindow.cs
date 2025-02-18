using LudeonTK;
using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.TextBlock;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 用于显示控件信息的窗口
    /// </summary>
    public sealed class LogicalTreeInfoWindow : ControlWindow
    {
        private static readonly LogicalTreeInfoWindow _instance;
        private static readonly SortedSet<Control> _sources = new SortedSet<Control>(ControlComparer.Instance);
        private static readonly OptionPanel _optionPanel;


        static LogicalTreeInfoWindow()
        {
            _optionPanel = new OptionPanel();
            _instance = new LogicalTreeInfoWindow { Content = _optionPanel };
        }

        private LogicalTreeInfoWindow()
        {
            draggable = true;
            doCloseButton = false;
            drawShadow = false;
            resizeable = true;

            layer = WindowLayer.Super;

            InitialWidth = 300f;
            InitialHeight = Verse.UI.screenHeight;
            IsDebugWindow = true;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Method

        /// <inheritdoc/>
        public override void PostClose()
        {
            base.PostClose();
            _optionPanel.Reset();
            _sources.Clear();
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


        //------------------------------------------------------
        //
        //  Internal Static Methods
        //
        //------------------------------------------------------

        #region Internal Static Methods

        internal static void AddSource(Control source)
        {
            _sources.Add(source);
        }

        internal static void RemoveSource(Control source)
        {
            _sources.Remove(source);
        }

        #endregion


        /// <inheritdoc/>
        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(0f, 0f, InitialWidth, InitialHeight);
        }

        private static Expander GenerateTreeInfo(Control source)
        {
            var node = new Expander
            {
                Text = source.ToString(),
                Tooltip = $"Type:\n{source.Type}\n" +
                    $"Name:\n{source.Name}\n" +
                    $"RenderRect:\n{source.RenderRect}\n" +
                    $"DesiredRect:\n{source.DesiredRect}\n" +
                    $"ContentRect:\n{source.ContentRect}\n" +
                    $"Visibility:\n{source.Visibility}\n" +
                    $"IsArrangeValid:\n{source.IsArrangeValid}\n" + 
                    $"IsMeasureValid:\n{source.IsMeasureValid}\n" + 
                    $"IsSegmentValid:\n{source.IsSegmentValid}\n"
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


        //------------------------------------------------------
        //
        //  Private Classes
        //
        //------------------------------------------------------

        #region Private Classes

        private class ControlComparer : IComparer<Control>
        {
            public static readonly IComparer<Control> Instance = new ControlComparer();

            public int Compare(Control x, Control y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }
                else if (x.Rank >= y.Rank)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
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
                    availableRect.y + 24f * 4f,
                    availableRect.width,
                    300f));

                _treeViewer.Arrange(new Rect(
                    availableRect.x,
                    availableRect.y + 24f * 4f + 308f,
                    availableRect.width,
                    availableRect.height - 24f * 4f - 308f));

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

                        DrawCheckBox(checkBoxRect, "Show Layout Rect", ref _sourceWindow.DebugDrawDesiredRect, false);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show Render Rect", ref _sourceWindow.DebugDrawRenderRect, false);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show HitBox Rect", ref _sourceWindow.DebugDrawHitTestRect, false);
                        checkBoxRect.y += 24f;
                        DrawCheckBox(checkBoxRect, "Show Visible Rect", ref _sourceWindow.DebugDrawContentRect, false);

                        Text.Anchor = anchor;
                    }
                    else
                    {
                        DrawEntry(checkBoxRect, "Show Layout Rect");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show Render Rect");
                        checkBoxRect.y += 24f;
                        DrawEntry(checkBoxRect, "Show HitBox Rect");
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
                _treeViewer.Measure(new Size(availableSize.Width, availableSize.Height - 24f * 4f - 308f));
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

            private static void DrawCheckBox(Rect renderRect, string text, ref bool result, bool isDisabled)
            {
                GUI.Label(renderRect, text);
                Widgets.Checkbox(
                    renderRect.xMax - 24f,
                    renderRect.y,
                    ref result,
                    disabled: isDisabled,
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

        #endregion
    }
}
