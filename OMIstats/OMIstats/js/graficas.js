var colores = ["red", "blue", "green", "yellow", "purple", "orange", "cyan", "black", "gray", "greenyellow", "ivory"];

function cargaGrafica(container, datasets, labels, max)
{
    var ctx = document.getElementById(container).getContext('2d');
    var sets = [];
    var emptyLab = [];
    for (var i = 0; i < datasets[0].length; i++) {
        emptyLab.push("");
    }
    for (var i = 0; i < datasets.length; i++) {
        sets.push({
            label: labels[i],
            data: datasets[i],
            backgroundColor: colores[i],
            borderColor: colores[i],
            fill: false,
            borderWidth: 1,
            cubicInterpolationMode: "monotone",
        });
    }

    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: emptyLab,
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
                    display: false
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