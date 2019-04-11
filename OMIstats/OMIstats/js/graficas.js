var coloresGraph = ["red", "blue", "green", "yellow", "purple", "orange", "cyan", "black", "gray", "greenyellow", "ivory"];

function cargaGrafica(container, datasets, labels, ejeX, max)
{
    var ctx = document.getElementById(container).getContext('2d');
    var sets = [];
    for (var i = 0; i < datasets.length; i++) {
        sets.push({
            label: labels[i],
            data: datasets[i],
            backgroundColor: coloresGraph[i],
            borderColor: coloresGraph[i],
            fill: false,
            borderWidth: 1,
            cubicInterpolationMode: "monotone",
        });
    }

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ejeX,
            datasets: sets
        },
        options: {
            legend: {
                display: datasets.length > 1
            },
            responsive: false,
            title: {
                display: false,
            },
            tooltips: {
                mode: 'point',
                intersect: false,
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Horas'
                    },
                    ticks: {
                        min: 0,
                        max: 10
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Puntos'
                    },
                    ticks: {
                        min: 0,
                        max: max
                    }
                }]
            }
        }
    });
}