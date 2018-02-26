function ocultaFila(resultado, fila)
{
    if (resultado == "ok")
    {
        esconde("row" + fila);
        quedan = quedan - 1;
        if (quedan == 0)
            location.reload();
        else
            if (typeof calculaTotal == 'function')
                calculaTotal();
    }
    else
        setVisible("error" + fila, true);
}

function eliminar(fila, base)
{
    setVisible("loading" + fila, true);

    llamadaAjax(base + "Request/Delete",
            { clave: fila },
            function (data) { ocultaFila(data, fila); },
            function (data) { ocultaFila("error", fila); });

}