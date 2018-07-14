namespace FlaxEngine.VR
{
    /// <summary>
    /// Specifies which VR mode is used.
    /// </summary>
    public enum VRMode
    {
        /// <summary>
        /// Render a camera attached to the central joint
        /// </summary>
        AttachedMode = 1,

        /// <summary>
        /// The cameras will render inside a HMD (Head Mounted Device
        /// </summary>
        HmdMode,

        /// <summary>
        /// Render all modes at the same time
        /// </summary>
        All
    }
}