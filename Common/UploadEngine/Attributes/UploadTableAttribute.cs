namespace MESWebDev.Common.UploadEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class UploadTableAttribute: Attribute
    {
        public string TableName { get; }
        public UploadTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
