namespace Steamworks
{
	public static class SteamController
	{
		public static bool Init(string pchAbsolutePathToControllerConfigVDF)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle pchAbsolutePathToControllerConfigVDF2 = new InteropHelp.UTF8StringHandle(pchAbsolutePathToControllerConfigVDF))
			{
				return NativeMethods.ISteamController_Init(pchAbsolutePathToControllerConfigVDF2);
			}
		}

		public static bool Shutdown()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamController_Shutdown();
		}

		public static void RunFrame()
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamController_RunFrame();
		}

		public static bool GetControllerState(uint unControllerIndex, out SteamControllerState_t pState)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamController_GetControllerState(unControllerIndex, out pState);
		}

		public static void TriggerHapticPulse(uint unControllerIndex, ESteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamController_TriggerHapticPulse(unControllerIndex, eTargetPad, usDurationMicroSec);
		}

		public static void SetOverrideMode(string pchMode)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle pchMode2 = new InteropHelp.UTF8StringHandle(pchMode))
			{
				NativeMethods.ISteamController_SetOverrideMode(pchMode2);
			}
		}
	}
}
