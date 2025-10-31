namespace MESWebDev.Models.Master.DTO
{
    public class UserPassUploadDTO
    {
        public string user_id { get; set; }
        public string password { get; set; }
        public byte[]? stored_salt { get; set; }
    }
}
