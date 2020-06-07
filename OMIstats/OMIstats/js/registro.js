var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";
var omi = "";
var searching = false;

function setUpAjax(url, claveEstado, claveOmi) {
    ajaxUrl = url;
    estado = claveEstado;
    omi = claveOmi;
}

function setUpSearch(tipo) {
    if (updating) {
        if (currentClave && !estado) {
            llenaClaves(currentClave.substr(0,3));
        }
    } else {
        tipoRegistro = tipo;
        setVisible("tablaRegistro", false);
        setVisible("info", false);
        setVisible("noResults", false);
        setVisible("mas10", false);

        var input = document.getElementById("nombre");
        input.focus();
        input.value = "";

        input.onkeydown = function (e) {
            if (e.keyCode == 13) { // Enter
                var button = document.getElementById("buscar");
                button.click();
            }
        };

        searching = false;

        if (estado) {
            setVisible("searching", true);
            callServer("Buscar", "");
        }
    }
}

function llenaClaves(subfijo) {
    var combo = document.getElementById("claveSelect");

    while (combo.options.length > 0) {
        combo.remove(combo.options.length - 1);
    }

    var lim = estadoSede == subfijo ? 8 : 4;
    for (i = 1; i <= lim; i++) {
        var opt = document.createElement('option');

        var tempClave = subfijo + "-" + i;
        opt.text = tempClave;
        opt.value = tempClave;
        if (tempClave == currentClave)
            opt.selected = true;

        combo.add(opt, null);
    }
}

function handleAjax(data) {
    setVisible("searching", false);
    setVisible("tablaRegistro", "block");
    setVisible("errorSearching", false);
    if (searching) {
        if (data.length == 0)
            setVisible("noResults", true);
        else
            setVisible("info", "block");
    }
    var i = 0;

    for (i = 0; i < data.length; i++) {
        if (i == 10) {
            setVisible("mas10", "block");
            break;
        }
        var li = setVisible("resultados" + i, "list-item");
        var a = li.firstChild;
        a.innerHTML = data[i].nombre;
    }
    for (; i < 10; i++) {
        setVisible("resultados" + i, false);
    }
}

function handleError() {
    setVisible("searching", false);
    setVisible("tablaRegistro", false);
    setVisible("errorSearching", true);
    setVisible("noResults", false);
    setVisible("info", false);
    setVisible("mas10", false);
}

function callServer(subUrl, query) {
    llamadaAjax(ajaxUrl + subUrl,
        { omi: omi, tipo: tipoRegistro, query: query, estado: estado },
        function (data) { handleAjax(data); },
        function (data) { handleError(); });
}

function buscar() {
    setVisible("tablaRegistro", false);
    setVisible("searching", true);
    setVisible("noResults", false);
    setVisible("info", false);
    setVisible("mas10", false);
    searching = true;

    var txt = document.getElementById("nombre");
    callServer("Buscar", txt.value);
}

function eliminarUsuario(tipoOlimpiada, clave, nombre) {
    var result = confirm("¿Eliminar a " + nombre + "?");
    if (result) {
        redirige(ajaxUrl, "Eliminar?omi=" + omi + "&tipo=" + tipoOlimpiada + "&estado=" + estado + "&clave=" + clave);
    }
}

function iniciaRegistro(tipo, clave) {
    var address = "Asistente?omi=" + omi;
    if (tipo)
        address += "&tipo=" + tipo;
    if (estado)
        address += "&estado=" + estado;
    if (clave)
        address += "&clave=" + clave;
    redirige(ajaxUrl, address);
}