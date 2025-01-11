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
    public class Button : IconButton
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

        private bool _isTextureAtlas = true;

        private Texture2D _mouseOverBackground = DefaultMouseOverBackground;
        private Texture2D _normalBackground = DefaultNormalBackground;
        private Texture2D _pressedBackground = DefaultPressedBackground;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

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
            ClickSound = SoundDefOf.Click;
        }


        /// <inheritdoc/>
        protected override void DrawBackground(Rect renderRect, bool isEnabled, bool isCursorOver)
        {
            Texture2D background = _normalBackground;
            if (isEnabled && isCursorOver)
            {
                background = _mouseOverBackground;

                if (Input.GetMouseButton(0))
                {
                    background = _pressedBackground;
                }
            }

            if (_isTextureAtlas)
            {
                Widgets.DrawAtlas(renderRect, background);
            }
            else
            {
                GUI.DrawTexture(renderRect, background);
            }
        }
    }
}
