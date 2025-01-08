using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using Verse;
using Grid = Nebulae.RimWorld.UI.Controls.Panels.Grid;
using TextBlock = Nebulae.RimWorld.UI.Controls.TextBlock;

namespace Nebulae.RimWorld.UI.Windows
{

    /// <summary>
    /// 消息窗口
    /// </summary>
    public class MessageWindow : ControlWindow
    {
        /// <inheritdoc/>
        public override Vector2 InitialSize => new Vector2(700f, 500f);


        /// <summary>
        /// 初始化 <see cref="MessageWindow"/> 的新实例
        /// </summary>
        /// <param name="message">要显示的文字</param>
        public MessageWindow(string message)
        {
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
            doCloseButton = false;
            doCloseX = false;
            forcePause = true;
            layer = WindowLayer.SubSuper;

            TextBlock textBlock = new TextBlock
            {
                Text = message,
            };

            ScrollViewer scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Content = new TextBlock { Text = message }
            };

            Button confirmButton = new Button
            {
                Margin = 4f,
                Text = "Confirm".Translate(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            confirmButton.Click += CloseWindow;

            Content = new Grid().SetSize(new float[] { 0.5f, 0.5f }, new float[] { Grid.Remain, 36f })
                .Set(new Control[]
                {
                    scrollViewer,   scrollViewer,
                    null,           confirmButton,
                });
        }
    }
}
