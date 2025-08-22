using System.ComponentModel.DataAnnotations;

namespace MESWebDev.Models.COMMON
{
    public class UploadFileMaster
    {
        [Key]
        public int id { get; set; }
        public string file_name { get; set; } // Name of the uploaded file
        public int header_row { get; set; } // Row number in the file that contains the header information
        public int col_index { get; set; } // Column number in the database where the file is stored
        public string db_col_name { get; set; } // Column name in the database where the file is stored    
        public string? col_type { get; set; } // Data type of the column (e.g., string, int, date)
        public string? file_type { get; set; } // Type of the file (e.g., CSV, Excel)
        public string? note { get; set; } // Additional notes or comments about the file
    }
}
