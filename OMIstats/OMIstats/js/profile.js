function revisa() {
    var txt = document.getElementById("nacimiento");
    if (txt.value == "")
        txt.value = "01/01/1900";
    $("#editProfile").submit();
}

function setDisponible() {
    setVisible("disponible", true);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", false);
    setVisible("size", false);
}

function setNoDisponible() {
    setVisible("disponible", false);
    setVisible("noDisponible", true);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", false);
    setVisible("size", false);
}

function setCambioUsuario() {
    setVisible("disponible", false);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", true);
    setVisible("alfanumerico", false);
    setVisible("size", false);
}

function setAlfanumerico() {
    setVisible("disponible", false);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", true);
    setVisible("size", false);
}

function setSize() {
    setVisible("disponible", false);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", false);
    setVisible("size", true);
}

function setErrorUsuario(errorUsuario) {
    if (errorUsuario == "ok")
        setDisponible();
    else if (errorUsuario == "taken")
        setNoDisponible();
    else if (errorUsuario == "number")
        setCambioUsuario();
    else if (errorUsuario == "alfanumeric")
        setAlfanumerico();
    else if (errorUsuario == "size")
        setSize();
    else
        setNoDisponible();
}

function getNombreUsuario() {
    return { usuario: document.getElementById("usuario").value };
}

$(document).ready(function () {
    configurarAjax("usuarioAjax", "/Profile/Check",
        function () { return getNombreUsuario() },
        function (data) { setErrorUsuario(data); },
        function (data) { setNoDisponible(); });

    var txt = document.getElementById("nacimiento");
    if (txt.value == "01/01/1900")
        txt.value = "";
    setCampoFechas("nacimiento", OMI_minimo + ":" + OMI_maximo);

    document.getElementById("genero").value = generoUsuario;

    if (errorImagen !== "")
    {
        setCampoError("file");
        if (errorImagen === "invalida")
            setVisible("imagenInvalida", true);
        if (errorImagen === "bytes")
            setVisible("imagenMuyGrande", true);
    }

    if (errorUsuario !== "")
    {
        setCampoError("usuario");
        setErrorUsuario(errorUsuario);
    }
});