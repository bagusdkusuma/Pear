(function (window, $, undefined) {
    var pear = window.Pear;
    pear.ProductionYieldCalculator = {};
    pear.ProductionYieldCalculator.Init = function () {
        var tonnes_mmscf = parseFloat($('.production-yield-all .tonnes-mmscf').val());
        var mmscf_tonnes = 1 / tonnes_mmscf;
        var kg_mmbtu = parseFloat($('.production-yield-all .kg-mmbtu').val());
        var mmbtu_kg = 1 / kg_mmbtu;
        var lng_yield = parseFloat($('.production-yield-all .lng-yield').val()) / 100;
        var cds_yield = parseFloat($('.production-yield-all .cds-yield').val()) / 100;
        var nm3_kg = parseFloat($('.production-yield-all .nm3-kg').val());
        var nm3_m3 = parseFloat($('.production-yield-all .nm3-m3').val());
        var pav = parseFloat($('.production-yield-all .pav').val());
        var bbl_m3 = parseFloat($('.production-yield-all .bbl-m3').val());

        var toTonnes = function (unit, input) {
            var $resultHolder = $('.tonnes-result');
            switch (unit) {
                case 'mmscf':
                    var result = input * tonnes_mmscf;
                    break;
                case 'kg':
                    var result = input / 1000;
                    break;
                case 'mmbtu':
                    var result = input * kg_mmbtu / 1000;
                    break;
                default :
                    var result = input;
                    break;
            }
            $resultHolder.html(result.format(4));
            return result;
        };
        var toLNG = function (tonnes) {
            var lngTonnes = tonnes * lng_yield;
            var lngMmbtu = lngTonnes * 1000 * mmbtu_kg;
            var lngM3 = lngTonnes * 1000 * nm3_kg / nm3_m3;
            var lngMtpa = lngTonnes * pav;
            $('#LNG_Tonnes').val(lngTonnes.format(4));
            $('#LNG_Mmbtu').val(lngMmbtu.format(4));
            $('#LNG_M3').val(lngM3.format(4));
            $('#LNG_Mtpa').val(lngMtpa.format(4));
        };

        var toCDS = function (tonnes) {
            var cdsTonnes = tonnes * cds_yield;
            var cdsMmbtu = cdsTonnes * 1000 * mmbtu_kg;
            var cdsM3 = cdsTonnes * 1000 * nm3_kg / nm3_m3;
            var cdsBbl = cdsM3 * bbl_m3;
            $('#CDS_Tonnes').val(cdsTonnes.format(4));
            $('#CDS_Mmbtu').val(cdsMmbtu.format(4));
            $('#CDS_M3').val(cdsM3.format(4));
            $('#CDS_Bbl').val(cdsBbl.format(4));
        };

        $('#MainInput').keyup(function (e) {
            e.preventDefault();
            var input = parseFloat($(this).val());
            var unit = $('#Unit').val();
            var tonnes = toTonnes(unit, input);
            toLNG(tonnes);
            toCDS(tonnes);
        });

        $('#Unit').change(function (e) {
            e.preventDefault();
            var input = parseFloat($('#MainInput').val());
            var unit = $(this).val();
            var tonnes = toTonnes(unit, input);
            toLNG(tonnes);
            toCDS(tonnes);
        });
    };
}(window, jQuery));