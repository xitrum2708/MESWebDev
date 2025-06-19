$(document).ready(function () {
    let scannedData = [];
    $('#lotNoSelect').change(function () {
        const lot = $(this).val();
        if (lot) {
            $.getJSON('@Url.Action("GetModelByLotNo", "TelstarAssy")', { lotNo: lot }, function (data) {
                $('#modelInput').val(data);
            });
        } else {
            $('#modelInput').val('');
        }
    });

    $('#qrCodeInput').keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();

            const qr = $(this).val().trim();
            const lot = $('#lotNoSelect').val();
            const model = $('#modelInput').val();
            const line = $('#SelectedLine').val();

            console.log('Submitting:', { qr, lot, model, line });

            if (!lot) {
                console.log('Validation failed: LotNo missing');
                alert('Please select a LotNo.');
                return;
            }
            if (!model) {
                console.log('Validation failed: Model missing');
                alert('Please ensure a valid Model is populated.');
                return;
            }
            if (!line) {
                console.log('Validation failed: Line missing');
                alert('Please select a Line.');
                return;
            }
            if (!qr) {
                console.log('Validation failed: QRCode missing');
                alert('Please enter a QRCode.');
                return;
            }

            const formData = new FormData(document.getElementById("telstarForm"));
            console.log("Form Data: ", $('#telstarForm').serialize());
            $.ajax({
                url: '/TelstarAssy/Create',
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        scannedData.push({ lot, model, line, qr });

                        $('#scannedTable tbody').append(
                            `<tr><td>${lot}</td><td>${model}</td><td>${line}</td><td>${qr}</td></tr>`
                        );

                        $('#totalByLot').text(scannedData.filter(d => d.lot === lot).length);
                        $('#totalByUser').text(scannedData.length);

                        $('#qrCodeInput').val('');
                        $('#lotNoSelect').val('');
                        $('#modelInput').val('');
                        $('#SelectedLine').val('');
                        $('#qrCodeInput').focus();
                    } else {
                        alert('Error: ' + response.errors.join(', '));
                    }
                },
                error: function (xhr) {
                    alert('Failed to submit QRCode: ' + (xhr.responseText || 'Unknown error'));
                }
            });
        }
    });
});