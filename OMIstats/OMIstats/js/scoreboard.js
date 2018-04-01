var lastServerUpdate = 0;
var ajaxUrl;
var omi;
var tipo;
var dia;
var problemas;
var ticks;
var intervalHandler;
var lastPing = 0;

function setVisibility(id, status) {
    document.getElementById(id).style.display = status;
}

function setTimes(server) {
    lastServerUpdate = server;
    lastPing = 0;
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
    lastPing++;
    timeToText(document.getElementById("lastServerUpdate"), ++lastServerUpdate);
}

function callServer() {
    llamadaAjax(ajaxUrl,
        { clave: omi, tipo: tipo, ticks: ticks },
        function (data) { handleAjax(data); },
        function (data) { handleError(); });
}

function handleError() {
    if (intervalHandler != -1) {
        clearInterval(intervalHandler);
        intervalHandler = -1;
    }

    setVisibility("updateContainer", "none");
    setVisibility("liveResults", "none");
    setVisibility("errorUpdateContainer", "block");

    setVisibility("retryLink", "inline");
    setVisibility("loading", "none");
}

function unhideElements() {
    setVisibility("updateContainer", "block");
    setVisibility("liveResults", "block");
    setVisibility("errorUpdateContainer", "none");
}

function finishContest() {
    clearInterval(intervalHandler);
    setVisibility("liveResults", "block");
    setVisibility("updateContainer", "none");
    setVisibility("errorUpdateContainer", "none");

    setVisibility("finished", "block");
}

function update() {
    updateTimes();
    if (lastPing > 20) {
        callServer();
        lastPing = 0;
    }
}

function handleAjax(ajax) {
    if (intervalHandler == -1) {
        startTimer();
        unhideElements();
    }

    switch (ajax.status) {
        case "UPDATED":
            {
                ticks = ajax.ticks;
                setTimes(ajax.secondsSinceUpdate);
                updatePoints(ajax.resultados);
                break;
            }
        case "NOT_CHANGED":
            {
                break;
            }
        case "FINISHED":
            {
                updatePoints(ajax.resultados);
                finishContest();
                break;
            }
        case "ERROR":
            {
                updatePoints(ajax.resultados);
                handleError();
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

        var lugarAnterior = parseInt(renglon[1].innerHTML);
        renglon[1].innerHTML = result.lugar;

        var indiceProblemas = 5;
        if (dia == 2)
            indiceProblemas += problemas + 1;

        for (var i = 0; i < problemas; i++) {
            renglon[indiceProblemas + i].innerHTML = (result.puntos[i] == null ? "-" : result.puntos[i]);
        }

        var ultimos = indiceProblemas + problemas;
        var totalAnterior = parseInt(renglon[ultimos].innerHTML);
        renglon[ultimos].innerHTML = result.totalDia;

        if (dia == 2) {
            totalAnterior = parseInt(renglon[++ultimos].innerHTML);
            renglon[ultimos].innerHTML = result.total;
        }

        var medalla = "- - -";
        if (result.medalla != "NADA")
            medalla = result.medalla;
        renglon[++ultimos].innerHTML = medalla;
        renglon[ultimos].setAttribute("medalla", result.lugar);

        var upImg = renglon[0].getElementsByClassName("up")[0];
        var downImg = renglon[0].getElementsByClassName("down")[0];

        upImg.style.display = "none";
        downImg.style.display = "none";

        if (totalAnterior > 0 || result.total > 0) {
            if (lugarAnterior > result.lugar ||
                totalAnterior == 0)
                upImg.style.display = "inline";
            else if (lugarAnterior < result.lugar)
                downImg.style.display = "inline";
        }
    });

    $("#tablaPuntos").trigger("update");
    setTimeout(function() {
        $("#tablaPuntos").trigger("sorton", [[[1, 0]]]);
    }, 0);
}

function retryAjax() {
    setVisibility("retryLink", "none");
    setVisibility("loading", "inline");
    callServer();
}