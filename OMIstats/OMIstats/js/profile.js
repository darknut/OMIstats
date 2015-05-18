$.datepicker.regional['es'] = {
    closeText: 'Cerrar',
    prevText: '<Ant',
    nextText: 'Sig>',
    currentText: 'Hoy',
    monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
    monthNamesShort: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
    dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
    dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
    dayNamesMin: ['D', 'L', 'M', 'M', 'J', 'V', 'S'],
    weekHeader: 'Sm',
    dateFormat: 'dd/mm/yy',
    firstDay: 0,
    isRTL: false,
    showMonthAfterYear: false,
    yearSuffix: ''
};
$.datepicker.setDefaults($.datepicker.regional['es']);
$(function () {
    var txt = document.getElementById("nacimiento");
    if (txt.value == "01/01/1900")
        txt.value = "";
    $("#nacimiento").datepicker({
        changeYear: true,
        changeMonth: true,
        yearRange: OMI_minimo + ":" + OMI_maximo,
        showButtonPanel: true
    });
});

function revisa() {
    var txt = document.getElementById("nacimiento");
    if (txt.value == "")
        txt.value = "01/01/1900";
    $("#editProfile").submit();
}

function setDisponible() {
    setVisible("disponible", true);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", false);
}

function setNoDisponible() {
    setVisible("disponible", false);
    setVisible("noDisponible", true);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", false);
}

function setCambioUsuario() {
    setVisible("disponible", false);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", true);
    setVisible("alfanumerico", false);
}

function setAlfanumerico() {
    setVisible("disponible", false);
    setVisible("noDisponible", false);
    setVisible("cambioUsuario", false);
    setVisible("alfanumerico", true);
}

$(document).ready(function () {
    $("#usuarioAjax").click(function () {
        $.ajax({
            url: '/Profile/Check',
            type: 'POST',
            dataType: 'json',
            data: { usuario: document.getElementById("usuario").value },
            success: function (data) {
                if (data == "ok")
                    setDisponible();
                else if (data == "taken")
                    setNoDisponible();
                else if (data == "number")
                    setCambioUsuario();
                else if (data == "alfanumeric")
                    setAlfanumerico();
                else
                    setNoDisponible();
            },
            error: function (data) {
                setNoDisponible();
            }
        });
    });
});