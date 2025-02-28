using Nebulae.RimWorld.UI.Utilities;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="TabControl"/> 的选项卡项
    /// </summary>
    [StaticConstructorOnStartup]
    public class TabItem : ButtonBase
    {
        /// <summary>
        /// 选项卡图集
        /// </summary>
        public static readonly Texture2D TabAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TabAtlas");


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private TabControl _container;
        private Control _content;

        private Rect _leftRect;
        private static readonly Rect _leftUVRect = new Rect(0f, 0f, 0.46875f, 1f);

        private Rect _midRect;
        private Rect _midUVRect;

        private Rect _rightRect;
        private static readonly Rect _rightUVRect = new Rect(0.53125f, 0f, 0.46875f, 1f);

        private Rect _bottomRect;
        private static readonly Rect _bottomUVRect = new Rect(0.5f, 0.01f, 0.01f, 0.01f);

        private bool _selected = false;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取选项卡所属的容器
        /// </summary>
        public TabControl Container
        {
            get => _container;
            internal set
            {
                if (!ReferenceEquals(_container, value))
                {
                    _selected = false;

                    _container = value;
                    SetParent(value);
                    _content?.SetParent(value);
                }
            }
        }

        /// <summary>
        /// 获取或设置选项卡内容
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(_content, value))
                {
                    _content?.SetParent(null);
                    _content = value;

                    if (_content is null)
                    {
                        return;
                    }

                    _content.SetParent(_container);
                    _content.InvalidateMeasure();

                    if (_selected)
                    {
                        _container?.Select(this);
                    }
                }
            }
        }

        /// <summary>
        /// 选项卡是否被选中
        /// </summary>
        public bool Selected
        {
            get => _selected;
            internal set
            {
                if (_selected != value)
                {
                    _selected = value;

                    PlayMouseOverSound = !value;
                }
            }
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="TabItem"/> 的新实例
        /// </summary>
        public TabItem()
        {
            ClickSound = SoundDefOf.RowTabSelect;
            CursorOverSound = SoundDefOf.Mouseover_Tab;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = base.ArrangeCore(availableRect);

            _leftRect = new Rect(renderRect.x, renderRect.y, 30f, renderRect.height);

            _rightRect = new Rect(renderRect.xMax - 30f, renderRect.y, 30f, renderRect.height);

            _midRect = new Rect(renderRect.x + 29f, renderRect.y, renderRect.width - 58f, renderRect.height);
            _midUVRect = new Rect(30f, 0f, 4f, TabAtlas.height).ToUVRect(new Vector2(TabAtlas.width, TabAtlas.height));

            _bottomRect = new Rect(renderRect.x, renderRect.yMax - 1f, renderRect.width, 1f);

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawButton(ButtonStatus status)
        {
            Color color = GUI.color;
            Color textColor;

            Rect labelRect;

            if (status is ButtonStatus.Pressed
                || _selected)
            {
                labelRect = RenderRect;
                textColor = Color.yellow;
            }
            else if (status is ButtonStatus.Hovered)
            {
                labelRect = new Rect(RenderRect.x + 1f, RenderRect.y, RenderRect.width, RenderRect.height);
                textColor = Color.yellow;
            }
            else
            {
                labelRect = RenderRect;
                textColor = Color.white;
            }

            if (status.HasFlag(ButtonStatus.Disabled))
            {
                GUI.color = Color.white * Widgets.InactiveColor;

                textColor *= Widgets.InactiveColor;
            }

            Widgets.DrawTexturePart(_leftRect, _leftUVRect, TabAtlas);
            Widgets.DrawTexturePart(_midRect, _midUVRect, TabAtlas);
            Widgets.DrawTexturePart(_rightRect, _rightUVRect, TabAtlas);

            if (!_selected)
            {
                Widgets.DrawTexturePart(_bottomRect, _bottomUVRect, TabAtlas);
            }

            GUI.color = textColor;

            TextAnchor anchor = GameText.Anchor;
            GameText.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(labelRect, Text);

            GameText.Anchor = anchor;
            GUI.color = color;
        }

        /// <inheritdoc/>
        protected override Rect HitTestCore(Rect contentRect)
        {
            return contentRect.IntersectWith(new Rect(
                RenderRect.x + TabControl.IntersectedWidth,
                RenderRect.y,
                RenderSize.Width - 2f * TabControl.IntersectedWidth,
                RenderSize.Height));
        }

        /// <inheritdoc/>
        protected override void OnClick()
        {
            if (_container is null)
            {
                return;
            }

            if (!_selected)
            {
                _container.Select(this);

                ClickSound?.PlayOneShotOnCamera();
            }
        }

        #endregion
    }
}
