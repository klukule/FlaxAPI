using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Valve.VR;

namespace FlaxEngine.VR
{
    /// <summary>
    /// OpenVR System
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class VRSystem
    {
        private TrackedDevicePose_t[] renderPoses;
        private VRPose[] poses;
        private TrackedDevicePose_t[] gamePoses;
        private List<VRTrackingReference> trackingReferences;
        private List<VRControllerState> controllers;
        private VRTrackingReference[] trackingReferencesArray;
        private VRControllerState[] controllersArray;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the HMD.
        /// </summary>
        /// <value>
        /// The HMD.
        /// </value>
        internal CVRSystem Hmd { get; private set; }

        /// <summary>
        /// Gets the overlay.
        /// </summary>
        /// <value>
        /// The overlay.
        /// </value>
        public CVROverlay Overlay => OpenVR.Overlay;

        /// <summary>
        /// Gets a value indicating whether OpenVR is initializing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if OpenVR is initializing; otherwise, <c>false</c>.
        /// </value>
        public bool VRInitializing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether OpenVR is calibrating.
        /// </summary>
        /// <value>
        ///   <c>true</c> if OpenVR is calibrating; otherwise, <c>false</c>.
        /// </value>
        public bool VRCalibrating { get; private set; }

        /// <summary>
        /// Gets a value indicating whether OpenVR is out of range.
        /// </summary>
        /// <value>
        ///   <c>true</c> if OpenVR is out of range; otherwise, <c>false</c>.
        /// </value>
        public bool VROutOfRange { get; private set; }

        /// <summary>
        /// Gets the eyes properties.
        /// </summary>
        /// <value>
        /// The eyes properties.
        /// </value>
        public VREye[] EyesProperties { get; private set; }

        /// <summary>
        /// Gets the tracking references.
        /// </summary>
        /// <value>
        /// The tracking references.
        /// </value>
        public VRTrackingReference[] TrackingReferences => trackingReferencesArray;

        /// <summary>
        /// Gets the controllers.
        /// </summary>
        /// <value>
        /// The controllers.
        /// </value>
        public VRControllerState[] Controllers => controllersArray;

        /// <summary>
        /// Gets the index of the left controller.
        /// </summary>
        /// <value>
        /// The index of the left controller.
        /// </value>
        public int LeftControllerIndex { get; private set; }

        /// <summary>
        /// Gets the index of the right controller.
        /// </summary>
        /// <value>
        /// The index of the right controller.
        /// </value>
        public int RightControllerIndex { get; private set; }

        /// <summary>
        /// Gets the left controller.
        /// </summary>
        /// <value>
        /// The left controller.
        /// </value>
        public VRControllerState LeftController
        {
            get
            {
                if (LeftControllerIndex >= 0 && controllersArray.Length > LeftControllerIndex)
                    return controllersArray[LeftControllerIndex];
                return null;
            }
        }

        /// <summary>
        /// Gets the right controller.
        /// </summary>
        /// <value>
        /// The right controller.
        /// </value>
        public VRControllerState RightController
        {
            get
            {
                if (RightControllerIndex >= 0 && controllersArray.Length > RightControllerIndex)
                {
                    return controllersArray[RightControllerIndex];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the tracker camera pose.
        /// </summary>
        /// <value>
        /// The tracker camera pose.
        /// </value>
        public VRPose TrackerCameraPose => TrackingReferences.Length != 0 ? TrackingReferences[0].Pose : default(VRPose);

        /// <summary>
        /// Gets the name of the tracking system.
        /// </summary>
        /// <value>
        /// The name of the tracking system.
        /// </value>
        public string TrackingSystemName => GetStringProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_TrackingSystemName_String);

        /// <summary>
        /// Gets the model number.
        /// </summary>
        /// <value>
        /// The model number.
        /// </value>
        public string ModelNumber => GetStringProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_ModelNumber_String);

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber => GetStringProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_SerialNumber_String);

        /// <summary>
        /// Gets the seconds from vsync to photons.
        /// </summary>
        /// <value>
        /// The seconds from vsync to photons.
        /// </value>
        public float SecondsFromVsyncToPhotons => GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);

