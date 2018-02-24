function agregaSorterMedallas() {
    $.tablesorter.addParser({
        id: 'medalla',
        is: function (s, table, cell, $cell) {
            return false;
        },
        format: function (s, table, cell, cellIndex) {
            var $cell = $(cell);
            var data = $cell.attr('medalla');
            if (data) {
                if (data == 0)
                    return 100;
                return data;
            }
            return s;
        },
        parsed: false,
        type: 'numeric'
    });
}

function agregaSorterOMI(nombre) {
    $.tablesorter.addParser({
        id: 'omi',
        is: function (s, table, cell, $cell) {
            return false;
        },
        format: function (s, table, cell, cellIndex) {
            var $cell = $(cell);
            if ($cell.attr('omi') == '8b')
                return 8.5;
            return parseInt($cell.attr('omi'));
        },
        parsed: false,
        type: 'numeric'
    });
}

function cargaSorter(nombre) {
    $("#" + nombre).tablesorter(
    {
    });
}