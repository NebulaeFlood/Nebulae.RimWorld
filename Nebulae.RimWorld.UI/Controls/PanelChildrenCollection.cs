using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Controls.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="Panel"/> 的子控件的有序集合
    /// </summary>
    /// <remarks>此集合不允许重复控件。</remarks>
    public sealed class PanelChildrenCollection : ControlCollection<Control>
    {
        /// <summary>
        /// 获取拥有该集合的 <see cref="Panel"/>
        /// </summary>
        public Panel Owner
        {
            get => _owner ?? throw new InvalidOperationException($"The collection has not been assigned to a panel.");
            internal set
            {
                if (_owner is null)
                {
                    _owner = value;
                }
                else
                {
                    throw new InvalidOperationException($"The collection already belongs to another panel of type '{_owner.GetType()}'.");
                }
            }
        }


        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="PanelChildrenCollection"/> 的新实例
        /// </summary>
        public PanelChildrenCollection() { }

        /// <summary>
        /// 初始化 <see cref="PanelChildrenCollection"/> 的新实例
        /// </summary>
        /// <param name="owner">该集合的 <see cref="Panel"/></param>
        public PanelChildrenCollection(Panel owner)
        {
            _owner = owner;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override void OnAddedControl(Control control)
        {
            Panel owner = Owner;
            control.Parent = owner;
        }

        /// <inheritdoc/>
        protected override void OnCollectionChanged()
        {
            Owner.InvalidateFilter();
        }

        /// <inheritdoc/>
        protected override void OnRemovedControl(Control control)
        {
            control.Parent = null;
        }

        #endregion


        private Panel _owner;
    }
}
