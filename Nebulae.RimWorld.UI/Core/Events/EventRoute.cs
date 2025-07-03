using System;

namespace Nebulae.RimWorld.UI.Core.Events
{
    internal sealed class EventRoute
    {
        static EventRoute()
        {
            for (int i = 0; i < 4; i++)
            {
                Routes[i] = new EventRoute();
            }
        }

        internal EventRoute()
        {
            _handlers = new IRoutedEventHandler[12];
            _count = 0;
        }


        //------------------------------------------------------
        //
        //  Internal Static Methods
        //
        //------------------------------------------------------

        #region Internal Static Methods

        internal static EventRoute Fetch()
        {
            if (_routeIndex < 4)
            {
                return Routes[_routeIndex++];
            }
            else
            {
                return new EventRoute();
            }
        }

        internal static void Return(EventRoute route)
        {
            if (_routeIndex > 0)
            {
                route.Clear();

                Routes[--_routeIndex] = route;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void Add(IRoutedEventHandler handlers)
        {
            if (_count == _handlers.Length)
            {
                Array.Resize(ref _handlers, _count + 8);
            }

            _handlers[_count] = handlers;
            _count++;
        }

        internal void Clear()
        {
            Array.Clear(_handlers, 0, _count);
            _count = 0;
        }

        internal void Route(object sender, RoutedEventArgs args)
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                _handlers[i].Invoke(sender, args);

                if (args.Handled)
                {
                    return;
                }
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Fields
        //
        //------------------------------------------------------

        #region Private Static Fields

        private static readonly EventRoute[] Routes = new EventRoute[4];

        private static int _routeIndex;

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private IRoutedEventHandler[] _handlers;
        private int _count;

        #endregion
    }
}
