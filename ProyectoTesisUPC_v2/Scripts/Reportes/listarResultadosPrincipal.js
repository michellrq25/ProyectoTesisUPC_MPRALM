$("#lnkListarResultados").click(function () {
    LimpiarBarra();
    ListarPorFiltro();
});

function ListarPorFiltro() {
    try {
        $("#tdExportarExcel").hide();
        
        var _estado = ($("select#ddlEstado").val() == '' ? '999' : $("select#ddlEstado").val());
        var _seccion = ($("select#ddlSecciones").val() == '' ? '0' : $("input#ddlSecciones").val());
        var _grado = ($("select#ddlGrados").val() == '' ? '0' : $("select#ddlGrados").val());
        $.ajax({
            url: "ListarResultadosPrincipal",
            type: 'Get',
            async: true,
            cache: true,
            data: {
                dniAlumno: $("input#dniAlumno").val(),
                seccion: _seccion,
                grado: _grado,
                estado: _estado,
                
            },
            success: function (data) {
                $("#divGrid").html(data);
                if ($("#hdCantidadRegistrosFiltrados").val() > 0) {
                    $("#tdExportarExcel").show();
                }
            },
            error: function () {
                alertModal("Se produjo un error al obtener los datos.");
            }
        });
    }
    catch (e) {
        throw e;
    }
};


function alertValida(mensaje) {
    var $textAndPic = $('<div></div>');
    $textAndPic.append(' <i class="icono_alerta_modal"><img src="../Images/iconos/color/icon-warning.png" alt="" style="width: 40px;height: 40px;" ></i> ');
    $textAndPic.append('<div class="content_texto_modal"><span class="error-msg">' + mensaje + '</span></div>');

    BootstrapDialog.show({
        title: 'MPRALM',
        type: BootstrapDialog.TYPE_WARNING,
        icon: 'glyphicon glyphicon-check',
        message: $textAndPic,
        closable: true,
        buttons: [{
            cssClass: 'btn-principal',
            label: 'Ok',
            icon: 'icono_check',
            action: function (dialogRef) {
                dialogRef.close();
            }
        }],
    });
};

function VistaPreviaAlumnoResultados(DNI, CodPrediccion) {
    $('#verAlumno').modal('show');
    $.ajax({
        url: "/Reportes/ConsultaResultadoUnico",
        type: 'Get',
        async: true,
        cache: false,
        data: {
            DNI: DNI,
            CodPrediccion: CodPrediccion
        },
        success: function (data) {
            if (data.IsValid == false) {
                alertValida("<a style='font-size: 1.5em; color:#6f79bd;'>Se produjo un error inesperado.</a> <br>Si el error persiste favor de comunicarse con Service Desk. <br>Código de Error: <b style='color:#FF6A39;'>" + data.ErrorMessage + "<b/>");
                return;
            }
            $("#RegistroAlumno").html(data);
        },
        error: function () {
            $('#verAlumno').modal('hide');
            alertValida("<a style='font-size: 1.5em; color:#6f79bd;'>Se produjo un error inesperado.</a> <br>Si el error persiste favor de comunicarse con Service Desk.");
        }
    });
};

function ObtenerCorreo(DNI, CodPrediccion) {
    $('#verAlumno').modal('show');
    $.ajax({
        url: "/Reportes/ObtenerCorreo",
        type: 'Get',
        async: true,
        cache: false,
        data: {
            DNI: DNI,
            CodPrediccion: CodPrediccion
        },
        success: function (data) {
            if (data.IsValid == false) {
                alertValida("<a style='font-size: 1.5em; color:#6f79bd;'>Se produjo un error inesperado.</a> <br>Si el error persiste favor de comunicarse con TI. <br>Código de Error: <b style='color:#FF6A39;'>" + data.ErrorMessage + "<b/>");
                return;
            }
            $("#RegistroAlumno").html(data);
        },
        error: function () {
            $('#verAlumno').modal('hide');
            alertValida("<a style='font-size: 1.5em; color:#6f79bd;'>Se produjo un error inesperado.</a> <br>Si el error persiste favor de comunicarse con TI.");
        }
    });
};

function LimpiarBarra() {
    $("#tdExportarExcel").hide();
};