using System;

namespace Nebulae.RimWorld.UI.Automation
{
    /// <summary>
    /// 标记设置条目拥有提示框
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class ContainsTooltipAttribute : Attribute
    {
    }
}
