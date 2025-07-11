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
    internal sealed class MemberDebugWindow : ControlWindow
    {
        public static readonly MemberDebugWindow Window = new MemberDebugWindow();


        internal MemberDebugWindow()
        {
            draggable = true;
            doCloseX = true;
            drawShadow = false;
            resizeable = true;

            layer = WindowLayer.Super;

            Content = DebugMemberView.View;
            InitialWidth = 300f;
            LayoutManager.DrawDebugButtons = false;
            Padding = 24f;
        }


        public void Show(Control control)
        {
            DebugMemberView.View.Bind(control);
            Find.WindowStack.Add(this);
        }

        public override void Close(bool doCloseSound = true)
        {
            Find.WindowStack.TryRemove(this, doCloseSound);
            DebugMemberView.View.Clear();
        }


        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(
                Verse.UI.screenWidth - InitialWidth, 
                0f, 
                InitialWidth,
                Verse.UI.screenHeight);
        }
    }
}
