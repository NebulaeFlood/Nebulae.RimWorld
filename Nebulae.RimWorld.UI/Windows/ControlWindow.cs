using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Windows
{
    /// <summary>
    /// 使用 <see cref="Control"/> 进行内容呈现的窗口
    /// </summary>
    public class ControlWindow : Window
    {
        //------------------------------------------------------
        //
        //  Public Const
        //
        //------------------------------------------------------

        #region Public Const

        /// <summary>
        /// 默认的关闭按钮（X）要占用的高度
        /// </summary>
        public const float DefaultCloseButtonHeight = 26f;

        /// <summary>
        /// 默认的窗口高度
        /// </summary>
        public const float DefaultWindowHeight = 700f;

        /// <summary>
        /// 默认的窗口宽度
        /// </summary>
        public const float DefaultWindowWidth = 900f;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 窗口的客户区域
        /// </summary>
        public Rect ClientRect => _clientRect;

        /// <summary>
        /// 内容控件
        /// </summary>
        public Control Content
        {
            get => _layoutManager.Root;
            set => _layoutManager.Root = value;
        }

        /// <summary>
        /// 窗口初始的大小
        /// </summary>
        public sealed override Vector2 InitialSize => new Vector2(_initialWidth, _initialHeight);

        /// <summary>
        /// 窗口的初始高度
        /// </summary>
        public float InitialHeight
        {
            get => _initialHeight;
            set
            {
                if (_initialHeight != value)
                {
                    _initialHeight = value;

                    _layoutManager.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// 窗口控件布局管理器
        /// </summary>
        public LayoutManager LayoutManager => _layoutManager;

        /// <summary>
        /// 窗口的初始宽度
        /// </summary>
        public float InitialWidth
        {
            get => _initialWidth;
            set
            {
                if (_initialWidth != value)
                {
                    _initialWidth = value;

                    _layoutManager.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// 窗口的非客户区
        /// </summary>
        public Rect NonClientRect => _nonClientRect;


        /// <summary>
        /// 窗口内容边距
        /// </summary>
        public Thickness Padding
        {
            get => _padding;
            set => _padding = value;
        }

        #endregion


        /// <summary>
        /// 窗口内容边距
        /// </summary>
        protected override sealed float Margin => 0f;


#if DEBUG
        /// <summary>
        /// <see cref="ControlWindow"/> 的析构函数
        /// </summary>
        ~ControlWindow()
        {
            System.Diagnostics.Debug.WriteLine($"[NebulaeFlood's Lib] A window of type {GetType()} has been collected.");
        }
#endif

        /// <summary>
        /// 初始化 <see cref="ControlWindow"/> 的新实例
        /// </summary>
        public ControlWindow()
        {
            _layoutManager = new LayoutManager(this);
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly LayoutManager _layoutManager;

        private Rect _clientRect;
        private Rect _nonClientRect;

        private Thickness _padding = 18f;

        private float _initialHeight = DefaultWindowHeight;
        private float _initialWidth = DefaultWindowWidth;

        #endregion


        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        /// <param name="inRect">窗口允许绘制的区域</param>
        public sealed override void DoWindowContents(Rect inRect)
        {
            _nonClientRect = inRect;
            inRect -= _padding;

            if (doCloseX)
            {
                inRect.yMin += Mathf.Abs(_padding.Top - DefaultCloseButtonHeight);
            }

            if (doCloseButton)
            {
                inRect.height -= Mathf.Max(_padding.Bottom, FooterRowHeight);
            }

            if (_clientRect != inRect)
            {
                _layoutManager.InvalidateLayout();
                _clientRect = inRect;
            }

            _layoutManager.Draw(inRect);

            if (UIUtility.DebugDrawMode && _layoutManager.DrawDebugButtons)
            {
                _layoutManager.DrawWindowDebugButtons(_nonClientRect);
            }
        }
    }
}
