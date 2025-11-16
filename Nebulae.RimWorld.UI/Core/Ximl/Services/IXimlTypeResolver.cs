using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Ximl.Services
{
    /// <summary>
    /// 定义 XIML 类型解析器
    /// </summary>
    public interface IXimlTypeResolver
    {
        /// <summary>
        /// 将 XIML 类型名解析为 CLR 类型
        /// </summary>
        /// <param name="qualifiedTypeName">XIML 类型名</param>
        /// <returns>若解析成功，返回 <see langword="true"/>；反正则返回 <see langword="false"/>。</returns>
        Type Resolve(string qualifiedTypeName);

        /// <summary>
        /// 尝试将 XIML 类型名解析为 CLR 类型
        /// </summary>
        /// <param name="qualifiedTypeName">XIML 类型名</param>
        /// <param name="type">解析到的 CLR 类型</param>
        /// <returns>若解析成功，返回 <see langword="true"/>；反正则返回 <see langword="false"/>。</returns>
        bool TryResolve(string qualifiedTypeName, out Type type);
    }
}
