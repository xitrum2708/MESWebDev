function initWorkingCalendarModel() {
    let currentYear = new Date().getFullYear();
    let currentMonth = new Date().getMonth();
    //const workingSet = new Set(workingDays); // Use the working days loaded from model
    const holidaySet = new Set(holidays); // Use the holidays loaded from model

    function renderCalendar() {
        const $calendarGrid = $('#calendarGrid2');
        $calendarGrid.empty();
        const firstDay = new Date(currentYear, currentMonth, 1);
        const startDay = firstDay.getDay();
        const daysInMonth = new Date(currentYear, currentMonth + 1, 0).getDate();
        const monthName = firstDay.toLocaleString('default', { month: 'long' });
        $('#calendarTitle2').text(`${monthName} ${currentYear}`);
        const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

        dayNames.forEach(d => $calendarGrid.append(`<div class="day-header">${d}</div>`));
        for (let i = 0; i < startDay; i++) $calendarGrid.append('<div></div>');

        for (let day = 1; day <= daysInMonth; day++) {
            const dateStr = `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
            const isChecked = holidaySet.has(dateStr);
            const $cell = $(`
                <div class="day-cell ${isChecked ? 'holiday-checked' : ''}">
                  <div class="day-number  fw-bold text-success fs-6 ">${day}</div>
                   ${isChecked ? '' :
                  `<div class="d-flex justify-content-center align-items-center workingHour">
                        <input type="number" value = "8" class="form-control p-0 ps-1 ip_workingHour" />
                        <span>H</span>
                  </div>`
                  }
                </div>
              `);
            $calendarGrid.append($cell);
        }        
    }
    function refreshCalendar() {
        //loadHolidays(currentYear, currentMonth + 1).then(renderCalendar);
        //filteredHolidays = loadHolidays(currentYear, currentMonth + 1); // assign result
        renderCalendar();
    }

    $('#prevMonth2').off().on('click', function () {
        currentMonth--;
        if (currentMonth < 0) { currentMonth = 11; currentYear--; }
        refreshCalendar();
    });

    $('#nextMonth2').off().on('click', function () {
        currentMonth++;
        if (currentMonth > 11) { currentMonth = 0; currentYear++; }
        refreshCalendar();
    });

    $('#whModal').off('shown.bs.modal').on('shown.bs.modal', refreshCalendar);

    $('.ip_workingHour').on('input', function () {
        const val = parseInt(this.value);
        if (val < 8) this.value = 8;
        if (val > 12) this.value = 12;
    });
}