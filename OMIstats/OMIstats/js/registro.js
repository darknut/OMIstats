var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";

function setUpAjax(url, claveEstado) {
    ajaxUrl = url;
    estado = claveEstado;
}

function handleAjax(data) {
    alert(data);
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
        { tipo: tipoRegistro, query: query, estado: estado },
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