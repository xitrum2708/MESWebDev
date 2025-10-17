$(document).ajaxStart(function () {
    alert('I Love you')
    $("#loading").show();
}).ajaxStop(function () {
    $("#loading").hide();
});
$(window).on('load', function () {
    $('#loading').hide();
});
