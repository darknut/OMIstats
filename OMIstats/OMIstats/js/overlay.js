var baseUrl = "";
var overlayNombre = null;
var overlayClave = null;
var overlayEstado = null;
var overlayFotoImg = null;
var overlayMedalla = null;

function setUpOverlay(base) {
    baseUrl = base;
    overlayNombre = document.getElementById('overlay-nombre');
    overlayClave = document.getElementById('overlay-clave');
    overlayEstado = document.getElementById('overlay-estado');
    overlayFotoImg = document.getElementById('overlay-foto-img');
    overlayMedalla = document.getElementById('overlay-medalla');
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
}

function closeOverlay() {
    setVisible('overlay-container', false);
    setVisible('overlay', false);

    overlayEstado.setAttribute("src", "");
    overlayMedalla.setAttribute("src", "");
    overlayFotoImg.setAttribute("src", baseUrl + "img/karel.bmp");
    overlayNombre.textContent = "";
    overlayClave.textContent = "";
}