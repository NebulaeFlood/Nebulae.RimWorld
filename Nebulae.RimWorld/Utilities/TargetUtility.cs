using RimWorld;
using RimWorld.Planet;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.Utilities
{
    /// <summary>
    /// 获取 <see cref="GlobalTargetInfo"/> 时判断当前对象是否可选中的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    /// <returns>当前对象是否可选中。</returns>
    public delegate bool GlobalTargetSelector(GlobalTargetInfo target);
    /// <summary>
    /// 获取 <see cref="LocalTargetInfo"/> 时验证目标的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    /// <returns>当前目标是否为可用目标。</returns>
    public delegate bool GlobalTargetValidator(GlobalTargetInfo target);

    /// <summary>
    /// 获取 <see cref="LocalTargetInfo"/> 时绘制 UI 的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    public delegate void LocalTargettingUIDrawer(LocalTargetInfo target);
    /// <summary>
    /// 获取 <see cref="LocalTargetInfo"/> 时绘制高亮的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    public delegate void LocalTargetHeighlighter(LocalTargetInfo target);
    /// <summary>
    /// 获取 <see cref="LocalTargetInfo"/> 时生成附加标签的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    /// <returns>光标旁显示的标签</returns>
    public delegate string LocalTargetMouseAttachedLabelGenerator(LocalTargetInfo target);
    /// <summary>
    /// 获取 <see cref="LocalTargetInfo"/> 时验证目标的委托
    /// </summary>
    /// <param name="target">当前的目标位置</param>
    /// <param name="showMessage">是否提示不可用的原因</param>
    /// <returns>当前目标是否为可用目标。</returns>
    public delegate bool LocalTargetValidator(LocalTargetInfo target, bool showMessage);


    /// <summary>
    /// 获取地图或世界地图的位置的工具类
    /// </summary>
    public static class TargetUtility
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly GlobalTargetSource _globalTargetSource = new GlobalTargetSource();
        private static GlobalTargetInfo _globalTargetCache = GlobalTargetInfo.Invalid;
        private static bool _globalTargetSelected;

        private static readonly LocalTargetingSource _localTargetingSource = new LocalTargetingSource();
        private static LocalTargetInfo _localTargetCache = LocalTargetInfo.Invalid;
        private static bool _localTargetSelected;

        #endregion


        //------------------------------------------------------
        //
        //  Get Global Target
        //
        //------------------------------------------------------

        #region Get Global Target

        /// <summary>
        /// 选取世界地图内的位置
        /// </summary>
        /// <param name="targetSelector">判断对象是否可选的委托</param>
        /// <param name="targetValidator">验证对象是否可用的委托</param>
        /// <param name="customUIDrawer">绘制选取位置时的 UI 的委托</param>
        /// <param name="attachedLabelGenerator">生成附加标签的委托</param>
        /// <param name="mouseAttachedIcon">附加图标</param>
        /// <param name="canTargetTiles">空地块（即没有任何基地等 <see cref="WorldObject"/>）是否可选中</param>
        /// <param name="quitWorldUIAtLast">结束选取后是否关闭世界地图</param>
        /// <returns>世界地图的指定位置。</returns>
        public static async Task<GlobalTargetInfo> GetGlobalTarget(
            GlobalTargetSelector targetSelector = null,
            GlobalTargetValidator targetValidator = null,
            Action customUIDrawer = null,
            Func<GlobalTargetInfo, string> attachedLabelGenerator = null,
            Texture2D mouseAttachedIcon = null,
            bool canTargetTiles = true,
            bool quitWorldUIAtLast = false)
        {
            _globalTargetSource.Initialize(targetSelector, targetValidator, null);

            Find.DesignatorManager.Deselect();
            CameraJumper.TryShowWorld();
            Find.WorldTargeter.BeginTargeting(
                _globalTargetSource.ValidateTarget,
                canTargetTiles,
                mouseAttachedIcon,
                quitWorldUIAtLast,
                customUIDrawer,
                attachedLabelGenerator,
                _globalTargetSource.IsTargetSelectable);

            await AwaitUtility.WaitForUnpauseAsync();

            if (!_globalTargetSelected)
            {
                _globalTargetSource.Reset();
                return GlobalTargetInfo.Invalid;
            }

            var target = _globalTargetCache;
            _globalTargetCache = GlobalTargetInfo.Invalid;
            _globalTargetSelected = false;

            return target;
        }

        /// <summary>
        /// 选取世界地图内的位置
        /// </summary>
        /// <param name="onSelected">结束选取后的执行的委托</param>
        /// <param name="targetSelector">判断对象是否可选的委托</param>
        /// <param name="targetValidator">验证对象是否可用的委托</param>
        /// <param name="customUIDrawer">绘制选取位置时的 UI 的委托</param>
        /// <param name="attachedLabelGenerator">生成附加标签的委托</param>
        /// <param name="mouseAttachedIcon">附加图标</param>
        /// <param name="canTargetTiles">空地块（即没有任何基地等 <see cref="WorldObject"/>）是否可选中</param>
        /// <param name="quitWorldUIAtLast">结束选取后是否关闭世界地图</param>
        /// <returns>世界地图的指定位置。</returns>
        public static void SelectGlobalTarget(
            Action<GlobalTargetInfo> onSelected,
            GlobalTargetSelector targetSelector = null,
            GlobalTargetValidator targetValidator = null,
            Action customUIDrawer = null,
            Func<GlobalTargetInfo, string> attachedLabelGenerator = null,
            Texture2D mouseAttachedIcon = null,
            bool canTargetTiles = true,
            bool quitWorldUIAtLast = false)
        {
            if (onSelected is null)
            {
                throw new ArgumentNullException(nameof(onSelected));
            }

            _globalTargetSource.Initialize(targetSelector, targetValidator, onSelected);

            Find.DesignatorManager.Deselect();
            CameraJumper.TryShowWorld();
            Find.WorldTargeter.BeginTargeting(
                _globalTargetSource.ValidateTarget,
                canTargetTiles,
                mouseAttachedIcon,
                quitWorldUIAtLast,
                customUIDrawer,
                attachedLabelGenerator,
                _globalTargetSource.IsTargetSelectable);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Get Local Target
        //
        //------------------------------------------------------

        #region Get Local Target

        /// <summary>
        /// 选取地图内的位置
        /// </summary>
        /// <param name="parameters">允许选择的对象类型</param>
        /// <param name="targetValidator">验证对象是否可用的委托</param>
        /// <param name="attachedLabelGenerator">生成附加标签的委托</param>
        /// <param name="customUIDrawer">绘制选取位置时的 UI 的委托</param>
        /// <param name="heighlighter">绘制选取位置时的高亮的委托</param>
        /// <param name="caster">要求选取位置的对象</param>
        /// <param name="mouseAttachedIcon">附加图标</param>
        /// <returns>地图内的指定位置。</returns>
        /// <exception cref="ArgumentNullException">当 <paramref name="parameters"/> 为 <see langword="null"/> 时发生。</exception>
        /// <remarks>右键取消时返回 <see cref="LocalTargetInfo.Invalid"/>。</remarks>
        public static async Task<LocalTargetInfo> GetLocalTarget(
            TargetingParameters parameters,
            LocalTargetValidator targetValidator = null,
            LocalTargetMouseAttachedLabelGenerator attachedLabelGenerator = null,
            LocalTargettingUIDrawer customUIDrawer = null,
            LocalTargetHeighlighter heighlighter = null,
            Pawn caster = null,
            Texture2D mouseAttachedIcon = null)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            _localTargetingSource.Initialize(
                customUIDrawer,
                heighlighter,
                attachedLabelGenerator,
                targetValidator,
                mouseAttachedIcon,
                null);

            Find.DesignatorManager.Deselect();
            Find.Targeter.BeginTargeting(
                parameters,
                _localTargetingSource.SelectTarget,
                _localTargetingSource.DrawHighlight,
                _localTargetingSource.ValidateTarget,
                caster,
                onGuiAction: _localTargetingSource.OnGUI);

            await AwaitUtility.WaitForUnpauseAsync();

            if (!_localTargetSelected)
            {
                _localTargetingSource.Reset();
                return LocalTargetInfo.Invalid;
            }

            var target = _localTargetCache;
            _localTargetCache = LocalTargetInfo.Invalid;
            _localTargetSelected = false;

            return target;
        }

        /// <summary>
        /// 选取地图内的位置
        /// </summary>
        /// <param name="parameters">允许选择的对象类型</param>
        /// <param name="onSelected">结束选取后的执行的委托</param>
        /// <param name="targetValidator">验证对象是否可用的委托</param>
        /// <param name="attachedLabelGenerator">生成附加标签的委托</param>
        /// <param name="customUIDrawer">绘制选取位置时的 UI 的委托</param>
        /// <param name="heighlighter">绘制选取位置时的高亮的委托</param>
        /// <param name="caster">要求选取位置的对象</param>
        /// <param name="mouseAttachedIcon">附加图标</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="parameters"/> 或 <paramref name="onSelected"/> 为 <see langword="null"/> 时发生。</exception>
        public static void SelectLocalTarget(
            TargetingParameters parameters,
            Action<LocalTargetInfo> onSelected,
            LocalTargetValidator targetValidator = null,
            LocalTargetMouseAttachedLabelGenerator attachedLabelGenerator = null,
            LocalTargettingUIDrawer customUIDrawer = null,
            LocalTargetHeighlighter heighlighter = null,
            Pawn caster = null,
            Texture2D mouseAttachedIcon = null)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (onSelected is null)
            {
                throw new ArgumentNullException(nameof(onSelected));
            }

            _localTargetingSource.Initialize(
                customUIDrawer,
                heighlighter,
                attachedLabelGenerator,
                targetValidator,
                mouseAttachedIcon,
                onSelected);

            Find.DesignatorManager.Deselect();
            Find.Targeter.BeginTargeting(
                parameters,
                _localTargetingSource.SelectTarget,
                _localTargetingSource.DrawHighlight,
                _localTargetingSource.ValidateTarget,
                caster,
                onGuiAction: _localTargetingSource.OnGUI);
        }

        #endregion


        internal static void NotifyTargetSelected(GlobalTargetInfo targetInfo)
        {
            _globalTargetCache = targetInfo;
            _globalTargetSource.Reset();
            _globalTargetSelected = true;
        }

        internal static void NotifyTargetSelected(LocalTargetInfo targetInfo)
        {
            _localTargetCache = targetInfo;
            _localTargetingSource.Reset();
            _localTargetSelected = true;
        }


        private sealed class GlobalTargetSource
        {
            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private Fields

            private GlobalTargetSelector _targetSelector;
            private GlobalTargetValidator _targetValidator;
            private Action<GlobalTargetInfo> _onSelected;

            #endregion


            //------------------------------------------------------
            //
            //  internal Methods
            //
            //------------------------------------------------------

            #region internal Methods

            internal void Initialize(
                GlobalTargetSelector targetSelector,
                GlobalTargetValidator targetValidator,
                Action<GlobalTargetInfo> onSelected)
            {
                _targetSelector = targetSelector;
                _targetValidator = targetValidator;
                _onSelected = onSelected;
            }

            internal bool IsTargetSelectable(GlobalTargetInfo target)
            {
                return _targetSelector is null || _targetSelector(target);
            }

            internal void Reset()
            {
                _targetSelector = null;
                _targetValidator = null;
            }

            internal bool ValidateTarget(GlobalTargetInfo target)
            {
                if (_targetValidator is null
                    || _targetValidator(target))
                {
                    if (_onSelected is null)
                    {
                        NotifyTargetSelected(target);
                    }
                    else
                    {
                        _onSelected(target);
                        Reset();
                    }

                    return true;
                }

                return false;
            }

            #endregion
        }

        private sealed class LocalTargetingSource
        {
            //------------------------------------------------------
            //
            //  Priate Fields
            //
            //------------------------------------------------------

            #region Priate Fields

            private Texture2D _mouseAttachedIcon;

            private LocalTargettingUIDrawer _targetUIDrawer;
            private LocalTargetHeighlighter _targetHeighlighter;
            private LocalTargetMouseAttachedLabelGenerator _targetLabelGenerator;
            private LocalTargetValidator _targetValidator;

            private Action<LocalTargetInfo> _onSelected;

            #endregion


            //------------------------------------------------------
            //
            //  Intetnal Methods
            //
            //------------------------------------------------------

            #region Intetnal Methods

            internal void DrawHighlight(LocalTargetInfo target)
            {
                if (_targetHeighlighter is null)
                {
                    if (target.IsValid)
                    {
                        GenDraw.DrawTargetHighlight(target);
                    }
                }
                else
                {
                    _targetHeighlighter(target);
                }
            }

            internal void Initialize(
                LocalTargettingUIDrawer targetUIDrawer,
                LocalTargetHeighlighter heighlighter,
                LocalTargetMouseAttachedLabelGenerator targetLabelGenerator,
                LocalTargetValidator targetValidator,
                Texture2D mouseAttachedIcon,
                Action<LocalTargetInfo> onSelected)
            {
                _targetUIDrawer = targetUIDrawer;
                _targetHeighlighter = heighlighter;
                _targetLabelGenerator = targetLabelGenerator;
                _targetValidator = targetValidator;
                _mouseAttachedIcon = mouseAttachedIcon;
                _onSelected = onSelected;
            }

            internal void OnGUI(LocalTargetInfo target)
            {
                if (_targetUIDrawer is null)
                {
                    string attachedText = _targetLabelGenerator?.Invoke(target);

                    if (_targetValidator is null || _targetValidator(target, false))
                    {
                        if (_mouseAttachedIcon != null)
                        {
                            GenUI.DrawMouseAttachment(_mouseAttachedIcon);
                        }

                        if (!string.IsNullOrEmpty(attachedText))
                        {
                            Widgets.MouseAttachedLabel(attachedText);
                        }
                    }
                    else
                    {
                        GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
                    }
                }
                else
                {
                    _targetUIDrawer(target);
                }
            }

            internal void Reset()
            {
                _targetUIDrawer = null;
                _targetHeighlighter = null;
                _targetLabelGenerator = null;
                _targetValidator = null;
                _mouseAttachedIcon = null;
                _onSelected = null;
            }

            internal void SelectTarget(LocalTargetInfo target)
            {
                if (_onSelected is null)
                {
                    NotifyTargetSelected(target);
                }
                else
                {
                    _onSelected(target);
                    Reset();
                }
            }

            internal bool ValidateTarget(LocalTargetInfo target)
            {
                return _targetValidator is null || _targetValidator(target, true);
            }

            #endregion
        }
    }
}
