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
    setVisible("errorLoading", false);
}

function handleError() {
    setVisible("loading", false);
    setVisible("tablaRegistro", false);
    setVisible("errorLoading", true);
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
}