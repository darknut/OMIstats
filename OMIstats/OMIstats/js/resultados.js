function extranjeros() {
    var value = document.getElementById("ckbExt").checked;

    var trs = document.getElementsByTagName("tr");
    for (var i = 0; i < trs.length; i++) {
        var isExt = trs[i].getAttribute("ext");
        if (isExt) {
            setVisible(trs[i].getAttribute("id"), value ? 'table-row' : 'none');
        }
    }
}