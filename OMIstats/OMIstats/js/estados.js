function mostrarDelegado() {
    setVisible("enlaceCambiarDelegado", false);
    setVisible("datosDelegado", true);
}

$(document).ready(function () {
    if (delegadoModificado == "True")
        mostrarDelegado();
});