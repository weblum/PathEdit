//---------------------------------------------------------------------------
// FILE NAME: IRegistryEditor.cs
// DATE:      Monday, April 22, 2019   3 pm
// WEATHER:   Not available.
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;

namespace PathEdit
{
	/// <summary>
	///     Specifies the operations required to read and write lists of
	///     strings from and to the registry keys that hold the PATH
	///     environment variables.
	/// </summary>
	public interface IRegistryEditor
	{
		/// <summary>
		///     Reads the strings for the PATH variable.
		/// </summary>
		/// <param name="hive">
		///     The hive indicates whether to read from the current user's
		///     data or from the system's data.
		/// </param>
		/// <returns>The paths in the given location.</returns>
		IEnumerable<string> GetPathStrings(Hive hive);

		/// <summary>
		///     Writes the strings for the PATH variable.
		/// </summary>
		/// <param name="hive">
		///     The hive indicates whether to read from the current user's
		///     data or from the system's data.
		/// </param>
		/// <param name="strings">
		///     The string to be written to the registry.
		/// </param>
		void SetPathStrings(Hive hive, IEnumerable<string> strings);
	}
}
