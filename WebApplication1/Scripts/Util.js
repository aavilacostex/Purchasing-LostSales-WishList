//Cambia los Datepickers al English
$(function ($) { 
    $.datepicker.regional['en'] = {
        closeText: 'Close',
        prevText: '<Prev',
        nextText: 'Next>',
        currentText: 'Today',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        weekHeader: 'Wk',
        dateFormat: 'mm/dd/yy',
        firstDay: 1,
        changeMonth: true,
        changeYear: true,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: '',
        yearRange: "-100:+10"
    };
    $.datepicker.setDefaults($.datepicker.regional['en']);
});