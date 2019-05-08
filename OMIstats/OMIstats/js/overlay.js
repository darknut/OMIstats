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
var medallaToGradient = ['','yellow', 'yellow', 'yellow', 'yellow', 'silver', 'brown', 'black'];

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

function dibujaGrafica(chart, puntos, tiempos, maxY, colorIndexes, valorMinimo, yInverso, labelsLineas, tituloEje, medallas, maxX, cambioDia) {
    var tiempo = 0;
    var labels = [];
    var linea = [];
    var colors = [];
    var allLabels = [];
    var gradientMedallas = [];
    var i = 0;
    var maxTiempo = MAX_SECONDS > tiempos[tiempos.length - 1] ? MAX_SECONDS : tiempos[tiempos.length - 1];
    if (maxX && maxX > maxTiempo)
        maxTiempo = maxX;
    for (var j = 0; j < puntos.length; j++)
        linea.push([]);
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

        if (tiempo % 3600 == 0) {
            colors.push("gray");
            labels.push(Math.floor(timestamp / 60) + ":" + extra + minutos);
        } else if (tiempo % 1200 == 0) {
            colors.push("#EEEEEE");
            labels.push(Math.floor(timestamp / 60) + ":" + extra + minutos);
        } else {
            colors.push("");
            labels.push("");
        }

        if (avanzo || i != tiempos.length - 1) {
            for (var j = 0; j < puntos.length; j++) {
                if (yInverso && puntos[j][i] == 0)
                    linea[j].push(maxY);
                else
                    linea[j].push(puntos[j][i]);
            }
            if (medallas) {
                gradientMedallas.push(medallaToGradient[medallas[i]]);
            }
        }
        allLabels.push(Math.floor(timestamp / 60) + ":" + extra + minutos);

        tiempo += SECONDS_PER_TICK;
        if (cambioDia)
            tiempo += SECONDS_PER_TICK;

        if (tiempo > maxTiempo && tiempo > tiempos[i]) {
            break;
        }
    }

    if (chart) {
        actualizaGrafica(chart, linea, labelsLineas, colorIndexes);
    } else {
        chart = cargaGrafica('chartPuntos', linea, labelsLineas, labels, maxY, colors, allLabels, colorIndexes, valorMinimo, yInverso, tituloEje, medallas ? gradientMedallas : null);
    }

    // Cambiamos la visibilidad del canvas
    setVisible('chartPuntos', true);

    return chart;
}

function scrollToBottom() {
    var objDiv = document.getElementById("overlay");
    objDiv.scrollTop = objDiv.scrollHeight;
}

function muestraChartTotal(ignoreScrolling) {
    destruyeChart();
    dibujaGrafica(null, [overlayData.puntosD1.puntos], overlayData.puntosD1.timestamp, (overlayProblemasDia1 + overlayProblemasDia2) * 100, [0], 0, false, ['Puntos'], 'Puntos', null, null, null);
    if (!ignoreScrolling)
        scrollToBottom();
}

function muestraChartLugar() {
    destruyeChart();
    if (overlayProblemasDia2 == 0) {
        dibujaGrafica(null, [overlayData.lugaresD1.lugar], overlayData.lugaresD1.timestamp, overlayCompetidores, [11], 1, true, ['Lugar'], 'Lugar', overlayData.lugaresD1.medalla, null, null);
    }
    else {
        var lugares = [];
        var timestamp = [];
        var medalla = [];
        var cambioDia = overlayData.lugaresD1.timestamp[overlayData.lugaresD1.timestamp.length - 1];

        for (var i = 0; i < overlayData.lugaresD1.timestamp.length; i++) {
            lugares.push(overlayData.lugaresD1.lugar[i]);
            timestamp.push(overlayData.lugaresD1.timestamp[i]);
            medalla.push(overlayData.lugaresD1.medalla[i]);
        }

        for (var i = 0; i < overlayData.lugaresD2.timestamp.length; i++) {
            lugares.push(overlayData.lugaresD2.lugar[i]);
            timestamp.push(overlayData.lugaresD2.timestamp[i] + cambioDia);
            medalla.push(overlayData.lugaresD2.medalla[i]);
        }

        cambioDia = Math.ceil(cambioDia / 60) * 60;

        dibujaGrafica(null, [lugares], timestamp, overlayCompetidores, [11], 1, true, ['Lugar'], 'Lugar', medalla, MAX_SECONDS * 2, cambioDia);
    }
    scrollToBottom();
}

