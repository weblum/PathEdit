using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace PathEdit
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                List<EditItem> EditItemList;
                if (e.Args.Length == 1 && File.Exists(e.Args[0]))
                    EditItemList = ProcessScriptFile(e.Args[0]);
                else
                    EditItemList = ProcessCommandLine(e.Args);
                ProcessActions(EditItemList);
            }
            else
                new MainWindow().Show();
        }

        private List<EditItem> ProcessScriptFile(string FileName)
        {
            return null;
        }

        private List<EditItem> ProcessCommandLine(string[] args)
        {
            return null;
        }

        void ProcessActions(List<EditItem> EditItemList)
        {
            foreach (var edit in EditItemList)
            {
                edit.Execute();
            }
        }
    }
}
