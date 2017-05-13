function setVisible(id, visible) {
    var elem = document.getElementById(id);
    if (visible)
        elem.style.display = 'inline';
    else
        elem.style.display = 'none';
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
    window.location.href = pagina + parametro;
}