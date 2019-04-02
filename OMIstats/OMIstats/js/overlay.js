function showOverlay(clave) {
    setVisible('overlay-container', true);
    setVisible('overlay', true);

    var tr = document.getElementById(clave);
    var tds = tr.getElementsByTagName("td");

    var container = document.getElementById('overlay-temp');
    container.textContent = tds[3].innerText;
}