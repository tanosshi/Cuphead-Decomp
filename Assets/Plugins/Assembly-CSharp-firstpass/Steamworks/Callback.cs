using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public sealed class Callback<T>
	{
		public delegate void DispatchDelegate(T param);

		private CCallbackBaseVTable VTable;

		private IntPtr m_pVTable = IntPtr.Zero;

		private CCallbackBase m_CCallbackBase;

		private GCHandle m_pCCallbackBase;

		private bool m_bGameServer;

		private readonly int m_size = Marshal.SizeOf(typeof(T));

		private event DispatchDelegate m_Func;

		public Callback(DispatchDelegate func, bool bGameServer = false)
		{
			m_bGameServer = bGameServer;
			BuildCCallbackBase();
			Register(func);
		}

		public static Callback<T> Create(DispatchDelegate func)
		{
			return new Callback<T>(func);
		}

		public static Callback<T> CreateGameServer(DispatchDelegate func)
		{
			return new Callback<T>(func, true);
		}

		~Callback()
		{
			Unregister();
			if (m_pVTable != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(m_pVTable);
			}
			if (m_pCCallbackBase.IsAllocated)
			{
				m_pCCallbackBase.Free();
			}
		}

		public void Register(DispatchDelegate func)
		{
			if (func == null)
			{
				throw new Exception("Callback function must not be null.");
			}
			if ((m_CCallbackBase.m_nCallbackFlags & 1) == 1)
			{
				Unregister();
			}
			if (m_bGameServer)
			{
				SetGameserverFlag();
			}
			this.m_Func = func;
			NativeMethods.SteamAPI_RegisterCallback(m_pCCallbackBase.AddrOfPinnedObject(), CallbackIdentities.GetCallbackIdentity(typeof(T)));
		}

		public void Unregister()
		{
			NativeMethods.SteamAPI_UnregisterCallback(m_pCCallbackBase.AddrOfPinnedObject());
		}

		public void SetGameserverFlag()
		{
			m_CCallbackBase.m_nCallbackFlags |= 2;
		}

		private void OnRunCallback(IntPtr pvParam)
		{
			try
			{
				this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)));
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		private void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall)
		{
			try
			{
				this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)));
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		private int OnGetCallbackSizeBytes()
		{
			return m_size;
		}

		private void BuildCCallbackBase()
		{
			VTable = new CCallbackBaseVTable
			{
				m_RunCallResult = OnRunCallResult,
				m_RunCallback = OnRunCallback,
				m_GetCallbackSizeBytes = OnGetCallbackSizeBytes
			};
			m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CCallbackBaseVTable)));
			Marshal.StructureToPtr(VTable, m_pVTable, false);
			m_CCallbackBase = new CCallbackBase
			{
				m_vfptr = m_pVTable,
				m_nCallbackFlags = 0,
				m_iCallback = CallbackIdentities.GetCallbackIdentity(typeof(T))
			};
			m_pCCallbackBase = GCHandle.Alloc(m_CCallbackBase, GCHandleType.Pinned);
		}
	}
}
