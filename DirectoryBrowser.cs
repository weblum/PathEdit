using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PathEdit
{
	public class DirectoryBrowser
	{
		private static FolderBrowserDialog dlg;

		public static string GetFolderFromUser()
		{
			using (dlg = new FolderBrowserDialog())
			{
				dlg.Description = "Browse for the new path.";
				dlg.RootFolder = Environment.SpecialFolder.MyComputer;
				dlg.SelectedPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
				DialogResult dialogResult = dlg.ShowDialog();
				return dialogResult == DialogResult.OK ? dlg.SelectedPath : null;
			}
		}
	}
}
