var coloresGraph = ["red", "blue", "green", "yellow", "purple", "orange", "cyan", "black", "gray", "greenyellow", "ivory", "black"];
var chart = null;
function cargaGrafica(container, datasets, labels, ejeX, max, colors, allLabels, colorIndexes, valorMinimo, yInverso, tituloEje, grandienteColores)
{
    var ctx = document.getElementById(container).getContext('2d');

    var sets = [];
    for (var i = 0; i < datasets.length; i++) {
        sets.push({
            label: labels[i],
            data: datasets[i],
            backgroundColor: coloresGraph[colorIndexes[i]],
            borderColor: coloresGraph[colorIndexes[i]],
            pointBackgroundColor: grandienteColores ? grandienteColores : undefined,
            pointBorderColor: grandienteColores ? grandienteColores : undefined,
            pointHoverBackgroundColor: grandienteColores ? grandienteColores : undefined,
            pointHoverBorderColor: grandienteColores ? grandienteColores : undefined,
            fill: false,
            borderWidth: grandienteColores ? 1 : 3,
            lineTension: 0,
            radius: grandienteColores ? 2 : 0
        });
    }

    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ejeX,
            datasets: sets
        },
        options: {
            legend: {
                display: datasets.length > 1
            },
            responsive: true,
            title: {
                display: false,
            },
            tooltips: {
                mode: 'point',
                intersect: false,
                callbacks: {
                    title: function (items) {
                        return allLabels[items[0].index];
                    }
                }
            },
            hover: {
                mode: 'nearest',
                intersect: false
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Horas'
                    },
                    gridLines: {
                        display: true,
                        color: colors
                    },
                    ticks: {
                        callback: function (value, index, values) {
                            if (value != "" || index == values.length - 1)
                                return value;
                            return null;
                        },
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: tituloEje
                    },
                    ticks: {
                        min: valorMinimo,
                        max: max,
                        reverse: yInverso,
                    }
                }]
            }
        }
    });

    return chart;
}

function destruyeChart() {
    if (chart != null) {
        chart.destroy();
        chart = null;
    }
}

function actualizaGrafica(grafica, datasets, labels, colors) {
    for (var i = 0; i < datasets.length; i++) {
        grafica.data.datasets.push({
            label: labels[i],
            data: datasets[i],
            backgroundColor: coloresGraph[colors[i]],
            borderColor: coloresGraph[colors[i]],
            fill: false,
            borderWidth: 3,
            lineTension: 0,
            radius: 0
        });
    }
    grafica.options.legend.display = true;
    grafica.update();
}