function mustraChartPorDias() {
    destruyeChart();

    var time = overlayProblemasDia1;
    if (overlayProblemasDia1 < overlayProblemasDia2)
        time = overlayProblemasDia2;
    time *= 100;

    var timestamp1 = overlayData.puntosD1.timestamp[overlayData.puntosD1.timestamp.length - 1];
    var timestamp2 = overlayData.puntosD2.timestamp[overlayData.puntosD2.timestamp.length - 1];
    var maxX = timestamp1;
    if (maxX < timestamp2)
        maxX = timestamp2;

    var chart = dibujaGrafica(null, [overlayData.puntosD1.puntos], overlayData.puntosD1.timestamp, time, [1], 0, false, ['Día 1'], 'Puntos', null, maxX, null);
    dibujaGrafica(chart, [overlayData.puntosD2.puntos], overlayData.puntosD2.timestamp, time, [2], 0, false, ['Día 2'], 'Puntos', null, maxX, null);

    scrollToBottom();
}

function muestraChartProblemas() {
    var puntos = [];
    var colores = [];
    var titulos = [];

    var temp = [];
    temp.push(overlayData.puntosD1.puntosP1);
    temp.push(overlayData.puntosD1.puntosP2);
    temp.push(overlayData.puntosD1.puntosP3);
    temp.push(overlayData.puntosD1.puntosP4);
    temp.push(overlayData.puntosD1.puntosP5);
    temp.push(overlayData.puntosD1.puntosP6);

    for (var i = 0; i < overlayProblemasDia1; i++) {
        puntos.push(temp[i]);
        colores.push(i + 3);
        titulos.push(document.getElementById('nombresD1P' + (i + 1)).innerHTML.substr(36));
    }

    var timestamp1 = overlayData.puntosD1.timestamp[overlayData.puntosD1.timestamp.length - 1];
    var timestamp2 = overlayProblemasDia2 > 0 ? overlayData.puntosD2.timestamp[overlayData.puntosD2.timestamp.length - 1] : 0;
    var maxX = timestamp1;
    if (maxX < timestamp2)
        maxX = timestamp2;

    destruyeChart();
    var chart = dibujaGrafica(null, puntos, overlayData.puntosD1.timestamp, 100, colores, 0, false, titulos, 'Puntos', null, maxX, null);

    if (overlayProblemasDia2 > 0) {
        puntos = [];
        colores = [];
        titulos = [];
        temp = [];

        temp.push(overlayData.puntosD2.puntosP1);
        temp.push(overlayData.puntosD2.puntosP2);
        temp.push(overlayData.puntosD2.puntosP3);
        temp.push(overlayData.puntosD2.puntosP4);
        temp.push(overlayData.puntosD2.puntosP5);
        temp.push(overlayData.puntosD2.puntosP6);

        for (var i = 0; i < overlayProblemasDia2; i++) {
            puntos.push(temp[i]);
            colores.push(i + 3 + overlayProblemasDia1);
            titulos.push(document.getElementById('nombresD2P' + (i + 1)).innerHTML.substr(36));
        }

        dibujaGrafica(chart, puntos, overlayData.puntosD2.timestamp, 100, colores, 0, false, titulos, 'Puntos', null, maxX, null);
    }

    scrollToBottom();
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
        muestraChartTotal(true);
    }
    setVisible('overlayLoading', false);
}