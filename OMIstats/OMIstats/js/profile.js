function revisa()
{
    setVisible("loading", true);
    var txt = document.getElementById("nacimiento");
    if (txt.value == "")
        txt.value = "01/01/1900";
}

$(document).ready(function ()
{
    var txt = document.getElementById("nacimiento");
    if (txt.value == "01/01/1900")
        txt.value = "";
    setCampoFechas("nacimiento", OMI_minimo + ":" + OMI_maximo);
});