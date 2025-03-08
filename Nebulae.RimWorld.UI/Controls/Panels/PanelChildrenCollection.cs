using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// <see cref="Panel"/> 的子控件的有序集合
    /// </summary>
    /// <remarks>此集合不允许重复控件。</remarks>
    public class PanelChildrenCollection : IEnumerable<Visual>
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private List<Visual> _children;
        private readonly Panel _owner;

        #endregion


        /// <summary>
        /// 获取集合中的控件数
        /// </summary>
        public int Count => _children.Count;


        /// <summary>
        /// 初始化 <see cref="PanelChildrenCollection"/> 的新实例
        /// </summary>
        /// <param name="panel">拥有此集合的 <see cref="Panel"/></param>
        public PanelChildrenCollection(Panel panel)
        {
            _owner = panel;
            _children = new List<Visual>();
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
        public void Add(Visual control)
        {
            if (control is null
                || _children.Contains(control))
            {
                return;
            }

            control.SetParent(_owner);
            _children.Add(control);
            _owner.InvalidateFilter();
        }

        /// <summary>
        /// 清除集合中的控件
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetParent(null);
            }

            _children.Clear();
            _children.TrimExcess();
            _owner.ClearInternal();
        }

        /// <summary>
        /// 确定集合是否包含特定控件
        /// </summary>
        /// <param name="control">要确认的控件</param>
        /// <returns>如果在集合中找到了 <paramref name="control"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(Visual control)
        {
            return _children.Contains(control);
        }

        /// <summary>
        /// 获取集合中满足指定条件的所有控件
        /// </summary>
        /// <param name="match">判断条件</param>
        /// <returns>集合中满足指定条件的所有控件。</returns>
        public List<Visual> FindAll(Predicate<Visual> match)
        {
            return _children.FindAll(match);
        }

        /// <summary>
        /// 将控件插入到集合中的指定控件之前
        /// </summary>
        /// <param name="control">要插入的控件</param>
        /// <param name="index">被挤开的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(Visual control, Visual index)
        {
            if (control is null || index is null)
            {
                return false;
            }

            int i = _children.IndexOf(index);

            if (i < 0)
            {
                return false;
            }

            control.SetParent(_owner);

            _children.Remove(control);
            _children.Insert(i, control);

            _owner.InvalidateFilter();

            return true;
        }

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器。</returns>
        public IEnumerator<Visual> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <param name="controls">要置入的控件</param>
        /// <remarks>将移除所有原有控件。</remarks>
        public void OverrideCollection(params Visual[] controls)
        {
            Clear();

            _children = controls.Where(x => x != null).Distinct().ToList();

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetParent(_owner);
            }

            _owner.InvalidateFilter();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <typeparam name="TOwner">拥有该集合的控件</typeparam>
        /// <param name="controls">要置入的控件</param>
        /// <returns>拥有该集合的控件。</returns>
        /// <remarks>将移除所有原有控件。</remarks>
        public TOwner OverrideCollection<TOwner>(params Visual[] controls) where TOwner : Panel
        {
            Clear();

            _children = controls.Where(x => x != null).Distinct().ToList();

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetParent(_owner);
            }

            _owner.InvalidateFilter();

            return (TOwner)_owner;
        }

        /// <summary>
        /// 从集合中删除指定控件
        /// </summary>
        /// <param name="control">要删除的控件</param>
        /// <returns>若删除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Visual control)
        {
            if (control is null)
            {
                return false;
            }

            if (_children.Remove(control))
            {
                control.SetParent(null);
                _owner.InvalidateFilter();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 将集合中的控件复制到新数组
        /// </summary>
        /// <returns>包含集合中的控件副本的数组。</returns>
        public Visual[] ToArray() => _children.ToArray();

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}
