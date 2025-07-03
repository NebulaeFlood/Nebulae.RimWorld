using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 按钮控件的基类
    /// </summary>
    public abstract class ButtonBase : FrameworkControl
    {
        #region Click
        /// <summary>
        /// 当按钮被单击时发生
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// 标识 <see cref="Click"/> 路由事件
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
            RoutedEvent.Register(nameof(Click), RoutingStrategy.Direct, typeof(ButtonBase), typeof(RoutedEventArgs));
        #endregion


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region ClickSound
        /// <summary>
        /// 获取或设置单击按钮时播放的音效
        /// </summary>
        public SoundDef ClickSound
        {
            get { return (SoundDef)GetValue(ClickSoundProperty); }
            set { SetValue(ClickSoundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ClickSound"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ClickSoundProperty =
            DependencyProperty.Register(nameof(ClickSound), typeof(SoundDef), typeof(ButtonBase),
                new PropertyMetadata());
        #endregion

        #region CursorOverSound
        /// <summary>
        /// 获取或设置光标进入按钮范围时播放的音效
        /// </summary>
        public SoundDef CursorEnterSound
        {
            get { return (SoundDef)GetValue(CursorOverSoundProperty); }
            set { SetValue(CursorOverSoundProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="CursorEnterSound"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty CursorOverSoundProperty =
            DependencyProperty.Register(nameof(CursorEnterSound), typeof(SoundDef), typeof(ButtonBase),
                new PropertyMetadata());
        #endregion

        #endregion


        static ButtonBase()
        {
            ClickEvent.RegisterClassHandler(typeof(ButtonBase), new RoutedEventHandler(OnClickTrunk));
        }

        /// <summary>
        /// 为 <see cref="ButtonBase"/> 派生类实现基本初始化
        /// </summary>
        protected ButtonBase()
        {
            IsHitTestVisible = true;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// 当 <see cref="Click"/> 未被处理时调用
        /// </summary>
        /// <param name="e">事件数据</param>
        protected virtual void OnClick(RoutedEventArgs e)
        {
            ClickSound?.PlayOneShotOnCamera();
            e.Handled = true;
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(RoutedEventArgs e)
        {
            if (!ControlStates.HasState(ControlState.Disabled | ControlState.Dragging | ControlState.Pressing))
            {
                CursorEnterSound?.PlayOneShotOnCamera();
            }

            e.Handled = true;
        }

        #endregion


        private static void OnClickTrunk(object sender, RoutedEventArgs e)
        {
            if (sender is ButtonBase control)
            {
                control.OnClick(e);
            }
        }
    }
}
