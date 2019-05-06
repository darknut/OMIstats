var baseUrl = "";
var overlayNombre = null;
var overlayClave = null;
var overlayEstado = null;
var overlayFotoImg = null;
var overlayMedalla = null;
var puntosTotal = null;
var lugarTotal = null;
var puntosD1 = null;
var lugarD1 = null;
var puntosD2 = null;
var lugarD2 = null;
var puntosD1P = [];
var lugarD1P = [];
var puntosD2P = [];
var lugarD2P = [];
var overlayAjax = "";
var overlayTipo = ""
var overlayOMI = "";

var overlayProblemasDia1 = 0;
var overlayProblemasDia2 = 0;
var overlayCompetidores = 0;
var SECONDS_PER_TICK = 60;
var MAX_SECONDS = 60 * 60 * 5;
var overlayData = null;

function setUpOverlay(url, base, omi, tipo, problemasDia1, problemasDia2, noCompetidores) {
    baseUrl = base;
    overlayAjax = url;
    overlayOMI = omi;
    overlayTipo = tipo;
    overlayCompetidores = noCompetidores;
    overlayNombre = document.getElementById('overlay-nombre');
    overlayClave = document.getElementById('overlay-clave');
    overlayEstado = document.getElementById('overlay-estado');
    overlayFotoImg = document.getElementById('overlay-foto-img');
    overlayMedalla = document.getElementById('overlay-medalla');
    puntosTotal = document.getElementById('puntos');
    lugarTotal = document.getElementById('lugar');

    overlayProblemasDia1 = problemasDia1;
    overlayProblemasDia2 = problemasDia2;

    if (problemasDia2 > 0) {
        puntosD1 = document.getElementById('puntosD1');
        lugarD1 = document.getElementById('lugarD1');

        puntosD2 = document.getElementById('puntosD2');
        lugarD2 = document.getElementById('lugarD2');
    }

    for (var i = 1; i <= problemasDia1; i++) {
        puntosD1P.push(document.getElementById('puntosD1P' + i));
        lugarD1P.push(document.getElementById('lugarD1P' + i));
    }

    for (var i = 1; i <= problemasDia1; i++) {
        puntosD2P.push(document.getElementById('puntosD2P' + i));
        lugarD2P.push(document.getElementById('lugarD2P' + i));
    }
}

function showOverlay(clave) {
    setVisible('overlay-container', true);
    setVisible('overlay', true);

    var tr = document.getElementById(clave);
    var tds = tr.getElementsByTagName("td");

    // Sacamos el nombre de la tabla
    overlayNombre.textContent = tds[3].innerText;

    // Sacamos la clave de la tabla
    overlayClave.textContent = tds[4].innerText;

    // Sacamos el estado de la URL de la clave
    var enlace = tds[4].getElementsByTagName("a")[0];
    var href = enlace.getAttribute("href");
    var estado = href.split("=")[2];

    overlayEstado.setAttribute("src", baseUrl + "img/estados/" + estado + ".png");

    // Sacamos la medalla actual
    var medalla = tds[tds.length - 2].innerText;
    if (medalla != "- - -") {
        overlayMedalla.setAttribute("src", baseUrl + "img/" + medalla + ".png");
    }

    // Sacamos el id de la posible foto
    var fotoId = tr.getAttribute("foto");
    overlayFotoImg.setAttribute("src", baseUrl + fotoId);

    // Puntos y lugares totales
    puntosTotal.textContent = tds[tds.length - 3].innerText;
    lugarTotal.textContent = tds[1].innerHTML;

    // Puntos por día
    if (puntosD1 != null) {
        puntosD1.textContent = tds[5 + overlayProblemasDia1].innerHTML;
        puntosD2.textContent = tds[tds.length - 4].innerText;
    }

    // Puntos por problema
    for (var i = 0; i < overlayProblemasDia1; i++) {
        puntosD1P[i].textContent = tds[5 + i].innerHTML;
    }

    for (var i = 0; i < overlayProblemasDia2; i++) {
        puntosD2P[i].textContent = tds[6 + overlayProblemasDia1 + i].innerHTML;
    }

    // Hacemos visible el cosito de cargando
    setVisible('overlayLoading', true);

    // Hacemos la llamada ajax para obtener los puntos detallados
    llamadaAjax(overlayAjax,
        { omi: overlayOMI, tipo: overlayTipo, clave: clave },
        function (data) { handleOverlayAjax(data); },
        function (data) { handleOverlayError(); });
}

