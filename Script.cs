using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PathEdit
{
    public class Script
    {
        private const string Title = "Path Editor";
        List<EditItem> editItemList=new List<EditItem>();

        public void Execute(IEnumerable<EditItem> items)
        {
	        editItemList = items.ToList();
	        ProcessHive(Hive.User);
	        ProcessHive(Hive.System);
        }

        private void ProcessHive(Hive hive)
        {
            ObservableCollection<string> data = ReadHive(hive);
            foreach (var edit in editItemList)
            {
                if (!edit.Execute(hive,data))
                    MessageBox.Show($"Execution problem with edit ({edit})");
            }
            SaveHive(hive,data);

        }

        private static ObservableCollection<string> ReadHive(Hive hive)
        {
            try
            {
                var editor = new RegistryEditor();
                return editor.GetPathStrings(hive);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Title,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }

        private static void SaveHive(Hive hive, ObservableCollection<string> data)
        {
            try
            {
                var editor = new RegistryEditor();
                editor.SetPathStrings(hive, data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Title,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
