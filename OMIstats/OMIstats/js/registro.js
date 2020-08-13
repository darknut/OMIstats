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

function getDataSearch(query)
{
    return { omi: omi, tipo: tipoRegistro, query: query, estado: estado };
}

function setUpSearch(tipo) {
    if (!updating && !resubmit) {
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
            callServer("Buscar", getDataSearch(""), handleAjax, handleError);
        }
    }
    if (!updating && document.getElementById("persona").value != 0) {
        disableCampo("nombre");
        disableCampo("apellidoPaterno");
        disableCampo("apellidoMaterno");
    }
}

function generaOpcion(text, value) {
    var opt = document.createElement("option");
    opt.text = text;
    opt.value = value;
    return opt;
}

function borrarOpciones(combo) {
    while (combo.options.length > 0) {
        combo.remove(combo.options.length - 1);
    }
}

function llenaClaves(subfijo) {
    var combo = document.getElementById("claveSelect");
    borrarOpciones(combo);

    var lim = estadoSede == subfijo ? 8 : 4;
    for (i = 1; i <= lim; i++) {
        var tempClave = subfijo + "-" + i;
        var opt = generaOpcion(tempClave, tempClave);
        if (tempClave == currentClave)
            opt.selected = true;
        combo.add(opt);
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

function agregaEscuelaACombo(nombre, clave) {
    var contenido = $('#selectEscuela').find('option[value="' + clave + '"]').val();
    var escuelas = document.getElementById("selectEscuela");
    if (!contenido) {
        escuelas.add(generaOpcion(nombre, clave));
    }
    escuelas.value = clave;
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

    if (document.getElementById("tipoAsistente").value == "COMPETIDOR" &&
        document.getElementById("estado").value &&
        persona.nombreEscuela) {
        agregaEscuelaACombo(persona.nombreEscuela, persona.claveEscuela);
        setCampo("selectNivelEscolar", persona.nivelEscuela);
        setCampo("selectAnioEscolar", persona.anioEscuela);
        nombreEscuela = persona.nombreEscuela;
        claveEscuela = persona.claveEscuela;
        anioEscuela = persona.anioEscuela;
        nivelEscuela = persona.nivelEscuela;
    }

    disableCampo("nombre");
    disableCampo("apellidoPaterno");
    disableCampo("apellidoMaterno");

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

function callServer(subUrl, data, successHandler, errorHandler) {
    llamadaAjax(ajaxUrl + subUrl, data,
        function (data) { successHandler(data); },
        function (data) { errorHandler(); });
}

function buscar() {
    setVisible("tablaRegistro", false);
    setVisible("searching", true);
    setVisible("noResults", false);
    setVisible("info", false);
    setVisible("mas10", false);
    searching = true;

    var txt = document.getElementById("nombreBuscar");
    callServer("Buscar", getDataSearch(txt.value), handleAjax, handleError);
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

function preparaAjaxEscuela() {
    var tipo = document.getElementById("tipo").value;
    var estado = document.getElementById("estado").value;

    setVisible("bloqueEscuela", "block");
    setVisible("panelEscuela", false);
    setVisible("panelSpinner", true);
    setVisible("panelError", false);
    callServer("Escuelas", { tipo: tipo, estado: estado }, receiveEscuelas, errorEscuelas);
}

function receiveEscuelas(data)
{
    var tipo = data[0];
    var estado = data[1];
    var escuelas = data[2];

    if (!(document.getElementById("tipo").value == tipo &&
        document.getElementById("estado").value == estado))
        return;

    var combo = document.getElementById("selectEscuela");
    borrarOpciones(combo);
    combo.add(generaOpcion("", ""));
    for (var i = 0; i < escuelas.length; i++) {
        combo.add(generaOpcion(escuelas[i].nombre, escuelas[i].clave));
    }
    combo.add(generaOpcion("--- La escuela no está listada ---", -1));
    if (nombreEscuela) {
        agregaEscuelaACombo(nombreEscuela, claveEscuela);
        setCampo("selectNivelEscolar", nivelEscuela);
        setCampo("selectAnioEscolar", anioEscuela);
    }

    setVisible("panelEscuela", "block");
    setVisible("panelSpinner", false);
}

function errorEscuelas()
{
    setVisible("panelError", "block");
    setVisible("panelSpinner", false);
}

function cambiaClavesCbo() {
    var tipo = document.getElementById("tipoAsistente").value;
    var estado = document.getElementById("estado").value;
    var span = document.getElementById("campoClave");
    var combo = document.getElementById("claveSelect");

    if (tipo === "COMPETIDOR" && estado) {
        if (hayResultados) {
            span.style.opacity = "1";
            combo.disabled = false;
            llenaClaves(estados[estado]);
        }
        preparaAjaxEscuela();
    } else {
        if (hayResultados) {
            span.style.opacity = "0";
            combo.disabled = true;
        }
        setVisible("bloqueEscuela", false);
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
    return false;
}

function reEnable(campo) {
    var obj = document.getElementById(campo);
    if (obj.disabled) {
        obj.disabled = false;
        obj.classList.add("mockDisabled");
    }
}

function disableCampo(campo) {
    var obj = document.getElementById(campo);
    if (!obj.disabled) {
        obj.disabled = true;
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
    if (!esSuperUsuario) {
        if (revisaNoVacio("emergencia"))
            return false;
        if (revisaNoVacio("telEmergencia"))
            return false;
    }

    var tipo = document.getElementById("tipoAsistente").value;
    if (tipo == "COMPETIDOR") {
        if (revisaNoVacio("selectEscuela"))
            return false;
        var escuela = document.getElementById("selectEscuela").value;
        if (escuela == -1) {
            if (revisaNoVacio("nombreEscuela"))
                return false;
        }
        if (revisaNoVacio("selectNivelEscolar"))
            return false;
        if (revisaNoVacio("selectAnioEscolar"))
            return false;
    }

    reEnable("tipoAsistente");
    reEnable("estado");
    reEnable("tipo");
    reEnable("nombre");
    reEnable("apellidoPaterno");
    reEnable("apellidoMaterno");

    setVisible("loading", true);
    return true;
}

function onEscuelaChanged() {
    var escuela = document.getElementById("selectEscuela").value;

    if (escuela == -1) {
        setVisible("sectionNombreEscuela", "flex");
        document.getElementById("nombreEscuela").focus();
    }
    else {
        setVisible("sectionNombreEscuela", false);
    }
}

function onOMIselected() {
    var tipo = document.getElementById("tipo").value;
    var combo = document.getElementById("selectNivelEscolar");
    borrarOpciones(combo);

    if (tipo != "OMIP")
        combo.add(generaOpcion("", ""));
    combo.add(generaOpcion("Primaria", "PRIMARIA"));
    if (tipo != "OMIP") {
        combo.add(generaOpcion("Secundaria", "SECUNDARIA"));
        if (tipo != "OMIS")
            combo.add(generaOpcion("Bachillerato/Preparatoria", "PREPARATORIA"));
    }

    var tipoA = document.getElementById("tipoAsistente").value;
    var estado = document.getElementById("estado").value;

    if (tipoA == "COMPETIDOR" && estado) {
        preparaAjaxEscuela();
    }
}

function onNivelEscolar() {
    var nivel = document.getElementById("selectNivelEscolar").value;
    var combo = document.getElementById("selectAnioEscolar");
    borrarOpciones(combo);

    combo.add(generaOpcion("", ""));
    combo.add(generaOpcion("1°", "1"));
    combo.add(generaOpcion("2°", "2"));
    combo.add(generaOpcion("3°", "3"));
    if (nivel == "PRIMARIA") {
        combo.add(generaOpcion("4°", "4"));
        combo.add(generaOpcion("5°", "5"));
        combo.add(generaOpcion("6°", "6"));
    }
}