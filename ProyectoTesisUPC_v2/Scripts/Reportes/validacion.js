function validaCorreo(p_correo) {
    var regex = /[\w-\.]{2,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;
    if (!regex.test(p_correo)) {
        return false;
    } else {
        return true;
    }
}