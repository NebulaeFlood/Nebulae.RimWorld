using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// <see cref="Panel"/> 的子控件的有序集合
    /// </summary>
    /// <remarks>此集合不允许重复控件。</remarks>
    public class PanelChildrenCollection : IEnumerable<Control>
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private List<Control> _controls;
        private readonly Panel _owner;

        #endregion


        internal int Version = 0;


        //------------------------------------------------------
        //
        //  Public Propertise
        //
        //------------------------------------------------------

        #region Public Propertise

        /// <summary>
        /// 集合中控件的实际数目
        /// </summary>
        public int Count => _controls.Count;

        /// <summary>
        /// 集合是否为只读集合
        /// </summary>
        public bool IsReadOnly => false;

        #endregion


        /// <summary>
        /// 初始化 <see cref="PanelChildrenCollection"/> 的新实例
        /// </summary>
        /// <param name="panel">拥有此集合的 <see cref="Panel"/></param>
        public PanelChildrenCollection(Panel panel)
        {
            _owner = panel;
            _controls = new List<Control>();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将控件添加到集合的末尾
        /// </summary>
        /// <param name="control">要添加到集合的控件</param>
        public void Add(Control control)
        {
            if (control is null)
            {
                return;
            }

            _controls.Add(control);
            _controls.Distinct();

            control.SetContainer(_owner);

            _owner.InvalidateMeasure();

            Version++;
        }

        /// <summary>
        /// 清除集合中的控件
        /// </summary>
        public void Clear()
        {
            _controls.ForEach(x => x.RemoveContainer());

            _controls.Clear();
            _controls.TrimExcess();
            _owner.InvalidateMeasure();

            Version++;
        }

        /// <summary>
        /// 确定集合是否包含特定控件
        /// </summary>
        /// <param name="control">要确认的控件</param>
        /// <returns>如果在集合中找到了 <paramref name="control"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(Control control)
        {
            return _controls.Contains(control);
        }

        /// <summary>
        /// 对集合的每个控件执行指定操作
        /// </summary>
        /// <param name="action">对每个控件执行的操作</param>
        public void ForEach(Action<Control> action)
        {
            _controls.ForEach(action);
        }

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器</returns>
        public IEnumerator<Control> GetEnumerator()
        {
            return _controls.GetEnumerator();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <param name="controls">要置入的控件</param>
        /// <remarks>将移除所有原有控件。</remarks>
        public void OverrideCollection(params Control[] controls)
        {
            Clear();

            _controls = controls.Where(x => x != null).Distinct().ToList();
            _controls.ForEach(x => x.SetContainer(_owner));
            // _owner.InvalidateMeasure();
            Version++;
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <typeparam name="TOwner">拥有该集合的控件</typeparam>
        /// <param name="controls">要置入的控件</param>
        /// <returns>拥有该集合的控件</returns>
        /// <remarks>将移除所有原有控件。</remarks>
        public TOwner OverrideCollection<TOwner>(params Control[] controls) where TOwner : Panel
        {
            Clear();

            _controls = controls.Where(x => x != null).Distinct().ToList();
            _controls.ForEach(x => x.SetContainer(_owner));
            // _owner.InvalidateMeasure();
            Version++;

            return (TOwner)_owner;
        }

        /// <summary>
        /// 从集合中删除指定控件
        /// </summary>
        /// <param name="control">要删除的控件</param>
        /// <returns>若删除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control control)
        {
            if (_controls.Remove(control))
            {
                control.RemoveContainer();
                _owner.InvalidateMeasure();
                Version++;
                return true;
            }
            return false;
        }

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _controls.GetEnumerator();
        }
    }
}
