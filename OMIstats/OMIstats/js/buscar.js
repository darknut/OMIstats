var base;

function preparar(url) {
    base = url;
    $(document).ready(function () {
        $('#searchbox').keypress(function (e) {
            setVisible("errorSize", false);
            if (e.keyCode == 13)
                $('#buscarButton').click();
        });
    });
}

function buscar() {
    var input = document.getElementById("searchbox").value.trim();

    if (input.length < 3) {
        setVisible("errorSize", true);
    }
    else {
        window.location = base + "Buscar?query=" + input;
    }
}