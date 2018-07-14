namespace FlaxEngine.VR
{
    /// <summary>
    /// OpenVR Tracking reference
    /// </summary>
    public class VRTrackingReference
    {
        /// <summary>
        /// Gets or sets the pose of the controller.
        /// </summary>
        /// <value>
        /// The pose.
        /// </value>
        public VRPose Pose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this pose is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this pose is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Updates the specified pose.
        /// </summary>
        /// <param name="pose">The pose.</param>
        internal void Update(VRPose pose)
        {
            IsConnected = true;
            Pose = pose;
        }
    }
}
