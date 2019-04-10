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

function setUpOverlay(url, base, omi, tipo, problemasDia1, problemasDia2) {
    baseUrl = base;
    overlayAjax = url;
    overlayOMI = omi;
    overlayTipo = tipo;
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

    // Hacemos la llamada ajax para obtener los puntos detallados
    llamadaAjax(overlayAjax,
        { omi: overlayOMI, tipo: overlayTipo, clave: clave },
        function (data) { handleOverlayAjax(data); },
        function (data) { handleOverlayError(); });
}

function closeOverlay() {
    setVisible('overlay-container', false);
    setVisible('overlay', false);

    overlayEstado.setAttribute("src", "");
    overlayMedalla.setAttribute("src", "");
    overlayFotoImg.setAttribute("src", "");
    overlayNombre.textContent = "";
    overlayClave.textContent = "";
    puntosTotal.textContent = "";
    lugarTotal.textContent = "";

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
}

function handleOverlayAjax(data) {
    console.log(data);
}