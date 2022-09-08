function calculaLugaresParaEstado(estado, clase) {
    if(res[estado][clase]) {
        Object.keys(res[estado][clase]).forEach((omi) => {
            Object.keys(res[estado][clase][omi]).forEach((persona) => {
                if (!isNaN(parseFloat(persona))) {
                    res[estado][clase][omi].lugarPromedio += res[estado][clase][omi][persona].lugar;
                    res[estado][clase][omi].puntosPromedio += res[estado][clase][omi][persona].puntos;
                    res[estado][clase][omi].totalCompetidores++;
                    if (res[estado][clase][omi][persona].medalla == 'ORO')
                        res[estado][clase][omi].oros++;
                    if (res[estado][clase][omi][persona].medalla == 'PLATA')
                        res[estado][clase][omi].platas++;
                }
            });
            res[estado][clase][omi].lugarPromedio /= res[estado][clase][omi].totalCompetidores;
            res[estado][clase][omi].puntosPromedio /= res[estado][clase][omi].totalCompetidores;
        });
    }
}

function calculaLugares() {
    Object.keys(res).forEach((estado) => {
        calculaLugaresParaEstado(estado, 'OMIP');
        calculaLugaresParaEstado(estado, 'OMIS');
    });
}