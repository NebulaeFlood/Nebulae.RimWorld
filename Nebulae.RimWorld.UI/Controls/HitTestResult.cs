using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using System.Diagnostics;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="Control.HitTestCore(Vector2)"/> 的返回结果
    /// </summary>
    [DebuggerStepThrough]
    public readonly struct HitTestResult
    {
        /// <summary>
        /// <see cref="HitTestResult"/> 的空实例，表示没有命中任何 <see cref="Basic.Control"/>
        /// </summary>
        public static readonly HitTestResult Empty = new HitTestResult(null, false);


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// <see cref="Basic.Control"/> 被命中的实例
        /// </summary>
        public readonly Control Control;

        /// <summary>
        /// <see cref="Basic.Control"/> 是否被命中
        /// </summary>
        public readonly bool IsHit;

        #endregion


        /// <summary>
        /// 初始化 <see cref="HitTestResult"/> 的新实例
        /// </summary>
        /// <param name="hitControl"><see cref="Basic.Control"/> 被命中的实例</param>
        /// <param name="isHit"><see cref="Basic.Control"/> 是否被命中</param>
        private HitTestResult(Control hitControl, bool isHit)
        {
            Control = hitControl;
            IsHit = isHit;
        }


        /// <summary>
        /// 使用 <see cref="Control.ControlRect"/> 和 <paramref name="hitPoint"/> 判断是否命中指定的 <see cref="Basic.Control"/>
        /// </summary>
        /// <param name="control">要判断是否被命中的 <see cref="Basic.Control"/></param>
        /// <param name="hitPoint">进行命中测试的点</param>
        /// <returns>命中测试结果。</returns>
        public static HitTestResult HitTest(Control control, Vector2 hitPoint)
        {
            if (!control.ControlRect.Contains(hitPoint))
            {
                return Empty;
            }

            if (HitTestUtility.inputHitTest && control.CanHitTest())
            {
                HitTestUtility.Results.Add(control);
            }

            return new HitTestResult(control, true);
        }

        /// <summary>
        /// 使用自定义逻辑命中测试指定的 <see cref="Basic.Control"/>
        /// </summary>
        /// <param name="control">要判断是否被命中的 <see cref="Basic.Control"/></param>
        /// <param name="isHit"><paramref name="control"/> 是否被命中</param>
        /// <returns>命中测试结果。</returns>
        public static HitTestResult HitTest(Control control, bool isHit)
        {
            if (!isHit)
            {
                return Empty;
            }

            if (HitTestUtility.inputHitTest && control.CanHitTest())
            {
                HitTestUtility.Results.Add(control);
            }

            return new HitTestResult(control, true);
        }
    }
}
