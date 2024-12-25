using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Data.Binding
{
    /// <summary>
    /// 定义一个表示对象指定成员间绑定关系的类型
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// 关系是否可用
        /// </summary>
        bool IsBindingValid { get; }

        /// <summary>
        /// 绑定源的类型
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        /// 绑定目标的类型
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// 取消对象成员间的绑定关系
        /// </summary>
        void Unbind();

        /// <summary>
        /// 强制以当前绑定模式同步绑定对象的值
        /// </summary>
        void Synchronize();
    }
}
