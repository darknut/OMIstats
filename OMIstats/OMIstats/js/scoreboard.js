var lastServerUpdate = 0;
var lastUpdateWithServer = 0;
var ajaxUrl;
var omi;
var tipo;

function setTimes(server, page) {
    lastServerUpdate = server;
    lastUpdateWithServer = page;
    updateTimes();
}

function setUpAjax(url, olimpiada, tipoOlimpiada) {
    ajaxUrl = url;
    omi = olimpiada;
    tipo = tipoOlimpiada;
}

function startTimer() {
    setInterval(update, 1000);
}

function timeToText(element, seconds) {
    var minutes = Math.round(seconds / 60);

    var text = "";

    if (seconds < 60)
    {
        text = seconds + " segundo";
        if (seconds != 1)
            text += "s";
    }
    else
    {
        text = minutes + " minuto";
        if (minutes != 1)
            text += "s";
    }

    element.innerHTML = text;
}

function updateTimes() {
    timeToText(document.getElementById("lastServerUpdate"), ++lastServerUpdate);
    timeToText(document.getElementById("lastPageUpdate"), ++lastUpdateWithServer);
}

function callServer() {
    llamadaAjax(ajaxUrl,
        { clave: omi, tipo: tipo },
        function (data) { actualizaPuntos(data); },
        function (data) {  });
}

function update() {
    updateTimes();
    if (lastUpdateWithServer > 60) {
        callServer();
    }
}

function actualizaPuntos(data) {
    console.log(data);
    setTimes(lastServerUpdate, 0);
}