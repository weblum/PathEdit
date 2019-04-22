using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PathEdit
{
    public class Script
    {
        private const string Title = "Path Editor";
        private List<EditItem> editItemList = new List<EditItem>();
        private readonly IRegistryEditor editor;

        public Script(IRegistryEditor editor)
        {
            this.editor = editor;
        }

        public void Execute(IEnumerable<EditItem> items)
        {
	        editItemList = items.ToList();
	        ProcessHive(Hive.User);
	        ProcessHive(Hive.System);
        }

        private void ProcessHive(Hive hive)
        {
            var data = ReadHive(hive).ToList();
            foreach (var edit in editItemList)
            {
                if (!edit.Execute(hive, data))
                    MessageBox.Show($"Execution problem with edit ({edit})");
            }
            SaveHive(hive, data);

        }

        private IEnumerable<string> ReadHive(Hive hive)
        {
            try
            {
                return editor.GetPathStrings(hive);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Title,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }

        private void SaveHive(Hive hive, IEnumerable<string> data)
        {
            try
            {
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
