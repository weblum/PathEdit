//---------------------------------------------------------------------------
// FILE NAME: App.xaml.cs
// DATE:      Wednesday, April 17, 2019   8 pm
// WEATHER:   Fair, Temp 63°F, Pressure 30.13",
//            Humidity 56%, Wind 8.1 mph from the West
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace PathEdit
{
	public partial class App //  : Application
	{
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
            if (EditItemList!=null)
    			ProcessActions(EditItemList);

			Shutdown();
		}

		private static List<EditItem> ProcessScriptFile(string FileName)
        {
            string[] lines = File.ReadAllLines(FileName);
            return EditItem.PrepareEditList(lines);
        }

        private static List<EditItem> ProcessCommandLine(string[] args)
        {
            return EditItem.PrepareEditList(args);
        }

        private static void ProcessActions(IEnumerable<EditItem> EditItemList)
        {
            foreach (var edit in EditItemList)
            {
                if (!edit.Execute())
                    MessageBox.Show($"Execution problem with edit ({edit})");
            }
        }
    }
}
