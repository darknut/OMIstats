var tipoRegistro = "";

function muestraRegistro(tipo) {
    tipoRegistro = tipo;
    setVisible("registro", true);
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
}