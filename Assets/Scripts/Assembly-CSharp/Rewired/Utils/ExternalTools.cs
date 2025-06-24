using System;
using System.ComponentModel;
using Rewired.Utils.Interfaces;
using UnityEngine;

namespace Rewired.Utils
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ExternalTools : IExternalTools
	{
		public event Action<uint, bool> XboxOneInput_OnGamepadStateChange;

		public bool LinuxInput_IsJoystickPreconfigured(string name)
		{
			return false;
		}

		public int XboxOneInput_GetUserIdForGamepad(uint id)
		{
			return 0;
		}

		public ulong XboxOneInput_GetControllerId(uint unityJoystickId)
		{
			return 0uL;
		}

		public bool XboxOneInput_IsGamepadActive(uint unityJoystickId)
		{
			return false;
		}

		public string XboxOneInput_GetControllerType(ulong xboxControllerId)
		{
			return string.Empty;
		}

		public uint XboxOneInput_GetJoystickId(ulong xboxControllerId)
		{
			return 0u;
		}

		public void XboxOne_Gamepad_UpdatePlugin()
		{
		}

		public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel)
		{
			return false;
		}

		public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS)
		{
		}

		public Vector3 PS4Input_GetLastAcceleration(int id)
		{
			return Vector3.zero;
		}

		public Vector3 PS4Input_GetLastGyro(int id)
		{
			return Vector3.zero;
		}

		public Vector4 PS4Input_GetLastOrientation(int id)
		{
			return Vector4.zero;
		}

		public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id)
		{
			touchNum = 0;
			touch0x = 0;
			touch0y = 0;
			touch0id = 0;
			touch1x = 0;
			touch1y = 0;
			touch1id = 0;
		}

		public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType)
		{
			touchpixelDensity = 0f;
			touchResolutionX = 0;
			touchResolutionY = 0;
			analogDeadZoneLeft = 0;
			analogDeadZoneright = 0;
			connectionType = 0;
		}

		public void PS4Input_PadSetMotionSensorState(int id, bool bEnable)
		{
		}

		public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable)
		{
		}

		public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable)
		{
		}

		public void PS4Input_PadSetLightBar(int id, int red, int green, int blue)
		{
		}

		public void PS4Input_PadResetLightBar(int id)
		{
		}

		public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor)
		{
		}

		public void PS4Input_PadResetOrientation(int id)
		{
		}

		public bool PS4Input_PadIsConnected(int id)
		{
			return false;
		}

		public object PS4Input_PadGetUsersDetails(int slot)
		{
			return null;
		}

		public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index)
		{
			return Vector3.zero;
		}

		public Vector3 PS4Input_GetLastMoveGyro(int id, int index)
		{
			return Vector3.zero;
		}

		public int PS4Input_MoveGetButtons(int id, int index)
		{
			return 0;
		}

		public int PS4Input_MoveGetAnalogButton(int id, int index)
		{
			return 0;
		}

		public bool PS4Input_MoveIsConnected(int id, int index)
		{
			return false;
		}

		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles)
		{
			return 0;
		}

		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles)
		{
			return 0;
		}

		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers)
		{
			return 0;
		}

		public IntPtr PS4Input_MoveGetControllerInputForTracking()
		{
			return IntPtr.Zero;
		}
	}
}
