using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// <see cref="Control"/> 对象的有序集合
    /// </summary>
    public class ControlCollection : IList<Control>
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private List<Control> _controls;
        private readonly Control _owner;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 集合中控件的实际数目
        /// </summary>
        public int Count => _controls.Count;

        /// <summary>
        /// 拥有此集合的控件
        /// </summary>
        public Control Owner => _owner;

        /// <summary>
        /// 获取或设置指定索引处的控件
        /// </summary>
        /// <param name="index">控件的从零开始的索引</param>
        /// <returns>指定索引处的控件</returns>
        /// <exception cref="ArgumentException"></exception>
        public Control this[int index]
        {
            get => _controls[index];
            set
            {
                _controls[index] = value;
                _owner.InvalidateArrange();
            }
        }

        /// <summary>
        /// 获取指定名称的控件
        /// </summary>
        /// <param name="name">控件的名称</param>
        /// <returns>若存在名为 <paramref name="name"/> 的控件，返回指定控件；反之则返回 null。</returns>
        public Control this[string name]
        {
            get => name != null ? _controls.Find(x => x.Name == name) : null;
        }

        /// <summary>
        /// 集合是否为只读集合
        /// </summary>
        public bool IsReadOnly => false;

        #endregion


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="ControlCollection"/> 的新实例
        /// </summary>
        /// <param name="owner">拥有此集合的控件</param>
        public ControlCollection(Control owner)
        {
            this._owner = owner;
            _controls = new List<Control>();
        }

        /// <summary>
        /// 初始化 <see cref="ControlCollection"/> 的新实例
        /// </summary>
        /// <param name="owner">拥有此集合的控件</param>
        /// <param name="controlCount">集合预计存储的控件数</param>
        public ControlCollection(Control owner, int controlCount)
        {
            this._owner = owner;
            _controls = new List<Control>(controlCount);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将控件添加到集合 
        /// </summary>
        /// <param name="control">要添加到集合的控件</param>
        public void Add(Control control)
        {
            _controls.Add(control);
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 清除集合中的控件
        /// </summary>
        public void Clear()
        {
            _controls.Clear();
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 确定集合是否包含特定控件
        /// </summary>
        /// <param name="control">要确认的控件</param>
        /// <returns>如果在集合中找到了 <paramref name="control"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(Control control) => _controls.Contains(control);

        /// <summary>
        /// 从特定索引开始，将集合的控件复制到 <paramref name="array"/>
        /// </summary>
        /// <param name="array">集合的控件将被复制到的目标</param>
        /// <param name="arrayIndex"><paramref name="array"/> 开始存放复制控件的索引</param>
        public void CopyTo(Control[] array, int arrayIndex) => _controls.CopyTo(array, arrayIndex);

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的控件
        /// </summary>
        /// <param name="match">要搜索的控件的条件</param>
        /// <returns>第一个匹配的控件</returns>
        public Control Find(Predicate<Control> match) => _controls.Find(match);

        /// <summary>
        /// 对集合的每个控件执行指定操作
        /// </summary>
        /// <param name="action">对每个控件执行的操作</param>
        public void ForEach(Action<Control> action) => _controls.ForEach(action);

        /// <summary>
        /// 搜索与指定谓词所定义的条件相匹配的所有控件
        /// </summary>
        /// <param name="match">要搜索的控件的条件</param>
        /// <returns>所有匹配的控件</returns>
        public List<Control> FindAll(Predicate<Control> match) => _controls.FindAll(match);

        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器</returns>
        public IEnumerator<Control> GetEnumerator() => _controls.GetEnumerator();

        /// <summary>
        /// 获取集合中特定控件的索引
        /// </summary>
        /// <param name="control">要获取索引的控件</param>
        /// <returns>特定控件的索引</returns>
        public int IndexOf(Control control) => _controls.IndexOf(control);

        /// <summary>
        /// 将控件插入到集合的指定索引处
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <param name="control">要插入的控件</param>
        public void Insert(int index, Control control)
        {
            _controls.Insert(index, control);
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <param name="controls">要置入的控件</param>
        /// <remarks>将移除所有原有控件。</remarks>
        public void OverrideCollection(params Control[] controls)
        {
            this._controls = controls.ToList();
            _owner.InvalidateMeasure();
        }

        /// <summary>
        /// 重新设置集合中的控件
        /// </summary>
        /// <typeparam name="TOwner">拥有该集合的控件</typeparam>
        /// <param name="controls">要置入的控件</param>
        /// <returns>拥有该集合的控件</returns>
        /// <remarks>将移除所有原有控件。</remarks>
        public TOwner OverrideCollection<TOwner>(params Control[] controls) where TOwner : Control
        {
            this._controls = controls.ToList();
            _owner.InvalidateMeasure();
            return (TOwner)_owner;
        }

        /// <summary>
        /// 从集合中删除指定控件
        /// </summary>
        /// <param name="control">要删除的控件</param>
        /// <returns>若删除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        /// <remarks>只移除第一个匹配项。</remarks>
        public bool Remove(Control control)
        {
            if (_controls.Remove(control))
            {
                _owner.InvalidateMeasure();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从集合中删除指定控件
        /// </summary>
        /// <param name="name">要删除的控件的名称</param>
        /// <returns>若删除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        /// <remarks>只移除第一个匹配项。</remarks>
        public bool Remove(string name)
        {
            if (this[name] is Control control && Remove(control))
            {
                _owner.InvalidateMeasure();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除指定索引处的控件
        /// </summary>
        /// <param name="index">指定索引</param>
        public void RemoveAt(int index)
        {
            if (index.IsInRange(0, _controls.Count - 1) && _controls[index] is Control)
            {
                RemoveAt(index);
                _owner.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 清空集合并设置容量
        /// </summary>
        /// <param name="capcity">新的容量</param>
        public void Resize(int capcity)
        {
            _controls.Clear();
            _controls.Capacity = capcity;
        }

        #endregion


        /// <summary>
        /// 获取循环访问集合的枚举器
        /// </summary>
        /// <returns>循环访问集合的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_controls).GetEnumerator();
    }
}
