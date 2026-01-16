//$(document).ready(function () {
function IniLineCalendar() {
    /* ================= CONFIG ================= */

    const dayOrder = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    const today = new Date();
    let year = today.getFullYear();
    let month = today.getMonth();

    const selectedDays = new Set();
    const selectedDates = new Set();

    /* ================= DOM ================= */

    const $popup = $("#calendarPopup");
    const popup = "#calendarPopup"; // calendarPopup
    const $grid = $(".calendar-grid");
    const $label = $(".cal-label");
    const $input = $("#calendarInput");

    if (!$popup.length) {
        console.error("Calendar HTML not found");
        return;
    }

    /* ================= OPEN / CLOSE ================= */

    //$(".calendar-trigger").on("click", function (e) {
    //    e.stopPropagation();
    //    loadFromInput($input.val());
    //    render();
    //    $popup.show();
    //});

    /* ================= OPEN ================= */
    $(document).on("click", ".calendar-trigger", function (e) {
        e.stopPropagation();
        loadFromInput($input.val());
        render();
        $popup.show();
    });
    ///* ================= PREVENT CLOSE WHEN CLICK INSIDE ================= */

    $(document).on("click", popup, function (e) {
        e.stopPropagation();
    });
    /* ================= CLOSE ================= */

    $(document).on("mousedown", function (e) {
        if (!$(e.target).closest("#calendarPopup, .calendar-trigger").length) {
            $("#calendarPopup").hide();
        }
    });



    //$(document).on("click", function () {
    //    $popup.hide();
    //});


    
    //$popup.on("click", function (e) {
    //    e.stopPropagation();
    //});

    ///* ================= NAV ================= */

    $(document).on("click", ".cal-prev-month", function () {
        month--; normalize(); render();
    });

    $(document).on("click", ".cal-next-month", function () {
        month++; normalize(); render();
    });

    $(document).on("click", ".cal-prev-year", function () {
        year--; render();
    });

    $(document).on("click", ".cal-next-year", function () {
        year++; render();
    });

    //$(".cal-prev-month").on("click", function () {
    //    month--; normalize(); render();
    //});

    //$(".cal-next-month").on("click", function () {
    //    month++; normalize(); render();
    //});

    //$(".cal-prev-year").on("click", function () {
    //    year--; render();
    //});

    //$(".cal-next-year").on("click", function () {
    //    year++; render();
    //});

    ///* ================= FOOTER ================= */
    $(document).on("click", ".cal-clear", function () {
            selectedDays.clear();
            selectedDates.clear();
            render();
    });
    $(document).on("click", ".cal-cancel", function () {
            $popup.hide();;
    });
    $(document).on("click", ".cal-ok", function () {
            $input.val(buildValueString());
            $popup.hide();
    });
    //$(".cal-clear").on("click", function () {
    //    selectedDays.clear();
    //    selectedDates.clear();
    //    render();
    //});

    //$(".cal-cancel").on("click", function () {
    //    $popup.hide();
    //});

    //$(".cal-ok").on("click", function () {
    //    $input.val(buildValueString());
    //    $popup.hide();
    //});

    /* ================= RENDER ================= */

    function render() {

        $grid.empty();

        const first = new Date(year, month, 1);
        const last = new Date(year, month + 1, 0);

        $label.text(first.toLocaleString("en-US", {
            month: "short",
            year: "numeric"
        }));

        /* ===== DAY HEADERS ===== */
        dayOrder.forEach(d => {
            $("<div/>", { text: d })
                .addClass("dow")
                .toggleClass("active", selectedDays.has(d))
                .appendTo($grid)
                .on("click", function () {
                    toggleDay(d);
                });
        });

        /* ===== EMPTY CELLS ===== */
        for (let i = 0; i < first.getDay(); i++) {
            $("<div/>").addClass("disabled").appendTo($grid);
        }

        /* ===== DATE CELLS ===== */
        for (let d = 1; d <= last.getDate(); d++) {

            const date = new Date(year, month, d);
            const key = format(date);
            const dow = dayOrder[date.getDay()];

            const isActive =
                selectedDates.has(key) ||
                selectedDays.has(dow);

            $("<div/>", { text: d })
                .addClass(dow.toLowerCase())
                .toggleClass("active", isActive)
                .appendTo($grid)
                .on("click", function () {

                    // Date overrides day
                    if (selectedDays.has(dow)) {
                        selectedDays.delete(dow);
                    }

                    selectedDates.has(key)
                        ? selectedDates.delete(key)
                        : selectedDates.add(key);

                    render();
                });
        }
    }

    /* ================= DAY TOGGLE ================= */

    function toggleDay(day) {

        if (selectedDays.has(day)) {
            selectedDays.delete(day);
        } else {
            selectedDays.add(day);
        }

        // Remove explicit dates of that day
        [...selectedDates].forEach(d => {
            const dt = parseDate(d);
            if (dayOrder[dt.getDay()] === day) {
                selectedDates.delete(d);
            }
        });

        render();
    }

    /* ================= LOAD INPUT ================= */

    function loadFromInput(value) {

        selectedDays.clear();
        selectedDates.clear();
        if (!value) return;

        value.split(";").forEach(p => {

            // Date range
            if (/^\d{4}\/\d{2}\/\d{2}-\d{4}\/\d{2}\/\d{2}$/.test(p)) {
                const [s, e] = p.split("-");
                addDateRange(s, e);
                return;
            }

            // Single date
            if (/^\d{4}\/\d{2}\/\d{2}$/.test(p)) {
                selectedDates.add(p);
                return;
            }

            // Day range or single day
            if (p.includes("-")) addDayRange(p);
            else if (dayOrder.includes(p)) selectedDays.add(p);
        });

        // Jump calendar to first date
        if (selectedDates.size) {
            const [y, m] = [...selectedDates][0].split("/").map(Number);
            year = y;
            month = m - 1;
        }
    }

    /* ================= BUILD VALUE ================= */

    function buildValueString() {
        return [compressDays(selectedDays), compressDates(selectedDates)]
            .filter(Boolean)
            .join(";");
    }

    /* ================= HELPERS ================= */

    function addDayRange(r) {
        let [a, b] = r.split("-");
        let s = dayOrder.indexOf(a);
        let e = dayOrder.indexOf(b);

        if (s === -1 || e === -1) return;

        if (s <= e) {
            for (; s <= e; s++) selectedDays.add(dayOrder[s]);
        } else {
            for (; s < 7; s++) selectedDays.add(dayOrder[s]);
            for (s = 0; s <= e; s++) selectedDays.add(dayOrder[s]);
        }
    }

    function addDateRange(a, b) {
        let d = parseDate(a), end = parseDate(b);
        while (d <= end) {
            selectedDates.add(format(d));
            d.setDate(d.getDate() + 1);
        }
    }

    function compressDays(set) {
        if (!set.size) return "";
        const idx = [...set].map(d => dayOrder.indexOf(d)).sort((a, b) => a - b);
        const out = [];
        let s = idx[0], p = idx[0];

        for (let i = 1; i < idx.length; i++) {
            if (idx[i] === p + 1) p = idx[i];
            else { out.push([s, p]); s = p = idx[i]; }
        }
        out.push([s, p]);

        return out.map(r =>
            r[0] === r[1]
                ? dayOrder[r[0]]
                : dayOrder[r[0]] + "-" + dayOrder[r[1]]
        ).join(";");
    }

    function compressDates(set) {
        if (!set.size) return "";
        const arr = [...set].sort();
        const out = [];
        let s = arr[0], p = arr[0];

        for (let i = 1; i < arr.length; i++) {
            if (isNextDay(p, arr[i])) p = arr[i];
            else { out.push([s, p]); s = p = arr[i]; }
        }
        out.push([s, p]);

        return out.map(r =>
            r[0] === r[1] ? r[0] : r[0] + "-" + r[1]
        ).join(";");
    }

    function parseDate(s) {
        const [y, m, d] = s.split("/").map(Number);
        return new Date(y, m - 1, d);
    }

    function format(d) {
        return d.getFullYear() + "/" +
            String(d.getMonth() + 1).padStart(2, "0") + "/" +
            String(d.getDate()).padStart(2, "0");
    }

    function isNextDay(a, b) {
        return parseDate(b) - parseDate(a) === 86400000;
    }

    function normalize() {
        if (month < 0) { month = 11; year--; }
        if (month > 11) { month = 0; year++; }
    }

    render();
}
