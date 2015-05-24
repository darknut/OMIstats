function llamadaAjax(direccion, datos, funcionExito, funcionError) {
    $.ajax({
        url: direccion,
        type: 'POST',
        dataType: 'json',
        data: datos,
        success: funcionExito,
        error: funcionError
    });
}

function setCampoError(campoID) {
    var txt = document.getElementById(campoID);
    txt.className = "input-validation-error";
}