using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Accessibility;

namespace KeyLayoutAutoSwitch
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
		private const uint EVENT_OBJECT_FOCUS = 0x8005;
		private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

		private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime);

		[DllImport("user32.dll")]
		static extern bool UnhookWinEvent(IntPtr hWinEventHook);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("oleacc.dll")]
		private static extern uint AccessibleObjectFromEvent(IntPtr hwnd, uint dwObjectID, uint dwChildID, out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
		private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr zero);
		
		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

		public delegate void FocusEvent(IntPtr hwnd, uint idObject, uint idChild);

		public static IDisposable AddFocusEventHook(FocusEvent eventHandler)
		{
			return new FocusWinEventHook(eventHandler);
		}

		private sealed class FocusWinEventHook : IDisposable
		{
			private readonly FocusEvent mEventHandler;
			private readonly IntPtr mhWinEventHook;
			private readonly WinEventDelegate mWinEventDelegate;

			public FocusWinEventHook(FocusEvent eventHandler)
			{
				mEventHandler = eventHandler;
				mWinEventDelegate = OnWinEvent;
				mhWinEventHook = SetWinEventHook(EVENT_OBJECT_FOCUS, EVENT_OBJECT_FOCUS, IntPtr.Zero, mWinEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
			}

			private void ReleaseUnmanagedResources()
			{
				UnhookWinEvent(mhWinEventHook);
			}

			public void Dispose()
			{
				ReleaseUnmanagedResources();
				GC.SuppressFinalize(this);
			}

			~FocusWinEventHook()
			{
				ReleaseUnmanagedResources();
			}

			private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime)
			{
				Debug.Assert(hWinEventHook == mhWinEventHook);
				Debug.Assert(eventType == EVENT_OBJECT_FOCUS);

				mEventHandler(hwnd, idObject, idChild);
			}
		}

		public static string GetWindowClassName(IntPtr hwnd)
		{
			// Pre-allocate 256 characters, since this is the maximum class name length.
			var classNameBuilder = new StringBuilder(256);
			GetClassName(hwnd, classNameBuilder, classNameBuilder.Capacity);
			var className = classNameBuilder.ToString();
			return className;
		}

		public static IAccessible GetAccessibleFromEvent(IntPtr hwnd, uint idObject, uint idChild)
		{
			AccessibleObjectFromEvent(hwnd, idObject, idChild, out var accessible, out var _);
			return accessible;
		}

		public static void SwitchKeyboardLayout(IntPtr hWnd, IntPtr hklInputMethod)
		{
			SendMessage(hWnd, WM_INPUTLANGCHANGEREQUEST, 0, hklInputMethod);
		}

		public static IntPtr GetKeyboardLayout(IntPtr hWnd)
		{
			var threadId = GetWindowThreadProcessId(hWnd, IntPtr.Zero);
			if (threadId != 0)
			{
				return GetKeyboardLayout(threadId);
			}
			return IntPtr.Zero;
		}

		public static string GetWindowProcessName(IntPtr hWnd)
		{
			GetWindowThreadProcessId(hWnd, out var processId);
			return Process.GetProcessById(processId).ProcessName;
		}
	}
}
