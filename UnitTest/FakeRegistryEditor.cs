//---------------------------------------------------------------------------
// FILE NAME: FakeRegistryEditor.cs
// GREETING:  Happy Earth Day
// DATE:      Monday, April 22, 2019   10 pm
// WEATHER:   Fair, Temp 67°F, Pressure 29.99",
//            Humidity 59%, Wind calm
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PathEdit;

namespace UnitTest
{
	public class FakeRegistryEditor : IRegistryEditor
	{
		private List<string> sysStrings;
		private List<string> useStrings;
		private readonly List<string> receivedUser = new List<string>();
		private readonly List<string> receivedSystem = new List<string>();

		public void SetSysStrings(IEnumerable<string> items)
		{
			sysStrings = new List<string>(items);
		}

		public void SetUseStrings(IEnumerable<string> items)
		{
			useStrings = new List<string>(items);
		}

		public IEnumerable<string> ReceivedUser => receivedUser;
		public IEnumerable<string> ReceivedSystem => receivedSystem;

		#region Implementation of IRegistryEditor

		public IEnumerable<string> GetPathStrings(Hive hive)
		{
			switch (hive)
			{
				case Hive.System:
					return sysStrings;
				case Hive.User:
					return useStrings;
				default:
					throw new ArgumentOutOfRangeException(nameof(hive), hive, null);
			}
		}

		public void SetPathStrings(Hive hive, IEnumerable<string> strings)
		{
			switch (hive)
			{
				case Hive.System:
					receivedSystem.AddRange(strings);
					break;
				case Hive.User:
					receivedUser.AddRange(strings);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(hive), hive, null);
			}
		}

		#endregion
	}
}