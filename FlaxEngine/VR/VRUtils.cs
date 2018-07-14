using System;
using System.Runtime.CompilerServices;
using Valve.VR;

namespace FlaxEngine.VR
{
    /// <summary>
    /// OpenVR Utilities
    /// </summary>
    public static class VRUtils
    {
        /// <summary>
        /// Converts <see cref="HmdMatrix34_t"/> to <see cref="Matrix"/>
        /// </summary>
        /// <param name="ovrMatrix34f">The input.</param>
        /// <param name="matrix">The output.</param>
        public static void ToMatrix(this HmdMatrix34_t ovrMatrix34f, out Matrix matrix)
        {
            // NOTE: automatic conversion from RH to LH

            matrix.M11 = ovrMatrix34f.m0;
            matrix.M12 = ovrMatrix34f.m4;
            matrix.M13 = -ovrMatrix34f.m8;
            matrix.M14 = 0f;
            matrix.M21 = ovrMatrix34f.m1;
            matrix.M22 = ovrMatrix34f.m5;
            matrix.M23 = -ovrMatrix34f.m9;
            matrix.M24 = 0f;
            matrix.M31 = -ovrMatrix34f.m2;
            matrix.M32 = -ovrMatrix34f.m6;
            matrix.M33 = ovrMatrix34f.m10;
            matrix.M34 = 0f;
            matrix.M41 = ovrMatrix34f.m3;
            matrix.M42 = ovrMatrix34f.m7;
            matrix.M43 = -ovrMatrix34f.m11;
            matrix.M44 = 1f;
        }

        /// <summary>
        /// Converts <see cref="HmdMatrix44_t"/> to <see cref="Matrix"/>
        /// </summary>
        /// <param name="ovrMatrix34f">The input.</param>
        /// <param name="matrix">The output.</param>
        public static void ToMatrix(this HmdMatrix44_t ovrMatrix34f, out Matrix matrix)
        {
            // NOTE: automatic conversion from RH to LH

            matrix.M11 = ovrMatrix34f.m0;
            matrix.M12 = ovrMatrix34f.m4;
            matrix.M13 = -ovrMatrix34f.m8;
            matrix.M14 = ovrMatrix34f.m12;
            matrix.M21 = ovrMatrix34f.m1;
            matrix.M22 = ovrMatrix34f.m5;
            matrix.M23 = -ovrMatrix34f.m9;
            matrix.M24 = ovrMatrix34f.m13;
            matrix.M31 = -ovrMatrix34f.m2;
            matrix.M32 = -ovrMatrix34f.m6;
            matrix.M33 = ovrMatrix34f.m10;
            matrix.M34 = -ovrMatrix34f.m14;
            matrix.M41 = ovrMatrix34f.m3;
            matrix.M42 = ovrMatrix34f.m7;
            matrix.M43 = -ovrMatrix34f.m11;
            matrix.M44 = ovrMatrix34f.m15;
        }

        /// <summary>
        /// Converts <see cref="TrackedDevicePose_t"/> to <see cref="Matrix"/>
        /// </summary>
        /// <param name="trackedPose">The tracked pose.</param>
        /// <param name="matrix">The matrix.</param>
        public static void ToMatrix(this TrackedDevicePose_t trackedPose, out Matrix matrix) => trackedPose.mDeviceToAbsoluteTracking.ToMatrix(out matrix);

        /// <summary>
        /// Converts <see cref="Matrix"/> to <see cref="VRPose"/>
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="pose">The pose.</param>
        public static void ToVRPose(this Matrix matrix, out VRPose pose)
        {
            pose.Position = matrix.TranslationVector * 100; //m -> cm
            pose.Orientation = Quaternion.RotationMatrix(matrix);
        }

        /// <summary>
        /// Converts <see cref="Boolean"/> to <see cref="VRButtonState"/>
        /// </summary>
        /// <param name="b">The input.</param>
        /// <returns>VRButtonState corresponding to input <paramref name="b"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VRButtonState ToButtonState(this bool b)
        {
            if (!b)
                return VRButtonState.Released;
            return VRButtonState.Pressed;
        }

        /// <summary>
        /// Converts <see cref="TrackedDevicePose_t"/> to <see cref="VRPose"/>
        /// </summary>
        /// <param name="trackedPose">The tracked pose.</param>
        /// <param name="pose">The pose.</param>
        public static void ToVRPose(this TrackedDevicePose_t trackedPose, out VRPose pose)
        {
            trackedPose.mDeviceToAbsoluteTracking.ToMatrix(out Matrix matrix);
            pose.Position = matrix.TranslationVector * 100; //m -> cm
            pose.Orientation = Quaternion.RotationMatrix(matrix);
        }

        /// <summary>
        /// Gets whether the button is pressed.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns></returns>
        public static bool GetButtonPressed(this VRControllerState_t state, EVRButtonId buttonId) => (state.ulButtonPressed & 1UL << (int)buttonId) != 0UL;

        /// <summary>
        /// Gets whether the button is touched.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns></returns>
        public static bool GetButtonTouched(this VRControllerState_t state, EVRButtonId buttonId) => (state.ulButtonTouched & 1UL << (int)buttonId) != 0UL;

        /// <summary>
        /// Reports the initialization error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static void ReportInitError(EVRInitError error)
        {
            if (error == EVRInitError.None)
                return;
            //TODO: Don't log to Debug, since this is visible in editor console
            Debug.LogError("[VR] Init error: `" + OpenVR.GetStringForHmdError(error) + "`");
        }

        /// <summary>
        /// Reports the compositor error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static void ReportCompositeError(EVRCompositorError error)
        {
            /*if (error == EVRCompositorError.None)
                return;

            //TODO: Don't log to Debug, since this is visible in editor console
            Debug.LogError("[VR] Compositor error: `" + error + "`");*/
        }
    }
}
