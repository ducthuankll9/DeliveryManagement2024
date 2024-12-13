//$(document).ready(function () {
//    $('#OrderID, #TotalWeight, #OrderPrice').on('input', function () {
//        var orderId = $('#OrderID').val();
//        var weight = $('#TotalWeight').val();
//        var price = $('#OrderPrice').val();
//        $.get('/OrdersManagement/_Calculate', { orderId: orderId, weight: weight, price: price }, function (data) {
//            $('#Fee').val(data);
//        });
//    });
});


//function calculate() {
//    document.getElementById("Fee").value = "-100";

//    var orderId = document.getElementById("OrderID").value;
//    var weight = document.getElementById("TotalWeight").value;
//    var price = document.getElementById("OrderPrice").value;

    

//    $.get('/OrdersManagement/_Calculate', { orderId: orderId, weight: weight, price: price }, function (data) {
//        document.getElementById("Fee").value = data;
//    })
//}