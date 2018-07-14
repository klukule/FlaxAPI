using FlaxEngine;
using FlaxEngine.VR;
using System.Collections.Generic;

namespace UI
{
    //[ExecuteInEditMode]
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="FlaxEngine.Script" />
    public class VRCameraRig : Script
    {
        private static readonly string trackingSpaceName = "TrackingSpace";
        private static readonly string trackerAnchorName = "TrackerAnchor";
        private static readonly string eyeAnchorName = "EyeAnchor";
        private static readonly string leftControllerAnchorName = "LeftControllerAnchor";
        private static readonly string rightControllerAnchorName = "RightControllerAnchor";

        private VRMode vrMode;
        private bool isInitialized = false;
        private float nearPlane;
        private float farPlane;
        public bool Monoscopic { get; set; }

        private List<VRController> vrControllers = new List<VRController>();

        [HideInEditor]
        public VRController LeftController;

        [HideInEditor]
        public VRController RightController;

        public VRMode VRMode
        {
            get
            {
                return this.vrMode;
            }
            set
            {
                vrMode = value;
                if (isInitialized)
                {
                    RefreshVRMode();
                }
            }
        }

        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
                RefreshCameraProperties();
            }
        }

        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
                RefreshCameraProperties();
            }
        }

        public bool Disable { get; set; }

        // Cameras
        [HideInEditor]
        public Camera LeftEyeCamera { get; private set; }

        [HideInEditor]
        public Camera AttachedCamera { get; private set; }

        [HideInEditor]
        public Camera RightEyeCamera { get; private set; }

        // Anchors
        [HideInEditor]
        public EmptyActor TrackingSpace { get; private set; }

        [HideInEditor]
        public EmptyActor LeftEyeAnchor { get; private set; }

        [HideInEditor]
        public EmptyActor RightEyeAnchor { get; private set; }

        [HideInEditor]
        public EmptyActor CenterEyeAnchor { get; private set; }

        [HideInEditor]
        public EmptyActor TrackerAnchor { get; private set; }

        [HideInEditor]
        public EmptyActor LeftControllerAnchor { get; private set; }

        [HideInEditor]
        public EmptyActor RightControllerAnchor { get; private set; }

        private void Awake()
        {
            SetupHierarchy();
        }

        private void SetupHierarchy()
        {
            if (TrackingSpace == null)
            {
                TrackingSpace = ConfigureRootAnchor(trackingSpaceName);
            }

            if (LeftEyeAnchor == null)
            {
                LeftEyeAnchor = ConfigureEyeAnchor(TrackingSpace, VREyeType.LeftEye);
            }

            if (CenterEyeAnchor == null)
            {
                CenterEyeAnchor = ConfigureEyeAnchor(TrackingSpace, VREyeType.CenterEye);
            }

            if (RightEyeAnchor == null)
            {
                RightEyeAnchor = ConfigureEyeAnchor(TrackingSpace, VREyeType.RightEye);
            }

            if (TrackerAnchor == null)
            {
                TrackerAnchor = ConfigureTrackerAnchor(TrackingSpace);
            }

            if (LeftControllerAnchor == null)
            {
                LeftControllerAnchor = ConfigureControllerAnchor(TrackingSpace, VRControllerRole.LeftHand);
                LeftController = LeftControllerAnchor.GetScript<VRController>();
            }

            if (RightControllerAnchor == null)
            {
                RightControllerAnchor = ConfigureControllerAnchor(TrackingSpace, VRControllerRole.RightHand);
                RightController = RightControllerAnchor.GetScript<VRController>();
            }

            if (LeftEyeCamera == null || RightEyeCamera == null || AttachedCamera == null)
            {
                LeftEyeCamera = LeftEyeAnchor.GetChild<Camera>();
                if (LeftEyeCamera == null)
                {
                    LeftEyeCamera = Camera.New();
                    LeftEyeAnchor.AddChild(LeftEyeCamera);
                }

                RightEyeCamera = RightEyeAnchor.GetChild<Camera>();
                if (RightEyeCamera == null)
                {
                    RightEyeCamera = Camera.New();
                    RightEyeAnchor.AddChild(RightEyeCamera);
                }

                AttachedCamera = CenterEyeAnchor.GetChild<Camera>();
                if (AttachedCamera == null)
                {
                    AttachedCamera = Camera.New();
                    CenterEyeAnchor.AddChild(AttachedCamera);
                }
            }

            vrControllers = Actor.GetScriptsRecursive<VRController>();

            RefreshVRMode();
            RefreshCameraProperties();

            isInitialized = true;
        }

        private EmptyActor ConfigureRootAnchor(string name)
        {
            EmptyActor anchor = Actor.GetChild(name) as EmptyActor;
            if (anchor == null)
            {
                anchor = EmptyActor.New();
                Actor.AddChild(anchor);
                anchor.Name = name;
            }
            return anchor;
        }

        private EmptyActor ConfigureEyeAnchor(EmptyActor root, VREyeType eye)
        {
            string type = (eye == VREyeType.CenterEye) ? "Center" : ((eye == VREyeType.LeftEye) ? "Left" : "Right");
            string name = type + eyeAnchorName;
            EmptyActor anchor = Actor.GetChild(name) as EmptyActor;
            if (anchor == null)
            {
                anchor = EmptyActor.New();
                root.AddChild(anchor);
                anchor.Name = name;
            }
            return anchor;
        }

        private EmptyActor ConfigureTrackerAnchor(EmptyActor root)
        {
            EmptyActor anchor = Actor.GetChild(trackerAnchorName) as EmptyActor;
            if (anchor == null)
            {
                anchor = EmptyActor.New();
                root.AddChild(anchor);
                anchor.Name = trackerAnchorName;
            }
            return anchor;
        }

        private EmptyActor ConfigureControllerAnchor(EmptyActor root, VRControllerRole role)
        {
            string name = role == VRControllerRole.LeftHand ? leftControllerAnchorName : role == VRControllerRole.RightHand ? rightControllerAnchorName : string.Empty;

            EmptyActor anchor = Actor.GetChild(name) as EmptyActor;

            if (anchor == null)
            {
                anchor = EmptyActor.New();
                var script = New<VRController>();
                script.Role = role;
                anchor.AddScript(script);
                root.AddChild(anchor);
                anchor.Name = name;
            }
            return anchor;
        }

        private void RefreshVRMode()
        {
            AttachedCamera.IsActive = VRMode.HasFlag(VRMode.AttachedMode);
            LeftEyeCamera.IsActive = RightEyeCamera.IsActive = VRMode.HasFlag(VRMode.HmdMode);
        }

        private void RefreshCameraProperties()
        {
            if (LeftEyeCamera == null || RightEyeCamera == null)
                return;

            AttachedCamera.NearPlane = LeftEyeCamera.NearPlane = RightEyeCamera.NearPlane = nearPlane;
            AttachedCamera.FarPlane = LeftEyeCamera.FarPlane = RightEyeCamera.FarPlane = farPlane;
        }

        private void UpdateCamera(Camera camera, int eyeIndex)
        {
            VREye eye = VRSystem.Instance.EyesProperties[eyeIndex];
            VRPose pose = eye.Pose;

            // HACK... TODO: better method?
            camera.FieldOfView = VRSystem.Instance.FieldOfView;
            if (eyeIndex == 0)
                VRSystem.Instance.Left.Camera = camera;
            else if (eyeIndex == 1)
                VRSystem.Instance.Right.Camera = camera;

            var transform = camera.LocalTransform;
            transform.Translation = Monoscopic ? CenterEyeAnchor.LocalTransform.Translation : pose.Position;
            transform.Orientation = Monoscopic ? CenterEyeAnchor.LocalTransform.Orientation : pose.Orientation;
            camera.LocalTransform = transform;
        }

        //NOTE: Late update used bc. OpenVR is updated during Update() and this way we ensure, that we have latest data and we're not frame behind
        private void LateUpdate()
        {
            if (VRSystem.Instance == null || !VRSystem.Instance.IsConnected)
                return;
            VREye[] eyes = VRSystem.Instance.EyesProperties;
            VRPose pose = VRSystem.Instance.TrackerCameraPose;

            UpdateCamera(LeftEyeCamera, 0);
            UpdateCamera(RightEyeCamera, 1);

            var transform = TrackerAnchor.LocalTransform;
            transform.Translation = pose.Position;
            transform.Orientation = pose.Orientation;

            TrackerAnchor.LocalTransform = transform;

            foreach (VRController controller in vrControllers)
            {
                VRControllerRole role = controller.Role;
                int cIndex;
                if (role == VRControllerRole.LeftHand)
                {
                    cIndex = VRSystem.Instance.LeftControllerIndex;
                }
                else if (role == VRControllerRole.RightHand)
                {
                    cIndex = VRSystem.Instance.RightControllerIndex;
                }
                else
                {
                    cIndex = controller.ControllerIndex;
                }

                VRControllerState[] states = VRSystem.Instance.Controllers;
                VRControllerState newState;

                if (cIndex >= 0 && states != null && cIndex < states.Length)
                {
                    newState = states[cIndex];
                }
                else
                {
                    newState = new VRControllerState();
                }

                controller.UpdateState(newState);
            }
            VRPose centerPose = eyes[2].Pose;
            transform = CenterEyeAnchor.LocalTransform;
            transform.Translation = centerPose.Position;
            transform.Orientation = centerPose.Orientation;

            CenterEyeAnchor.LocalTransform = transform;
        }

        private void OnDestroy()
        {
            VRSystem.Instance.Left.Camera = null;
            VRSystem.Instance.Right.Camera = null;
        }
    }
}