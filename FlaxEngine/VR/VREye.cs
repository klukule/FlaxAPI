namespace FlaxEngine.VR
{
    /// <summary>
    /// Stores Eye properties
    /// </summary>
    public class VREye
    {
        /// <summary>
        /// Gets or sets the eye pose.
        /// </summary>
        /// <value>
        /// The pose.
        /// </value>
        public VRPose Pose { get; set; }

        /// <summary>
        /// Gets or sets the eye projection matrix.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        public Matrix Projection { get; set; }

        /// <summary>
        /// Gets or sets the eye texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public VREyeTexture Texture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VREye"/> class.
        /// </summary>
        public VREye()
        {
            Pose = VRPose.DefaultPose;
            Projection = Matrix.Identity;
        }
    }
}