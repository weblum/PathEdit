//---------------------------------------------------------------------------
// FILE NAME: RegistryEditor.cs
// DATE:      Sunday, September 27, 2015   2 pm
// WEATHER:   Not available.
// Programmer's Cuvee XLI
// Copyright (C) 2015 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PathEdit
{
	public class RegistryEditor : IRegistryEditor
    {
		private delegate RegistryKey EnvironmentKey(bool writable);
		private const string Path = "PATH";

        #region Implementation of IRegistryEditor interface

        public IEnumerable<string> GetPathStrings(Hive hive)
        {
            EnvironmentKey access = Access(hive);

            using (var environment = access(writable:false))
            {
                if (environment == null)
                    throw new Error("Failed to open registry key");

                object values = environment.GetValue(Path) 
                                ?? "No PATH variable";

                IEnumerable<string> result = values.ToString().Split(';');
                return result;
            }
        }

        public void SetPathStrings(Hive hive, IEnumerable<string> strings)
        {
            string value = string.Join(";", strings);

            EnvironmentKey access = Access(hive);

            using (var environment = access(writable: true))
            {
                if (environment == null) 
                    throw new Error("Failed to open registry key");
                environment.SetValue(Path, value);
                NotifySettingsChange();
            }
        }

        #endregion

        #region Private Utility Methods

        private static EnvironmentKey Access(Hive hive)
		{
			switch (hive)
			{
				case Hive.System:
					return OpenSystemEnvironmentKey;
				case Hive.User:
					return OpenUserEnvironmentKey;
				default:
					throw new ArgumentOutOfRangeException("hive");
			}
		}

		private static RegistryKey OpenUserEnvironmentKey(bool writable)
		{
			const string name = "Environment";
			return Registry.CurrentUser.OpenSubKey(name, writable:writable);
		}

		private static RegistryKey OpenSystemEnvironmentKey(bool writable)
		{
			const string name = @"SYSTEM\CurrentControlSet\Control\" 
				+ @"Session Manager\Environment";
			return Registry.LocalMachine.OpenSubKey(name, writable: writable);
		}

		private static void NotifySettingsChange()
		{
			const int HWND_BROADCAST = 0xffff;
			const int WM_SETTINGCHANGE = 0x001A;
			const int SMTO_ABORTIFHUNG = 0x0002;

			IntPtr hwnd = new IntPtr(HWND_BROADCAST);
			const uint msg = WM_SETTINGCHANGE;
			UIntPtr wParam = UIntPtr.Zero;
			const uint flags = SMTO_ABORTIFHUNG;
			const uint timeout = 5000;

			IntPtr lParam = IntPtr.Zero;
			try
			{
				lParam = Marshal.StringToCoTaskMemAuto("Environment");
				uint result;
				IntPtr code = SendMessageTimeout(hwnd, msg, wParam, lParam,
					flags, timeout, out result);
				
				if (code != IntPtr.Zero)
					return;

				var cd = Marshal.GetLastWin32Error();
				throw new Error("NotifySettingsChange failed: {0}", cd);
			}
			finally
			{
				Marshal.FreeCoTaskMem(lParam);
			}
		}

		[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		private static extern IntPtr SendMessageTimeout(
			IntPtr windowHandle, 
			uint Msg,
			UIntPtr wParam, 
			IntPtr lParam, 
			uint flags, 
			uint timeout, 
			out uint result);

        #endregion
    }
}

#if false
See https://support.microsoft.com/en-us/kb/104011

SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, 0,
    (LPARAM) "Environment", SMTO_ABORTIFHUNG,
    5000, &dwReturnValue);

#endif
