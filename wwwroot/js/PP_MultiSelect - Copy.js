// Generic reusable initializer
function initMultiSelect($button, button, $dropdown, dropdown, data, placeholder) {
    //alert('Hi there !!!');
    function bindDropdown() {
        //$dropdown.empty();
        if ($dropdown.children().length == 0) {
            $.each(data, function (_, item) {
                $dropdown.append(`
                <li>
                    <label class="dropdown-item">
                        <input type="checkbox" value="${item.value}">
                        ${item.text}
                    </label>
                </li>
            `);
            });
        }
    }

    function getSelectedValues() {
        return $dropdown
            .find("input:checked")
            .map(function () {
                return this.value;
            })
            .get();
    }

    // Bind checkbox change
    $(document).on("change", dropdown + " input[type='checkbox']", function () {
        const selected = getSelectedValues();
        $button.val(
            selected.length ? selected.join(", ") : placeholder
        );
    });

    $(document).on("click", function (e) {
        $dropdown.hide();
    });

    $(document).on("click", button, function (e) {
        e.stopPropagation();

        const dd = $(dropdown);
        dd.toggle();
    });

    // Prevent close
    $(document).on("click", dropdown, function (e) {
        e.stopPropagation();
    });

    // Initial bind
    bindDropdown();
}