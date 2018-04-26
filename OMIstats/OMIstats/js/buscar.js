var base;

function preparar(url) {
    base = url;
    $(document).ready(function () {
        $('#searchbox').keypress(function (e) {
            if (e.keyCode == 13)
                $('#buscarButton').click();
        });
    });
}

function buscar() {
    var input = document.getElementById("searchbox");
    window.location = base + "Buscar?query=" + input.value;
}