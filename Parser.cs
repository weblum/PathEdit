//---------------------------------------------------------------------------
// FILE NAME: Parser.cs
// DATE:      Saturday, April 20, 2019   7 pm
// WEATHER:   Mostly Cloudy, Temp 54°F, Pressure 29.96",
//            Humidity 72%, Wind 19.6 mph from the Southwest
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;

namespace PathEdit
{
	internal class Parser
	{
		private const string Title = "Path Editor";
		List<EditItem> EditItemList = new List<EditItem>();

		public IEnumerable<EditItem> Parse(string[] tokens)
		{
			//defaults
			EditItem.Location location = EditItem.Location.Beginning;
			Hive hive = Hive.User;

			char[] charsToTrim = { '\'', '\"' };
			foreach (var token in tokens)
			{
				string toke = token.Trim(charsToTrim);
				if (toke.StartsWith("+"))  //Add
					EditItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Add, hive, location));
				else if (toke.StartsWith("-")) //Delete
					EditItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Delete));
				else if (toke.StartsWith("[")) //parameter
				{
					if (toke.EndsWith("]"))
					{
						string[] parms = toke.Substring(1, toke.Length - 2).Split(',');
						foreach (var parm in parms)
						{
							if (parm.ToUpper().StartsWith("B"))
								location = EditItem.Location.Beginning;
							else if (parm.ToUpper().StartsWith("E"))
								location = EditItem.Location.End;
							else if (parm.ToUpper().StartsWith("U"))
								hive = Hive.User;
							else if (parm.ToUpper().StartsWith("S"))
								hive = Hive.System;
							else
							{
								MessageBox.Show($"Bad token: {token}", Title);
								return new List<EditItem>();
							}
						}
					}
					else
					{
						MessageBox.Show($"Bad token: {token}", Title);
						return new List<EditItem>();
					}
				}
				else
				{
					MessageBox.Show($"Bad token: {token}", Title);
					return new List<EditItem>();
				}
			}
			return EditItemList;
		}
	}
}