using RimWorld;
using RimWorld.Planet;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Verse;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

namespace Nebulae.RimWorld.Utilities
{
    //------------------------------------------------------
    //
    //  Global Targer Delegates
    //
    //------------------------------------------------------

    #region Global Targer Delegates

    /// <summary>
    /// 过滤世界地图中的位置以留下可选位置的方法
    /// </summary>
    /// <param name="target">要判断的世界地图中的位置</param>
    /// <returns>若 <paramref name="target"/> 是可选位置，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool GlobalTargetFilter(GlobalTargetInfo target);

    /// <summary>
    /// 选中世界地图中的位置时的回调函数
    /// </summary>
    /// <param name="target">选中的世界地图中的位置</param>
    public delegate void GlobalTargetSelectedCallback(GlobalTargetInfo target);

    /// <summary>
    /// 验证选取的世界地图位置是否可用的方法
    /// </summary>
    /// <param name="target">要验证的世界地图中的位置</param>
    /// <param name="message">当前位置对应的信息</param>
    /// <returns>若 <paramref name="target"/> 是可用位置，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool GlobalTargetValidator(GlobalTargetInfo target, out TaggedString message);

    #endregion


    //------------------------------------------------------
    //
    //  Local Target Delegates
    //
    //------------------------------------------------------

    #region Local Target Delegates

    /// <summary>
    /// 过滤当前地图中的位置以留下可选位置的方法
    /// </summary>
    /// <param name="target">要判断的当前地图中的位置</param>
    /// <returns>若 <paramref name="target"/> 是可选位置，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool LocalTargetFilter(LocalTargetInfo target);

    /// <summary>
    /// 验证选取的当前地图位置是否可用的方法
    /// </summary>
    /// <param name="target">要验证的当前地图中的位置</param>
    /// <param name="message">当前位置对应的信息</param>
    /// <returns>若 <paramref name="target"/> 是可用位置，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
    public delegate bool LocalTargetValidator(LocalTargetInfo target, out TaggedString message);

    /// <summary>
    /// 选取位置时绘制自定义 UI 的方法
    /// </summary>
    /// <param name="target">当前光标指向的位置</param>
    public delegate void LocalTargeterUIDrawer(LocalTargetInfo target);

    #endregion


    /// <summary>
    /// 选取地图或世界地图的位置的工具类
    /// </summary>
    public static class TargetUtility
    {
        /// <summary>
        /// 开始选取当前地图中的位置
        /// </summary>
        /// <param name="callback">成功选取位置时的回调函数</param>
        /// <param name="parameters">可被选取的位置类型</param>
        /// <param name="icon">在光标旁显示的图标</param>
        /// <param name="filter">过滤地图位置的方法</param>
        /// <param name="validator">验证位置是否可用的方法</param>
        /// <param name="drawer">选取位置时绘制自定义 UI 的方法</param>
        public static void TargetLocal(
            Action<LocalTargetInfo> callback,
            TargetingParameters parameters,
            Texture2D icon = null,
            LocalTargetFilter filter = null,
            LocalTargetValidator validator = null,
            LocalTargeterUIDrawer drawer = null)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var targetQuest = new LocalTargetQuset(icon, filter, validator, drawer);

            Find.Targeter.BeginTargeting(
                targetParams: parameters,
                action: callback,
                highlightAction: null,
                targetValidator: targetQuest.ValidateTarget,
                caster: null,
                actionWhenFinished: null,
                mouseAttachment: null,
                playSoundOnAction: true,
                onGuiAction: targetQuest.DrawUI,
                onUpdateAction: null);
        }

        /// <summary>
        /// 以当前地图作为起始视角，开始选取世界地图中的位置
        /// </summary>
        /// <param name="callback">成功选取位置时的回调函数</param>
        /// <param name="icon">在光标旁显示的图标</param>
        /// <param name="filter">过滤地图位置的方法</param>
        /// <param name="validator">验证位置是否可用的方法</param>
        /// <param name="drawer">选取位置时绘制自定义 UI 的方法</param>
        /// <param name="closeMap">结束选取时是否关闭世界地图</param>
        public static void TargetWorld(
            GlobalTargetSelectedCallback callback,
            Texture2D icon = null,
            GlobalTargetFilter filter = null,
            GlobalTargetValidator validator = null,
            Action drawer = null,
            bool closeMap = true)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var map = Find.CurrentMap;

