//---------------------------------------------------------------------------
// FILE NAME: Script.cs
// GREETING:  Happy Earth Day
// DATE:      Monday, April 22, 2019   10 pm
// WEATHER:   Fair, Temp 62°F, Pressure 30.01",
//            Humidity 65%, Wind 3.5 mph from the East
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace PathEdit
{
	public class Script
	{
		private readonly IRegistryEditor editor;
		private List<EditItem> editItemList = new List<EditItem>();

		public Script(IRegistryEditor editor)
		{
			this.editor = editor;
		}

		public void Execute(IEnumerable<EditItem> items)
		{
			editItemList = items.ToList();
			ProcessHive(Hive.User);
			ProcessHive(Hive.System);
		}

		private void ProcessHive(Hive hive)
		{
			var data = editor.GetPathStrings(hive).ToList();

			foreach (EditItem edit in editItemList)
				edit.Execute(hive, data);

			editor.SetPathStrings(hive, data);
		}
	}
}