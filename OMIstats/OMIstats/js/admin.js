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