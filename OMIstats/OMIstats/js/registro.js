var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";

function setUpAjax(url, claveEstado) {
    ajaxUrl = url;
    estado = claveEstado;
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

function callServer(subUrl, tipo, query) {
    llamadaAjax(ajaxUrl + subUrl,
        { tipo: tipo, query: query },
        function (data) { handleAjax(data); },
        function (data) { handleError(); });
}

function muestraRegistro(tipo) {
    tipoRegistro = tipo;
    setVisible("registro", "block");
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
    callServer("Buscar", tipo, "");
}