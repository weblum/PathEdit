using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PathEdit
{
    public class Script
    {
        private const string Title = "Path Editor";
        List<EditItem> EditItemList=new List<EditItem>();
        private bool SuccessfulParse;

        public Script(string[] args)
        {
            if (args.Length == 1 && File.Exists(args[0]))
                SuccessfulParse = ParseScriptFile(args[0]);
            else
                SuccessfulParse = ParseCommandLine(args);
        }

        private bool ParseScriptFile(string FileName)
        {
            string[] lines = File.ReadAllLines(FileName);
            return ParseScriptLines(lines);
        }

        private bool ParseCommandLine(string[] args)
        {
            return ParseScriptLines(args);
        }


        /*
            An array of input tokens is used to create a list of EditItems.

            An input token consists of
            +Path	where the + is mandatory
            -Path	where the - is mandatory
            [parameter[,parameter]] where the outer [] are mandatory. There may be one or two parameters separated by a comma.

            A parameter is one of:
            Beginning (or B): following add paths to be at beginning of hive
            End (or E): following add paths to be at beginning of hive
            User (or U): following add paths to be in HKCU hive
            System (or S)following add paths to be in HKLM hive

            Examples:
            [Beginning]
            [E]
            [B,User]

            Default parameters at start of processing are Beginning of User hive.

            Path commands (including + or -) must be in quotes when on command line.
            Quotes are not required when appearing in a script file
        */

        private bool ParseScriptLines(string[] tokens)
        {
            //defaults
            EditItem.Location location = EditItem.Location.Beginning;
            Hive hive = Hive.User;

            char[] charsToTrim = { '\'', '\"' };
            foreach (var token in tokens)
            {
                string toke = token.Trim(charsToTrim);
                if (toke.StartsWith("+"))  //Add
                    EditItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Add, hive, location));
                else if (toke.StartsWith("-")) //Delete
                    EditItemList.Add(new EditItem(toke.Substring(1), EditItem.Action.Delete));
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
                            {
                                MessageBox.Show($"Bad token: {token}", Title);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Bad token: {token}", Title);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show($"Bad token: {token}", Title);
                    return false;
                }
            }
            return true;
        }

        public void Execute()
        {
            if (!SuccessfulParse)
                return;
            ProcessHive(Hive.User);
            ProcessHive(Hive.System);
        }

        public void ProcessHive(Hive hive)   // IEnumerable<EditItem> EditItemList)
        {
            ObservableCollection<string> data = ReadHive(hive);
            foreach (var edit in EditItemList)
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
