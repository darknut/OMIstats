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

function eliminar(fila)
{
    llamadaAjax("/Request/Delete",
            { clave: fila },
            function (data) { ocultaFila(data, fila); },
            function (data) { ocultaFila("error", fila); });

}

function aplicaErrores()
{
    if (errorImagen !== "")
    {
        setCampoError("file");
        setVisible(errorImagen, true);
    }

    if (errorUsuario !== "")
    {
        setCampoError("usuario");
        setVisible(errorUsuario, true);
    }

    if (errorMail !== "")
    {
        setCampoError("correo");
        setVisible("error_mail", true);
    }
}