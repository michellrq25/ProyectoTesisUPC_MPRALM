function ValidaEmail(email) {
    try {
        var regex = /[\w-\.]{2,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;
        if (!regex.test(email.trim())) {
            return false;
        }
        return true;
    }
    catch (e) {
        throw e;
    }
}
function validarFormatoPassword(password) {
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&#.$($)$-$_])[A-Za-z\d$@$!%*?&#.$($)$-$_]{8,12}$/;
    var pass = regex.exec(password);
    return pass;
}
function validarFormatoPassword2(password) {
    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&#.$($)$-$_])[A-Za-z\d$@$!%*?&#.$($)$-$_]{10,12}$/;
    var pass = regex.exec(password);
    return pass;
}