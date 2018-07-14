using FlaxEngine.Rendering;

namespace FlaxEngine.VR
{
    /// <summary>
    /// Contains all the fields used by each eye.
    /// </summary>
    public class VREyeTexture
    {
        /// <summary>
        /// Eye viewport
        /// </summary>
        public Viewport Viewport;

        /// <summary>
        /// Eye RenderTarget
        /// </summary>
        public RenderTarget RenderTarget;

        /// <summary>
        /// Eye Camera Near clip
        /// </summary>
        public float NearPlane;

        /// <summary>
        /// Eye Camera Far clip
        /// </summary>
        public float FarPlane;

        /// <summary>
        /// Initializes a new instance of the <see cref="VREyeTexture"/> class.
        /// </summary>
        public VREyeTexture()
        {
            NearPlane = 0.1f;
            FarPlane = 1000f;
            Viewport = new Viewport(0f, 0f, 1f, 1f);
        }
    }
}