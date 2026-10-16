
/* global $, bootstrap */

/**
 * Production Capacity Management
 * ------------------------------------------------------------------
 * Client-side controller for the Production Capacity module.
 *
 * Features:
 * 1. Search production capacity records.
 * 2. Upload production capacity data from Excel.
 * 3. Add and edit production capacity records.
 * 4. Delete multiple selected records.
 * 5. Export Excel using server-side Session criteria.
 * 6. Maintain checkbox selection across paginated records.
 * 7. Display success and validation error messages.
 *
 * Architecture:
 * - jQuery handles DOM manipulation, events and AJAX requests.
 * - ASP.NET Core MVC processes requests and returns PartialViews.
 * - SQL Server performs filtering and business calculations.
 * - The existing panigation.js handles client-side pagination.
 *
 * Expected MVC responses:
 * - Success: HTML PartialView (ProdCapacity/_Result).
 * - Error: JSON containing an "error" property.
 *
 * Dependencies:
 * - jQuery
 * - Bootstrap Modal
 * - jQuery Validate / Unobtrusive Validation (optional)
 * - Toastr (optional)
 * - panigation.js
 */

(function ($) {
    'use strict';

    $(function () {

        // ============================================================
        // 1. INITIALIZATION
        // ============================================================

        const $page = $('#prodCapacityPage');

        if (!$page.length) return;

        const $result = $('#ResultArea');

        // Retrieve the ASP.NET Core antiforgery token.
        const antiforgery = $page
            .find('input[name="__RequestVerificationToken"]')
            .first().val()
            || $('input[name="__RequestVerificationToken"]').first().val();

        // Read MVC action URLs from Index.cshtml.
        const urls = {
            index: $page.attr('data-url-index'),
            search: $page.attr('data-url-search'),
            export: $page.attr('data-url-export'),
            delete: $page.attr('data-url-delete'),
            detail: $page.attr('data-url-detail'),
            upload: $page.attr('data-url-upload'),
            add: $page.attr('data-url-add'),
            edit: $page.attr('data-url-edit')
        };

        /**
         * Returns the current result table.
         *
         * Important:
         * The entire table is replaced after AJAX operations.
         * Never store the original table element permanently.
         */
        function table() {
            return $result.find('#mytable');
        }

        /**
         * Returns the antiforgery headers for POST requests.
         */
        function csrfHeaders() {
            return antiforgery
                ? { RequestVerificationToken: antiforgery }
                : {};
        }

        // ============================================================
        // 2. NOTIFICATION AND ERROR HANDLING
        // ============================================================

        /**
         * Extracts an error message from an MVC AJAX response.
         *
         * Supports:
         * - { error: "..." }
         * - { message: "..." }
         * - ASP.NET Core validation error dictionaries.
         *
         * @param {object} xhr - jQuery AJAX response object.
         * @param {string} fallback - Default error message.
         */
        function getErrorMessage(xhr, fallback) {
            const response = xhr && xhr.responseJSON;

            if (response) {
                if (typeof response.error === 'string'
                    && response.error.trim()) {
                    return response.error;
                }

                if (typeof response.message === 'string'
                    && response.message.trim()) {
                    return response.message;
                }

                // Handle ASP.NET Core ValidationProblemDetails.
                if (response.errors
                    && typeof response.errors === 'object') {

                    const messages = [];

                    $.each(response.errors, function (_, errors) {
                        if (Array.isArray(errors)) {
                            messages.push.apply(messages, errors);
                        }
                    });

                    if (messages.length) {
                        return messages.join(' ');
                    }
                }
            }

            return fallback;
        }

        /**
         * Displays a toast notification when Toastr is available.
         * Otherwise, uses the standard browser alert dialog.
         */
        function notify(message, type) {
            if (window.toastr
                && typeof window.toastr[type] === 'function') {
                window.toastr[type](message);
            } else {
                window.alert(message);
            }
        }

        /**
         * Gets or creates the global notification area.
         */
        function messageArea() {
            let $alert = $('#capacityMessage');

            if (!$alert.length) {
                $alert = $('<div>', {
                    id: 'capacityMessage',
                    class: 'alert py-2 d-none',
                    role: 'alert'
                }).prependTo(
                    $page.find('.card-body').first()
                );
            }

            return $alert;
        }

        /**
         * Displays a temporary notification above the table.
         *
         * Types: success, danger, warning.
         * The notification disappears automatically after 4 seconds.
         */
        function showCapacityMessage(message, type = 'success') {
            messageArea()
                .stop(true, true)
                .removeClass(
                    'd-none alert-success alert-danger alert-warning'
                )
                .addClass('alert-' + type)
                .text(message)
                .show()
                .delay(4000)
                .fadeOut(200);
        }

        /**
         * Clears the current notification and pending animations.
         */
        function clearCapacityMessage() {
            messageArea()
                .stop(true, true)
                .addClass('d-none')
                .hide()
                .empty();
        }

        /**
         * Displays a validation or server error inside a modal.
         *
         * The error is displayed inside the modal body so that
         * the Bootstrap overlay does not cover the message.
         */
        function showModalError(modalId, message, existingErrorId) {
            const $modal = $('#' + modalId);

            let $error = existingErrorId
                ? $('#' + existingErrorId)
                : $();

            if (!$error.length) {
                $error = $modal
                    .find('.capacity-ajax-error')
                    .first();
            }

            if (!$error.length) {
                $error = $('<div>', {
                    class: 'alert alert-danger capacity-ajax-error',
                    role: 'alert'
                }).prependTo(
                    $modal.find('.modal-body').first()
                );
            }

            $error
                .removeClass('d-none')
                .text(message)
                .show();
        }

        /**
         * Removes previous error messages from a modal.
         */
        function clearModalError(modalId) {
            const $modal = $('#' + modalId);

            $modal.find('.capacity-ajax-error').remove();

            $modal
                .find('#capacityUploadError, #capacitySearchError')
                .addClass('d-none')
                .empty();
        }

        // ============================================================
        // 3. BOOTSTRAP MODAL MANAGEMENT
        // ============================================================

        /**
         * Opens a modal.
         *
         * Uses the jQuery modal plugin when available.
         * Falls back to Bootstrap 5 Modal API.
         */
        function showModal(id) {
            const $modal = $('#' + id);

            if (!$modal.length) return;

            if (typeof $.fn.modal === 'function') {
                $modal.modal('show');
            }
            else if (window.bootstrap && bootstrap.Modal) {
                bootstrap.Modal
                    .getOrCreateInstance($modal.get(0))
                    .show();
            }
        }

        /**
         * Closes an existing modal.
         */
        function hideModal(id) {
            const $modal = $('#' + id);

            if (!$modal.length) return;

            if (typeof $.fn.modal === 'function') {
                $modal.modal('hide');
            }
            else if (window.bootstrap && bootstrap.Modal) {
                const instance = bootstrap.Modal
                    .getInstance($modal.get(0));

                if (instance) {
                    instance.hide();
                }
            }
        }

        // ============================================================
        // 4. CHECKBOX SELECTION MANAGEMENT
        // ============================================================

        /**
         * Returns all selected production capacity IDs.
         *
         * Checked records on hidden pagination pages are included
         * because those rows remain in the table DOM.
         *
         * @returns {number[]} Selected record IDs.
         */
        function selectedIds() {
            return table()
                .find('tbody .capacity-check:checked')
                .map(function () {
                    return Number($(this).val());
                })
                .get()
                .filter(function (id) {
                    return Number.isInteger(id) && id > 0;
                });
        }

        /**
         * Refreshes checkbox selection states.
         *
         * Responsibilities:
         * - Update selected record counter.
         * - Highlight selected rows.
         * - Update Select All checkbox.
         * - Handle partially selected pages.
         */
        function refreshSelection() {
            const $table = table();

            $('#selectedCapacityCount')
                .text(selectedIds().length);

            // Highlight selected records.
            $table.find('tbody tr').each(function () {
                const $row = $(this);

                $row.toggleClass(
                    'capacity-selected',
                    $row.find('.capacity-check').is(':checked')
                );
            });

            // Select All applies only to visible rows.
            const $visible = $table
                .find('tbody tr:visible .capacity-check');

            const checked = $visible.filter(':checked').length;

            $('#capacitySelectAll')
                .prop(
                    'checked',
                    $visible.length > 0
                    && checked === $visible.length
                )
                .prop(
                    'indeterminate',
                    checked > 0 && checked < $visible.length
                );
        }

        /**
         * Selects or deselects all rows on the current page.
         */
        $(document).on(
            'change',
            '#capacitySelectAll',
            function () {
                table()
                    .find('tbody tr:visible .capacity-check')
                    .prop('checked', $(this).is(':checked'));

                refreshSelection();
            }
        );

        /**
         * Updates the selected record count whenever
         * an individual checkbox changes.
         */
        $(document).on(
            'change',
            '#mytable .capacity-check',
            refreshSelection
        );

        // Refresh checkbox states after pagination changes.
        $result.on('dataChanged', refreshSelection);

        // ============================================================
        // 5. RESULT TABLE MANAGEMENT
        // ============================================================

        /**
         * Replaces the result table with HTML returned by MVC.
         *
         * Called after successful:
         * - Search
         * - Upload
         * - Add
         * - Edit
         * - Delete
         *
         * The main Index view, breadcrumb and page header
         * are never reloaded.
         *
         * @param {string} html - HTML returned by _Result.
         */
        function replaceResult(html) {
            $result.html(html);

            const $metadata = $result
                .find('.capacity-result')
                .first();

            const count = $metadata.length
                ? Number(
                    $metadata.attr('data-record-count')
                ) || 0
                : table().find('tbody tr').length;

            // Display pagination only when records exist.
            $('#prodCapacityPager').toggle(count > 0);

            // Reset selected record count.
            $('#selectedCapacityCount').text(0);

            // Notify the existing pagination component.
            $result.trigger('dataChanged');

            refreshSelection();
        }

        // ============================================================
        // 6. SEARCH MANAGEMENT
        // ============================================================

        /**
         * Loads the Search PartialView using AJAX.
         *
         * The modal is retrieved only once and reused afterwards.
         */
        function showSearch() {
            if ($('#searchModal').length) {
                showModal('searchModal');
                return;
            }

            $.ajax({
                url: urls.search,
                type: 'GET',

                success: function (html) {
                    $('#capacitySearchArea').html(html);
                    showModal('searchModal');
                },

                error: function (xhr) {
                    notify(
                        getErrorMessage(
                            xhr,
                            'Unable to load search form.'
                        ),
                        'error'
                    );
                }
            });
        }

        // Open Search modal.
        $('#btnCapacitySearch').on('click', function (e) {
            e.preventDefault();
            showSearch();
        });

        /**
         * Handles Search buttons implemented as anchors
         * or non-submit buttons.
         *
         * Native submit buttons are handled by the form itself.
         */
        $(document).on('click', '#btnSearch', function (e) {
            const $button = $(this);

            if ($button.is(
                'button:not([type]), ' +
                'button[type="submit"], ' +
                'input[type="submit"]'
            )) {
                return;
            }

            e.preventDefault();

            $('#fSearch').trigger('submit');
        });

        /**
         * Submits Search conditions to the MVC controller.
         *
         * The MVC controller passes the conditions to
         * spweb_UV_PE_GetProdCapacity.
         */
        $(document).on('submit', '#fSearch', function (e) {
            e.preventDefault();

            const $form = $(this);

            if ($form.data('submitting')) return;

            clearModalError('searchModal');

            const from = String(
                $form.find('[name="StartDate"]').val() || ''
            ).replace(/-/g, '/');

            const to = String(
                $form.find('[name="EndDate"]').val() || ''
            ).replace(/-/g, '/');

            // Client-side date range validation.
            // The server is responsible for final validation.
            if (
                /^\d{4}\/\d{2}\/\d{2}$/.test(from) &&
                /^\d{4}\/\d{2}\/\d{2}$/.test(to) &&
                from > to
            ) {
                showModalError(
                    'searchModal',
                    'From Date cannot be later than To Date.',
                    'capacitySearchError'
                );

                return;
            }

            const $button = $form.find('#btnSearch');

            $form.data('submitting', true);
            $button.prop('disabled', true);

            $.ajax({
                url: $form.attr('action') || urls.search,
                type: 'POST',
                data: $form.serialize(),
                headers: csrfHeaders(),

                success: function (html) {
                    replaceResult(html);
                    hideModal('searchModal');
                    clearCapacityMessage();
                },

                error: function (xhr) {
                    showModalError(
                        'searchModal',
                        getErrorMessage(
                            xhr,
                            'Unable to search Production Capacity.'
                        ),
                        'capacitySearchError'
                    );
                },

                complete: function () {
                    $form.removeData('submitting');
                    $button.prop('disabled', false);
                }
            });
        });

        /**
         * Resets all Search conditions and reloads
         * the default latest 1000 records.
         */
        $(document).on(
            'click',
            '#btnCapacityResetSearch',
            function (e) {
                e.preventDefault();

                const $form = $('#fSearch');

                if (!$form.length) return;

                $form.trigger('reset');

                clearModalError('searchModal');

                $form.trigger('submit');
            }
        );

        // ============================================================
        // 7. EXPORT MANAGEMENT
        // ============================================================

        /**
         * Downloads the Production Capacity Excel report.
         *
         * Export conditions are maintained by the server-side Session.
         *
         * - Default: Export all database records.
         * - After Search: Export matching Search results.
         * - After Upload: Export records from the uploaded file.
         */
        $('#btnCapacityExport').on('click', function (e) {
            e.preventDefault();

            if (urls.export) {
                window.location.assign(urls.export);
            }
        });

        // ============================================================
        // 8. ADD PRODUCTION CAPACITY
        // ============================================================

        /**
         * Opens the Add Production Capacity modal.
         */
        $('#btnCapacityAdd').on('click', function (e) {
            e.preventDefault();

            clearModalError('addModal');

            showModal('addModal');
        });

        // ============================================================
        // 9. ADD / EDIT FORM SUBMISSION
        // ============================================================

        /**
         * Submits Add and Edit forms using jQuery AJAX.
         *
         * Form data is URL-encoded using jQuery.serialize().
         * FormData is not required because these forms
         * do not contain file upload fields.
         *
         * The controller returns:
         * - HTML PartialView on success.
         * - JSON error message on failure.
         */
        $(document).on(
            'submit',
            '#addCapacityForm, #editCapacityForm',
            function (e) {
                e.preventDefault();

                const $form = $(this);

                // Prevent duplicate submissions.
                if ($form.data('submitting')) return;

                // Execute jQuery form validation if available.
                if (
                    typeof $form.valid === 'function' &&
                    !$form.valid()
                ) {
                    return;
                }

                const isEdit = $form.is('#editCapacityForm');

                const modalId = isEdit
                    ? 'editModal'
                    : 'addModal';

                const url = $form.attr('action') ||
                    (isEdit ? urls.edit : urls.add);

                const $button = $form.find('[type="submit"]');

                clearModalError(modalId);

                $form.data('submitting', true);
                $button.prop('disabled', true);

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: $form.serialize(),
                    headers: csrfHeaders(),

                    success: function (html) {
                        replaceResult(html);

                        hideModal(modalId);

                        if (!isEdit) {
                            $form.trigger('reset');
                        }

                        showCapacityMessage(
                            isEdit
                                ? 'Production Capacity updated successfully.'
                                : 'Production Capacity added successfully.',
                            'success'
                        );
                    },

                    error: function (xhr) {
                        showModalError(
                            modalId,
                            getErrorMessage(
                                xhr,
                                'Unable to save Production Capacity.'
                            )
                        );
                    },

                    complete: function () {
                        $form.removeData('submitting');
                        $button.prop('disabled', false);
                    }
                });
            }
        );

        // ============================================================
        // 10. EDIT PRODUCTION CAPACITY
        // ============================================================

        /**
         * Loads the selected production capacity record
         * into the Edit modal using AJAX.
         *
         * Since the Edit PartialView is inserted dynamically,
         * jQuery Unobtrusive Validation is initialized again.
         */
        $(document).on(
            'click',
            '#mytable .capacity-edit',
            function (e) {
                e.preventDefault();

                const id = Number(
                    $(this).attr('data-id')
                );

                if (!Number.isInteger(id) || id <= 0) {
                    return;
                }

                $.ajax({
                    url: urls.detail,
                    type: 'GET',
                    data: { id: id },

                    success: function (html) {
                        $('#capacityEditArea').html(html);

                        const $form = $('#editCapacityForm');

                        // Reinitialize validation for dynamic content.
                        if (
                            $form.length &&
                            $.validator &&
                            $.validator.unobtrusive
                        ) {
                            $form
                                .removeData('validator')
                                .removeData('unobtrusiveValidation');

                            $.validator.unobtrusive.parse($form);
                        }

                        showModal('editModal');
                    },

                    error: function (xhr) {
                        notify(
                            getErrorMessage(
                                xhr,
                                'Unable to load edit form.'
                            ),
                            'error'
                        );
                    }
                });
            }
        );

        // ============================================================
        // 11. UPLOAD PRODUCTION CAPACITY
        // ============================================================

        /**
         * Opens the Excel Upload modal.
         */
        $('#btnCapacityUpload').on('click', function (e) {
            e.preventDefault();

            clearModalError('uploadModal');

            showModal('uploadModal');
        });

        /**
         * Uploads an Excel file asynchronously.
         *
         * FormData is required because the request contains
         * binary Excel file content.
         *
         * Validation:
         * - A file must be selected.
         * - Allowed extensions: .xlsx, .xls.
         * - Maximum size: 20 MB.
         *
         * On successful upload:
         * - Replace the result table.
         * - Refresh pagination.
         * - Close the modal.
         * - Display a success message.
         *
         * On failure:
         * - Keep the modal open.
         * - Display the validation error inside the modal.
         */
        $(document).on(
            'submit',
            '#UploadCapacityForm',
            function (e) {
                e.preventDefault();

                const $form = $(this);

                if ($form.data('submitting')) return;

                clearCapacityMessage();
                clearModalError('uploadModal');

                // Retrieve the selected file through jQuery.
                const files = $form
                    .find('input[type="file"]')
                    .first()
                    .prop('files');

                const file = files && files.length
                    ? files[0]
                    : null;

                if (!file) {
                    showModalError(
                        'uploadModal',
                        'Please select an Excel file.',
                        'capacityUploadError'
                    );

                    return;
                }

                // Validate extension and size.
                if (
                    !/\.(xlsx|xls)$/i.test(file.name) ||
                    file.size > 20 * 1024 * 1024
                ) {
                    showModalError(
                        'uploadModal',
                        'Please select an .xlsx or .xls file smaller than 20 MB.',
                        'capacityUploadError'
                    );

                    return;
                }

                // FormData is necessary for binary file uploads.
                const formData = new FormData($form.get(0));

                if (
                    antiforgery &&
                    !formData.has('__RequestVerificationToken')
                ) {
                    formData.append(
                        '__RequestVerificationToken',
                        antiforgery
                    );
                }

                const $button = $('#btnUploadCapacity');

                $form.data('submitting', true);
                $button.prop('disabled', true);

                $.ajax({
                    url: $form.attr('action') || urls.upload,
                    type: 'POST',
                    data: formData,

                    // Required for multipart/form-data uploads.
                    processData: false,
                    contentType: false,

                    headers: csrfHeaders(),

                    success: function (html) {
                        replaceResult(html);

                        hideModal('uploadModal');

                        $form.trigger('reset');

                        clearModalError('uploadModal');

                        showCapacityMessage(
                            'File uploaded and processed successfully.',
                            'success'
                        );
                    },

                    error: function (xhr) {
                        showModalError(
                            'uploadModal',
                            getErrorMessage(
                                xhr,
                                'Upload failed.'
                            ),
                            'capacityUploadError'
                        );
                    },

                    complete: function () {
                        $form.removeData('submitting');
                        $button.prop('disabled', false);
                    }
                });
            }
        );

        // ============================================================
        // 12. DELETE PRODUCTION CAPACITY
        // ============================================================

        /**
         * Opens the Delete confirmation modal.
         *
         * The modal displays the total number of selected records,
         * including records selected on other pagination pages.
         */
        $('#btnCapacityDelete').on('click', function (e) {
            e.preventDefault();

            const ids = selectedIds();

            if (!ids.length) {
                notify(
                    'Please select at least one record.',
                    'warning'
                );

                return;
            }

            $('#capacityDeleteCount').text(ids.length);

            clearModalError('deleteModal');

            showModal('deleteModal');
        });

        /**
         * Deletes selected production capacity records.
         *
         * The selected IDs are sent as a JSON array to the MVC
         * controller with the ASP.NET Core antiforgery header.
         *
         * On success, the controller returns an updated _Result
         * PartialView instead of a JSON success flag.
         */
        $(document).on(
            'click',
            '#confirmDeleteCapacity',
            function (e) {
                e.preventDefault();

                const ids = selectedIds();

                if (!ids.length) return;

                const $button = $(this);

                if ($button.prop('disabled')) return;

                $button.prop('disabled', true);

                clearModalError('deleteModal');

                $.ajax({
                    url: urls.delete,
                    type: 'POST',

                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(ids),

                    headers: csrfHeaders(),

                    success: function (html) {
                        replaceResult(html);

                        hideModal('deleteModal');

                        showCapacityMessage(
                            'Selected records deleted successfully.',
                            'success'
                        );
                    },

                    error: function (xhr) {
                        showModalError(
                            'deleteModal',
                            getErrorMessage(
                                xhr,
                                'Unable to delete records.'
                            )
                        );
                    },

                    complete: function () {
                        $button.prop('disabled', false);
                    }
                });
            }
        );

    });
})(jQuery);
