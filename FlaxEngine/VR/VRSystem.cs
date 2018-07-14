using System;
using System.Collections.Generic;
using Valve.VR;

namespace FlaxEngine.VR
{
    /// <summary>
    /// OpenVR System
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class VRSystem : IDisposable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static VRSystem Instance { get; private set; }

        /// <summary>
        /// Initializes the <see cref="VRSystem"/> class.
        /// </summary>
        static VRSystem()
        {
            Debug.Log("[VR] System starting");
            Instance = new VRSystem();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VRSystem"/> class.
        /// </summary>
        public VRSystem()
        {
            // ShowHMDMirrorTexture = false;

            // Setup arrays
            renderPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            gamePoses = new TrackedDevicePose_t[0];
            poses = new VRPose[OpenVR.k_unMaxTrackedDeviceCount];
            controllers = new List<VRControllerState>();
            trackingReferences = new List<VRTrackingReference>();
            trackingReferencesArray = trackingReferences.ToArray();
            controllersArray = controllers.ToArray();

            // Initialize
            InitOpenVR();
            InitRendering();
            Scripting.Update += Update;
            Scripting.Exit += Dispose;
        }

        /// <summary>
        /// Shutsdown <see cref="OpenVR"/>
        /// </summary>
        public void Dispose()
        {
            Scripting.Update -= Update;
            if (IsConnected)
            {
                Debug.Log("[VR] System disposing");
                OpenVR.Shutdown();
            }
        }
    }
}
