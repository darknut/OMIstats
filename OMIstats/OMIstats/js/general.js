function setVisible(id, visible) {
    var elem = document.getElementById(id);
    if (visible)
        elem.style.display = 'inline';
    else
        elem.style.display = 'none';
}

function esconde(id)
{
    $("#" + id).hide("slow");
}