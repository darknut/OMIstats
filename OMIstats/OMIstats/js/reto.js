var ajaxUrl = '';
var imageUrl = '';
var maxSize = 5000000;
var extensionesValidas = ["jpg", "png", "jfif", "jpeg", "heic"];

function setUpAjax(u, v) {
    ajaxUrl = u;
    imageUrl = v;
}

function setError(id, errorStr) {
    setVisible("loading" + id, false);
    setVisible("error" + id, true);
    var div = document.getElementById("error" + id);
    div.innerText = errorStr;
}

async function onPhotoUpload(id) {
    var e = document.getElementById("input" + id);
    var file = e.files[0];

    if (file) {
        setVisible("loading" + id, true);
        setVisible("error" + id, false);

        if (file.size > maxSize) {
            setError(id, "El tamaño máximo es 5MB");
            return;
        }

        if (extensionesValidas.findIndex((ext) => file.type.endsWith(ext)) == -1) {
            setError(id, "Los tipos de archivo soportados son: " + extensionesValidas.join(', '));
            return;
        }

        var formData = new FormData();
        formData.append("file", file);
        formData.append("reto", id);
        var response = await fetch(ajaxUrl, {
            method: "POST",
            body: formData
        });

        var result = await response.json();

        if (result == "error") {
            setError(id, "Ocurrió un error en el sistema :(");
        } else if (result == "login") {
            setError(id, "La sesión expiró, recarga esta página y vuelve a intentarlo");
        } else if (result == "cerrado") {
            setError(id, "La competencia ya terminó :(");
        } else {
            setVisible("loading" + id, false);
            setVisible("error" + id, false);
            setVisible("ok" + id, true);
            setVisible("check" + id, true);
            var check = document.getElementById("check" + id).setAttribute("href", imageUrl + result);
            setTimeout(() => setVisible("ok" + id, false), 5000);
        }
    }
}
