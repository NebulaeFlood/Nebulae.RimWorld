#if RIMWORLD_1_7
namespace Nebulae.RimWorld.UI.Core.Events
{
    /// <summary>
    /// Mouse button values used in RimWorld 1.7, where Unity's UIElements MouseButton is unavailable.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>Left mouse button.</summary>
        LeftMouse = 0,
        /// <summary>Right mouse button.</summary>
        RightMouse = 1,
        /// <summary>Middle mouse button.</summary>
        MiddleMouse = 2
    }
}
#endif
