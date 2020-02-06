var tipoRegistro = "";

function muestraRegistro(tipo) {
    tipoRegistro = tipo;
    setVisible("registro", "block");
    setVisible("tablaRegistro", false);
    setVisible("loading", true);
}