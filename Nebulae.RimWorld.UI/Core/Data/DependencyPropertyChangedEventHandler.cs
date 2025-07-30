using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Data
{
    /// <summary>
    /// 表示依赖属性的值更改后调用的方法
    /// </summary>
    /// <param name="d">属性值更改的依赖属性</param>
    /// <param name="e">有关属性更改的数据</param>
    public delegate void DependencyPropertyChangedEventHandler(DependencyObject d, DependencyPropertyChangedEventArgs e);
}
