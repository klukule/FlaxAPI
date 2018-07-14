using FlaxEngine.Rendering;
using Valve.VR;

namespace FlaxEngine.VR
{
    /// <summary>
    /// OpenVR System
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class VRSystem
    {
        private VRTextureBounds_t leftEyeTextureBounds;
        private VRTextureBounds_t rightEyeTextureBounds;
        private Texture_t leftEyeTexture;
        private Texture_t rightEyeTexture;
        private RenderTarget leftEyeRenderTarget;
        private RenderTarget rightEyeRenderTarget;
        private SceneRenderTask leftEyeRenderTask;
        private SceneRenderTask rightEyeRenderTask;

        /// <summary>
        /// Gets the field of view used by VR cameras.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        /// <remarks>
        /// Temporary, should be replaced by Projection matrices
        /// </remarks>
        public float FieldOfView { get; private set; }

        /// <summary>
        /// Gets the eye textures.
        /// </summary>
        /// <value>
        /// The eye textures.
        /// </value>
        internal VREyeTexture[] EyeTextures { get; private set; }

        //NOTE: HMD mirorring nyi.
        /*/// <summary>
        /// Gets the HMD mirror render target.
        /// </summary>
        /// <value>
        /// The HMD mirror render target.
        /// </value>
        internal RenderTarget HMDMirrorRenderTarget { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show HMD mirror texture.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show HMD mirror texture]; otherwise, <c>false</c>.
        /// </value>
        internal bool ShowHMDMirrorTexture { get; set; }*/

        //NOTE: Temporary
        /// <summary>
        /// Gets the left render task.
        /// </summary>
        /// <value>
        /// The left render task.
        /// </value>
        public SceneRenderTask Left => leftEyeRenderTask;

        //NOTE: Temporary
        /// <summary>
        /// Gets the right render task.
        /// </summary>
        /// <value>
        /// The right render task.
        /// </value>
        public SceneRenderTask Right => rightEyeRenderTask;

        /// <summary>
        /// Initializes the rendering stuff.
        /// </summary>
        private void InitRendering()
        {
            Debug.Log("[VR] rendering init begin");

            // Get HMD display size
            uint w = 0;
            uint h = 0;
            Hmd.GetRecommendedRenderTargetSize(ref w, ref h);

            int width = (int)w;
            int height = (int)h;

            // Create RT for each eye
            //TODO: Combine two RTs to one with [w*2, h]

            leftEyeRenderTarget = RenderTarget.New();
            leftEyeRenderTarget.Init(PixelFormat.R8G8B8A8_UNorm, width, height);

            rightEyeRenderTarget = RenderTarget.New();
            rightEyeRenderTarget.Init(PixelFormat.R8G8B8A8_UNorm, width, height);

            // Create texture structs for OpenVR
            leftEyeTexture = new Texture_t
            {
                handle = leftEyeRenderTarget.NativePtr,
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.DirectX
            };

            rightEyeTexture = new Texture_t
            {
                handle = rightEyeRenderTarget.NativePtr,
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.DirectX
            };

            // Calculate bounds and FOV

            // bounds and FOV calculation could be replaced with uv [0,0] - [1,1] and custom projection matrix from eye.Projection
            float l_left = 0.0f, l_right = 0.0f, l_top = 0.0f, l_bottom = 0.0f;
            Hmd.GetProjectionRaw(EVREye.Eye_Left, ref l_left, ref l_right, ref l_top, ref l_bottom);

            float r_left = 0.0f, r_right = 0.0f, r_top = 0.0f, r_bottom = 0.0f;
            Hmd.GetProjectionRaw(EVREye.Eye_Right, ref r_left, ref r_right, ref r_top, ref r_bottom);

            var tanHalfFov = new Vector2(
            Mathf.Max(-l_left, l_right, -r_left, r_right),
            Mathf.Max(-l_top, l_bottom, -r_top, r_bottom));

            leftEyeTextureBounds = new VRTextureBounds_t
            {
                uMin = 0.5f + 0.5f * l_left / tanHalfFov.X,
                uMax = 0.5f + 0.5f * l_right / tanHalfFov.X,
                vMin = 0.5f - 0.5f * l_bottom / tanHalfFov.Y,
                vMax = 0.5f - 0.5f * l_top / tanHalfFov.Y
            };

            rightEyeTextureBounds = new VRTextureBounds_t
            {
                uMin = 0.5f + 0.5f * r_left / tanHalfFov.X,
                uMax = 0.5f + 0.5f * r_right / tanHalfFov.X,
                vMin = 0.5f - 0.5f * r_bottom / tanHalfFov.Y,
                vMax = 0.5f - 0.5f * r_top / tanHalfFov.Y
            };

            FieldOfView = 2.0f * Mathf.Atan(tanHalfFov.Y) * Mathf.Rad2Deg;

            ///////

            // Create Eye textures

            EyeTextures = new VREyeTexture[2];

            EyeTextures[0] = new VREyeTexture
            {
                Viewport = new Viewport(0, 0, width, height),
                RenderTarget = leftEyeRenderTarget
            };

            EyeTextures[1] = new VREyeTexture
            {
                Viewport = new Viewport(0, 0, width, height),
                RenderTarget = rightEyeRenderTarget
            };

            // HMDMirrorRenderTarget = leftEyeRenderTarget;

            // Create render tasks

            leftEyeRenderTask = RenderTask.Create<SceneRenderTask>();
            // Camera
            leftEyeRenderTask.Output = leftEyeRenderTarget;
            leftEyeRenderTask.End += (task, ctx) => { Submit(EVREye.Eye_Left, ref leftEyeTexture, ref leftEyeTextureBounds); };

            rightEyeRenderTask = RenderTask.Create<SceneRenderTask>();
            // Camera
            rightEyeRenderTask.Output = rightEyeRenderTarget;
            rightEyeRenderTask.End += (task, ctx) => { Submit(EVREye.Eye_Right, ref rightEyeTexture, ref rightEyeTextureBounds); };

            // Create eyes
            EyesProperties = new VREye[3];
            for (int i = 0; i < EyesProperties.Length; i++)
            {
                VREye eye = new VREye();
                if (i < EyeTextures.Length)
                    eye.Texture = EyeTextures[i];
                EyesProperties[i] = eye;
            }

            Debug.Log("[VR] rendering init end");

            IsConnected = true;
        }

        /// <summary>
        /// Submits the data for specified eye.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="bounds">The bounds.</param>
        private void Submit(EVREye eye, ref Texture_t texture, ref VRTextureBounds_t bounds)
        {
            VRUtils.ReportCompositeError(OpenVR.Compositor.Submit(eye, ref texture, ref bounds, EVRSubmitFlags.Submit_Default));
        }
    }
}
