$(function () {
    var placeholderElement = $('#modal-placeholder');
    let product_id = $("#product-id").data('product-id');
    let product_price = $("#product-id").data('product-price');
    let product_detail_amount = parseInt($("#product-detail-amount").html());

    $('button[data-toggle="order-modal"]').click(function (event) {
        var url = $(this).data('url');
        $(".success-order").remove();
        
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
            $("#ProductId").val(product_id);
            $("#product-unit-price").html(product_price);
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        var order_amount = form.find('[id="Amount"]').val();

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            placeholderElement.find('.modal-body').replaceWith(newBody);
            var isValid = newBody.find('[name="IsValid"]').val() == 'True';

            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
                $("#product-detail-title").append('<div class="success-order shadow-lg p-3 mb-5 mt-2 bg-white rounded"><h5 class="text-center text-success">Your order completed as successfully. <br/>You can look the all orders from your order list.</h5></div>');
                $("#product-detail-amount").html(product_detail_amount - order_amount);
            }
            else
            {
                $("#Amount").keyup(function() {
                    let product_unit_price = parseInt($('#product-unit-price').html());
                    $('#order-total-price').html( product_unit_price * $(this).val());
                });
            }
        });
    });
});