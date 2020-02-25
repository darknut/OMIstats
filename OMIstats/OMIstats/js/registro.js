var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";
var omi = "";

function setUpAjax(url, claveEstado, claveOmi) {
    ajaxUrl = url;
    estado = claveEstado;
    omi = claveOmi;
}

function handleAjax(data) {
    setVisible("loading", false);
    setVisible("tablaRegistro", true);
    var i = 0;

    for (i = 0; i < data.length; i++) {
        var li = setVisible("resultados" + i, true);
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

    var txt = document.getElementById("nombre");
    callServer("Buscar", txt.value);
}

function muestraRegistro(tipo) {
    tipoRegistro = tipo;
    setVisible("registro", "block");
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
    callServer("Buscar", "");

    var input = document.getElementById("nombre");
    input.focus();
    input.onkeydown = function (e) {
        if (e.keyCode == 13) { // Enter
            var button = document.getElementById("buscar");
            button.click();
        }
    };
}