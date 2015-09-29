(function (window, jQuery, undefined) {
    var pear = window.Pear;
    pear.StandardCalculator = {};
    pear.StandardCalculator.Init = function () {
        console.log('---standard-calculator goes here---');
        var units = {};
        units.weight = {
            units : {
                nano_gram: 'nano gram',
                micro_gram: 'micro gram',
                kilo_gram: 'kilo gram',
                tonnes: 'tonnes',
                kilo_tonnes: 'kilo tonnes',
                million_tonnes : 'million tonnes'
            },
            conversions: {
                nano_gram: new BigNumber(1e-9),
                micro_gram: new BigNumber(1e-6),
                kilo_gram: new BigNumber(1e3),
                tonnes: new BigNumber(1e6),
                kilo_tonnes: new BigNumber(1e9),
                million_tonnes: new BigNumber(1e12)
            }
        };
        units.thermal = {
            units: {
                btu: 'btu',
                kwh: 'kwh',
                mmbtu: 'mmbtu',
                bbtu: 'bbtu',
                tbtu:'tbtu'
            },
            conversions: {
                btu: new BigNumber(1),
                kwh: new BigNumber(3412.14163312794),
                mmbtu: new BigNumber(1e6),
                bbtu: new BigNumber(1e9),
                tbtu: new BigNumber(1e12),
                
            }
        };
        units.volume = {
            units: {
                scf: 'scf',
                mmscf: 'mmscf',
                bscf: 'bscf',
                tcf: 'tcf',
                m3: 'm3',
                nm3 : 'nm3',
                liter: 'liter',
                kilo_liter: 'kilo liter',
                bbl: 'bbl',
                gallon: 'gallon',
                imperial_gallon : 'imperial_gallon'
            },
            conversions: {
                scf: new BigNumber(1),
                mmscf: new BigNumber(1e6),
                bscf: new BigNumber(1e9),
                tcf: new BigNumber(1e12),
                m3: new BigNumber(35.3147),
                nm3 : new BigNumber(35.3147).dividedBy(new BigNumber(1.057)),
                liter: new BigNumber(0.0353147),
                kilo_liter : new BigNumber(0.0353147e3),
                bbl: new BigNumber(5.8975549),
                gallon: new BigNumber(0.133681),
                imperial_gallon : new BigNumber(0.160544),
            }
        };
        units.length = {
            units: {
                m: 'm',
                km: 'km',
                ft: 'ft',
                yard: 'yard'
            },
            conversions: {
                m: new BigNumber(1),
                km: new BigNumber(1000),
                ft: new BigNumber(0.3048),
                yard: new BigNumber(0.9144)
            }
        };
        units.pressure = {
            units: {
                atm: 'atm',
                bar: 'bar',
                psig: 'psig',
                psi: 'psi',
                psia: 'psiA',
                kg_cm2g: 'kg/cm2g'
            },
            conversions: {
                atm: new BigNumber(1),
                bar: new BigNumber(0.986923267),
                psig: new BigNumber(0.0703069578296),
                psi: new BigNumber(0.0680459639),
                psia: new BigNumber(0.0680459570643),
                kg_cm2g: new BigNumber(0.97)
            }
        };
        //units for density ???
    
        units.temperature = {
            units: {
                f: 'F',
                c: 'C',
                r: 'R',
                k: 'K'
            },
            conversions: {
                f_c: function (input) {
                    return input.minus(32).times(5).dividedBy(9);
                },
                c_c: function (input) {
                    return input;
                },
                r_c: function (input) {
                    return input.minus(491.67).times(5).dividedBy(9);
                },
                k_c: function (input) {
                    return input.minus(273.15);
                },
                c_f: function (input) {
                    return input.times(9).dividedBy(5).plus(32);
                },
                c_r: function (input) {
                    return input.times(9).dividedBy(5).plus(491.67);
                },
                c_k: function (input) {
                    return input.plus(273.15);
                }
            }
        };
        units.storage = {
            units: {
                bit: 'bit',
                byte: 'byte',
                kbyte: 'KByte',
                mbyte: 'Mega Byte',
                gbyte: 'Giga Byte',
                tbyte: 'Terra Byte',
                pbyte: 'Peta Byte',
                ebyte : 'Exa Byte',
            },
            conversions: {
                bit:  new BigNumber(1),
                byte: new BigNumber(8),
                kbyte: new BigNumber(8e3),
                mbyte: new BigNumber(8e6),
                gbyte: new BigNumber(8e9),
                tbyte: new BigNumber(8e12),
                pbyte: new BigNumber(8e15),
                ebyte: new BigNumber(8e18)
            }
        }
        $('#InputUnitGroup,#OutputUnitGroup').change(function (e) {
            e.preventDefault();
            var unitGroup = $(this).val();
            var inputUnits = $('.input-units');
            if ($(this).attr('id') == 'OutputUnitGroup') {
                inputUnits = $('.output-units');
            }
            inputUnits.html('');
            for (var unit in units[unitGroup].units) {
                $('<li/>', {
                    id: unit
                }).html(units[unitGroup].units[unit])
                .addClass('unit')
                .appendTo(inputUnits);
            }
            $('li.unit').click(function (e) {
                e.preventDefault();
                var parent = $(this).closest('ul');
                $(this).closest('ul').find('.active').removeClass('active');
                $(this).addClass('active');
                if ((parent.hasClass('input-units') && !$('.output-units li.active').length)
                    || (parent.hasClass('output-units') && !('.input-units li.active').length)) {
                    return;
                }
                var inputGroup = $('#InputUnitGroup').val();
                var outputGroup = $('#OutputUnitGroup').val();
                var inputUnit = $('.input-units li.active').attr('id');
                var outputUnit = $('.output-units li.active').attr('id');
                var result = 0;
                var input = $('#Input').val();
                if (inputGroup == outputGroup && inputGroup != 'temperature') {
                    result = units[inputGroup].conversions[inputUnit].times(new BigNumber(input)).dividedBy(units[outputGroup].conversions[outputUnit]);
                } else if (inputGroup == outputGroup && inputGroup == 'temperature') {
                    if (inputUnit == outputUnit) {
                        result = input;
                    } else {
                        var in_c = units[inputGroup].conversions[inputUnit + '_c'](new BigNumber(input));
                        result = units[outputGroup].conversions['c_' + outputUnit](new BigNumber(in_c));
                    }
                }
                $('#Output').val(result);
            });
        });
        $('#InputUnitGroup').change();
        $('#OutputUnitGroup').change();
    };
}(window, jQuery));