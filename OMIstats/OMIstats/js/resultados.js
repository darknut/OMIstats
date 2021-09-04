function extranjeros() {
    var value = document.getElementById("ckbExt").checked;

    var trs = document.getElementsByTagName("tr");
    for (var i = 0; i < trs.length; i++) {
        var isExt = trs[i].getAttribute("ext");
        if (isExt) {
            var row = $("#" + trs[i].getAttribute("id"));
            if (value)
                row.fadeIn(250);
            else
                row.fadeOut(250);
        }
    }
}