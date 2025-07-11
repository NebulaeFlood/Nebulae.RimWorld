using Nebulae.RimWorld.UI.Automation.Diagnostics.Views;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Automation.Diagnostics
{
    internal sealed class LogicalTreeDebugWindow : ControlWindow
    {
        public static readonly LogicalTreeDebugWindow Window = new LogicalTreeDebugWindow();


        private LogicalTreeDebugWindow()
        {
            draggable = true;
            doCloseX = true;
            drawShadow = false;
            resizeable = true;

            layer = WindowLayer.Super;

            Content = LogicalTreeView.View;
            InitialWidth = 300f;
            LayoutManager.DrawDebugButtons = false;
            Padding = 24f;
        }


        public void Show(Control control)
        {
            LogicalTreeView.View.Bind(control);
            Find.WindowStack.Add(this);
        }

        public override void Close(bool doCloseSound = true)
        {
            Find.WindowStack.TryRemove(this, doCloseSound);
            LogicalTreeView.View.Clear();
            MemberDebugWindow.Window.Close();
        }


        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(0f, 0f, InitialWidth, Verse.UI.screenHeight);
        }
    }
}
