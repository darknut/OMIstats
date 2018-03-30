var lastServerUpdate = 0;
var lastUpdateWithServer = 0;

function setTimes(server, page) {
    lastServerUpdate = server;
    lastUpdateWithServer = page;
    updateTimes();
}

function startTimer() {
    setInterval(update, 1000);
}

function timeToText(element, seconds) {
    var minutes = Math.round(seconds / 60);

    var text = "";

    if (minutes == 0)
    {
        text = seconds + " segundo";
        if (seconds != 1)
            text += "s";
    }
    else
    {
        text = minutes + " minuto";
        if (minutes != 1)
            text += "s";
    }

    element.innerHTML = text;
}

function updateTimes() {
    lastServerUpdate++;
    lastUpdateWithServer++

    timeToText(document.getElementById("lastServerUpdate"), lastServerUpdate);
    timeToText(document.getElementById("lastPageUpdate"), lastUpdateWithServer);
}

function update() {
    updateTimes();
}