        /// <summary>
        /// Gets the display frequency.
        /// </summary>
        /// <value>
        /// The display frequency.
        /// </value>
        public float DisplayFrequency => GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);

        /// <summary>
        /// Initializes the OpenVR subsystem.
        /// </summary>
        private void InitOpenVR()
        {
            Debug.Log("[VR] OpenVR init begin");

            // Initialize OpenVR
            EVRInitError error = EVRInitError.None;
            Hmd = OpenVR.Init(ref error);

            bool success = true;

            if (error != EVRInitError.None)
            {
                VRUtils.ReportInitError(error);
                success = false;
            }

            // Check if compositor is present

            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);

            if (error != EVRInitError.None)
            {
                VRUtils.ReportInitError(error);
                success = false;
            }

            // Check if overlay is present

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);

            if (error != EVRInitError.None)
            {
                VRUtils.ReportInitError(error);
                success = false;
            }

            // Shutdown on error
            if (!success)
                OpenVR.Shutdown();

            Debug.Log("[VR] OpenVR init end");
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (IsConnected && OpenVR.Compositor != null)
            {
                //TODO: Move somewhere else since this is a hack
                Time.DrawFPS = 90;
                Time.UpdateFPS = 90;

                // Get poses
                VRUtils.ReportCompositeError(OpenVR.Compositor.WaitGetPoses(renderPoses, gamePoses));

                // Update

                UpdateHMD();
                UpdateDevices();
            }
        }

        /// <summary>
        /// Updates the HMD.
        /// </summary>
        private void UpdateHMD()
        {
            CVRSystem hmd = Hmd;
            if (renderPoses.Length > 0)
            {
                ETrackingResult result = renderPoses[0].eTrackingResult;
                VRInitializing = result == ETrackingResult.Uninitialized;
                VRCalibrating = result == ETrackingResult.Calibrating_InProgress || result == ETrackingResult.Calibrating_OutOfRange;
                VROutOfRange = result == ETrackingResult.Running_OutOfRange || result == ETrackingResult.Calibrating_OutOfRange;

                if (result == ETrackingResult.Running_OK)
                {
                    // Get poses for each eye (left, right, center)

                    renderPoses[0].ToMatrix(out Matrix mCenter);
                    mCenter.ToVRPose(out VRPose center);
                    EyesProperties[2].Pose = center;

                    for (int i = 0; i < 2; i++)
                    {
                        VREye eye = EyesProperties[i];
                        VREyeTexture texture = eye.Texture;
                        hmd.GetProjectionMatrix((EVREye)i, texture.NearPlane, texture.FarPlane).ToMatrix(out Matrix projection);
                        projection.Invert();
                        eye.Projection = projection;

                        hmd.GetEyeToHeadTransform((EVREye)i).ToMatrix(out Matrix mOffset);
                        mOffset *= mCenter;
                        mOffset.ToVRPose(out VRPose pose);
                        eye.Pose = pose;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the devices.
        /// </summary>
        private void UpdateDevices()
        {
            // Cache HMD
            CVRSystem hmd = Hmd;

            // Get indexes of left and right controllers
            LeftControllerIndex = -1;
            RightControllerIndex = -1;
            int lhIndex = (int)hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            int rhIndex = (int)hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

            controllers.Clear();

            // Update all tracked devices
            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                TrackedDevicePose_t pose = renderPoses[i];

                // We are interested in valid and connected devices only
                if (pose.bDeviceIsConnected && pose.bPoseIsValid)
                {
                    pose.ToVRPose(out poses[i]);
                    ETrackedDeviceClass c = hmd.GetTrackedDeviceClass(i);

                    // Update controller
                    if (c == ETrackedDeviceClass.Controller)
                    {
                        VRControllerState_t state_t = default(VRControllerState_t);
                        if (hmd.GetControllerState(i, ref state_t, (uint)Marshal.SizeOf(state_t)))
                        {
                            VRControllerRole role;
                            if (i == lhIndex)
                            {
                                role = VRControllerRole.LeftHand;
                                LeftControllerIndex = controllers.Count;
                            }
                            else if (i == rhIndex)
                            {
                                role = VRControllerRole.RightHand;
                                RightControllerIndex = controllers.Count;
                            }
                            else
                            {
                                role = VRControllerRole.Undefined;
                            }

                            VRControllerState state = new VRControllerState();
                            state.Update(role, ref state_t, ref poses[i]);
                            controllers.Add(state);
                        }
                    }
                    // Update generic reference (base station etc...)
                    else if (c == ETrackedDeviceClass.TrackingReference)
                    {
                        VRTrackingReference reference = new VRTrackingReference();
                        reference.Update(poses[i]);
                        trackingReferences.Add(reference);
                    }
                }
            }
            // Convert to array
            trackingReferencesArray = trackingReferences.ToArray();
            controllersArray = controllers.ToArray();
        }

        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="deviceIndex">Index of the device.</param>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        private string GetStringProperty(uint deviceIndex, ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            CVRSystem hmd = Hmd;
            var capactiy = hmd.GetStringTrackedDeviceProperty(deviceIndex, prop, null, 0, ref error);
            if (capactiy > 1)
            {
                var result = new StringBuilder((int)capactiy);
                hmd.GetStringTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, result, capactiy, ref error);
                return result.ToString();
            }
            return (error != ETrackedPropertyError.TrackedProp_Success) ? error.ToString() : "<unknown>";
        }

        /// <summary>
        /// Gets the float property.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns></returns>
        private float GetFloatProperty(ETrackedDeviceProperty prop)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            return Hmd.GetFloatTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, prop, ref error);
        }
    }
}
