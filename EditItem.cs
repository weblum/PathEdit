using System.Collections.Generic;
using System.Windows;

namespace PathEdit
{
    public class EditItem
    {
        private enum eAction
        {
            Add,
            Delete
        }

        private enum eHive
        {
            NotSpecified,
            User,
            Machine
        }

        private enum eLocation
        {
            NotSpecified,
            Beginning,
            End
        }

        private const string Title = "Path Editor";

        private string PathString { get; }
        private eAction Action { get; }
        private eHive Hive { get; }
        private eLocation Location { get; }

        private EditItem(string path, eAction action, eHive hive=eHive.NotSpecified, eLocation location=eLocation.NotSpecified)
        {
            PathString = path;
            Action = action;
            Hive = hive;
            Location = location;
        }

        public bool Execute()
        {
            return false;   //fail!
        }

        #region Overrides of Object
        public override string ToString()
        {
	        return $"{Action}, {PathString}, {Hive}, {Location}";
        }
        #endregion


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
        Machine (or M)following add paths to be in HKLM hive

        Examples:
        [Beginning]
        [E]
        [B,User]

        Default parameters at start of processing are Beginning of User hive.
        */

        public static List<EditItem> PrepareEditList(string[] tokens)
        {
            List<EditItem> eiList = new List<EditItem>();

            //defaults
            EditItem.eLocation Location = eLocation.Beginning;
            EditItem.eHive Hive = eHive.User;

            char[] charsToTrim = { '\'', '\"' };
            foreach (var token in tokens)
            {
                string toke = token.Trim(charsToTrim);
                if (toke.StartsWith("+"))  //Add
                    eiList.Add(new EditItem(toke.Substring(1), eAction.Add, Hive, Location));
                else if (toke.StartsWith("-")) //Delete
                    eiList.Add(new EditItem(toke.Substring(1), eAction.Delete));
                else if (toke.StartsWith("[")) //parameter
                {
                    if (toke.EndsWith("]"))
                    {
                        string[] parms = toke.Substring(1, toke.Length - 2).Split(',');
                        foreach (var parm in parms)
                        {
                            if (parm.ToUpper().StartsWith("B"))
                                Location = eLocation.Beginning;
                            else if (parm.ToUpper().StartsWith("E"))
                                Location = eLocation.End;
                            else if (parm.ToUpper().StartsWith("U"))
                                Hive = eHive.User;
                            else if (parm.ToUpper().StartsWith("M"))
                                Hive = eHive.Machine;
                            else
                            {
                                MessageBox.Show($"Bad token: {token}", Title);
                                return null;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Bad token: {token}", Title);
                        return null;
                    }
                }
                else
                {
                    MessageBox.Show($"Bad token: {token}", Title);
                    return null;
                }
            }
            return eiList;
        }
    }
}