// wwwroot/js/holidayCalendar.js
function initHolidayCalendarModal() {
    let currentYear = new Date().getFullYear();
    let currentMonth = new Date().getMonth();
    const holidaySet = new Set(holidays); // ← Use the holidays loaded from model
    //alert('I LOVE YOU !');
    function renderCalendar() {
        const $calendarGrid = $('#calendarGrid');
        $calendarGrid.empty();

        const firstDay = new Date(currentYear, currentMonth, 1);
        const startDay = firstDay.getDay();
        const daysInMonth = new Date(currentYear, currentMonth + 1, 0).getDate();
        const monthName = firstDay.toLocaleString('default', { month: 'long' });

        $('#calendarTitle').text(`${monthName} ${currentYear}`);

        const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        dayNames.forEach(d => $calendarGrid.append(`<div class="day-header">${d}</div>`));

        for (let i = 0; i < startDay; i++) $calendarGrid.append('<div></div>');

        for (let day = 1; day <= daysInMonth; day++) {
            const dateStr = `${currentYear}-${String(currentMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
            const isChecked = holidaySet.has(dateStr);
            const $cell = $(`
                <div class="day-cell ${isChecked ? 'holiday-checked' : ''}">
                  <div class="day-number">${day}</div>
                  <input type="checkbox" class="holiday-checkbox form-check-input" ${isChecked ? 'checked' : ''}>
                </div>
              `);
            $cell.find('input').on('change', function () {
                if ($(this).is(':checked')) {
                    holidaySet.add(dateStr);
                    $cell.addClass('holiday-checked');
                }
                else {
                    holidaySet.delete(dateStr);
                    $cell.removeClass('holiday-checked');
                }
            });
            $calendarGrid.append($cell);
        }
    }
    //function loadHolidays(year, month) {
    //    alert(holidays);
    //    return $.get(`/ProdPlan/GetHolidays?year=${year}&month=${month}`, function (data) {
    //        holidaySet.clear();
    //        data.forEach(date => holidaySet.add(date));
    //    });
    //}
    //function loadHolidays(year, month) {
    //    //alert(holidays);
    //    return $.get(`/ProdPlan/GetHolidays?year=${year}&month=${month}`, function (data) {
    //        data.forEach(date => {
    //            if (!holidaySet.has(date)) holidaySet.add(date);
    //        });
    //    });
    //}

    function loadHolidays(year, month) {
        //alert(holidays);
        return Array.from(holidaySet).filter(dateStr => {
            const date = new Date(dateStr);
            return date.getFullYear() === year && (date.getMonth() + 1) === month;
        });
    }
    function refreshCalendar() {
        //loadHolidays(currentYear, currentMonth + 1).then(renderCalendar);
        filteredHolidays = loadHolidays(currentYear, currentMonth + 1); // assign result
        renderCalendar();
    }

    $('#prevMonth').off().on('click', function () {
        currentMonth--;
        if (currentMonth < 0) { currentMonth = 11; currentYear--; }
        refreshCalendar();
    });

    $('#nextMonth').off().on('click', function () {
        currentMonth++;
        if (currentMonth > 11) { currentMonth = 0; currentYear++; }
        refreshCalendar();
    });

    $('#saveHolidays').off().on('click', function () {
        //const holidayList = Array.from(holidaySet);
        $.ajax({
            url: '/ProdPlan/SaveHolidays',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(Array.from(holidaySet)),
            success: function (data) {
                toastr.success('✅ Holidays saved successfully!');
                holidays = data;
                hasChanges = true;
            },
            error: () => alert('❌ Failed to save holidays.')
        });
    });

    $('#holidayModal').off('shown.bs.modal').on('shown.bs.modal', refreshCalendar);
}
