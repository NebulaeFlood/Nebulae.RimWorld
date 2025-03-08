using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 所有控件的基类，定义了控件的共同特性
    /// </summary>
    public abstract partial class Control : Visual
    {
        //------------------------------------------------------
        //
        //  Events
        //
        //------------------------------------------------------

        #region Events

        #region Click

        internal readonly WeakEvent<Control, EventArgs> _click = new WeakEvent<Control, EventArgs>();

        /// <summary>
        /// 单击按钮时发生的弱事件
        /// </summary>
        public event Action<Control, EventArgs> Click
        {
            add => _click.Add(value, value.Invoke);
            remove => _click.Remove(value);
        }

        #endregion

        #endregion


        /// <summary>
        /// 提示框文字
        /// </summary>
        public TipSignal Tooltip
        {
            get => TooltipContent;
            set
            {
                TooltipContent = value;
                ShouldShowTooltip = IsEnabled 
                    && (!string.IsNullOrEmpty(value.text)
                        || value.textGetter != null);
            }
        }
    }
}
