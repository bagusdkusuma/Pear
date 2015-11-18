(function (window, $, undefined) {
    
    
    var pear = window.Pear;
    pear.PlantAvailabilityCalculator = {};
    pear.PlantAvailabilityCalculator.Init = function () {
        $('.datepicker').datetimepicker({
            format: "YYYY"
        });

        $('.datepicker').on("dp.change", function (e) {
            calculate();
        });
        
        $('#Year, #PlantAvailable').keyup(function (e) {
            e.preventDefault();
            calculate();
        });

        $('#pa-unit').change(function() {
            calculate();
        });

        var calculate = function () {
            var theYear = $('#Year').val();
            var year = 0;
            for (var i = 0; i < 12; i++) {
                year += new Date(theYear, i, 0).getDate();
            }
            
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