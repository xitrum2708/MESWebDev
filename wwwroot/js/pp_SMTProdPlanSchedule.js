// FullCalendar 
function loadProPlanSchedule() {
    document.addEventListener('DOMContentLoaded', function () {
        var calendarEl = document.getElementById('calendar');
        //I Love you
        window.calendar = new FullCalendar.Calendar(calendarEl, {
            schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
            timeZone: 'local',
            locale: 'en-GB', // This sets the locale
            initialView: 'resourceTimelineDay',
            resourceAreaHeaderContent: 'Line No.',
            resourceAreaWidth: 'auto',
            resourceAreaWidth: '100px',
            aspectRatio: 2,
            editable: true,
            //eventResizableFromStart: false,  // can't resize from start
            //eventDurationEditable: false,  // disable resizing completely


            slotMinWidth: baseSlotWidth,


            //scrollTimeReset: false,
            //contentHeight: 'auto',
            contentHeight: 'auto', // Allow dynamic sizing
            height: 'auto',        // Allow calendar to size within container

            stickyHeaderDates: true, // sticky header

            headerToolbar: false,
            //headerToolbar: {
            //    left: 'prev,next today',
            //    center: 'title',
            //    right: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth,resourceTimelineYear'
            //},
            //NEW
            buttonText: {
                today: 'Today',
                resourceTimelineDay: '🗓️ Day',
                resourceTimelineWeek: 'Week',
                resourceTimelineMonth: 'Month',
                resourceTimelineYear: 'Year'
            },
            views: {
                resourceTimelineDay: {
                    slotDuration: '01:00:00',
                    snapDuration: '00:00:01',
                    slotMinTime: '00:00:00',
                    slotMaxTime: '24:00:00',
                    slotLabelFormat: [{ year: 'numeric', month: '2-digit', day: '2-digit' }, // yyyy/MM/dd
                    { hour: '2-digit', hour12: false } // HH
                    ]

                },
                resourceTimelineWeek: {
                    slotDuration: '01:00:00',
                    snapDuration: '00:00:01',
                    slotMinTime: '00:00:00',
                    slotMaxTime: '24:00:00',
                    slotLabelFormat: [
                        { weekday: 'short', day: '2-digit', month: '2-digit' },
                        { hour: '2-digit', hour12: false }
                    ]

                },
                resourceTimelineMonth: {
                    slotDuration: { days: 1 },
                    slotMinWidth: 120, // give room for longer text
                    slotLabelFormat: [
                        { year: 'numeric', month: '2-digit' },
                        { day: '2-digit' }
                    ]
                }
            },
            resources: resources,

            // Load events from controller
            events: events,
          
            eventContent: function (arg) {
                //const title = arg.event.title; // Correct way to access title
                const { PCBKey, Lotno, Qty } = arg.event.extendedProps;
                return {
                    html: `
                    <div class="content_inside_cell">
                    <strong><font class="event-text-white-shadow">${PCBKey} </font>
                        <font class="event-text-black-box"> ${Qty} </font></strong>
                        <div class="event-text-outline">${Lotno}</div>
                  </div>
                `
                };
            },

            //datesSet: function () {
            //    if (Array.isArray(holidays)) {
            //        setTimeout(() => {
            //            if (!Array.isArray(holidays)) return;
            //            holidays.forEach(dateStr => {
            //                //alert(dateStr);
            //                // document.querySelectorAll(`[data-date='${dateStr}']`).forEach(cell => {
            //                document.querySelectorAll(`[data-date='${dateStr}'], [data-date^='${dateStr}T']`)
            //                    .forEach(cell => {
            //                        cell.classList.add('fc-day-holiday');
            //                    });
            //            });
            //            $("td[data-date$='T08:00:00']").addClass("day-start-slot");
            //            $("th").addClass("day-start-slot");
            //            //$("th[data-date$='T08:00:00']").addClass("day-start-slot");
            //        }, 20); // Delay slightly to ensure DOM is rendered
            //    }

            //},

            // Click to go to that day
            dateClick: function (info) {
                const now = new Date().getTime();
                // If last click was within 400ms, treat as double click
                if (now - lastClickTime < 400) {
                    // Change view when clicking to a specific day
                    window.calendar.changeView('resourceTimelineDay', info.dateStr);
                }
                lastClickTime = now;
            },

            // Detect drag and drog
            eventDrop: function (info) {
                hasChanges = true;
                validateLot(info);
            },

            // Tooltip
            eventDidMount: function (info) {
                const { LineCode, Model, Lotno,PCBKey, Qty, BalQty, TargetPerHour85, LotSize } = info.event.extendedProps;
                const start = info.event.start;
                const end = info.event.end;
                const tooltip = document.createElement('div');
                tooltip.className = 'custom-tooltip';
                let html = `
                <div class="row">
                  <div class="col-5 text-start">Line:</div>
                  <div class="col-7 fw-bold text-start">${LineCode}</div>
                  <div class="col-5 text-start">Model:</div>
                  <div class="col-7 fw-bold text-start">${Model}</div>
                  <div class="col-5 text-start">Lot No:</div>
                  <div class="col-7 fw-bold text-start">${Lotno}</div>
                  <div class="col-5 text-start">PCBKey:</div>
                  <div class="col-7 fw-bold text-start">${PCBKey}</div>
                  <div class="col-5 text-start">Lot Size:</div>
                  <div class="col-7 fw-bold text-start">${LotSize}</div>
                  <div class="col-5 text-start">Balance Qty:</div>
                  <div class="col-7 fw-bold text-start">${BalQty}</div>
                  <div class="col-5 text-start">Capacity Qty:</div>
                  <div class="col-7 fw-bold text-start">${TargetPerHour85}</div>
                  <div class="col-5 text-start">Quantity:</div>
                  <div class="col-7 fw-bold text-start">${Qty}</div>
              `;

                //if (is_new) {
                //    html += `
                //    <div class="col-5 text-start">New:</div>
                //    <div class="col-7 fw-bold text-start">Yes</div>
                //    `;
                //}

                //if (is_fpp) {
                //    html += `
                //        <div class="col-5 text-start">FPP:</div>
                //        <div class="col-7 fw-bold text-start">Yes</div>`;
                //}
                html += `
                    <div class="col-5 text-start">Start:</div>
                    <div class="col-7 fw-bold text-start">${formatDateTime(start)}</div>
                    <div class="col-5 text-start">End:</div>
                    <div class="col-7 fw-bold text-start">${formatDateTime(end)}</div>
                </div>`;

                tooltip.innerHTML = html;

                document.body.appendChild(tooltip);
                document.addEventListener('mousemove', (e) => {
                    if (!info.el.contains(e.target)) {
                        tooltip.style.display = 'none';
                    }
                });
                let showTooltipTimeout;

                info.el.addEventListener('mouseenter', (e) => {
                    showTooltipTimeout = setTimeout(() => {
                        tooltip.style.visibility = 'hidden';
                        tooltip.style.display = 'block';

                        const tooltipWidth = tooltip.offsetWidth;
                        tooltip.style.left = (e.pageX - tooltipWidth - 10) + 'px';
                        tooltip.style.top = e.pageY + 'px';

                        tooltip.style.visibility = 'visible';
                    }, 100); // delay in ms
                });

                info.el.addEventListener('mouseleave', () => {
                    clearTimeout(showTooltipTimeout);
                    tooltip.style.display = 'none';
                });
            }
            
            // Detect drag & drop

        });
        window.calendar.render();
        ChangeHeaderDate();
        // Title sync
        function updateTitle() {
            document.getElementById("calendar-title").innerText =
                window.calendar.view.title;
        }

        updateTitle();
        
        window.calendar.on('datesSet', function () {
            updateTitle();
        });

        // Navigation
        document.getElementById("btnPrev").onclick = function () {
            window.calendar.prev();
            ChangeHeaderDate();
        };

        document.getElementById("btnNext").onclick = function () {
            window.calendar.next();
            ChangeHeaderDate();
        };

        document.getElementById("btnToday").onclick = function () {
            window.calendar.today();
            ChangeHeaderDate();
        };

        // Change view
        document.querySelectorAll("[data-view]").forEach(btn => {
            btn.addEventListener("click", function () {
                const viewName = this.dataset.view;
                window.calendar.changeView(viewName);
                // Only run for Day view
                if (viewName.toLowerCase().includes('day')) {
                    ChangeHeaderDate();
                }
            });
        });

        document.querySelectorAll(".view-group button")
            .forEach(btn => {
                btn.addEventListener("click", function () {

                    // find group closest
                    const group = this.closest(".view-group");

                    // Remove active
                    group.querySelectorAll(".view-group button")
                        .forEach(b => b.classList.remove("active"));

                    // Add active
                    this.classList.add("active");
                });
            });


        let slotMinutes = 60;

document.getElementById("calendar")
    .addEventListener("wheel", function (e) {

        if (!e.ctrlKey) return;

        e.preventDefault();

        if (e.deltaY < 0 && slotMinutes > 15) {
            slotMinutes /= 2;
        } else if (e.deltaY > 0 && slotMinutes < 240) {
            slotMinutes *= 2;
        }

        window.calendar.setOption("slotDuration", "00:" + slotMinutes + ":00");
    });
    });
    function formatDateTime(date) {
        //alert(date);
        return new Intl.DateTimeFormat('en-CA', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: false
        }).format(new Date(date));
    }
    function ChangeHeaderDate() {

        $('.fc-timeline-slot-cushion.fc-sticky').each(function () {

            let currentText = $(this).text().trim();

            if (/^\d{2}\/\d{2}\/\d{4}$/.test(currentText)) {

                let [dd, mm, yyyy] = currentText.split('/');
                let iso = `${yyyy}-${mm}-${dd}`;

                let parsedDate = dayjs(iso);

                if (parsedDate.isValid()) {
                    $(this).text(parsedDate.format('YYYY/MM/DD'));
                }

            }

        });
    }
    //function ChangeHeaderDate() {
    //    //dayjs.extend(dayjs_plugin_customParseFormat);
    //    dayjs.extend(dayjs_plugin_customParseFormat);
    //    //alert('Change Header Date');
    //    $('.fc-timeline-slot-cushion.fc-sticky').each(function () {


    //        let currentText = $(this).text().trim();

    //        // Exact format: 01/06/2026 only
    //        let regex = /^\d{2}\/\d{2}\/\d{4}$/;

    //        if (regex.test(currentText)) {

    //            let parsedDate = dayjs(currentText, 'DD/MM/YYYY', true);

    //            if (parsedDate.isValid()) {
    //                let formatted = parsedDate.format(dateFormat);
    //                $(this).text(formatted);
    //            }

    //        }

    //    });
    //}

    //function formatDateTime(date) {
    //    alert(date);
    //    const pad = n => n.toString().padStart(2, '0');
    //    return `${date.getFullYear()}/${pad(date.getMonth() + 1)}/${pad(date.getDate())} ` +
    //        `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
    //}




}
