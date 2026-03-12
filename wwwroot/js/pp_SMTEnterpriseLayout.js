$(document).ready(function () {

    const $divider = $(".divider");
    const $left = $(".panel-left");
    const $right = $(".panel-right");

    let isDragging = false;

    // Load saved width
    let savedWidth = localStorage.getItem("splitWidth");
    if (savedWidth) {
        $left.css("width", savedWidth + "%");
        $right.css("width", (100 - savedWidth) + "%");
    }

    // Start drag
    $divider.on("mousedown", function () {
        isDragging = true;
        $("body").css("cursor", "col-resize");
    });

    // Dragging
    $(document).on("mousemove", function (e) {

        if (!isDragging) return;

        let containerOffset = $(".enterprise-container").offset().left;
        let containerWidth = $(".enterprise-container").width();

        let newLeftWidth = ((e.clientX - containerOffset) / containerWidth) * 100;

        if (newLeftWidth < 20) newLeftWidth = 20;
        if (newLeftWidth > 80) newLeftWidth = 80;

        $left.css("width", newLeftWidth + "%");
        $right.css("width", (100 - newLeftWidth) + "%");

        localStorage.setItem("splitWidth", newLeftWidth);

        if (window.calendar) {
            window.calendar.updateSize();
        }
    });

    // Stop drag
    $(document).on("mouseup", function () {
        isDragging = false;
        $("body").css("cursor", "default");
    });

    // Fullscreen toggle
    $(".btn-full").on("click", function () {

        let target = $(this).data("target");
        let $panel = $("#" + target);
        let $icon = $(this).find("i");

        $panel.toggleClass("fullscreen");
        $icon.toggleClass("bi-fullscreen bi-fullscreen-exit");

        setTimeout(function () {
            if (window.calendar) {
                window.calendar.updateSize();
            }
        }, 300);
    });

});