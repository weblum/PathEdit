//---------------------------------------------------------------------------
// FILE NAME: Parser.cs
// DATE:      Saturday, April 20, 2019   7 pm
// WEATHER:   Mostly Cloudy, Temp 54°F, Pressure 29.96",
//            Humidity 72%, Wind 19.6 mph from the Southwest
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace PathEdit
{
    /// <summary>
    ///     The parser is responsible for parsing a list of strings and
    ///     producing a corresponding list of EditItem.
    /// </summary>
    /// <remarks>
    ///     An array of input tokens is used to create a list of EditItems.
    /// 
    ///     An input token consists of
    ///     +Path	where the + is mandatory
    ///     -Path	where the - is mandatory
    ///     [parameter[,parameter]] where the outer [] are mandatory. There
    ///     may be one or two parameters separated by a comma.
    /// 
    ///     A parameter is one of:
    ///     Beginning (or B): following add paths to be at beginning of hive
    ///     End (or E): following add paths to be at beginning of hive
    ///     User (or U): following add paths to be in HKCU hive
    ///     System (or S)following add paths to be in HKLM hive
    /// 
    ///     Examples:
    ///     [Beginning]
    ///     [E]
    ///     [B,User]
    /// 
    ///     Default parameters at start of processing are Beginning of User
    ///     hive. Path commands (including + or -) must be in quotes when on
    ///     command line. Quotes are not required when appearing in a script
    ///     file
    /// </remarks>
	public class Parser
	{
        private readonly List<EditItem> editItemList = new List<EditItem>();

		public IEnumerable<EditItem> Parse(IEnumerable<string> tokens)
		{
			//defaults
			EditItem.Location location = EditItem.Location.Beginning;
			Hive hive = Hive.User;

			char[] charsToTrim = { '\'', '\"' };
			foreach (var token in tokens)
			{
				string toke = token.Trim(charsToTrim);
				if (toke.StartsWith("+"))  //Add
					editItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Add, hive, location));
				else if (toke.StartsWith("-")) //Delete
					editItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Delete));
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
								throw new Exception($"Bad token: {token}");
						}
					}
					else
						throw new Exception($"Bad token: {token}");
				}
				else
					throw new Exception($"Bad token: {token}");
			}
			return editItemList;
		}
	}
}