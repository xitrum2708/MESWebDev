// ----- Hide and show dropdown here ----
jQuery(function ($) {
    

    $(document).ready(function () {
        //$("#thirdDiv").click(function() {
        //  $(this).toggleClass("additional-class", true); // Only add the class
        //});

        //--- Paging table
        // Click next or prev
        $('table.paginated').each(function () {
            //debugger
            panigation();
        });

        // Select Number per page change
        //$('.page-select').on('change', function (e) {
        //    panigation();
        //});

        $(document).on('change', '.page-select', function () {
            panigation();
        });

        // table bind data
        if ($('.result-area').length > 0) {
            //alert('Data area found, binding data changed event!');
            $(document).on('dataChanged', '.result-area', function () {
                $('table.paginated').each(function () {
                    //alert('abc');
                    panigation();
                });
            })
        }

        //$(".result-area").on("dataChanged", function () {
        //    alert("Div content has been updated.");
        //    // Additional actions can be performed here
        //});
        function panigation() {
            //alert('dfsssssssssssssss');
            var currentPage = 0;
            var numPerPage = parseInt($('.page-select').val());
            var $table = $('table.paginated');
            $table.bind('repaginate', function () {
                $table.find('tbody tr').hide().slice(currentPage * numPerPage, (currentPage + 1) * numPerPage).show();
            });
            $table.trigger('repaginate');
            var numRows = $table.find('tbody tr').length;
            var numPages = Math.ceil(numRows / numPerPage);

            $('.page-total').html('Page total: ' + numPages);
            $('.page-rows').html('Item total: ' + numRows);
            if ($('.pager').length) {
                $('.page-total').next('.pager').remove();
            }

            var $pager = $('<div class="pager"></div>');
            var $previous = $('<span class="previous fs-6"><i class="bi bi-caret-left-square"></i></span>');
            var $next = $('<span class="next"><i class="bi bi-caret-right-square"></i></span>');
            for (var page = 0; page < numPages; page++) {
                $('<span class="page-number"></span>').text(page + 1).bind('click', {
                    newPage: page
                }, function (event) {
                    currentPage = event.data['newPage'];
                    $table.trigger('repaginate');
                    $(this).addClass('active').siblings().removeClass('active');

                }).appendTo($pager).addClass('clickable');
            }

            //$pager.insertBefore($table).find('span.page-number:first').addClass('active');
            $pager.insertAfter('span.page-total').find('span.page-number:first').addClass('active');

            $previous.insertBefore('span.page-number:first');
            $next.insertAfter('span.page-number:last');

            $next.click(function (e) {
                $previous.addClass('clickable');
                $pager.find('.active').next('.page-number.clickable').click();
            });
            $previous.click(function (e) {
                $next.addClass('clickable');
                $pager.find('.active').prev('.page-number.clickable').click();
            });
            $table.on('repaginate', function () {
                $next.addClass('clickable');
                $previous.addClass('clickable');
                $next.show();
                $previous.show();

                setTimeout(function () {
                    var $active = $pager.find('.page-number.active');
                    if ($active.next('.page-number.clickable').length === 0) {
                        $next.removeClass('clickable');
                        $next.hide();
                    } else if ($active.prev('.page-number.clickable').length === 0) {
                        $previous.removeClass('clickable');
                        $previous.hide();
                    }
                });
            });
            $table.trigger('repaginate');
        }

    });
});


