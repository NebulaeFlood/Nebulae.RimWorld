using System;
using Verse;

namespace Nebulae.RimWorld.UI
{
    /// <summary>
    /// <see cref="NebulaeModSettings{T}.Saved"/> 的事件处理程序
    /// </summary>
    /// <typeparam name="T">Mod 设置类型</typeparam>
    /// <param name="sender">保存的设置</param>
    /// <param name="args">事件数据</param>
    public delegate void SettingsSavedEventHandler<T>(T sender, EventArgs args) where T : NebulaeModSettings<T>;


    /// <summary>
    /// NebulaeFlood's Lib 对 <see cref="ModSettings"/> 的基本实现
    /// </summary>
    public abstract class NebulaeModSettings<T> : ModSettings where T : NebulaeModSettings<T>
    {
        #region Saved
        private readonly WeakEvent<T, EventArgs> _savedEvent = new WeakEvent<T, EventArgs>();

        /// <summary>
        /// 当设置被保存时触发的事件
        /// </summary>
        public event SettingsSavedEventHandler<T> Saved
        {
            add { _savedEvent.AddHandler(value); }
            remove { _savedEvent.RemoveHandler(value); }
        }
        #endregion


        /// <summary>
        /// 向 <see cref="Scribe"/> 暴露数据
        /// </summary>
        public override void ExposeData()
        {
            if (Scribe.mode is LoadSaveMode.Saving)
            {
                _savedEvent.Invoke((T)this, EventArgs.Empty);
            }
        }
    }
}
