using System;
using System.Collections.ObjectModel;

namespace PathEdit
{
    public class EditItem
    {
        public enum Action
        {
            Add,
            Delete
        }

        public enum Location
        {
            Beginning,
            End
        }

        private const string Title = "Path Editor";

        private string pathString { get; }
        private Action action { get; }
        private Hive hive { get; }
        private Location location { get; }

        public EditItem(string path, Action act, Hive hiv, Location loc)
        {
            pathString = path;
            action = act;
            hive = hiv;
            location = loc;
        }

        public EditItem(string path, Action act)
        {
            pathString = path;
            action = act;
        }

        public bool Execute(Hive hive,ObservableCollection<string> data)
        {
            switch (action)
            {
                case Action.Add:
                    return ExecuteAdd(hive,data);
                case Action.Delete:
                    ExecuteDelete(data);
                    return true;
            }
            return false;
        }

        private bool ExecuteAdd(Hive hiv,ObservableCollection<string> data)
        {
            if (hive != hiv)
                return true;
            switch (location)
            {
                case Location.Beginning:
                    data.Insert(0, pathString);
                    return true;
                case Location.End:
                    data.Add(pathString);
                    return true;
                default:
                    return false;

            }
        }

        private void ExecuteDelete(ObservableCollection<string> data)
        {
            string check = StripTrailingSlash(pathString);
            for (int i = 0; i < data.Count; i++)
            {
                if (PathEqual(check,data[i]))
                {
                    data.RemoveAt(i);
                    return;
                }
            }
        }

        static string StripTrailingSlash(string inp)
        {
            return inp.EndsWith(@"\") ? inp.Substring(0, inp.Length - 1) : inp;
        }

        private static bool PathEqual(string candidate, string target)
        {
            bool aNull = string.IsNullOrWhiteSpace(candidate);
            bool bNull = string.IsNullOrWhiteSpace(target);

            /*if (aNull && bNull) return true; */

            if (aNull || bNull)
                return false;

            string a = StripTrailingSlash(candidate);
            string b = StripTrailingSlash(target);
            return string.Equals(a, b,
                StringComparison.InvariantCultureIgnoreCase);
        }

        #region Overrides of Object
        public override string ToString()
        {
	        return $"{action}, {pathString}, {hive}, {location}";
        }
        #endregion
    }
}