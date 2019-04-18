using System;

namespace PathEdit
{
    public class EditItem
    {
        enum eAction
        {
            Add,
            Delete
        }

        enum eHive
        {
            NotSpecified,
            User,
            Global
        }

        enum eLocation
        {
            Front,
            Back
        }

        private string PathString { get; set; }
        private eAction Action { get; set; }
        private eHive Hive { get; set; }
        private eLocation Location { get; set; }

        public void Parse(string[] args)
        {
	        bool success = true;

	        if (Enum.TryParse(args[0], out eAction action))
		        Action = action;
	        else success = false;

	        if (Enum.TryParse(args[1], out eHive hive))
		        Hive = hive;
	        else success = false;

	        if (Enum.TryParse(args[2], out eLocation location))
		        Location = location;
	        else success = false;

	        PathString = args[3];

	        if (success) return;

			throw new ApplicationException("Invalid arguments.");
        }

        public bool Execute()
        {
            return true;
        }

        #region Overrides of Object

        public override string ToString()
        {
	        return $"{Action}, {Hive}, {Location}, {PathString}";
        }

        #endregion
    }
}