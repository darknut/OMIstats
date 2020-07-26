var tipoRegistro = "";
var ajaxUrl = "";
var estado = "";
var omi = "";
var searching = false;
var resultados = [];

function setUpAjax(url, claveEstado, claveOmi) {
    ajaxUrl = url;
    estado = claveEstado;
    omi = claveOmi;
}

function setUpSearch(tipo) {
    if (!updating || !resubmit) {
        tipoRegistro = tipo;
        setVisible("tablaRegistro", false);
        setVisible("info", false);
        setVisible("noResults", false);
        setVisible("mas10", false);

        var input = document.getElementById("nombreBuscar");
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
    resultados = data;

    for (i = 0; i < data.length; i++) {
        if (i == 10) {
            setVisible("mas10", "block");
            break;
        }
        var li = setVisible("resultados" + i, "list-item");
        var a = li.firstChild;
        a.value = data[i].clave;
        a.innerHTML = getNombreCompleto(data[i]);
    }
    for (; i < 10; i++) {
        setVisible("resultados" + i, false);
    }
}

function getNombreCompleto(p)
{
    return (p.nombre + " " + p.apellidoP + " " + p.apellidoM).trim();
}

function setCampo(id, value) {
    var input = document.getElementById(id);
    input.value = value;
}

function personaSeleccionada(a) {
    var persona = resultados.filter(function (p) {
        return p.clave == a.value;
    })[0];

    setCampo("nombre", persona.nombre);
    setCampo("apellidoPaterno", persona.apellidoP);
    setCampo("apellidoMaterno", persona.apellidoM);
    setCampo("nacimiento", persona.nacimiento);
    setCampo("correo", persona.correo);
    setCampo("celular", persona.celular);
    setCampo("telefono", persona.telefono);
    setCampo("direccion", persona.direccion);
    setCampo("omegaup", persona.omegaup);
    setCampo("emergencia", persona.emergencia);
    setCampo("parentesco", persona.parentesco);
    setCampo("telEmergencia", persona.telEmergencia);
    setCampo("medicina", persona.medicina);
    setCampo("alergias", persona.alergias);
    setCampo("genero", persona.genero);
    setCampo("persona", persona.clave);

    var button = document.getElementById("botonGuardar");
    button.focus();
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

    var txt = document.getElementById("nombreBuscar");
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

function cambiaClavesCbo() {
    var tipo = document.getElementById("tipoAsistente").value;
    var estado = document.getElementById("estado").value;
    var span = document.getElementById("campoClave");
    var combo = document.getElementById("claveSelect");

    if (tipo === "COMPETIDOR" && estado) {
        span.style.opacity = "1";
        combo.disabled = false;
        llenaClaves(estados[estado]);
    } else {
        span.style.opacity = "0";
        combo.disabled = true;
    }
}

function revisaNoVacio(campo) {
    var campoObj = document.getElementById(campo);
    if (campoObj.value == "") {
        setVisible("error" + campo, true);
        campoObj.focus();
        campoObj.classList.add("backgroundError");
        return true;
    }
    setVisible("error" + campo, false);
    campoObj.classList.remove("backgroundError");
}

function reEnable(campo) {
    var obj = document.getElementById(campo);
    if (obj.disabled) {
        obj.disabled = false;
        obj.classList.add("mockDisabled");
    }
}

function validar() {
    if (revisaNoVacio("tipoAsistente"))
        return false;
    if (revisaNoVacio("estado"))
        return false;
    if (revisaNoVacio("correo"))
        return false;
    if (revisaNoVacio("celular"))
        return false;
    if (revisaNoVacio("emergencia"))
        return false;
    if (revisaNoVacio("telEmergencia"))
        return false;

    reEnable("tipoAsistente");
    reEnable("estado");
    reEnable("tipo");

    setVisible("loading", true);
    return true;
}