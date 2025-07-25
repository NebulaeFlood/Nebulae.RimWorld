﻿using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Resources
{
    /// <summary>
    /// <see cref="TabItem"/> 的资源
    /// </summary>
    [StaticConstructorOnStartup]
    public static class TabItemResources
    {
        /// <summary>
        /// 标题背景
        /// </summary>
        public static readonly Texture2D HeaderAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TabAtlas");
    }
}
