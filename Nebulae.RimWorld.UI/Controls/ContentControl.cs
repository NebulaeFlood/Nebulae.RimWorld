using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 包含一段任意类型内容的控件
    /// </summary>
    public abstract class ContentControl : FrameworkControl
    {
        private object _content;

        /// <summary>
        /// 控件内容
        /// </summary>
        public object Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    if (value is Control control)
                    {
                        control.Container = this;
                    }
                    else if (_content is Control currentControl)
                    {
                        currentControl.Container = null;
                    }

                    _content = value;

                    OnContentChanged(_content);
                }
            }
        }


        /// <summary>
        /// 初始化 <see cref="ContentControl"/> 的新实例
        /// </summary>
        public ContentControl()
        {
        }


        /// <summary>
        /// 当 <see cref="Content"/> 发生变化时执行的方法
        /// </summary>
        /// <param name="content">新的控件内容</param>
        protected virtual void OnContentChanged(object content) { }
    }
}
