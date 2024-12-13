function updateHiddenOP(value) {
    document.getElementById("OrderPrice").value = value;
}

function loadValue() {
    var op = document.getElementById("OrderPrice");
    var oop = document.getElementById("Order_OrderPrice");
    if (op != null && oop != null) {
        op.value = oop.value;
    }
}

function editableFee() {
    var fee = document.getElementById("Fee");
    fee.removeAttribute("readonly");
}
