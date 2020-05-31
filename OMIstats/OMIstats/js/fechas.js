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
$.datepicker._generateMonthYearHeader_original = $.datepicker._generateMonthYearHeader;
$.datepicker._generateMonthYearHeader = function (inst, dm, dy, mnd, mxd, s, mn, mns) {
    var header = $($.datepicker._generateMonthYearHeader_original(inst, dm, dy, mnd, mxd, s, mn, mns)),
        years = header.find('.ui-datepicker-year');

    // reverse the years
    years.html(Array.prototype.reverse.apply(years.children()));

    // return our new html
    return $('<div />').append(header).html();
}

function setCampoFechas(ID, rango)
{
    $("#" + ID).datepicker({
        changeYear: true,
        changeMonth: true,
        yearRange: rango,
        showButtonPanel: true
    });
}