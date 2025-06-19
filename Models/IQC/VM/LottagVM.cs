namespace MESWebDev.Models.IQC.VM
{
    public class LottagVM
    {
        public string id { get; set; }
        public string yusen_invno { get; set; }
        public DateTime? rcv_date { get; set; }
        public string invoice { get; set; }
        public string vender_name { get; set; }
        public string vender_code { get; set; }
        public string abbre_group { get; set; }
        public string partcode { get; set; }
        public string partname { get; set; }
        public string partspec { get; set; }
        public string purchase_order { get; set; }
        public int? count_pono { get; set; } = 0;
        public int? qty { get; set; } = 0;
        public string location_rec { get; set; }
        public string iqc_status { get; set; }
        public DateTime? iqc_rec_lottag { get; set; }
        public string iqc_rec_person { get; set; }
        public string status_lottag { get; set; }

        public IQueryable<LottagVM> ApplySearch(IQueryable<LottagVM> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            return query.Where(t => t.yusen_invno.Contains(searchTerm) ||
                               t.invoice.Contains(searchTerm) ||
                               t.vender_code.Contains(searchTerm) ||
                               t.vender_name.Contains(searchTerm) ||
                               t.partcode.Contains(searchTerm) ||
                               t.partname.Contains(searchTerm) ||
                               t.partspec.Contains(searchTerm) ||
                               t.purchase_order.Contains(searchTerm) ||
                               t.location_rec.Contains(searchTerm) ||
                               t.iqc_rec_person.Contains(searchTerm) ||
                               t.status_lottag.Contains(searchTerm) ||
                               t.id.Contains(searchTerm));
        }
    }
}