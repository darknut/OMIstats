function aprobar(fila)
{
    llamadaAjax("/Request/Aprove",
        { clave: fila },
        function (data) { ocultaFila(data, fila); },
        function (data) { ocultaFila("error", fila); });
}

function calculaTotal()
{
    total = total - 1;
    document.getElementById("quedan").innerHTML = quedan;
    document.getElementById("total").innerHTML = total;
}