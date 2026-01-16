function initMultiSelect($container, data, placeholder) {

    const $button = $container.find(".multi-select-btn");
    const $dropdown = $container.find(".multi-select-menu");

    /* ===== bind data once ===== */
    if (!$dropdown.children().length) {
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

    /* ===== toggle dropdown + sync ===== */
    $button.off("click").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        $(".multi-select-menu").not($dropdown).hide();
        $dropdown.toggle();

        //  SYNC CHECKBOX WITH INPUT VALUE
        const selectedValues = $button.val();
        if (!selectedValues || selectedValues === placeholder) return;

        const values = selectedValues.split(",").map(v => v.trim());

        $dropdown.find("input[type='checkbox']").each(function () {
            $(this).prop("checked", values.includes(this.value));
        });
    });

    /* ===== prevent close inside dropdown ===== */
    $dropdown.off("click").on("click", function (e) {
        e.stopPropagation();
    });

    /* ===== checkbox change ===== */
    $dropdown.off("change").on("change", "input[type='checkbox']", function () {
        const selected = $dropdown
            .find("input:checked")
            .map((_, el) => el.value)
            .get();

        $button.val(selected.length ? selected.join(", ") : placeholder);
    });
}

/* ===== global click close ===== */
$(document).off("click.multiSelect").on("click.multiSelect", function () {
    $(".multi-select-menu").hide();
});
