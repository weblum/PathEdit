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

        public bool Execute()
        {
            return true;
        }
    }
}