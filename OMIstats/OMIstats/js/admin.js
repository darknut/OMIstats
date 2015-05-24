function aprobar(fila)
{
    llamadaAjax("/Request/Aprove",
        { clave: fila },
        function (data) { ocultaFila(data, fila); },
        function (data) { ocultaFila("error", fila); });
}