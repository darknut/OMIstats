function configurarAjax(ID, direccion, funcionDatos, funcionExito, funcionError)
{
    $("#" + ID).click(function () {
        $.ajax({
            url: direccion,
            type: 'POST',
            dataType: 'json',
            data: funcionDatos(),
            success: funcionExito,
            error: funcionError
        });
    });
}

function setCampoError(campoID) {
    var txt = document.getElementById(campoID);
    txt.className = "input-validation-error";
}