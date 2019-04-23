//---------------------------------------------------------------------------
// FILE NAME: App.xaml.cs
// DATE:      Wednesday, April 17, 2019   8 pm
// WEATHER:   Fair, Temp 63°F, Pressure 30.13",
//            Humidity 56%, Wind 8.1 mph from the West
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;

namespace PathEdit
{
	public partial class App //  : Application
	{
		private const string Title = "Path Editor";

		#region Overrides of Application

		protected override void OnStartup(StartupEventArgs e)
		{
			if (e.Args.Length > 0)
				DoCommandsAndExit(e);
			else
				new MainWindow().Show();
			base.OnStartup(e);
		}

		#endregion

		private void DoCommandsAndExit(StartupEventArgs e)
		{
			try
			{
				var parser = new Parser();
				IEnumerable<EditItem> commands = parser.Parse(e.Args);
				IRegistryEditor editor = new RegistryEditor();
				var script = new Script(editor);
				script.Execute(commands);
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Title);
			}
			Shutdown();
		}
	}
}