function closeOverlay() {
    setVisible('overlay-container', false);
    setVisible('overlay', false);
    setVisible('overlayLoading', false);
    setVisible('chartPuntos', false);

    overlayEstado.setAttribute("src", "");
    overlayMedalla.setAttribute("src", "");
    overlayFotoImg.setAttribute("src", "");
    overlayNombre.textContent = "";
    overlayClave.textContent = "";
    puntosTotal.textContent = "";
    lugarTotal.textContent = "";
    overlayData = null;

    if (puntosD1 != null) {
        puntosD1.textContent = "";
        lugarD1.textContent = "";

        puntosD2.textContent = "";
        lugarD2.textContent = "";
    }

    for (var i = 0; i < overlayProblemasDia1; i++) {
        puntosD1P[i].textContent = "";
        lugarD1P[i].textContent = "";
    }

    for (var i = 0; i < overlayProblemasDia2; i++) {
        puntosD2P[i].textContent = "";
        lugarD2P[i].textContent = "";
    }

    destruyeChart();
}

function dibujaGrafica(puntos, tiempos, canvas, maxY) {
    var tiempo = 0;
    var labels = [];
    var linea = [];
    var colors = [];
    var allLabels = [];
    var i = 0;
    var maxTiempo = MAX_SECONDS > tiempos[tiempos.length - 1] ? MAX_SECONDS : tiempos[tiempos.length - 1];
    if (maxTiempo % SECONDS_PER_TICK != 0)
        maxTiempo += SECONDS_PER_TICK;
    while (true)
    {
        var avanzo = false;
        while (true) {
            if (tiempo == 0)
                break;
            if (i == tiempos.length - 1)
                break;
            if (tiempo < tiempos[i + 1])
                break;
            i++;
            avanzo = true;
        }

        var timestamp = Math.floor(tiempo / 60);
        var minutos = timestamp % 60;
        var extra = "";

        if (minutos < 10)
            extra = "0";
        if (avanzo || tiempo % 3600 == 0) {
            labels.push(Math.floor(timestamp / 60) + ":" + extra + minutos);
            if (tiempo % 3600 == 0) {
                colors.push("gray");
            } else {
                colors.push("#EEEEEE");
            }
        } else {
            labels.push("");
            colors.push("");
        }
        if (avanzo || i != tiempos.length - 1)
            linea.push(puntos[i]);
        allLabels.push(Math.floor(timestamp / 60) + ":" + extra + minutos);

        tiempo += SECONDS_PER_TICK;

        if (tiempo > maxTiempo && tiempo > tiempos[i]) {
            break;
        }
    }

    cargaGrafica(canvas, [linea], ['Puntos'], labels, maxY, colors, allLabels);

    // Cambiamos la visibilidad del canvas
    setVisible(canvas, true);
}

function muestraChartTotal() {
    destruyeChart();
    // Dibujamos las gráficas, primero la de los puntos totales
    dibujaGrafica(overlayData.puntosD1.puntos, overlayData.puntosD1.timestamp, 'chartPuntos', (overlayProblemasDia1 + overlayProblemasDia2) * 100);
}

function handleOverlayAjax(data) {
    overlayData = data;

    // Ponemos los lugares en la tabla
    for (var i = 0; i < overlayProblemasDia1; i++) {
        lugarD1P[i].textContent = data.problemas[i] == 0 ? overlayCompetidores : data.problemas[i];
    }

    if (overlayProblemasDia2 > 0) {
        lugarD1.textContent = data.problemas[overlayProblemasDia1] == 0 ? overlayCompetidores : data.problemas[overlayProblemasDia1];

        for (var i = 0; i < overlayProblemasDia2; i++) {
            lugarD2P[i].textContent = data.problemas[overlayProblemasDia1 + i + 1] == 0 ? overlayCompetidores : data.problemas[overlayProblemasDia1 + i + 1];
        }

        lugarD2.textContent = data.problemas[overlayProblemasDia1 + overlayProblemasDia2 + 1] == 0 ? overlayCompetidores : data.problemas[overlayProblemasDia1 + overlayProblemasDia2 + 1];
    }

    if (data.puntosD1 != null && data.puntosD1.puntos.length > 0) {
        muestraChartTotal();
    }
    setVisible('overlayLoading', false);
}