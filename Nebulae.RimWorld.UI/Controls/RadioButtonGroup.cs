using Nebulae.RimWorld.UI.Controls.Basic;
using System.Collections;
using System.Collections.Generic;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 表示按钮所在组的集合
    /// </summary>
    public class RadioButtonGroup : ICollection<RadioButton>
    {
        private readonly HashSet<RadioButton> _buttons;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 集合中按钮的实际数目
        /// </summary>
        public int Count => _buttons.Count;

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
        /// 初始化 <see cref="RadioButtonGroup"/> 的新实例
        /// </summary>
        public RadioButtonGroup()
        {
            _buttons = new HashSet<RadioButton>();
        }

        /// <summary>
        /// 初始化 <see cref="RadioButtonGroup"/> 的新实例
        /// </summary>
        /// <param name="buttonCount">组内预计的按钮数量</param>
        public RadioButtonGroup(int buttonCount)
        {
            _buttons = new HashSet<RadioButton>(buttonCount);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 将按钮添加到组
        /// </summary>
        /// <param name="button">要添加到组的按钮</param>
        public void Add(RadioButton button)
        {
            if (button.Status is ToggleStatus.Checked)
            {
                foreach (var btn in _buttons)
                {
                    btn.Status = ToggleStatus.Unchecked;
                }
            }
            _buttons.Add(button);
        }

        /// <summary>
        /// 清除组中的按钮
        /// </summary>
        public void Clear() => _buttons.Clear();

        /// <summary>
        /// 确定组中是否包含特定按钮
        /// </summary>
        /// <param name="button">要确认的按钮</param>
        /// <returns>如果在组中找到了 <paramref name="button"/>，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Contains(RadioButton button) => _buttons.Contains(button);

        /// <summary>
        /// 将组的内复制到指定数组
        /// </summary>
        /// <param name="array">要复制到的数组</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中复制开始处的索引</param>
        public void CopyTo(RadioButton[] array, int arrayIndex) => _buttons.CopyTo(array, arrayIndex);

        /// <summary>
        /// 获取循环访问组的枚举器
        /// </summary>
        /// <returns>循环访问组的枚举器</returns>
        public IEnumerator<RadioButton> GetEnumerator() => _buttons.GetEnumerator();

        /// <summary>
        /// 从组中删除指定按钮
        /// </summary>
        /// <param name="button">要删除的按钮</param>
        /// <returns>若删除了指定按钮，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(RadioButton button)
        {
            if (_buttons.Remove(button))
            {
                if (button.Status is ToggleStatus.Checked)
                {
                    foreach (var btn in _buttons)
                    {
                        btn.Status = ToggleStatus.Indeterminate;
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 获取循环访问组的枚举器
        /// </summary>
        /// <returns>获取循环访问组的枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_buttons).GetEnumerator();
    }
}
