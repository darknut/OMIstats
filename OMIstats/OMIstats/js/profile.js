function revisa()
{
    var txt = document.getElementById("nacimiento");
    if (txt.value == "")
        txt.value = "01/01/1900";
    $("#editProfile").submit();
}

function mostrarPassword()
{
    setVisible("enlaceCambiarPassword", false);
    setVisible("cambiarPassword", true);
}

var arrayErroresUsuario = ["disponible", "taken", "number", "alfanumeric", "size"];

function setErrorUsuario(elem)
{
    for (i = 0; i < arrayErroresUsuario.length; i++)
        setVisible(arrayErroresUsuario[i], arrayErroresUsuario[i] == elem);
}

function checarUsuario()
{
    llamadaAjax("/Profile/Check",
                { usuario: document.getElementById("usuario").value },
                function (data) { setErrorUsuario(data); },
                function (data) { setNoDisponible(); });
}

$(document).ready(function ()
{
    var txt = document.getElementById("nacimiento");
    if (txt.value == "01/01/1900")
        txt.value = "";
    setCampoFechas("nacimiento", OMI_minimo + ":" + OMI_maximo);

    if (errorUsuario !== "")
    {
        setCampoError("usuario");
        setErrorUsuario(errorUsuario);
    }

    if (passwordModificado == "True")
        mostrarPassword();

    var usuario = document.getElementById("usuario");
    if (isFinite(usuario.value))
        usuario.value = "";
});