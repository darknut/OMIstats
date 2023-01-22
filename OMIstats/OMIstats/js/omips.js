var BASE = 2.5;
var FACTOR_ORO = 1;
var FACTOR_PLATA = 0.5;
var FACTOR_INVERSIONES = 0.01;
var FACTOR_LUGAR_PROMEDIO = 1 / 6;
var CORTE_OROS = 5;
var CORTE_PLATAS = 10;
var NUM_PLATAS = 5;
var MINIMO = 1;
var MAXIMO = 4;
var puntos = [];

function obtenerLugar(puntos, todos)
{
    for (var i = 0; i < todos.length; i++)
    {
        if (todos[i] <= puntos)
            return i + 1;
    }
}

function calculaLugaresParaEstado(estado, clase) {
    var pequeña = clase + "O";
    if(res[estado][clase]) {
        var oros = 0;
        var platas = 0;
        var participantesTotal = 0;
        var inversiones = 0;
        var permitidos = BASE;
        Object.keys(res[estado][clase]).forEach((omi) => {
            var lugarPromedio = 0;
            var puntosTotal = 0;
            var participantes = 0;
            Object.keys(res[estado][clase][omi]).forEach((persona) => {
                if (res[estado][clase][omi][persona].medalla == 'NADA')
                    return;
                puntosTotal += res[estado][clase][omi][persona].puntos;
                participantesTotal++;
                participantes++;
                if (res[estado][clase][omi][persona].medalla == 'ORO')
                    oros++;
                if (res[estado][clase][omi][persona].medalla == 'PLATA')
                    platas++;
                if (!res[estado][pequeña] || !res[estado][pequeña][omi])
                    return;
                var inversion = res[estado][clase][omi][persona].lugar - res[estado][pequeña][omi][persona].lugar;
                if (inversion > 0)
                    inversiones += inversion;
            });
            if (participantes == 0)
                lugarPromedio = 15;
            else
                lugarPromedio = obtenerLugar(puntosTotal / participantes, puntos[clase][omi]);
            if (lugarPromedio <= CORTE_OROS)
                permitidos += FACTOR_LUGAR_PROMEDIO + (FACTOR_LUGAR_PROMEDIO * ((CORTE_OROS - lugarPromedio + 1) / CORTE_OROS));
            else if (lugarPromedio <= CORTE_PLATAS)
                permitidos += FACTOR_LUGAR_PROMEDIO * ((CORTE_PLATAS - lugarPromedio + 1) / NUM_PLATAS);
        });
        if (participantesTotal > 0)
        {
            permitidos += (oros * FACTOR_ORO) / participantesTotal;
            permitidos += (platas * FACTOR_PLATA) / participantesTotal;
            permitidos -= (inversiones * FACTOR_INVERSIONES);
        }

        permitidos = Math.trunc(permitidos);
        if (permitidos > MAXIMO)
            permitidos = MAXIMO;
        if (permitidos < MINIMO)
            permitidos = MINIMO;

        var celda = document.getElementById(clase + "-" + estado);
        celda.innerText = permitidos;
    }
}

function calculaPuntosPorEstado(estado, clase)
{
    if (!puntos[clase])
        puntos[clase] = [];
    if(res[estado][clase]) {
        Object.keys(res[estado][clase]).forEach((omi) => {
            if (!puntos[clase][omi])
                puntos[clase][omi] = [];
            Object.keys(res[estado][clase][omi]).forEach((persona) => {
                puntos[clase][omi].push(res[estado][clase][omi][persona].puntos);
            });
        });
    }
}

function calculaPuntos()
{
    Object.keys(res).forEach((estado) => {
        calculaPuntosPorEstado(estado, 'OMIP');
        calculaPuntosPorEstado(estado, 'OMIS');
    });

    Object.keys(puntos['OMIP']).forEach((omi) => {
        puntos['OMIP'][omi].sort(function (a, b) { return b-a; });
    });
    Object.keys(puntos['OMIS']).forEach((omi) => {
        puntos['OMIS'][omi].sort(function (a, b) { return b-a; });
    });
}

function calculaLugares() {
    calculaPuntos();
    Object.keys(res).forEach((estado) => {
        calculaLugaresParaEstado(estado, 'OMIP');
        calculaLugaresParaEstado(estado, 'OMIS');
    });
}