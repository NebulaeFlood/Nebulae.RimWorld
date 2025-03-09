using Nebulae.RimWorld.UI.Automation;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.Basic.TextBlock;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    [StaticConstructorOnStartup]
    internal sealed class DebugPanel : Control
    {
        private static readonly Texture2D _background = new Color(0.2f, 0.2f, 0.2f).ToBrush();
        private static readonly Texture2D _borderBrush = new Color(0.6f, 0.6f, 0.6f).ToBrush();


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Grid _root;

        private readonly TextBlock _infoBox;
        private readonly ScrollViewer _infoViewer;
        private readonly ScrollViewer _treeViewer;

        private bool _isEmpty = true;
        private Visual _currentNode;

        private bool _initialized = false;
        private bool _sourceInLayoutTree;

        private bool _sourceDrawContentRect;
        private bool _sourceDrawDesiredRect;
        private bool _sourceDrawControlRect;
        private bool _sourceDrawRenderRect;

        private readonly List<DebugInfo> _cachedInfos = new List<DebugInfo>();

        #endregion


        public DebugPanel()
        {
            _infoBox = new TextBlock();
            _infoViewer = new ScrollViewer
            {
                Margin = 4f,
                Content = _infoBox,
                IsHitTestVisible = true
            };

            _treeViewer = new ScrollViewer
            {
                Margin = 4f,
                IsHitTestVisible = true
            };

            _root = new Grid().DefineRows(0.7f, 0.3f)
                .Set(
                    new Border
                    {
                        Background = _background,
                        BorderBrush = _borderBrush,
                        BorderThickness = 1f,
                        Content = _infoViewer,
                        Margin = 2f,
                        Padding = 2f
                    },
                    new Border
                    {
                        Background = _background,
                        BorderBrush = _borderBrush,
                        BorderThickness = 1,
                        Content = _treeViewer,
                        Margin = 2f,
                        Padding = 2f
                    });
            _root.SetParentSilently(this);
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

            _cachedInfos.Clear();
            _cachedInfos.TrimExcess();

            if (!_isEmpty)
            {
                _currentNode.DebugContent = DebugContent.Empty;
            }

            _currentNode = null;

            _infoBox.Text = string.Empty;
            _initialized = false;

            _isEmpty = true;

            if (!_sourceInLayoutTree)
            {
                return;
            }

            _sourceInLayoutTree = false;
        }

        public void ResetInfo()
        {
            _cachedInfos.Clear();
            _cachedInfos.TrimExcess();

            if (!_isEmpty)
            {
                _currentNode.DebugContent = DebugContent.Empty;
            }

            _currentNode = null;

            _isEmpty = true;

            _sourceDrawContentRect = false;
            _sourceDrawDesiredRect = false;
            _sourceDrawControlRect = false;
            _sourceDrawRenderRect = false;

            _infoViewer.ScrollToTop();
        }

        public void SetSourceTree(Visual source)
        {
            if (source is null)
            {
                Reset();
                return;
            }

            _initialized = true;

            if (source.LayoutManager is null)
            {
                _sourceInLayoutTree = false;
            }
            else
            {
                _sourceInLayoutTree = true;
            }

            _treeViewer.Content = GenerateTree(source);
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
            _root.Arrange(new Rect(
                availableRect.x,
                availableRect.y + 120f,
                availableRect.width,
                availableRect.height - 120f));

            return availableRect;
        }

        protected override void DrawCore()
        {
            if (_initialized)
            {
                _infoBox.Text = GenerateInfo();

                if (!_isEmpty
                    && _sourceInLayoutTree)
                {
                    Rect checkBoxRect = new Rect(RenderRect.x, RenderRect.y, RenderRect.width, 24f);

                    var anchor = Text.Anchor;
                    Text.Anchor = TextAnchor.MiddleLeft;

                    DrawCheckBox(checkBoxRect, "Show Desired Rect", ref _sourceDrawDesiredRect);
                    checkBoxRect.y += 30f;
                    DrawCheckBox(checkBoxRect, "Show Render Rect", ref _sourceDrawRenderRect);
                    checkBoxRect.y += 30f;
                    DrawCheckBox(checkBoxRect, "Show Control Rect", ref _sourceDrawControlRect);
                    checkBoxRect.y += 30f;
                    DrawCheckBox(checkBoxRect, "Show Content Rect", ref _sourceDrawContentRect);

                    Text.Anchor = anchor;

                    _currentNode.DebugDrawContentRect = _sourceDrawContentRect;
                    _currentNode.DebugDrawDesiredRect = _sourceDrawDesiredRect;
                    _currentNode.DebugDrawControlRect = _sourceDrawControlRect;
                    _currentNode.DebugDrawRenderRect = _sourceDrawRenderRect;
                }
                else
                {
                    var anchor = Text.Anchor;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(new Rect(RenderRect.x, RenderRect.y, RenderRect.width, 120f),
                        "Click any expander's title to get info.");
                    Text.Anchor = anchor;
                }

                _root.Draw();
            }
            else
            {
                var anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(RenderRect, "Please select logical tree to get info.");
                Text.Anchor = anchor;
            }
        }

        protected internal override IEnumerable<Visual> EnumerateLogicalChildren()
        {
            yield return _root;
        }

        protected override Size MeasureCore(Size availableSize)
        {
            _root.Measure(new Size(availableSize.Width, availableSize.Height - 120f));
            return availableSize;
        }

        protected override Rect SegmentCore(Rect visiableRect)
        {
            _root.Segment(visiableRect);
            return visiableRect;
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
            Widgets.Label(renderRect, text);
            Widgets.Checkbox(
                renderRect.xMax - 24f,
                renderRect.y,
                ref result,
                disabled: false,
                texChecked: Widgets.CheckboxOnTex,
                texUnchecked: Widgets.CheckboxOffTex);
        }

        private string GenerateInfo()
        {
            if (_isEmpty)
            {
                return "Click any expander's title to get info.";
            }
            else if (_cachedInfos.Count > 0)
            {
                string text = _cachedInfos[0].Info;

                for (int i = 1; i < _cachedInfos.Count; i++)
                {
                    text += "\n" + _cachedInfos[i].Info;
                }

                return text;
            }
            else
            {
                var members = _currentNode.Type.GetMembers();

                for (int i = 0; i < members.Length; i++)
                {
                    if (DebugInfo.TryCreate(members[i], _currentNode, out var info))
                    {
                        _cachedInfos.Add(info);
                    }
                }

                _cachedInfos.Sort(DebugInfo.Comparison);

                string text = _cachedInfos[0].Info;

                for (int i = 1; i < _cachedInfos.Count; i++)
                {
                    text += "\n" + _cachedInfos[i].Info;
                }

                return text;
            }
        }

        private Expander GenerateTree(Visual root)
        {
            var node = new Expander
            {
                Title = root.ToString(),
                TitleHitTestVisible = true
            };
            node.Click += delegate { SelectNode(root); };
            var children = root.EnumerateLogicalChildren();

            if (children.Any())
            {
                var contentPanel = new VirtualizingStackPanel();

                foreach (var child in root.EnumerateLogicalChildren())
                {
                    contentPanel.Append(GenerateTree(child));
                }

                node.Content = contentPanel;
            }

            return node;
        }

        private void SelectNode(Visual node)
        {
            if (ReferenceEquals(node, _currentNode))
            {
                return;
            }

            ResetInfo();

            _currentNode = node;
            _isEmpty = node is null;
        }

        #endregion
    }
}
