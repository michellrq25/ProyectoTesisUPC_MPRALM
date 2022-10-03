$(".solo-numero").keypress(function (event) {
    try {
        var regex = new RegExp("^[0-9]+$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
    catch (e) {
        return;
    }
});

$("#lnkListarUsuarios").click(function () {
    ListarUsuarioPorFiltro();
});
function ListarUsuarioPorFiltro() {    
    $.ajax({
        url: "/Usuarios/ListarUsuariosPrincipal",
        type: 'Get',
        data: {
            login: $("#usuario").val(),
            dniUsuario: $("#dniUsuario").val(),
            rol: $("select#ddlRoles").val(),
            estado: $("select#ddlEstado").val()
        },
        async: true,
        cache: false,
        success: function (data) {
            $("#divGrid").html(data);
        },
        error: function () {
            alertModal("Se produjo un error al obtener los datos.");
        }
    });
};

function mostrar(ID_usuario) {
    $('#verInformacionEmpresa').modal('show');
    $.ajax({
        url: "/Usuarios/EditarUsuario",
        type: 'Get',
        data: {
            idUsuario: ID_usuario
        },
        async: true,
        cache: false,
        success: function (data) {

            $("#RegistroObservacionesInformacion2").html(data);
        },
        error: function () {
            alertModal("Se produjo un error al obtener los datos.");
        }
    });
}


