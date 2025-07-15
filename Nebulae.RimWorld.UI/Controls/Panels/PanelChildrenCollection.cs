using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nebulae.RimWorld.UI.Controls.Panels
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

        private readonly List<Control> _children;
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
            _children = new List<Control>();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将控件添加或设置到集合的末尾
        /// </summary>
        /// <param name="control">要添加到集合的控件</param>
        public void Add(Control control)
        {
            if (control is null)
            {
                return;
            }

            int i = _children.IndexOf(control);

            if (i < 0)
            {
                control.Parent = _owner;
                _children.Add(control);
            }
            else
            {
                _children.RemoveAt(i);
                _children.Add(control);
            }

            _owner.InvalidateFilter();
        }

        /// <summary>
        /// 清除集合中的控件
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Parent = null;
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
        public bool Contains(Control control)
        {
            return _children.Contains(control);
        }

        /// <summary>
        /// 获取集合中满足指定条件的所有控件
        /// </summary>
        /// <param name="match">判断条件</param>
        /// <returns>集合中满足指定条件的所有控件。</returns>
        public List<Control> FindAll(Predicate<Control> match)
        {
            return _children.FindAll(match);
        }

        /// <summary>
        /// 获取集合中指定控件的后一个控件
        /// </summary>
        /// <param name="control">作为索引的控件</param>
        /// <returns>指定控件的后一个控件。</returns>
        public Control FindNext(Control control)
        {
            if (_children.Count < 1)
            {
                return null;
            }

            int i = _children.IndexOf(control);
            int last = _children.Count - 1;

            if (i < 0)
            {
                return _children[last];
            }
            else if (i < last)
            {
                return _children[i + 1];
            }
            else
            {
                return _children[last];
            }
        }

        /// <summary>
        /// 获取集合中指定控件的前一个控件
        /// </summary>
        /// <param name="control">作为索引的控件</param>
        /// <returns>指定控件的前一个控件。</returns>
        public Control FindPrevious(Control control)
        {
            if (_children.Count < 1)
            {
                return null;
            }

            int i = _children.IndexOf(control);

            if (i <= 0)
            {
                return _children[0];
            }
            else
            {
                return _children[i - 1];
            }
        }

        /// <summary>
        /// 对集合中的每个控件执行指定操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public void ForEach(Action<Control> action)
        {
            _children.ForEach(action);
        }

        /// <summary>
        /// 将控件插入到集合中的指定控件之前
        /// </summary>
        /// <param name="index">作为索引的控件</param>
        /// <param name="control">要插入的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(Control index, Control control)
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

            int j = _children.IndexOf(control);

            if (j < 0)
            {
                control.Parent = _owner;
                _children.Insert(i, control);
            }
            else
            {
                _children.RemoveAt(j);

                if (j < i)
                {
                    _children.Insert(i - 1, control);
                }
                else
                {
                    _children.Insert(i, control);
                }
            }

            _owner.InvalidateFilter();

            return true;
        }

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器。</returns>
        public IEnumerator<Control> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <param name="controls">要置入的控件，应保证所有控件不重复且不为 <see langword="null"/></param>
        /// <remarks>将移除所有原有控件。</remarks>
        public void OverrideCollection(IEnumerable<Control> controls)
        {
            if (controls is null)
            {
                throw new ArgumentNullException(nameof(controls));
            }

            Clear();

            _children.AddRange(controls);

            for (int i = _children.Count - 1; i >= 0; i--)
            {
                _children[i].Parent = _owner;
            }

            _owner.InvalidateFilter();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <typeparam name="TOwner">拥有该集合的控件</typeparam>
        /// <param name="controls">要置入的控件，应保证所有控件不重复且不为 <see langword="null"/></param>
        /// <returns>拥有该集合的控件。</returns>
        /// <remarks>将移除所有原有控件。</remarks>
        public TOwner OverrideCollection<TOwner>(IEnumerable<Control> controls) where TOwner : Panel
        {
            OverrideCollection(controls);

            return (TOwner)_owner;
        }

        /// <summary>
        /// 从集合中删除指定控件
        /// </summary>
        /// <param name="control">要删除的控件</param>
        /// <returns>若删除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control control)
        {
            if (control is null)
            {
                return false;
            }

            if (_children.Remove(control))
            {
                control.Parent = null;
                _owner.InvalidateFilter();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 使用指定的比较器排序子控件
        /// </summary>
        /// <param name="comparison">比较子控件时使用的比较器</param>
        public void Sort(Comparison<Control> comparison)
        {
            _children.Sort(comparison);
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 使用指定的比较器排序子控件
        /// </summary>
        /// <param name="comparer">比较子控件时使用的比较器</param>
        public void Sort(IComparer<Control> comparer)
        {
            _children.Sort(comparer);
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 将集合中的控件复制到新数组
        /// </summary>
        /// <returns>包含集合中的控件副本的数组。</returns>
        public Control[] ToArray() => _children.ToArray();

        /// <summary>
        /// 将控件插入到集合中的指定控件的前方或后方，取决于视觉效果
        /// </summary>
        /// <param name="index">作为索引的控件</param>
        /// <param name="control">要插入的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool VisualInsert(Control index, Control control)
        {
            if (control is null)
            {
                return false;
            }

            if (index is null)
            {
                _children.Add(control);
                _owner.InvalidateFilter();

                return true;
            }

            int i = _children.IndexOf(index);

            if (i < 0)
            {
                return false;
            }

            int j = _children.IndexOf(control);

            if (j < 0)
            {
                control.Parent = _owner;
            }
            else
            {
                _children.RemoveAt(j);
            }

            _children.Insert(i, control);
            _owner.InvalidateFilter();

            return true;
        }

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
