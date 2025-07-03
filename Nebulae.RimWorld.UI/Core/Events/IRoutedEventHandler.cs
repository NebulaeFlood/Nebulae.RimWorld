using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Core.Events
{
    internal interface IRoutedEventHandler : IWeakEventHandler<object, RoutedEventArgs> { }
}
