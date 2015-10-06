(function (window, $, undefined) {
    var pear = window.Pear;
    pear.PlantAvailabilityCalculator = {};
    pear.PlantAvailabilityCalculator.Init = function () {
        $('#Year').keyup(function (e) {
            e.preventDefault();
            calculate();
        });

        $('#PlantAvailable').keyup(function (e) {
            e.preventDefault();
            calculate();
        });

        $('#pa-unit').change(function() {
            calculate();
        });

        var calculate = function () {
            var year = $('#Year').val();
            var plantAvailable = $('#PlantAvailable').val();
            var unit = $('#pa-unit').val();
            if (unit === 'days') {
                $('#plantAvailabilityDays').val(plantAvailable);
                $('#shutDownDays').val(year - plantAvailable);
                $('#plantAvailabilityPercent').val( (plantAvailable / year * 100).format(2) );
                $('#shutDownPercent').val( ((year - plantAvailable) / year * 100).format(2) );
            } else {
                $('#plantAvailabilityDays').val( (plantAvailable / 100 * year).format(2));
                $('#shutDownDays').val( (((100 - plantAvailable) / 100) * year).format(2) );
                $('#plantAvailabilityPercent').val(plantAvailable);
                $('#shutDownPercent').val( (100 - plantAvailable).format(2));
            }
        };


    };
}(window, jQuery));