            if (map is null)
            {
                TargetWorldInternal(callback, PlanetTile.Invalid, icon, filter, validator, drawer, closeMap);
            }
            else
            {
                TargetWorldInternal(callback, map.Tile, icon, filter, validator, drawer, closeMap);
            }
        }

        /// <summary>
        /// 以指定位置作为起始视角，开始选取世界地图中的位置
        /// </summary>
        /// <param name="callback">成功选取位置时的回调函数</param>
        /// <param name="startTile">作为起始视角的位置</param>
        /// <param name="icon">在光标旁显示的图标</param>
        /// <param name="filter">过滤地图位置的方法</param>
        /// <param name="validator">验证位置是否可用的方法</param>
        /// <param name="drawer">选取位置时绘制自定义 UI 的方法</param>
        /// <param name="closeMap">结束选取时是否关闭世界地图</param>
        public static void TargetWorld(
            GlobalTargetSelectedCallback callback,
            PlanetTile startTile,
            Texture2D icon = null,
            GlobalTargetFilter filter = null,
            GlobalTargetValidator validator = null,
            Action drawer = null,
            bool closeMap = false)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            TargetWorldInternal(callback, startTile, icon, filter, validator, drawer, closeMap);
        }


        private static void TargetWorldInternal(GlobalTargetSelectedCallback callback, PlanetTile startTile, Texture2D icon, GlobalTargetFilter filter, GlobalTargetValidator validator, Action drawer, bool closeMap)
        {
            var targetQuest = new WorldTargetQuest(callback, icon, filter, validator);

            Find.DesignatorManager.Deselect();

            if (startTile.Valid)
            {
                CameraJumper.TryJump(new GlobalTargetInfo(startTile));
                Find.WorldTargeter.BeginTargeting(
                    action: targetQuest.ValidateTarget,
                    canTargetTiles: true,
                    mouseAttachment: icon,
                    closeWorldTabWhenFinished: closeMap,
                    onUpdate: drawer,
                    extraLabelGetter: targetQuest.GetAttachedLabel,
                    canSelectTarget: targetQuest.FilterTarget,
                    originForClosest: startTile,
                    showCancelButton: true);
            }
            else
            {
                CameraJumper.TryShowWorld();
                Find.WorldTargeter.BeginTargeting(
                    action: targetQuest.ValidateTarget,
                    canTargetTiles: true,
                    mouseAttachment: icon,
                    closeWorldTabWhenFinished: closeMap,
                    onUpdate: drawer,
                    extraLabelGetter: targetQuest.GetAttachedLabel,
                    canSelectTarget: targetQuest.FilterTarget,
                    originForClosest: null,
                    showCancelButton: true);
            }
        }


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields



        #endregion


        private readonly struct WorldTargetQuest
        {
            public readonly GlobalTargetSelectedCallback Callback;
            public readonly Texture2D Icon;
            public readonly GlobalTargetFilter Filter;
            public readonly GlobalTargetValidator Validator;


            static WorldTargetQuest()
            {
                var type = typeof(WorldTargeter);
                var field = type.GetField("mouseAttachment", BindingFlags.Instance | BindingFlags.NonPublic);

                var targeterExp = Expression.Parameter(type, "targeter");
                var iconExp = Expression.Parameter(typeof(Texture2D), "icon");
                var fieldExp = Expression.Field(targeterExp, field);

                IconSetter = Expression.Lambda<Action<WorldTargeter, Texture2D>>(
                    Expression.Assign(fieldExp, iconExp), targeterExp, iconExp)
                    .Compile();
            }

            public WorldTargetQuest(GlobalTargetSelectedCallback callback, Texture2D icon, GlobalTargetFilter filter, GlobalTargetValidator validator)
            {
                Callback = callback;
                Icon = icon ?? TexCommand.Attack;
                Filter = filter;
                Validator = validator;
            }


            public bool FilterTarget(GlobalTargetInfo target)
            {
                return Filter?.Invoke(target) ?? true;
            }

            public TaggedString GetAttachedLabel(GlobalTargetInfo target)
            {
                if (Validator is null)
                {
                    IconSetter(Find.WorldTargeter, Icon);
                    return TaggedString.Empty;
                }
                else if (Validator(target, out var message))
                {
                    IconSetter(Find.WorldTargeter, Icon);
                    return message;
                }
                else
                {
                    IconSetter(Find.WorldTargeter, TexCommand.CannotShoot);
                    return message;
                }
            }

            public bool ValidateTarget(GlobalTargetInfo target)
            {
                var message = TaggedString.Empty;
                bool isValid = Validator?.Invoke(target, out message) ?? true;

                if (isValid)
                {
                    Callback(target);
                }
                else if (!message.NullOrEmpty())
                {
                    Messages.Message(message, MessageTypeDefOf.RejectInput, historical: false);
                }

                return isValid;
            }


            private static readonly Action<WorldTargeter, Texture2D> IconSetter;
        }

        private readonly struct LocalTargetQuset
        {
            public readonly Texture2D Icon;
            public readonly LocalTargetFilter Filter;
            public readonly LocalTargetValidator Validator;
            public readonly LocalTargeterUIDrawer Drawer;


            public LocalTargetQuset(Texture2D icon, LocalTargetFilter filter, LocalTargetValidator validator, LocalTargeterUIDrawer drawer)
            {
                Icon = icon ?? TexCommand.Attack;
                Filter = filter;
                Validator = validator;
                Drawer = drawer;
            }


            public void DrawUI(LocalTargetInfo target)
            {
                var message = TaggedString.Empty;
                bool isValid = Validator?.Invoke(target, out message) ?? true;

                if (isValid)
                {
                    GenUI.DrawMouseAttachment(Icon, message);
                }
                else
                {
                    GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                }

                Drawer?.Invoke(target);
            }

            public bool FilterTarget(LocalTargetInfo target)
            {
                return Filter?.Invoke(target) ?? true;
            }

            public bool ValidateTarget(LocalTargetInfo target)
            {
                var message = TaggedString.Empty;
                bool isValid = Validator?.Invoke(target, out message) ?? true;

                if (!isValid && !message.NullOrEmpty())
                {
                    Messages.Message(message, MessageTypeDefOf.RejectInput, historical: false);
                }

                return isValid;
            }
        }
    }
}
