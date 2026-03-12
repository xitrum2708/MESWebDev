function BindLineUtilization() {
/* ======== HELPERS ======== */
    //const fmtDate = (s) => new Date(s).toLocaleDateString("en-US", { year: "numeric", month: "2-digit", day: "2-digit" });
    //alert(dateFormat);
    //const fmtDate = (s, dateFormat) => dayjs(s).format(dateFormat);
    const fmtDate = (s) =>
        dayjs(s).format(dateFormat);

    console.log(fmtDate("2026-03-02T00:00:00", "YYYY/MM/DD"));
   // alert(fmtDate);
        const pct = (n) => `${n.toFixed(0)}%`;

        function niceGradientFor(val){
        // Soft, professional gradients by tier
        if(val >= 90)   return `linear-gradient(90deg, var(--grad-d), var(--grad-e))`;
        if(val >= 75)   return `linear-gradient(90deg, var(--grad-c), var(--grad-d))`;
        if(val >= 50)   return `linear-gradient(90deg, var(--grad-b), var(--grad-c))`;
        return `linear-gradient(90deg, var(--grad-a), var(--grad-b))`;
        }

        function statusLabel(val){
            if (val >= 90) return { text: "Very High", color: "var(--grad-e)"};
            if (val >= 75) return { text: "High", color: "var(--grad-c)"};
            if (val >= 50) return { text: "Medium", color: "var(--grad-b)"};
            return { text: "Low", color: "var(--grad-a)"};
        }

        function computeKPIs(data){
        const vals = data.map(d=>d.UsagePercent).filter(v=>typeof v === "number");
        const avg = vals.length ? vals.reduce((a,b)=>a+b,0) / vals.length : 0;
        let max = -Infinity, min = Infinity, maxObj=null, minObj=null;
        for(const d of data){
        if(typeof d.UsagePercent !== "number") continue;
        if(d.UsagePercent > max){max = d.UsagePercent; maxObj = d; }
            if(d.UsagePercent < min){min = d.UsagePercent; minObj = d; }
        }
        const lineSet = [...new Set(data.map(d=>d.LineCode))].sort();
        const dateArr = [...new Set(data.map(d=>d.UsageDate))].sort();
        const firstDate = dateArr[0] ? fmtDate(dateArr[0]) : "—";
        const lastDate  = dateArr.length ? fmtDate(dateArr[dateArr.length-1]) : "—";
        return {
            avg, max, min,
            maxObj, minObj,
            lineCount: lineSet.length,
            dateCount: dateArr.length,
            firstDate, lastDate
          };
        }

        function buildMap(data, lines, dates){
            const map = { };
            lines.forEach(l => {
                    map[l] = {};
            dates.forEach(dt => map[l][dt] = null);
            });
            data.forEach(x => {map[x.LineCode][x.UsageDate] = x.UsagePercent; });
            return map;
        }

        /* ======== RENDER (jQuery) ======== */
        $(function(){
        const $headerRow = $("#headerRow");
        const $bodyRows  = $("#bodyRows");
        const $inputFilter = $("#lineFilter");

        const $kpiAvg = $("#kpiAvg");
        const $kpiMax = $("#kpiMax");
        const $kpiMaxMeta = $("#kpiMaxMeta");
        const $kpiMin = $("#kpiMin");
        const $kpiMinMeta = $("#kpiMinMeta");
        const $kpiScope = $("#kpiScope");
        const $kpiScopeMeta = $("#kpiScopeMeta");

        function clearTable(){
            $headerRow.find("th:not(:first)").remove();
            $bodyRows.empty();
        }

        function renderHeader(dates) {
            //alert(fmtDate(date));
            dates.forEach(date => {
                $("<th/>").text(fmtDate(date)).appendTo($headerRow);
            });
        }

        function renderBody(map, lines, dates){
            lines.forEach(line => {
                const $tr = $("<tr/>");
                $("<td/>", { class: "line-cell", text: line }).appendTo($tr);

                dates.forEach(date => {
                    const val = map[line][date];
                    const $td = $('<td class="progress-cell"></td>');

                    if (val !== null && typeof val === "number") {
                        const $fill = $('<div class="progress-fill"></div>').css("background", niceGradientFor(val));
                        const $gloss = $('<div class="progress-gloss"></div>');
                        const $text = $('<div class="progress-text">0%</div>');
                        const st = statusLabel(val);
                        const $tip = $(`
                            <div class="tooltip">
                              <div><span class="dot me-2" style="background:${st.color}"></span><strong>${st.text}</strong></div>
                              <div>Line: <strong>${line}</strong></div>
                              <div>Date: <strong>${fmtDate(date)}</strong></div>
                              <div>Utilization: <strong>${pct(val)}</strong></div>
                            </div>
                          `);

                        $td.append($fill, $gloss, $text, $tip);

                        // Animate bar width (jQuery)
                        $fill.animate({ width: val + "%" }, 900, "swing");
                        // Set gloss width via CSS variable
                        $gloss.css("--w", val + "%");

                        // Animate number (0 -> val)
                        $({ count: 0 }).animate({ count: val }, {
                            duration: 900,
                            easing: "swing",
                            step: function (now) { $text.text(Math.round(now) + "%"); }
                        });

                        // Auto text contrast (white on dark bars)
                        if (val >= 75) { $text.addClass("white"); }

                    } else {
                        $('<div class="progress-text" style="color:#94a3b8">—</div>').appendTo($td);
                    }

                    $tr.append($td);
                });

                $bodyRows.append($tr);
            });
  }

        function renderKPIs(dataScope){
        const k = computeKPIs(dataScope);
        $kpiAvg.text(pct(k.avg || 0));
        $kpiMax.text(k.max === -Infinity ? "—" : pct(k.max));
        $kpiMin.text(k.min ===  Infinity ? "—" : pct(k.min));
        $kpiMaxMeta.text(k.maxObj ? `${k.maxObj.LineCode} • ${fmtDate(k.maxObj.UsageDate)}` : "—");
        $kpiMinMeta.text(k.minObj ? `${k.minObj.LineCode} • ${fmtDate(k.minObj.UsageDate)}` : "—");
        $kpiScope.text(`${k.lineCount} lines • ${k.dateCount} days`);
        $kpiScopeMeta.text((k.firstDate !== "—" && k.lastDate !== "—") ? `${k.firstDate} → ${k.lastDate}` : "—");
  }

    function render() {
    const lineText = $("#lineFilter").val().trim().toLowerCase();
    const dateText = $("#dateFilter").val();  // yyyy-mm-dd

    let dataScope = usageLineData.filter(d => {
      let keep = true;

      if (lineText) {
        keep = keep && (d.LineCode.toLowerCase().includes(lineText));
      }

      if (dateText) {
        // extracted YYYY-MM-DD from raw timestamp
        const dOnly = d.UsageDate.substring(0, 10);
        keep = keep && (dOnly === dateText);
      }

      return keep;
    });

    const lines = [...new Set(dataScope.map(x => x.LineCode))].sort();
    const dates = [...new Set(dataScope.map(x => x.UsageDate))].sort();

    clearTable();
    renderHeader(dates);

    const map = buildMap(dataScope, lines, dates);
    renderBody(map, lines, dates);
    renderKPIs(dataScope);
  }

  // Initial render
  render();

  // Filter handler
  $("#lineFilter, #dateFilter").on("input change", function () {
    render();
  });
});
}