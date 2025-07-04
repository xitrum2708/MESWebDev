// FullCalendar 
function loadProPlanSchedule() {
    document.addEventListener('DOMContentLoaded', function () {
        var calendarEl = document.getElementById('calendar');

        calendar = new FullCalendar.Calendar(calendarEl, {
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

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth,resourceTimelineYear'
            },
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
                    slotMinTime: '08:00:00',
                    slotMaxTime: '17:00:00',
                    slotLabelFormat: [{ year: 'numeric', month: '2-digit', day: '2-digit' }, // yyyy/MM/dd
                    { hour: '2-digit', hour12: false } // HH
                    ]

                },
                resourceTimelineWeek: {
                    slotDuration: '01:00:00',
                    snapDuration: '00:00:01',
                    slotMinTime: '08:00:00',
                    slotMaxTime: '17:00:00',
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
                const { model, lot_no, qty } = arg.event.extendedProps;

                return {
                    html: `
                <div class="content_inside_cell">
                <strong><font class="event-text-white-shadow">${model} </font>
                    <font class="event-text-black-box"> ${qty} </font></strong>
                    <div class="event-text-outline">${lot_no}</div>
                  </div>
                `
                };
            },

            datesSet: function () {
                setTimeout(() => {
                    holidays.forEach(dateStr => {
                        //alert(dateStr);
                        // document.querySelectorAll(`[data-date='${dateStr}']`).forEach(cell => {
                        document.querySelectorAll(`[data-date='${dateStr}'], [data-date^='${dateStr}T']`)
                            .forEach(cell => {
                                cell.classList.add('fc-day-holiday');
                            });
                    });
                    $("td[data-date$='T08:00:00']").addClass("day-start-slot");
                    $("th").addClass("day-start-slot");
                    //$("th[data-date$='T08:00:00']").addClass("day-start-slot");
                }, 20); // Delay slightly to ensure DOM is rendered

            },

            // Click to go to that day
            dateClick: function (info) {
                const now = new Date().getTime();
                // If last click was within 400ms, treat as double click
                if (now - lastClickTime < 400) {
                    // Change view when clicking to a specific day
                    calendar.changeView('resourceTimelineDay', info.dateStr);
                }
                lastClickTime = now;
            },

            // Detect drag and drog
            eventDrop: function (info) {
                hasChanges = true;
            },

            // Tooltip
            eventDidMount: function (info) {
                const { line, model, lot_no, qty, bal_qty, capa_qty, is_new, is_fpp, lot_size } = info.event.extendedProps;
                const start = info.event.start;
                const end = info.event.end;
                const tooltip = document.createElement('div');
                tooltip.className = 'custom-tooltip';
                let html = `
                <div class="row">
                  <div class="col-5 text-start">Line:</div>
                  <div class="col-7 fw-bold text-start">${line}</div>
                  <div class="col-5 text-start">Model:</div>
                  <div class="col-7 fw-bold text-start">${model}</div>
                  <div class="col-5 text-start">Lot No:</div>
                  <div class="col-7 fw-bold text-start">${lot_no}</div>
                  <div class="col-5 text-start">Lot Size:</div>
                  <div class="col-7 fw-bold text-start">${lot_size}</div>
                  <div class="col-5 text-start">Balance Qty:</div>
                  <div class="col-7 fw-bold text-start">${bal_qty}</div>
                  <div class="col-5 text-start">Capacity Qty:</div>
                  <div class="col-7 fw-bold text-start">${capa_qty}</div>
                  <div class="col-5 text-start">Quantity:</div>
                  <div class="col-7 fw-bold text-start">${qty}</div>
              `;

                if (is_new) {
                    html += `
                    <div class="col-5 text-start">New:</div>
                    <div class="col-7 fw-bold text-start">Yes</div>
                    `;
                }

                if (is_fpp) {
                    html += `
                        <div class="col-5 text-start">FPP:</div>
                        <div class="col-7 fw-bold text-start">Yes</div>`;
                }
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
        calendar.render();
    });

    function formatDateTime(date) {
        const pad = n => n.toString().padStart(2, '0');
        return `${date.getFullYear()}/${pad(date.getMonth() + 1)}/${pad(date.getDate())} ` +
            `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
    }




}
