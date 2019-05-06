var coloresGraph = ["red", "blue", "green", "yellow", "purple", "orange", "cyan", "black", "gray", "greenyellow", "ivory", "black"];
var chart = null;
function cargaGrafica(container, datasets, labels, ejeX, max, colors, allLabels, colorIndex, valorMinimo, yInverso, tituloEje, grandienteColores)
{
    var ctx = document.getElementById(container).getContext('2d');

    var sets = [];
    for (var i = 0; i < datasets.length; i++) {
        sets.push({
            label: labels[i],
            data: datasets[i],
            backgroundColor: coloresGraph[colorIndex],
            borderColor: coloresGraph[colorIndex],
            pointBackgroundColor: grandienteColores ? grandienteColores : coloresGraph[colorIndex],
            pointBorderColor: grandienteColores ? grandienteColores : coloresGraph[colorIndex],
            pointHoverBackgroundColor: grandienteColores ? grandienteColores : coloresGraph[colorIndex],
            pointHoverBorderColor: grandienteColores ? grandienteColores : coloresGraph[colorIndex],
            fill: false,
            borderWidth: 1,
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
}

function destruyeChart() {
    if (chart != null) {
        chart.destroy();
        chart = null;
    }
}