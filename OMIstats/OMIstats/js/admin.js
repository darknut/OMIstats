function aprobar(fila, base)
{
    setVisible("loading" + fila, true);

    var control = document.getElementById("correoRespuesta" + fila);
    var mensaje = "";

    if (control != null)
        mensaje = control.value;

    llamadaAjax(base + "Request/Aprove",
        { clave: fila, mensaje: mensaje },
        function (data) { ocultaFila(data, fila); },
        function (data) { ocultaFila("error", fila); });
}

function calculaTotal()
{
    total = total - 1;
    document.getElementById("quedan").innerHTML = quedan;
    document.getElementById("total").innerHTML = total;
}

function logs(base)
{
    var count = document.getElementById("count").value;
    if (count == "")
        count = 0;
    var tipoSelect = document.getElementById("tipo");
    var tipo = tipoSelect.options[tipoSelect.selectedIndex].value;

    window.location = base + "Admin/Logs?count=" + count + "&tipo=" + tipo;
}