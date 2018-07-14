namespace FlaxEngine.VR
{
    /// <summary>
    /// Stores the VR element pose
    /// </summary>
    public struct VRPose
    {
        /// <summary>
        /// Gets the default pose
        /// </summary>
        /// <value>
        /// The default pose.
        /// </value>
        public static VRPose DefaultPose => new VRPose { Orientation = Quaternion.Identity };

        /// <summary>
        /// The eye position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The eye orientation
        /// </summary>
        public Quaternion Orientation;
    }
}