using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine;
using FlaxEngine.VR;

namespace UI
{
	public class Movement : Script
	{
        VRController left;
        VRController right;
        Camera camera;
		private void Start()
		{
            var controllers = Actor.GetScriptsRecursive<VRController>();
            left = controllers.Where(c => c.Role == VRControllerRole.LeftHand).First();
            right = controllers.Where(c => c.Role == VRControllerRole.RightHand).First();

            camera = Actor.GetScript<VRCameraRig>().LeftEyeCamera;
        }

		private void Update()
		{
            // Left trackpad controls X and Z
            Vector2 xz = left.State.Trackpad;
 
            Vector3 movement = new Vector3(xz.X, 0, xz.Y);

            // Relative to camera orientation
            movement *= camera.Orientation;

            // Right trackpad controls the Elevation
            movement.Y = right.State.Trackpad.Y;

            // Movement is normalized
            movement.Normalize();

            // And sped up
            movement *= 10;

            // If you want to go faster, just press left trigger
            movement *= 1 + left.State.Trigger * 3;

            Actor.Position += movement;
        }
	}
}
