﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 可虚拟化子控件的面板控件的基类
    /// </summary>
    public abstract class VirtualizingPanel : Panel
    {
        /// <inheritdoc/>
        protected override bool IsDrawable(Control child)
        {
            return base.IsDrawable(child)
                && Segment().Overlaps(child.DesiredRect);
        }
    }
}
