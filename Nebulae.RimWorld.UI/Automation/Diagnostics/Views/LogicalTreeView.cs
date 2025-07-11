using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Composites;
using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Automation.Diagnostics.Views
{
    internal sealed class LogicalTreeView : CompositeControl
    {
        public static readonly LogicalTreeView View = new LogicalTreeView();


        #region NodeControl
        public static WeakReference<Control> GetNodeControl(DependencyObject obj)
        {
            return (WeakReference<Control>)obj.GetValue(NodeControlProperty);
        }

        public static void SetNodeControl(DependencyObject obj, WeakReference<Control> value)
        {
            obj.SetValue(NodeControlProperty, value);
        }

        public static readonly DependencyProperty NodeControlProperty =
            DependencyProperty.RegisterAttached("NodeControl", typeof(WeakReference<Control>), typeof(LogicalTreeView),
                new PropertyMetadata(null));
        #endregion


        private LogicalTreeView()
        {
            _scrollViewer = new ScrollViewer();

            Initialize();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public void Bind(Control control)
        {
            Clear();

            _scrollViewer.Content = CreateTreeNode(control);
        }

        public void Clear()
        {
            if (_scrollViewer.IsEmpty)
            {
                return;
            }

            _scrollViewer.Content.Parent = null;
            _scrollViewer.Content = null;
        }

        #endregion


        protected override Control CreateContent()
        {
            _scrollViewer.Parent = null;

            return new Border
            {
                Background = BrushUtility.DarkGrey,
                BorderBrush = BrushUtility.Grey,
                BorderThickness = 1f,
                Content = _scrollViewer
            };
        }


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static Expander CreateTreeNode(Control control)
        {
            var node = new Expander
            {
                ClickSound = null,
                Header = control.ToString()
            };
            node.Click += OnNodeClick;

            if (control.LogicalChildren.Any())
            {
                var panel = new StackPanel();

                foreach (var child in control.LogicalChildren)
                {
                    panel.Append(CreateTreeNode(child));
                }

                node.Content = panel;
            }

            SetNodeControl(node, new WeakReference<Control>(control));
            return node;
        }

        static void OnNodeClick(object sender, RoutedEventArgs e)
        {
            if (GetNodeControl((DependencyObject)sender).TryGetTarget(out var target))
            {
                MemberDebugWindow.Window.Show(target);
            }
        }

        #endregion


        private readonly ScrollViewer _scrollViewer;
    }
}
