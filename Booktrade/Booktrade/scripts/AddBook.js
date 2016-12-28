
    $(function () {
        $('input[name="deliveryBool1"]').on('click', function () {
            if ($(this).val() == 'true') {
                $('#1t').show();
            } else {
                $('#1t').hide();
            }
        });
        $('input[name="deliveryBool2"]').on('click', function () {
            if ($(this).val() == 'true') {
                $('#2t').show();
            } else {
                $('#2t').hide();
            }
        });
        $('input[name="deliveryBool3"]').on('click', function () {
            if ($(this).val() == 'true') {
                $('#3t').show();
            } else {
                $('#3t').hide();
            }
        });
        $('input[name="deliveryBool4"]').on('click', function () {
            if ($(this).val() == 'true') {
                $('#4t').show();
            } else {
                $('#4t').hide();
            }
        });

    });
