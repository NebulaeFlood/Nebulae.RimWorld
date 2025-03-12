using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 控件次序信息
    /// </summary>
    public readonly struct ControlOrder
    {
        internal static readonly Comparison<ControlOrder> Comparison = Compare;

        /// <summary>
        /// 信息是否有效
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// 控件次序
        /// </summary>
        public readonly int Order;

        /// <summary>
        /// 控件
        /// </summary>
        public readonly Visual Control;


        /// <summary>
        /// 创建 <see cref="ControlOrder"/> 的新实例
        /// </summary>
        /// <param name="order">控件次序</param>
        /// <param name="control">控件</param>
        public ControlOrder(int order, Visual control)
        {
            IsValid = !(control is null);
            Order = order;
            Control = control;
        }

        internal static int Compare(ControlOrder x, ControlOrder y)
        {
            return x.Order > y.Order ? 1 : -1;
        }
    }
}
