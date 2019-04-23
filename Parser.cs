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
using System.Text.RegularExpressions;

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
				string trimmed = token.Trim(charsToTrim);
				PathParser pathParser = new PathParser(trimmed);
				ParmParser parmParser = new ParmParser(trimmed);

				if (pathParser.IsPath)
				{
					var item = new EditItem(pathParser.Path, pathParser.Action, hive, location);
					editItemList.Add(item);
				}
				else if (parmParser.IsParm)
				{
					if (parmParser.HiveParm.HasValue)
						hive = parmParser.HiveParm.Value;
					if (parmParser.LocationParm.HasValue)
						location = parmParser.LocationParm.Value;
				}
				else
					throw new Exception($"Bad parameter: {token}");
			}
			return editItemList;
		}

		private class PathParser
		{
			private const int ActionIndex = 1;
			private const int PathIndex = 2;
			private const string Pattern = @"(\+|-)(\S+)";
			private const string BadAction = "Bad action";
			private static readonly Regex Rex = new Regex(Pattern);

			public string Path { get; }
			public EditItem.Action Action { get; }
			public bool IsPath { get; }

			public PathParser(string trimmed)
			{
				Match m = Rex.Match(trimmed);
				IsPath = m.Success;

				if (!IsPath)
					return;

				Path = m.Groups[PathIndex].Value;
				switch (m.Groups[ActionIndex].Value)
				{
					case "+":
						Action = EditItem.Action.Add;
						break;
					case "-":
						Action = EditItem.Action.Delete;
						break;
					default:
						throw new Exception(BadAction);
				}
			}
		}

		private class ParmParser
		{
			private const int FirstIndex = 2;
			private const int SecondIndex = 4;
			private const string Pattern = @"\s*\[(([beus])[a-z]*)(,([beus])[a-z]*)?]\s*";
			private static readonly Regex Rex = new Regex(Pattern, RegexOptions.IgnoreCase);

			public Hive? HiveParm { get; private set; }
			public EditItem.Location? LocationParm { get; private set; }
			public bool IsParm { get; }

			// The regular expression either fails or extracts one or two
			// characters. On success, the method always parses the first
			// character. The first character might be for a hive or for a
			// location. If there are two characters, and the first was for a
			// location, you parse the second character as a hive; but if the
			// first was for a hive, you parse the second character as a
			// location.
			public ParmParser(string trimmed)
			{
				Match m = Rex.Match(trimmed);
				IsParm = m.Success;

				if (!IsParm)
					return;

				string firstChar = m.Groups[FirstIndex].Value.ToLower();
				string secondChar = m.Groups[SecondIndex].Value.ToLower();

				bool isFirstLocation = ParseFirstChar(firstChar);

				if (secondChar.Length == 0)
					return;

				if (isFirstLocation)
					ParseSecondAsHive(secondChar);
				else
					ParseSecondAsLocation(secondChar);
			}

			private bool ParseFirstChar(string firstChar)
			{
				bool result;

				switch (firstChar)
				{
					case "b":
						LocationParm = EditItem.Location.Beginning;
						result = true;
						break;
					case "e":
						LocationParm = EditItem.Location.End;
						result = true;
						break;
					case "u":
						HiveParm = Hive.User;
						result = false;
						break;
					case "s":
						HiveParm = Hive.System;
						result = false;
						break;
					default:
						throw new Exception("Bad parameter");
				}

				return result;
			}

			private void ParseSecondAsHive(string secondChar)
			{
				switch (secondChar)
				{
					case "u":
						HiveParm = Hive.User;
						break;
					case "s":
						HiveParm = Hive.System;
						break;
					default:
						throw new Exception("Bad parameter");
				}
			}

			private void ParseSecondAsLocation(string secondChar)
			{
				switch (secondChar)
				{
					case "b":
						LocationParm = EditItem.Location.Beginning;
						break;
					case "e":
						LocationParm = EditItem.Location.End;
						break;
					default:
						throw new Exception("Bad parameter");
				}
			}
		}
	}
}
