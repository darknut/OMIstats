function ocultaFila(resultado, fila)
{
    if (resultado == "ok")
        esconde("row" + fila);
    else
        setVisible("error" + fila, true);
}

function eliminar(fila)
{
    llamadaAjax("/Request/Delete",
            { clave: fila },
            function (data) { ocultaFila(data, fila); },
            function (data) { ocultaFila("error", fila); });

}