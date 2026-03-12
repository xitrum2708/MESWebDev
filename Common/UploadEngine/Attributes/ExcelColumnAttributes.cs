namespace MESWebDev.Common.UploadEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnAttributes:Attribute
    {
        public string[] Names { get; set; }
        
        public ExcelColumnAttributes(params string[] names)
        {
            //example LotNo, lotno, LOTNO, Lotno
            Names = names;
        }
    }
}
