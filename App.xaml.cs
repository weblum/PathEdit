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
using System.IO;

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
			List<EditItem> EditItemList;
			if (e.Args.Length == 1 && File.Exists(e.Args[0]))
				EditItemList = ProcessScriptFile(e.Args[0]);
			else
				EditItemList = ProcessCommandLine(e.Args);
			ProcessActions(EditItemList);

			Shutdown();
		}

		private static List<EditItem> ProcessScriptFile(string FileName)
        {
	        return new List<EditItem>();
        }

        private static List<EditItem> ProcessCommandLine(string[] args)
        {
	        var result = new List<EditItem>();

	        try
	        {
		        EditItem item = new EditItem();
		        item.Parse(args);
		        result.Add(item);
	        }
	        catch (ApplicationException x)
	        {
		        MessageBox.Show(x.Message, Title);
	        }

	        return result;
        }

        private static void ProcessActions(IEnumerable<EditItem> EditItemList)
        {
            foreach (var edit in EditItemList)
            {
	            string msg = $"I pretend to process this: {edit}";
	            MessageBox.Show(msg, Title);
                edit.Execute();
            }
        }
    }
}
