using RimWorld;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    [StaticConstructorOnStartup]
    public class Button : ButtonBase
    {
        /// <summary>
        /// 默认的光标位于按钮上方时的按钮背景
        /// </summary>
        public static readonly Texture2D DefaultMouseOverBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover");
        /// <summary>
        /// 默认的一般状态下的按钮背景
        /// </summary>
        public static readonly Texture2D DefaultNormalBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG");
        /// <summary>
        /// 默认的按钮被按下时的按钮背景
        /// </summary>
        public static readonly Texture2D DefaultPressedBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick");


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Size _cachedContentSize;
        private Rect _cachedIconRect;
        private Rect _cachedTextRect;
        private Color _compositionColor;
        private Texture2D _icon;
        private bool _isTextureAtlas;
        private Texture2D _mouseOverBackground;
        private Texture2D _normalBackground;
        private Texture2D _pressedBackground;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 按钮激活时，按钮材质的混合色
        /// </summary>
        /// <remarks>关闭按钮的混合色为<code>new Color(1f, 0.3f, 0.35f)</code></remarks>
        public Color CompositionColor
        {
            get => _compositionColor;
            set => _compositionColor = value;
        }

        /// <summary>
        /// 按钮图标
        /// </summary>
        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }

        /// <summary>
        /// 按钮材质是否为纹理图集（图像精灵）
        /// </summary>
        public bool IsTextureAtlas
        {
            get => _isTextureAtlas;
            set => _isTextureAtlas = value;
        }

        /// <summary>
        /// 光标位于按钮上方时的按钮背景
        /// </summary>
        public Texture2D MouseOverBackground
        {
            get => _mouseOverBackground;
            set => _mouseOverBackground = value;
        }

        /// <summary>
        /// 一般状态下的按钮背景
        /// </summary>
        public Texture2D NormalBackground
        {
            get => _normalBackground;
            set => _normalBackground = value;
        }

        /// <summary>
        /// 按钮被按下时的按钮背景
        /// </summary>
        public Texture2D PressedBackground
        {
            get => _pressedBackground;
            set => _pressedBackground = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="Button"/> 的新实例
        /// </summary>
        public Button()
        {
            _compositionColor = Color.white;
            _isTextureAtlas = true;
            _mouseOverBackground = DefaultMouseOverBackground;
            _normalBackground = DefaultNormalBackground;
            _pressedBackground = DefaultPressedBackground;

            ClickSound = SoundDefOf.Click;
        }

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect desiredRect = base.ArrangeCore(availableRect);
            Rect contentRect = _cachedContentSize.AlignRectToArea(desiredRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            _cachedIconRect = new Rect(
                contentRect.x, contentRect.y,
                _cachedContentSize.Height, _cachedContentSize.Height);
            _cachedTextRect = new Rect(
                contentRect.x + _cachedContentSize.Height, contentRect.y,
                _cachedContentSize.Width - _cachedContentSize.Height, _cachedContentSize.Height);
            return desiredRect;
        }

        /// <inheritdoc/>
        protected override Rect DrawButton(Rect renderRect)
        {
            Color currentColor = GUI.color;
            GUI.color = _compositionColor;

            Texture2D background = _normalBackground;
            if (Mouse.IsOver(renderRect))
            {
                background = _mouseOverBackground;
                if (Input.GetMouseButton(0))
                {
                    background = _pressedBackground;
                }
            }
            Widgets.DrawAtlas(renderRect, background);
            GUI.color = currentColor;

            if (_icon != null)
            {
                GUI.DrawTexture(_cachedIconRect, _icon, ScaleMode.ScaleToFit);
            }

            GameFont currentFont = GameText.Font;
            GameText.Font = FontSize;
            Widgets.Label(_cachedTextRect, Text);
            GameText.Font = currentFont;
            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size textSize = Text.CalculateLineSize(FontSize);
            _cachedContentSize = new Size(textSize.Width + textSize.Height, textSize.Height);
            return base.MeasureCore(availableSize);
        }
    }
}
