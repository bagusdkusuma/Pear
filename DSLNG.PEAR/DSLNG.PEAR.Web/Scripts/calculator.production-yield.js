(function (window, $, undefined) {
    var pear = window.Pear;
    pear.ProductionYieldCalculator = {};
    pear.ProductionYieldCalculator.Init = function () {
        var toTonnes = function (unit, input) {
            var tonnes_mmscf = parseFloat($('.production-yield-all .tonnes-mmscf').val());
            var kg_mmbtu = parseFloat($('.production-yield-all .kg-mmbtu').val());
            var $resultHolder = $('.tonnes-result');
            var result;
            switch (unit) {
                case 'mmscf':
                    result = input * tonnes_mmscf;
                    break;
                case 'kg':
                    result = input / 1000;
                    break;
                case 'mmbtu':
                    result = input * kg_mmbtu / 1000;
                    break;
                default:
                    result = input;
                    break;
            }
            $resultHolder.html(result.format(2));
            return result;
        };
        var toLNG = function (tonnes) {
            /*var kg_mmbtu = parseFloat($('.production-yield-all .kg-mmbtu').val());
            var mmbtu_kg = 1 / kg_mmbtu;
            var lng_yield = parseFloat($('.production-yield-all .lng-yield').val()) / 100;
            var nm3_m3 = parseFloat($('.production-yield-all .nm3-m3').val());
            var pav = parseFloat($('.production-yield-all .pav').val());
            var nm3_kg = parseFloat($('.production-yield-all .nm3-kg').val());

            var lngTonnes = tonnes * lng_yield;
            var lngMmbtu = lngTonnes * 1000 * mmbtu_kg;
            var lngM3 = lngTonnes * 1000 * nm3_kg / nm3_m3;
            var lngMtpa = lngTonnes * pav;
            
            $('#LNG_Tonnes').val(lngTonnes.format(4));
            $('#LNG_Mmbtu').val(lngMmbtu.format(4));
            $('#LNG_M3').val(lngM3.format(4));
            $('#LNG_Mtpa').val(lngMtpa.format(4));*/
            var mmbtu_m3 = parseFloat($('.mmbtu-m3.lng').val());
            var lng_yield = parseFloat($('.lng.lng-yield').val()) / 100;
            var pav = parseFloat($('.pav.lng').val());
            
            var lngTonnes = lng_yield * $('.feedgas-tonnes').data('value');
            var lngMmbtu = (lngTonnes * 1000) / $('.kg-mmbtu.lng ').val();
            var lngM3 = parseFloat(lngMmbtu / mmbtu_m3);
            var lngMtpa = (lngTonnes * pav) / 1000000;

            $('#LNG_Tonnes').val(lngTonnes.format(2));
            $('#LNG_Mmbtu').val(lngMmbtu.format(2));
            $('#LNG_M3').val(lngM3.format(2));
            $('#LNG_Mtpa').val(lngMtpa.format(2));
        };

        var toCDS = function (tonnes) {
            /*var kg_mmbtu = parseFloat($('.production-yield-all .kg-mmbtu').val());
            var mmbtu_kg = 1 / kg_mmbtu;
            var nm3_m3 = parseFloat($('.production-yield-all .nm3-m3').val());
            var bbl_m3 = parseFloat($('.production-yield-all .bbl-m3').val());
            var nm3_kg = parseFloat($('.production-yield-all .nm3-kg').val());
            var cds_yield = parseFloat($('.production-yield-all .cds-yield').val()) / 100;

            var cdsTonnes = tonnes * cds_yield;
            var cdsMmbtu = cdsTonnes * 1000 * mmbtu_kg;
            var cdsM3 = cdsTonnes * 1000 * nm3_kg / nm3_m3;
            var cdsBbl = cdsM3 * bbl_m3;
            $('#CDS_Tonnes').val(cdsTonnes.format(4));
            $('#CDS_Mmbtu').val(cdsMmbtu.format(4));
            $('#CDS_M3').val(cdsM3.format(4));
            $('#CDS_Bbl').val(cdsBbl.format(4));*/
            
            var cds_yield = parseFloat($('.cds.cds-yield').val()) / 100;
            var cdsTonnes = cds_yield * $('.feedgas-tonnes').data('value');
            var cdsMmbtu = cdsTonnes * 1000 * $('.kg-mmbtu.cds').val();
            var mmbtu_m3 = parseFloat($('.mmbtu-m3.cds').val());
            var m3_bbl = parseFloat($('.m3-bbl.cds').val());
            var cdsM3 = cdsMmbtu / mmbtu_m3;
            var cdsBbl = cdsM3 / m3_bbl;
            $('#CDS_Tonnes').val(cdsTonnes.format(2));
            $('#CDS_Mmbtu').val(cdsMmbtu.format(2));
            $('#CDS_M3').val(cdsM3.format(2));
            $('#CDS_Bbl').val(cdsBbl.format(2));
        };

        var convertToOtherUnit = function (input, unit) {
            //alert(input + ' ' + unit);
            console.log(unit);
            switch (unit) {
                case "tonnes":
                    $('.feedgas-tonnes').data('value', input);
                    
                    $('.feedgas-tonnes').html($('.feedgas-tonnes').data('value').format(2));
                    
                    $('.feedgas-mmscf').data('value', parseFloat(input / $('.general.tonnes-mmscf').val()));
                    $('.feedgas-mmscf').html($('.feedgas-mmscf').data('value').format(2));
                    
                    $('.feedgas-mmbtu').data('value', parseFloat((input * 1000) / $('.general.kg-mmbtu').val()));
                    $('.feedgas-mmbtu').html($('.feedgas-mmbtu').data('value').format(2));
                    
                    $('.feedgas-kg').data('value', input * 1000);
                    $('.feedgas-kg').html($('.feedgas-kg').data('value').format(2));
                    break;
                case "mmscf":
                    $('.feedgas-tonnes').data('value', parseFloat(input * $('.general.tonnes-mmscf').val()));
                    $('.feedgas-tonnes').html($('.feedgas-tonnes').data('value').format(2));

                    $('.feedgas-mmscf').data('value', input);
                    $('.feedgas-mmscf').html($('.feedgas-mmscf').data('value').format(2));

                    $('.feedgas-mmbtu').data('value', parseFloat(($('.feedgas-tonnes').data('value') * 1000) / $('.general.kg-mmbtu').val()));
                    $('.feedgas-mmbtu').html($('.feedgas-mmbtu').data('value').format(2));

                    $('.feedgas-kg').data('value', $('.feedgas-tonnes').data('value') * 1000);
                    $('.feedgas-kg').html($('.feedgas-kg').data('value').format(2));
                    break;
                case "kg":
                    $('.feedgas-tonnes').data('value', input / 1000);
                    $('.feedgas-tonnes').html($('.feedgas-tonnes').data('value').format(2));

                    $('.feedgas-mmscf').data('value', parseFloat((input / 1000) / $('.general.tonnes-mmscf').val()));
                    $('.feedgas-mmscf').html($('.feedgas-mmscf').data('value').format(2));

                    $('.feedgas-mmbtu').data('value', parseFloat(input / $('.general.kg-mmbtu').val()));
                    $('.feedgas-mmbtu').html($('.feedgas-mmbtu').data('value').format(2));

                    $('.feedgas-kg').data('value', input);
                    $('.feedgas-kg').html($('.feedgas-kg').data('value').format(2));
                    break;
                case "mmbtu":
                    $('.feedgas-tonnes').data('value', input * $('.general.kg-mmbtu').val() / 1000);
                    $('.feedgas-tonnes').html($('.feedgas-tonnes').data('value').format(2));

                    $('.feedgas-mmscf').data('value', parseFloat((input / 1000) / $('.general.tonnes-mmscf').val()));
                    $('.feedgas-mmscf').html($('.feedgas-mmscf').data('value').format(2));

                    $('.feedgas-mmbtu').data('value', input);
                    $('.feedgas-mmbtu').html($('.feedgas-mmbtu').data('value').format(2));

                    $('.feedgas-kg').data('value', input * $('.general.kg-mmbtu').val());
                    $('.feedgas-kg').html($('.feedgas-kg').data('value').format(2));
                    break;
                
            }
        };

        var toMCHE = function() {
            var m3_hr = parseFloat($('.m3-hr').val());
            var mcheCapacity = $('.feedgas-mmscf').data('value') * m3_hr;
            $('#MCHE_M3PerHr').val(mcheCapacity.format(2));
        };
        $('#MainInput').keyup(function (e) {
            e.preventDefault();
            var input = parseFloat($(this).val());
            var unit = $('#Unit').val();
            //var tonnes = toTonnes(unit, input);
            convertToOtherUnit(input, unit);
            toLNG(tonnes);
            toCDS(tonnes);
            toMCHE();
        });

        $('#Unit').change(function (e) {
            e.preventDefault();
            var input = parseFloat($('#MainInput').val());
            var unit = $(this).val();
            convertToOtherUnit(input, unit);
            toLNG(tonnes);
            toCDS(tonnes);
            toMCHE();
            /*var tonnes = toTonnes(unit, input);
            toLNG(tonnes);
            toCDS(tonnes);*/
        });

        $('.pricing-constanta-wrapper input[type="text"]').change(function () {
            var input = parseFloat($('#MainInput').val());
            if (input != 0) {
                var unit = $('#Unit').val();
                var tonnes = toTonnes(unit, input);
                convertToOtherUnit(input, unit);
                toLNG(tonnes);
                toCDS(tonnes);
                toMCHE();
            }
        });
        
        $('.constant-production-wrapper').hide();
        $('.show-production-constant').click(function () {
            if ($('.constant-production-wrapper').is(":visible")) {
                $('.constant-production-wrapper').slideUp();
                $('.show-production-constant').html('Show Constant');
            } else {
                $('.constant-production-wrapper').slideDown();
                $('.show-production-constant').html('Hide Constant');
            }
        });


    };
}(window, jQuery));