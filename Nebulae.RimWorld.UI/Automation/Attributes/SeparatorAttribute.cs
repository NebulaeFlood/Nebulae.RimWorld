using Nebulae.RimWorld.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Automation.Attributes
{
    /// <summary>
    /// 在指定成员前方或后方添加一个分割线，取决于特性的位置
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SeparatorAttribute : Attribute
    {
        /// <summary>
        /// 分割线的长度
        /// </summary>
        public float Length = 1f;

        /// <summary>
        /// 分割线的颜色
        /// </summary>
        public Color Stroke = ColorResources.LightGrey;

        /// <summary>
        /// 分割线的粗细
        /// </summary>
        public float Thickness = 1f;


        /// <summary>
        /// 初始化 <see cref="SeparatorAttribute"/> 的新实例
        /// </summary>
        public SeparatorAttribute() { }
    }
}
