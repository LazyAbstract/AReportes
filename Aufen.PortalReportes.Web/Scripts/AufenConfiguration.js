$(function () {
    jQuery(function ($) {
        $.datepicker.regional['es'] = {
            closeText: 'Cerrar',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
                'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
                'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Mi&eacute;rcoles', 'Jueves', 'Viernes', 'S&aacute;bado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mi&eacute;', 'Juv', 'Vie', 'S&aacute;b'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'S&aacute;'],
            weekHeader: 'Sm',
            dateFormat: 'dd-mm-yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };
        $.datepicker.setDefaults($.datepicker.regional['es']);
    });

    Globalize.culture('es-CL');
    //Tell the validator, for example,
    // that we want numbers parsed a certain way!
    $.validator.methods.number = function (value, element) {
        if (Globalize.parseFloat(value) || value == 0) {
            return true;
        }
        return false;
    }

    $('.fechaNacimiento').on("click", function () {
        $(this).datepicker({
            showOn: 'focus',
            changeMonth: true,
            changeYear: true,
            yearRange: '-80:-0',
            defaultDate: '-18y',
            maxDate: '-18y'
        }).focus();
    });

    $('.fecha').on("click", function () {
        $(this).datepicker({
            showOn: 'focus',
            changeMonth: true,
            yearRange: '-5:+1',
            defaultDate: '-0y',
            maxDate: '+1y'
        }).focus();
    });
    $('.fadeout-5').fadeTo(5000, 1).fadeOut('slow');
});