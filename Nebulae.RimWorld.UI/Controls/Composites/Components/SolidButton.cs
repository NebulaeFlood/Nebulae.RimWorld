using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Composites.Components
{
    internal sealed class SolidButton : Control
    {
        internal event RoutedEventHandler Click
        {
            add { AddHandler(ButtonBase.ClickEvent, value); }
            remove { RemoveHandler(ButtonBase.ClickEvent, value); }
        }


        internal Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }


        internal SolidButton()
        {
            IsHitTestVisible = true;
        }


        protected override void DrawCore(ControlState states)
        {
            if (states.HasState(ControlState.Disabled))
            {
                GUI.color *= Widgets.InactiveColor;
            }
            else if (states.HasState(ControlState.CursorDirectlyOver | ControlState.Pressing))
            {
                GUI.color *= GenUI.MouseoverColor;
            }

            GUI.DrawTexture(DesiredRect, _icon, ScaleMode.ScaleToFit);
        }


        private Texture2D _icon;
    }
}
