$(function () {
    if ($("#startDate").length > 0) {
        $("#startDate").datepicker({
            dateFormat: "yy/mm/dd",
            changeMonth: true,
            firstDay: 1, // Monday as the first day of the week
            changeYear: true,
            onClose: function (selectedDate) {
                //Set the minDate of endDate to the selected date
                $("#endDate").datepicker("option", "minDate", selectedDate);
                //Set the maxDate of endDate to one month after the selected date
                //var maxDate = new Date(selectedDate);
                //maxDate.setMonth(maxDate.getMonth() + 1);
                //$("#endDate").datepicker("option", "maxDate", maxDate);
            }
        });


        $("#endDate").datepicker({
            dateFormat: "yy/mm/dd",
            changeMonth: true,
            firstDay: 1, // Monday as the first day of the week
            changeYear: true,
            onClose: function (selectedDate) {
                // Set the maxDate of startDate to the selected date
                $("#startDate").datepicker("option", "maxDate", selectedDate);
            }
        });
    }


    if ($(".input-group-text").length > 0) {
        $(".input-group-text").click(function () {
            var $input = $(this).closest('div').find('input');
            $input.focus();
        });
    }
});