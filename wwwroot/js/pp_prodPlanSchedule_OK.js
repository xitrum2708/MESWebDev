// FullCalendar 
function loadProPlanSchedule() {
    document.addEventListener('DOMContentLoaded', function () {
        var calendarEl = document.getElementById('calendar');

        /* ------+ 2025-07-08 +-------
            Pending task:
            1. save working hour settup
            2. Bind it here
            3. Calculate base on line/date/working hour        
        */


        const slotTimeMap = {
            '2025-07-05': '18:00:00',
            '2025-07-06': '19:00:00',
            '2025-07-07': '18:00:00',
            '2025-07-08': '19:00:00',
            '2025-07-09': '20:00:00',
            '2025-07-10': '21:00:00',
            '2025-07-11': '18:00:00',
            '2025-07-12': '19:00:00'
        };

        function getSlotTimeRange(dateStr) {
            const maxTime = slotTimeMap[dateStr] || '17:00:00';
            return { min: '08:00:00', max: maxTime }
        };

        function getMaxSlotTimeInWeek(startDate, endDate) {
            let maxTime = '17:00:00';
            const date = new Date(startDate);
            while (date <= new Date(endDate)) {
                let dateStr = formatDateTime2(date);// date.toISOString().slice(0, 10);
                let time = slotTimeMap[dateStr];
                if (time > maxTime) {
                    maxTime = time;
                }
                date.setDate(date.getDate() + 1);
            }
            return maxTime;
        }

        let currentDate = formatDateTime2(start_sch_dt);

        function renderCalendar(datestr) {
            const getSlotRange = getSlotTimeRange(datestr);
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
                        slotMinTime: getSlotRange.min,
                        slotMaxTime: getSlotRange.max,
                        slotLabelFormat: [{ year: 'numeric', month: '2-digit', day: '2-digit' }, // yyyy/MM/dd
                        { hour: '2-digit', hour12: false } // HH
                        ]

                    },
                    resourceTimelineWeek: {
                        slotDuration: '01:00:00',
                        snapDuration: '00:00:01',
                        slotMinTime: getSlotRange.min,
                        slotMaxTime: getSlotRange.max,
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

                datesSet: function (info) {
                    setTimeout(() => {
                        holidays.forEach(dateStr => {
                            // alert(dateStr);
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

                    const viewType = info.view.type;
                    let dateForSlot = info.startStr.slice(0, 10);
                    if (viewType === 'resourceTimelineDay') {
                        const { min, max } = getSlotTimeRange(dateForSlot);
                        calendar.setOption('slotMinTime', min);
                        calendar.setOption('slotMaxTime', max);
                    }
                    else if (viewType === 'resourceTimelineWeek') {
                        const maxTime = getMaxSlotTimeInWeek(info.start, info.end);
                        calendar.setOption('slotMaxTime', maxTime);

                        setTimeout(() => {
                            document.querySelectorAll("td[data-date]").forEach(cell => {
                                const fullDate = cell.getAttribute("data-date");
                                const dateStr = fullDate.slice(0, 10);
                                const timeStr = fullDate.slice(11);
                                const maxAllowedTime = slotTimeMap[dateStr] || '17:00:00';

                                if (timeStr >= maxAllowedTime && !cell.classList.contains('fc-day-holiday')) {
                                    cell.classList.add("fc-slot-disabled");

                                    // Also disable the matching header time cell
                                    document.querySelectorAll(`th[data-time='${timeStr}']`).forEach(th => {
                                        th.classList.add("fc-slot-header-disabled");
                                    });
                                }
                            });
                        }, 50); // Wait for DOM render
                    }

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
                                    <div class="col-7 fw-bold text-start">Yes</div>
                                `;
                    }
                    html += `
                                <div class="col-5 text-start">Start:</div>
                                <div class="col-7 fw-bold text-start">${formatDateTime(start)}</div>
                                <div class="col-5 text-start">End:</div>
                                <div class="col-7 fw-bold text-start">${formatDateTime(end)}</div>
                                </div>
                            `;

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
        }
        renderCalendar(currentDate);    
    });    

    function formatDateTime(date) {
        const pad = n => n.toString().padStart(2, '0');
        return `${date.getFullYear()}/${pad(date.getMonth() + 1)}/${pad(date.getDate())} ` +
            `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
    };

    function formatDateTime2(date) {
        const pad = n => n.toString().padStart(2, '0');
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
    };
}
