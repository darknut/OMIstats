var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";
var omi = "";
var searching = false;

function setUpAjax(url, claveEstado, claveOmi) {
    ajaxUrl = url;
    estado = claveEstado;
    omi = claveOmi;
}

function handleAjax(data) {
    setVisible("loading", false);
    setVisible("tablaRegistro", "block");
    setVisible("errorLoading", false);
    if (searching) {
        if (data.length == 0)
            setVisible("noResults", true);
        else
            setVisible("info", "block");
    }
    var i = 0;

    for (i = 0; i < data.length; i++) {
        if (i == 10) {
            setVisible("mas10", "block");
            break;
        }
        var li = setVisible("resultados" + i, "list-item");
        var a = li.firstChild;
        a.innerHTML = data[i].nombre;
    }
    for (; i < 10; i++) {
        setVisible("resultados" + i, false);
    }
}

function handleError() {
    setVisible("loading", false);
    setVisible("tablaRegistro", false);
    setVisible("errorLoading", true);
    setVisible("noResults", false);
    setVisible("info", false);
    setVisible("mas10", false);
}

function callServer(subUrl, query) {
    llamadaAjax(ajaxUrl + subUrl,
        { omi: omi, tipo: tipoRegistro, query: query, estado: estado },
        function (data) { handleAjax(data); },
        function (data) { handleError(); });
}

function buscar() {
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
    setVisible("noResults", false);
    setVisible("info", false);
    setVisible("mas10", false);
    searching = true;

    var txt = document.getElementById("nombre");
    callServer("Buscar", txt.value);
}

function muestraRegistro(tipo) {
    tipoRegistro = tipo;
    setVisible("registro", "block");
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
    setVisible("info", false);
    setVisible("noResults", false);
    setVisible("mas10", false);

    var input = document.getElementById("nombre");
    input.focus();
    input.value = "";

    input.onkeydown = function (e) {
        if (e.keyCode == 13) { // Enter
            var button = document.getElementById("buscar");
            button.click();
        }
    };

    searching = false;
    callServer("Buscar", "");
}