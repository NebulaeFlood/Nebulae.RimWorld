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
        /// <summary>
        /// 破坏性按钮的默认混合色
        /// </summary>
        public static readonly Color DistructiveButtonComposition = new Color(1f, 0.3f, 0.35f);


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _isTextureAtlas = true;

        private Texture2D _hoveredBackground = DefaultMouseOverBackground;
        private Texture2D _normalBackground = DefaultNormalBackground;
        private Texture2D _pressedBackground = DefaultPressedBackground;

        private Color _compositionColor = Color.white;

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
        public Color CompositionColor
        {
            get => _compositionColor;
            set => _compositionColor = value;
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
            get => _hoveredBackground;
            set => _hoveredBackground = value;
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
            ClickSound = SoundDefOf.Click;
        }


        /// <inheritdoc/>
        protected override void DrawButton(ButtonStatus status)
        {
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            bool isDisabled = status.HasFlag(ButtonStatus.Disabled);

            if (isDisabled)
            {
                GUI.color = _compositionColor * Widgets.InactiveColor * color;
                GUI.contentColor = _compositionColor * Widgets.InactiveColor * contentColor;
            }

            Texture2D background;

            if (status is ButtonStatus.Pressed)
            {
                background = _pressedBackground;
            }
            else if (status is ButtonStatus.Hovered)
            {
                background = _hoveredBackground;
            }
            else
            {
                background = _normalBackground;
            }

            if (_isTextureAtlas)
            {
                Widgets.DrawAtlas(RenderRect, background);
            }
            else
            {
                GUI.DrawTexture(RenderRect, background);
            }

            string text = Text;

            if (!string.IsNullOrEmpty(text))
            {
                TextAnchor anchor = GameText.Anchor;
                GameFont font = GameText.Font;
                GameText.Anchor = TextAnchor.MiddleCenter;
                GameText.Font = FontSize;

                Widgets.Label(RenderRect, text);

                GameText.Font = font;
                GameText.Anchor = anchor;
            }

            GUI.color = color;
            GUI.contentColor = contentColor;
        }
    }
}
