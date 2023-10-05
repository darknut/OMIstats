function setVisible(id, visible) {
    var elem = document.getElementById(id);
    if (visible === true)
        elem.style.display = 'inline';
    else if (visible === false)
        elem.style.display = 'none';
    else
        elem.style.display = visible;
    return elem;
}

function esconde(id)
{
    $("#" + id).hide("slow");
}

function muestraLoadGifYSubmit(formId) {
    setVisible("loading", true);
    $("#" + formId).submit();
}

function redirige(pagina, parametro) {
    if (parametro != '-') {
        window.location.href = pagina + parametro;
    }
}

function tryConfirm(url, texto) {
    var result = confirm(texto);
    if (result) {
        window.location.href = url;
    }
}

function removeBanners() {
    setVisible("logo", false);
    setVisible("circulo", false);
    setVisible("header", false);
    document.getElementsByClassName("mainpage")[0].style.minWidth = "60px";
    document.getElementsByClassName("content")[0].style.minWidth = "60px";
}

function addFavicon(base, numero) {
    var icon = document.querySelector("link[rel~='icon']");
    icon.href = base + "favicon-" + numero + ".ico";
}