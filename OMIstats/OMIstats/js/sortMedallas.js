function agregaSorter() {
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