namespace MESWebDev.Models.ProdPlan.SMT.DTO
{
    public class FullCalendarDTO
    {
        public record FcResource(string id, string title);

        public class FcEvent
        {
            public string id { get; set; } = default!;
            public string resourceId { get; set; } = default!;
            public string title { get; set; } = default!;
            public string start { get; set; } = default!;  // ISO string
            public string end { get; set; } = default!;
            public bool allDay { get; set; } = false;

            public string? display { get; set; } // "background" for breaks
            public string? backgroundColor { get; set; }
            public bool? overlap { get; set; }

            public object? extendedProps { get; set; }
        }

        public class PreviewResponse
        {
            public List<FcResource> resources { get; set; } = new();
            public List<FcEvent> events { get; set; } = new();
            public List<FcEvent> backgroundEvents { get; set; } = new();
        }


        public class ValidateRequest
        {
            public string lotno { get; set; } = "";
            public List<ValidateEvent> events { get; set; } = new();
        }

        public class ValidateEvent
        {
            public string id { get; set; } = "";
            public string lotno { get; set; } = "";
            public string pcbKey { get; set; } = "";
            public int runOrder { get; set; }
            public int qty { get; set; }
            public string lineCode { get; set; } = "";
            public DateTime start { get; set; }
            public DateTime end { get; set; }
        }

    }
}
