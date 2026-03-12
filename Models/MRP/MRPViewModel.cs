using MESWebDev.Models.MRP.DTO;
using MESWebDev.Models.Setting.DTO;
using System.Data;


namespace MESWebDev.Models.MRP
{
    public class MRPViewModel
    {
        public DataTable? Data { get; set; }
        public string? UploadedFile { get; set; }

        public MRPBOMModel? BOM { get; set; }
        public List<MRPBOMModel>? BOMList { get; set; }
        public List<MRPBOMDTO>? BOMDTOList { get; set; }
        public List<MRPBOMUpload>? BOMUploadList { get; set; }

        public MRPDataModel? MRP { get; set; }
        public List<MRPDataModel>? MRPList { get; set; }
        public List<MRPDataDTO>? MRPDTOList { get; set; }

        public MRPOBLModel? OBL { get; set; }
        public List<MRPOBLModel>? OBLList { get; set; }
        public List<MRPOBLDTO>? OBLDTOList { get; set; }
        public List<MRPOBLUpload>? OBLUploadList { get; set; }

        public MRPOHModel? OH { get; set; }
        public List<MRPOHModel>? OHList { get; set; }
        public List<MRPOHDTO>? OHDTOList { get; set; }
        public List<MRPOHUpload>? OHUploadList { get; set; }

        public MRPSPOModel? SPO { get; set; }
        public List<MRPSPOModel>? SPOList { get; set; }
        public List<MRPSPODTO>? SPODTOList { get; set; }
        public List<MRPSPOUpload>? SPOUploadList { get; set; }

        public FormatRazorDTO? FormatRazorDTO { get; set; }
        public bool UploadedError { get; set; } = false;
        public string? error_msg { get; set; }
        public DateTime CreatedDt { get; set; } = DateTime.Now;
        public Dictionary<string, object>? Dic { get; set; }    
    }
}
