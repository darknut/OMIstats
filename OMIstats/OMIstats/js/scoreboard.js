var lastServerUpdate = 0;
var lastUpdateWithServer = 0;
var ajaxUrl;
var omi;
var tipo;
var dia;
var problemas;
var ticks;
var intervalHandler;

function setTimes(server, page) {
    lastServerUpdate = server;
    lastUpdateWithServer = page;
    updateTimes();
}

function setUpAjax(url, olimpiada, tipoOlimpiada, d, p, t) {
    ajaxUrl = url;
    omi = olimpiada;
    tipo = tipoOlimpiada;
    dia = d;
    problemas = p;
    ticks = t;
}

function startTimer() {
    intervalHandler = setInterval(update, 1000);
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
        { clave: omi, tipo: tipo, ticks: ticks },
        function (data) { handleAjax(data); },
        function (data) { handleError(); });
}

function handleError() {
    clearInterval(intervalHandler);
    document.getElementById("updateContainer").style.display = "none";
    document.getElementById("liveResults").style.display = "none";
    document.getElementById("errorUpdateContainer").style.display = "inline";
}

function update() {
    updateTimes();
    if (lastUpdateWithServer > 60) {
        callServer();
    }
}

function handleAjax(ajax) {
    switch (ajax.status) {
        case "UPDATED":
            {
                setTimes(ajax.secondsSinceUpdate, 0);
                updatePoints(ajax.resultados);
                break;
            }
    }
}

function updatePoints(results) {
    results.forEach(function (result) {
        var renglon = document.getElementById(result.clave).getElementsByTagName("td");
        var css = "";

        switch (result.medalla) {
            case "ORO":
                css += "fondoOro";
                break;
            case "PLATA":
                css += "fondoPlata";
                break;
            case "BRONCE":
                css += "fondoBronce";
                break;
        }

        for (var i = 1; i < renglon.length; i++) {
            renglon[i].classList.remove("fondoOro");
            renglon[i].classList.remove("fondoPlata");
            renglon[i].classList.remove("fondoBronce");

            if (css.length > 0)
                renglon[i].classList.add(css);
        }

        renglon[1].innerHTML = result.lugar;

        var indiceProblemas = 5;
        if (dia == 2)
            indiceProblemas += problemas + 1;

        for (var i = 0; i < problemas; i++) {
            renglon[indiceProblemas + i].innerHTML = (result.puntos[i] == null ? "-" : result.puntos[i]);
        }

        var ultimos = indiceProblemas + problemas;
        renglon[ultimos].innerHTML = result.totalDia;

        if (dia == 2) {
            renglon[++ultimos].innerHTML = result.total;
        }

        var medalla = "- - -";
        if (result.medalla != "NADA")
            medalla = result.medalla;
        renglon[++ultimos].innerHTML = medalla;
    });
}