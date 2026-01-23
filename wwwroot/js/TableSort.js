function enableTableSort(tableId) {

    const $table = $("#" + tableId);
    const $thead = $table.find("thead");
    const $tbody = $table.find("tbody");

    let sortState = {}; // asc / desc per column

    $thead.find("th").each(function (index) {

        const $th = $(this);

        // Skip checkbox / empty
        if (
            $th.find("input[type=checkbox]").length > 0 ||
            $.trim($th.text()) === ""
        ) return;

        sortState[index] = true;

        // add icon
        $th.append(' <i class="bi bi-arrow-down-up sort-icon"></i>')
            .css("cursor", "pointer");

        $th.on("click", function () {

            const asc = sortState[index];
            sortState[index] = !asc;

            let rows = $tbody.find("tr").toArray();

            rows.sort(function (a, b) {

                let A = $(a).children("td").eq(index).text().trim();
                let B = $(b).children("td").eq(index).text().trim();

                let valA = parseValue(A);
                let valB = parseValue(B);

                if (valA.type === "number" && valB.type === "number") {
                    return asc ? valA.value - valB.value : valB.value - valA.value;
                }

                if (valA.type === "date" && valB.type === "date") {
                    return asc ? valA.value - valB.value : valB.value - valA.value;
                }

                // fallback string
                return asc
                    ? A.localeCompare(B, undefined, { numeric: true })
                    : B.localeCompare(A, undefined, { numeric: true });
            });

            $.each(rows, function (_, row) {
                $tbody.append(row);
            });

            // reset icons
            $thead.find(".sort-icon")
                .removeClass("bi-arrow-up bi-arrow-down")
                .addClass("bi-arrow-down-up");

            // active icon
            $th.find(".sort-icon")
                .removeClass("bi-arrow-down-up")
                .addClass(asc ? "bi-arrow-up" : "bi-arrow-down");
        });
    });
}

/* ===== Parse helper ===== */
function parseValue(text) {

    if (!text) return { type: "string", value: "" };

    // ===== NORMALIZE =====
    text = text
        .replace(/\u00A0/g, ' ')
        .replace(/\s+/g, ' ')
        .trim();

    // ===== DATE (YYYY/MM/DD or YYYY-MM-DD) =====
    let m = text.match(/^(\d{4})[\/\-](\d{2})[\/\-](\d{2})$/);
    if (m) {
        let y = parseInt(m[1]);
        let mo = parseInt(m[2]) - 1;
        let d = parseInt(m[3]);
        let ts = Date.UTC(y, mo, d);
        return { type: "date", value: ts };
    }

    // ===== DATETIME =====
    let dt = text.match(/^(\d{4})[\/\-](\d{2})[\/\-](\d{2})\s+(\d{2}):(\d{2})(?::(\d{2}))?$/);
    if (dt) {
        let ts = Date.UTC(
            dt[1],
            dt[2] - 1,
            dt[3],
            dt[4],
            dt[5],
            dt[6] || 0
        );
        return { type: "date", value: ts };
    }

    // ===== NUMBER =====
    let num = parseFloat(text.replace(/,/g, ""));
    if (!isNaN(num)) {
        return { type: "number", value: num };
    }

    // ===== STRING =====
    return { type: "string", value: text.toLowerCase() };
}
