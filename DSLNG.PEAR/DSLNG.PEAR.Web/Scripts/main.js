// Common
/*
 * Date Format 1.2.3
 * (c) 2007-2009 Steven Levithan <stevenlevithan.com>
 * MIT license
 *
 * Includes enhancements by Scott Trenda <scott.trenda.net>
 * and Kris Kowal <cixar.com/~kris.kowal/>
 *
 * Accepts a date, a mask, or a date and a mask.
 * Returns a formatted version of the given date.
 * The date defaults to the current date/time.
 * The mask defaults to dateFormat.masks.default.
 */

var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
        timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
        timezoneClip = /[^-+\dA-Z]/g,
        pad = function (val, len) {
            val = String(val);
            len = len || 2;
            while (val.length < len) val = "0" + val;
            return val;
        };

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
            d = date[_ + "Date"](),
            D = date[_ + "Day"](),
            m = date[_ + "Month"](),
            y = date[_ + "FullYear"](),
            H = date[_ + "Hours"](),
            M = date[_ + "Minutes"](),
            s = date[_ + "Seconds"](),
            L = date[_ + "Milliseconds"](),
            o = utc ? 0 : date.getTimezoneOffset(),
            flags = {
                d: d,
                dd: pad(d),
                ddd: dF.i18n.dayNames[D],
                dddd: dF.i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: dF.i18n.monthNames[m],
                mmmm: dF.i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
            };

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
        "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ],
    monthNames: [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    ]
};

// For convenience...
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};

String.prototype.startsWith = function (str) {
    return this.substr(0, str.length) === str;
};
String.prototype.endsWith = function (str) {
    return this.indexOf(str, this.length - str.length) !== -1;
};
String.prototype.isNullOrEmpty = function () {
    return this == false || this === '';
};
function round(value, exp, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (exp > 0 ? '\\.' : '$') + ')';
    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return (+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp))).toString().replace(new RegExp(re, 'g'), '$&,');
}
/**
 * Number.prototype.format(n, x)
 * 
 * @param integer n: length of decimal
 * @param integer x: length of sections
 */
Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    if (parseFloat(this % 1) == parseFloat(0)) {
        return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
    }
    var y = round(this, n, x);
    if (parseFloat(y % 1) == parseFloat(0)) {
        return parseFloat(y).toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
    }
    return y;
};

//var a = 397053424;
//var b = 397053424.9;
//var c = 397053424.99;
//var d = 397053424.999;
//console.log(a.format(2));
//console.log(b.format(2));
//console.log(c.format(2));
//console.log(d.format(2));

(function (window, $, undefined) {
    var Pear = {};
    Pear.Artifact = {};
    Pear.Artifact.Designer = {};
    Pear.Artifact.Designer.Multiaxis = {};
    Pear.Template = {};
    Pear.Template.Editor = {};
    Pear.Loading = {};
    Pear.Highlight = {};
    Pear.VesselSchedule = {};
    Pear.NLS = {};
    Pear.ConstantUsage = {};
    Pear.Calculator = {};
    Pear.OutputConfig = {};
    Pear.EconomicSummary = {};
    Pear.OperationConfig = {};

    Pear.Loading.Show = function (container) {
        var loadingImage = $('#dataLayout').attr('data-content-url') + '/img/ajax-loader2.gif';
        container.css('background-position', 'center center');
        container.css('background-repeat', 'no-repeat');
        container.css('background-image', 'url(' + loadingImage + ')');
    };

    Pear.Loading.Stop = function (container) {
        container.css('background', 'none');
    };

    var artifactDesigner = Pear.Artifact.Designer;

    //helper
    artifactDesigner._formatKpi = function (kpi) {
        //console.log(kpi);
        if (kpi.loading) return kpi.text;
        return '<div class="clearfix"><div class="col-sm-12">' + kpi.Name + '</div></div>';
    };
    artifactDesigner._formatKpiSelection = function (kpi) {
        return kpi.Name || kpi.text;
    };
    artifactDesigner._kpiAutoComplete = function (context, useMeasurement, uniqueMeasurement) {
        var measurement = typeof useMeasurement == 'undefined' ? true : useMeasurement;
        var measurementContext = typeof uniqueMeasurement == 'undefined' ? $(document) : uniqueMeasurement;
        context.find('.kpi-list').select2({
            ajax: {
                url: $('#hidden-fields-holder').data('kpi-url'),
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    if (measurement) {
                        return {
                            term: params.term, // search term
                            measurementId: measurementContext.find('.measurement').val()
                        };
                    } else {
                        return {
                            term: params.term, // search term
                        };
                    }
                },
                processResults: function (data, page) {
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };
    artifactDesigner._colorPicker = function (context) {
        context.find('.colorpicker input').colpick({
            submit: 0,
            onChange: function (hsb, hex, rgb, el, bySetColor) {
                $(el).closest('.colorpicker').find('i').css('background-color', '#' + hex);
                if (!bySetColor) $(el).val('#' + hex);
            }
        }).keyup(function () {
            $(this).colpickSetColor(this.value.replace('#', ''));
        });
    };
    artifactDesigner._toJavascriptDate = function (value, periodeType) {
        if (value == undefined) return '';
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        switch (periodeType) {
            case 'Daily':
                return dt.format('dd mmm yyyy');
            case 'Monthly':
                return dt.format('mmm yyyy');
            default:
                return dt.format('yyyy');
        }
    };

    artifactDesigner.ListSetup = function () {
        $(document).on('click', '.artifact-view', function (e) {
            e.preventDefault();
            Pear.Loading.Show($('#container'));
            $('#graphic-preview').modal('show');
            var $this = $(this);
            var callback = Pear.Artifact.Designer._previewCallbacks;
            $.ajax({
                url: $this.attr('href'),
                method: 'GET',
                success: function (data) {
                    if (callback.hasOwnProperty(data.GraphicType)) {
                        Pear.Loading.Stop($('#container'));
                        callback[data.GraphicType](data, $('#container'));
                    }
                }
            });

            $('#graphic-preview').on('show.bs.modal', function () {
                $('#container').css('visibility', 'hidden');
            });

            $('#graphic-preview').on('shown.bs.modal', function () {
                if ($('#container').highcharts() !== undefined)
                    $('#container').highcharts().reflow();
                $('#container').css('visibility', 'visible');
            });

            $('#graphic-preview').on('hidden.bs.modal', function () {
                $('#container').html('');
            });
        });
    };
    artifactDesigner.GraphicSettingSetup = function () {
        var callback = Pear.Artifact.Designer._setupCallbacks;
        var loadGraph = function (url, type) {
            $.ajax({
                url: url,
                data: 'type=' + type,
                cache: true,
                method: 'GET',
                success: function (data) {
                    $('#graphic-settings').html(data);
                    var $hiddenFields = $('#hidden-fields');
                    $('#hidden-fields-holder').html($hiddenFields.html());
                    $hiddenFields.remove();
                    $('.graphic-properties').each(function (i, val) {
                        $(val).html('');
                    });
                    $('#graphic-settings').prev('.form-group').css('display', 'block');
                    $('#general-graphic-settings').css('display', 'block');
                    $('.form-measurement').css('display', 'block');
                    $('.main-value-axis').css('display', 'block');
                    if (['speedometer', 'trafficlight', 'tabular', 'tank', 'pie', 'multiaxis'].indexOf(type) > -1) {
                        $('.scale').css('display', 'none');
                    } else {
                        $('.scale').css('display', 'block');
                    }
                    if (callback.hasOwnProperty(type)) {
                        callback[type]();
                    }
                }
            });
        };
        var rangeDatePicker = function () {
            $('.datepicker').datetimepicker({
                format: "MM/DD/YYYY"
            });
            $('.datepicker').change(function (e) {
                //console.log(this);
            });
            $('#PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                var clearValue = $('.datepicker').each(function (i, val) {
                    $(val).val('');
                    if ($(val).data("DateTimePicker") !== undefined) {
                        $(val).data("DateTimePicker").destroy();
                    }
                });
                switch ($this.val().toLowerCase().trim()) {
                    case 'hourly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY hh:00 A"
                        });
                        break;
                    case 'daily':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY"
                        });
                        break;
                    case 'weekly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY",
                            daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                        });
                        break;
                    case 'monthly':
                        $('.datepicker').datetimepicker({
                            format: "MM/YYYY"
                        });
                        break;
                    case 'yearly':
                        $('.datepicker').datetimepicker({
                            format: "YYYY"
                        });
                        break;
                    default:

                }
            });
        };
        var rangeControl = function () {
            $('#RangeFilter').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                $('#range-holder').prop('class', $this.val().toLowerCase().trim());
            });
            var original = $('#RangeFilter').clone(true);
            var rangeFilterSetup = function (periodeType) {
                var graphicType = $('#graphic-type').val();
                var toRemove = {};
                switch (graphicType) {
                    case "tabular":
                    case "tank":
                    case "speedometer":
                    case "traffic":
                    case "pie":
                        toRemove.hourly = ['AllExistingYears'];
                        toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.weekly = ['AllExistingYears'];
                        toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                        toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'AllExistingYears'];
                        break;
                    default:
                        toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        break;
                }
                var originalClone = original.clone(true);
                originalClone.find('option').each(function (i, val) {
                    if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                        originalClone.find(val).remove();
                    }
                });
                $('#RangeFilter').replaceWith(originalClone);
            };

            rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
            $('#PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                rangeFilterSetup($this.val().toLowerCase().trim());
                $('#range-holder').removeAttr('class');
            });

        };
        var specificDate = function () {
            $(".datepicker").on("dp.change", function (e) {
                if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                    $('#EndInDisplay').val($('#StartInDisplay').val());
                }
            });
        };
        $('#graphic-type').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            loadGraph($this.data('graph-url'), $this.val());
            if ($this.val() === 'bar') {
                $('.netback-chart-opt').show();
            } else {
                $('.netback-chart-opt').hide();
            }
            $('#PeriodeType').change();
        });

        var initialGraphicType = $('#graphic-type');
        if (initialGraphicType.val() == 'bar') {
            $('.netback-chart-opt').show();
        }
        loadGraph(initialGraphicType.data('graph-url'), initialGraphicType.val());
        rangeControl();
        rangeDatePicker();
        specificDate();
    };

    artifactDesigner.EditSetup = function () {
        var callback = Pear.Artifact.Designer._setupCallbacks;
        var loadGraph = function (url, type) {
            $.ajax({
                url: url,
                data: 'type=' + type,
                cache: true,
                method: 'GET',
                success: function (data) {
                    $('#graphic-settings').html(data);
                    var $hiddenFields = $('#hidden-fields');
                    $('#hidden-fields-holder').html($hiddenFields.html());
                    $hiddenFields.remove();
                    $('.graphic-properties').each(function (i, val) {
                        $(val).html('');
                    });
                    $('#graphic-settings').prev('.form-group').css('display', 'block');
                    $('#general-graphic-settings').css('display', 'block');
                    $('.form-measurement').css('display', 'block');
                    $('.main-value-axis').css('display', 'block');
                    if (callback.hasOwnProperty(type)) {
                        callback[type]();
                    }
                }
            });
        };
        $('#graphic-preview-btn').click(function (e) {
            e.preventDefault();
            var $this = $(this);
            Pear.Loading.Show($('#container'));
            $('#graphic-preview').modal('show');
            var callback = Pear.Artifact.Designer._previewCallbacks;
            $.ajax({
                url: $this.data('preview-url'),
                data: $this.closest('form').serialize(),
                method: 'POST',
                success: function (data) {
                    if (callback.hasOwnProperty(data.GraphicType)) {
                        Pear.Loading.Stop($('#container'));
                        callback[data.GraphicType](data, $('#container'));
                    }
                }
            });
        });

        $('#graphic-preview').on('show.bs.modal', function () {
            $('#container').css('visibility', 'hidden');
        });

        $('#graphic-preview').on('shown.bs.modal', function () {
            if ($('#container').highcharts() !== undefined)
                $('#container').highcharts().reflow();
            $('#container').css('visibility', 'visible');
        });

        $('#graphic-preview').on('hidden.bs.modal', function () {
            $('#container').html('');
        });

        var rangeDatePicker = function () {
            var format = $('#datetime-attr').attr('data-datepickerformat');
            $('.datepicker').datetimepicker({
                format: format,
            });
            $('.datepicker').change(function (e) {
                //console.log(this);
            });
            $('#PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                var clearValue = $('.datepicker').each(function (i, val) {
                    $(val).val('');
                    $(val).data("DateTimePicker").destroy();
                });
                switch ($this.val().toLowerCase().trim()) {
                    case 'hourly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY hh:00 A"
                        });
                        break;
                    case 'daily':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY"
                        });
                        break;
                    case 'weekly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY",
                            daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                        });
                        break;
                    case 'monthly':
                        $('.datepicker').datetimepicker({
                            format: "MM/YYYY"
                        });
                        break;
                    case 'yearly':
                        $('.datepicker').datetimepicker({
                            format: "YYYY"
                        });
                        break;
                    default:
                }
            });
        };
        var rangeControl = function () {
            var graphicType = $('#graphic-type').val();
            $('#RangeFilter').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                $('#range-holder').prop('class', $this.val().toLowerCase().trim());
            });
            var original = $('#RangeFilter').clone(true);
            var rangeFilterSetup = function (periodeType) {
                var toRemove = {};
                switch (graphicType) {
                    case "tabular":
                    case "tank":
                    case "speedometer":
                    case "traffic":
                    case "pie":
                        toRemove.hourly = [];
                        toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear'];
                        toRemove.weekly = [];
                        toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear'];
                        toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth'];
                        break;
                    default:
                        toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                        break;
                }
                var originalClone = original.clone(true);
                originalClone.find('option').each(function (i, val) {
                    if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                        originalClone.find(val).remove();
                    }
                });
                $('#RangeFilter').replaceWith(originalClone);
            };

            rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
            $('#PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                rangeFilterSetup($this.val().toLowerCase().trim());
                $('#range-holder').removeAttr('class');
            });

        };

        $('#graphic-type').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            loadGraph($this.data('graph-url'), $this.val());
            if ($this.val() === 'bar') {
                $('.netback-chart-opt').show();
            } else {
                $('.netback-chart-opt').hide();
            }
        });
        if ($('#graphic-type').val() == 'bar') {
            $('.netback-chart-opt').show();
        }
        var specificDate = function () {
            $(".datepicker").on("dp.change", function (e) {
                if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                    $('#EndInDisplay').val($('#StartInDisplay').val());
                }
            });
        };
        specificDate();
        rangeControl();
        rangeDatePicker();
        if (['speedometer', 'trafficlight', 'tabular', 'tank', 'pie', 'multiaxis'].indexOf($('#graphic-type').val()) > -1) {
            $('.scale').css('display', 'none');
        } else {
            $('.scale').css('display', 'block');
        }
        switch ($('#graphic-type').val()) {
            case 'multiaxis':
                var $hiddenFields = $('#hidden-fields');
                var chartTemplate = $hiddenFields.find('.chart-template.original');
                var chartTemplateClone = chartTemplate.clone(true);
                chartTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(chartTemplateClone);
                chartTemplate.remove();
                $('#charts-holder').append($hiddenFields.html());
                $('#charts-holder').find('.chart-template').each(function (i, val) {
                    var $thisTemplate = $(val);
                    Pear.Artifact.Designer._colorPicker($thisTemplate.find('.value-axis-color'));
                    $thisTemplate.find('.multiaxis-graphic-type').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        artifactDesigner.Multiaxis._loadGraph($this.data('graph-url'), $this.val(), $thisTemplate);
                    });
                    switch ($thisTemplate.find('.multiaxis-graphic-type').val()) {
                        case 'line':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                $this.addClass('singlestack');
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this, true, $this.closest('.chart-template'));
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            Pear.Artifact.Designer.Multiaxis._setupCallbacks.line($thisTemplate);
                            break;
                        case 'bar':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this, true, $this.closest('.chart-template'));
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Multiaxis._setupCallbacks.bar($thisTemplate);
                            break;
                        case 'baraccumulative':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });

                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this, true, $this.closest('.chart-template'));
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Multiaxis._setupCallbacks.baraccumulative($thisTemplate);
                            break;
                        case 'barachievement':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });

                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this, true, $this.closest('.chart-template'));
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Multiaxis._setupCallbacks.baraccumulative($thisTemplate);
                            break;
                        case 'area':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this, true, $this.closest('.chart-template'));
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Multiaxis._setupCallbacks.area($thisTemplate);
                            break;
                    }
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.multiaxis();
                break;
            case 'combo':
                var $hiddenFields = $('#hidden-fields');
                var chartTemplate = $hiddenFields.find('.chart-template.original');
                var chartTemplateClone = chartTemplate.clone(true);
                chartTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(chartTemplateClone);
                chartTemplate.remove();
                $('#charts-holder').append($hiddenFields.html());
                $('#charts-holder').find('.chart-template').each(function (i, val) {
                    var $thisTemplate = $(val);
                    Pear.Artifact.Designer._colorPicker($thisTemplate.find('.value-axis-color'));
                    $thisTemplate.find('.multiaxis-graphic-type').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        artifactDesigner.Combo._loadGraph($this.data('graph-url'), $this.val(), $thisTemplate);
                    });
                    switch ($thisTemplate.find('.multiaxis-graphic-type').val()) {
                        case 'line':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                $this.addClass('singlestack');
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this);
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            Pear.Artifact.Designer.Combo._setupCallbacks.line($thisTemplate);
                            break;
                        case 'bar':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this);
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Combo._setupCallbacks.bar($thisTemplate);
                            break;
                        case 'baraccumulative':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });

                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this);
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Combo._setupCallbacks.baraccumulative($thisTemplate);
                            break;
                        case 'barachievement':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });

                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this);
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            $hiddenFields.remove();
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            Pear.Artifact.Designer.Combo._setupCallbacks.baraccumulative($thisTemplate);
                            break;
                        case 'area':
                            var $hiddenFields = $thisTemplate.find('.hidden-fields');
                            $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                                $this = $(val);
                                if ($this.find('.stack-template').length) {
                                    $this.addClass('multistacks');
                                } else {
                                    $this.addClass('singlestack');
                                }
                                $this.addClass($thisTemplate.find('.value-axis-opt').val());
                                $this.addClass($thisTemplate.find('.multiaxis-graphic-type').val());
                            });
                            var seriesTemplate = $hiddenFields.find('.series-template.original');
                            var seriesTemplateClone = seriesTemplate.clone(true);
                            seriesTemplateClone.children('input:first-child').remove();
                            seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                            $thisTemplate.find('.hidden-fields-holder').html(seriesTemplateClone);
                            seriesTemplate.remove();
                            $thisTemplate.find('.series-holder').append($hiddenFields.html());
                            $thisTemplate.find('.series-holder').find('.series-template').each(function (i, val) {
                                var $this = $(val);
                                Pear.Artifact.Designer._kpiAutoComplete($this);
                                Pear.Artifact.Designer._colorPicker($this);
                            });
                            var stackTemplate = $thisTemplate.find('.hidden-fields-holder').find('.stack-template.original');
                            var stackTemplateClone = stackTemplate.clone(true);
                            stackTemplate.closest('.hidden-fields-holder').append(stackTemplateClone);
                            stackTemplate.remove();
                            $hiddenFields.remove();
                            Pear.Artifact.Designer.Combo._setupCallbacks.area($thisTemplate);
                            break;
                    }
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.combo();
                break;
            case 'speedometer':
                var $hiddenFields = $('#hidden-fields');
                var plotTemplate = $hiddenFields.find('.plot-band-template.original');
                var plotTemplateClone = plotTemplate.clone(true);
                plotTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(plotTemplateClone);
                plotTemplate.remove();
                $('#plot-bands-holder').append($hiddenFields.html());
                $('#plot-bands-holder').find('.plot-band-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.speedometer();
                break;
            case 'trafficlight':
                var $hiddenFields = $('#hidden-fields');
                var plotTemplate = $hiddenFields.find('.plot-band-template.original');
                var plotTemplateClone = plotTemplate.clone(true);
                plotTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(plotTemplateClone);
                plotTemplate.remove();
                $('#plot-bands-holder').append($hiddenFields.html());
                $('#plot-bands-holder').find('.plot-band-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.trafficlight();
                break;
            case 'line':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    $this.addClass('singlestack');
                    $this.addClass($('#bar-value-axis').val());
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.line();
                break;
            case 'bar':
            case 'barhorizontal':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    if ($this.find('.stack-template').length) {
                        $this.addClass('multistacks');
                    } else {
                        $this.addClass('singlestack');
                    }
                    $this.addClass($('#bar-value-axis').val());
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                var stackTemplate = $('#hidden-fields-holder').find('.stack-template.original');
                var stackTemplateClone = stackTemplate.clone(true);
                stackTemplate.closest('#hidden-fields-holder').append(stackTemplateClone);
                stackTemplate.remove();
                Pear.Artifact.Designer._setupCallbacks.bar();
                break;
            case 'baraccumulative':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    if ($this.find('.stack-template').length) {
                        $this.addClass('multistacks');
                    } else {
                        $this.addClass('singlestack');
                    }
                    $this.addClass($('#bar-value-axis').val());
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                var stackTemplate = $('#hidden-fields-holder').find('.stack-template.original');
                var stackTemplateClone = stackTemplate.clone(true);
                stackTemplate.closest('#hidden-fields-holder').append(stackTemplateClone);
                stackTemplate.remove();
                Pear.Artifact.Designer._setupCallbacks.baraccumulative();
                break;
            case 'barachievement':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    if ($this.find('.stack-template').length) {
                        $this.addClass('multistacks');
                    } else {
                        $this.addClass('singlestack');
                    }
                    $this.addClass($('#bar-value-axis').val());
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                var stackTemplate = $('#hidden-fields-holder').find('.stack-template.original');
                var stackTemplateClone = stackTemplate.clone(true);
                stackTemplate.closest('#hidden-fields-holder').append(stackTemplateClone);
                stackTemplate.remove();
                Pear.Artifact.Designer._setupCallbacks.baraccumulative();
                break;
            case 'tank':
                $('.main-value-axis').css('display', 'none');
                $('.form-measurement').css('display', 'none');
                Pear.Artifact.Designer._setupCallbacks.tank();
            case 'area':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    if ($this.find('.stack-template').length) {
                        $this.addClass('multistacks');
                    } else {
                        $this.addClass('singlestack');
                    }
                    $this.addClass($('#bar-value-axis').val());
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                seriesTemplateClone.find('.stack-template').children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                var stackTemplate = $('#hidden-fields-holder').find('.stack-template.original');
                var stackTemplateClone = stackTemplate.clone(true);
                stackTemplate.closest('#hidden-fields-holder').append(stackTemplateClone);
                stackTemplate.remove();
                Pear.Artifact.Designer._setupCallbacks.area();
                break;
            case 'tabular':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.row-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    $this.addClass('singlestack');
                });
                var rowsTemplate = $hiddenFields.find('.row-template.original');
                var rowsTemplateClone = rowsTemplate.clone(true);
                rowsTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(rowsTemplateClone);
                rowsTemplate.remove();
                $('#rows-holder').append($hiddenFields.html());
                $('#rows-holder').find('.row-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this, false);
                    Pear.Artifact.Designer._colorPicker($this);
                    if ($this.find('.range-filter').val().indexOf('Specific') > -1) {
                        $this.find('#range-holder').attr('style', 'display:block !important');
                        $this.find('.end-in-display').hide();
                    }
                    if ($this.find('.range-filter').val().indexOf('Interval') > -1) {
                        $this.find('#range-holder').attr('style', 'display:block !important');
                    }
                    artifactDesigner._setupCallbacks.tabularrow.rangeDatePicker($this);
                    artifactDesigner._setupCallbacks.tabularrow.rangeControl($this);
                    artifactDesigner._setupCallbacks.tabularrow.specificDate($this);
                    var periodeType = $this.find('.periode-type').val();
                    /*if(periodeType === 'Monthly')
                    {
                        $this.find('.datepicker').datetimepicker({
                            format: "MM/YYYY"
                        });
                    }*/

                });
                $hiddenFields.remove();
                $('#general-graphic-settings').css('display', 'none');
                $('.form-measurement').css('display', 'none');

                Pear.Artifact.Designer._setupCallbacks.tabular();


                break;
            case 'pie':
                var $hiddenFields = $('#hidden-fields');
                $hiddenFields.find('.series-template:not(.original)').each(function (i, val) {
                    $this = $(val);
                    $this.addClass($('#graphic-type').val());
                });
                var seriesTemplate = $hiddenFields.find('.series-template.original');
                var seriesTemplateClone = seriesTemplate.clone(true);
                seriesTemplateClone.children('input:first-child').remove();
                $('#hidden-fields-holder').html(seriesTemplateClone);
                seriesTemplate.remove();
                $('#series-holder').append($hiddenFields.html());
                $('#series-holder').find('.series-template').each(function (i, val) {
                    var $this = $(val);
                    Pear.Artifact.Designer._kpiAutoComplete($this);
                    Pear.Artifact.Designer._colorPicker($this);
                });
                $hiddenFields.remove();
                Pear.Artifact.Designer._setupCallbacks.pie();
                break;
        }

        //$('#PeriodeType').change();
        $('#RangeFilter').change();
    };

    artifactDesigner._setupCallbacks = {};

    artifactDesigner.Preview = function () {
        $('#graphic-preview-btn').click(function (e) {
            e.preventDefault();
            Pear.Loading.Show($('#container'));
            $('#graphic-preview').modal('show');
            var $this = $(this);
            var callback = Pear.Artifact.Designer._previewCallbacks;
            $.ajax({
                url: $this.data('preview-url'),
                data: $this.closest('form').serialize(),
                method: 'POST',
                success: function (data) {
                    if (callback.hasOwnProperty(data.GraphicType)) {
                        Pear.Loading.Stop($('#container'));
                        callback[data.GraphicType](data, $('#container'));
                    }
                }
            });
        });

        $('#graphic-preview').on('show.bs.modal', function () {
            $('#container').css('visibility', 'hidden');
        });

        $('#graphic-preview').on('shown.bs.modal', function () {
            $('#container').css('visibility', 'visible');
            if ($('#container').highcharts() !== undefined)
                $('#container').highcharts().reflow();
        });

        $('#graphic-preview').on('hidden.bs.modal', function () {
            $('#container').html('');
        });
    };

    artifactDesigner._previewCallbacks = {};

    //bar chart
    artifactDesigner._setupCallbacks.bar = function () {
        var removeSeriesOrStack = function () {
            $('.series-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
            $('.stack-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.stack-template').remove();
            });
        }
        var addSeries = function () {
            var seriesCount = $('#series-holder').find('.series-template').length + 1;
            $('#add-series').click(function (e) {
                e.preventDefault();
                var seriesTemplate = $('.series-template.original').clone(true);

                Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'BarChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'ValueAxis', 'Color', 'PreviousColor'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('#BarChart_Series_0__' + field).attr('name', 'BarChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass($('#seriesType').val().toLowerCase());
                seriesTemplate.addClass($('#bar-value-axis').val());
                seriesTemplate.addClass($('#graphic-type').val());
                $('#series-holder').append(seriesTemplate);
                seriesCount++;
            });
        };
        var addStack = function () {
            var stackCount = $('#series-holder').find('.stack-template').length + 1;
            $('.add-stack').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var stackTemplate = $('.stack-template.original').clone(true);
                Pear.Artifact.Designer._kpiAutoComplete(stackTemplate);
                Pear.Artifact.Designer._colorPicker(stackTemplate);
                stackTemplate.removeClass('original');
                var seriesPos = $this.closest('.series-template').data('series-pos');
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'BarChart.Series[' + seriesPos + '].Stacks.Index',
                    value: stackCount
                }).appendTo(stackTemplate);
                var fields = ['Label', 'KpiId', 'ValueAxis', 'Color'];
                for (var i in fields) {
                    var field = fields[i];
                    stackTemplate.find('#BarChart_Series_0__Stacks_0__' + field).attr('name', 'BarChart.Series[' + seriesPos + '].Stacks[' + stackCount + '].' + field);
                }
                $this.closest('.stacks-holder').append(stackTemplate);
                stackCount++;
            });
        };

        removeSeriesOrStack();
        addSeries();
        addStack();
    };

    artifactDesigner._previewCallbacks.bar = function (data, container) {
        //console.log(data.BarChart.SeriesType);
        if (data.BarChart.SeriesType == "single-stack") {
            Pear.Artifact.Designer._displayBasicBarChart(data, container);
        } else if (data.BarChart.SeriesType == "multi-stack") {
            Pear.Artifact.Designer._displayMultistacksBarChart(data, container);
        } else {
            Pear.Artifact.Designer._displayMultistacksGroupedBarChart(data, container);
        }
    };

    artifactDesigner._displayBasicBarChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'column',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                labels: {
                    style: {
                        color: '#fff'
                    }
                },
                gridLineColor: '#fff',
            },
            yAxis: {
                //min: 0,
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        color: '#fff'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    var netbackValue = 0;
                    for (var i in this.points) {
                        if (this.points[i].series.name.trim().indexOf('invisible')) {
                            tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';
                        }
                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';
                        if (data.AsNetbackChart) {
                            if (i == 0) {
                                netbackValue += this.points[i].y;
                            } else {
                                if (this.points[i].series.name.trim().indexOf('invisible')) {
                                    netbackValue -= this.points[i].y;
                                }
                            }

                            if (i == this.points.length - 2) {
                                //tooltip += '<strong>Net back value : ' + netbackValue.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                tooltip += '<strong>' + this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<strong><br/>';
                            }
                        } else {
                            if (typeof this.points[i].series.stackKey !== 'undefined') {
                                //total in process
                                //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                totalInProcess += this.points[i].y;
                                //}

                                if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                    (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                    tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                    //totalInProcess = 0;
                                }
                                if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                    totalInProcess = 0;
                                }
                            }

                        }


                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            //tooltip: {
            //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            //        '<td style="padding:0"><b>{point.y:.1f} ' + data.BarChart.ValueAxisTitle + '</b></td></tr>',
            //    footerFormat: '</table>',
            //    shared: true,
            //    useHTML: true
            //},
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: data.BarChart.Series
        });
    };

    artifactDesigner._displayMultistacksBarChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'column',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    "color": '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    "color": '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
            },
            yAxis: {
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        "color": '#fff'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //total in process
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>' +
            //            'Total: ' + this.point.stackTotal.format(2) + ' ' + data.BarChart.ValueAxisTitle;
            //    }
            //},
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                column: {
                    stacking: 'normal'
                }
            },
            series: data.BarChart.Series
        });
    };

    artifactDesigner._displayMultistacksGroupedBarChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'column',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    "color": '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    "color": '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                }
            },
            yAxis: {
                //min: 0,
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    //console.log(data);
                    var tooltip = '';
                    if (data.BarChart.Periodes.length == 1) {
                        tooltip += '<b>' + data.BarChart.Subtitle + '</b><br/>';
                    } else if (data.TimePeriodes.length) {
                        tooltip += '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    } else {

                        tooltip += '<b>' + data.BarChart.Subtitle + '</b><br/>';
                    }
                    var totalInProcess = 0;
                    var netbackValue = 0;
                    for (var i in this.points) {
                        if (this.points[i].series.name.trim().indexOf('invisible')) {
                            tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';
                        }
                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';
                        if (data.AsNetbackChart) {
                            if (i == 0) {
                                netbackValue += this.points[i].y;
                            } else {
                                if (this.points[i].series.name.trim().indexOf('invisible')) {
                                    netbackValue -= this.points[i].y;
                                }
                            }

                            if (i == this.points.length - 2) {
                                //console.log(this.points[i].series.name);
                                //tooltip += '<strong>Net back value : ' + netbackValue.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                tooltip += '<strong>' + this.points[i].series.name.trim() + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<strong><br/>';
                            }
                        } else {
                            if (typeof this.points[i].series.stackKey !== 'undefined') {
                                //total in process
                                //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                totalInProcess += this.points[i].y;
                                //}

                                if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                    (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                    tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                    //totalInProcess = 0;
                                }
                                if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                    totalInProcess = 0;
                                }
                            }
                        }
                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights !== null && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>' +
            //            'Total: ' + this.point.stackTotal.format(2) + ' ' + data.BarChart.ValueAxisTitle;
            //    }
            //},
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                column: {
                    stacking: 'normal'
                }
            },
            series: data.BarChart.Series
        });
    };

    artifactDesigner._setupCallbacks.baraccumulative = function () {
        Pear.Artifact.Designer._setupCallbacks.bar();
    };

    artifactDesigner._previewCallbacks.baraccumulative = function (data, container) {
        Pear.Artifact.Designer._previewCallbacks.bar(data, container);
    };

    artifactDesigner._setupCallbacks.barachievement = function () {
        $('#bar-value-axis').val('KpiActual');
        $('.main-value-axis').css('display', 'none');
        Pear.Artifact.Designer._setupCallbacks.bar();
    };

    artifactDesigner._previewCallbacks.barachievement = function (data, container) {
        Pear.Artifact.Designer._previewCallbacks.bar(data, container);
    };

    artifactDesigner._setupCallbacks.barhorizontal = function () {
        artifactDesigner._setupCallbacks.bar();
    }

    artifactDesigner._previewCallbacks.barhorizontal = function (data, container) {
        //console.log(data.BarChart.SeriesType);
        if (data.BarChart.SeriesType == "single-stack") {
            Pear.Artifact.Designer._displayBasicBarhorizontalChart(data, container);
        } else if (data.BarChart.SeriesType == "multi-stack") {
            Pear.Artifact.Designer._displayMultistacksBarhorizontalChart(data, container);
        } else {
            Pear.Artifact.Designer._displayMultistacksGroupedBarhorizontalChart(data, container);
        }
    };

    artifactDesigner._displayBasicBarhorizontalChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'bar',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                labels: {
                    style: {
                        color: '#fff'
                    }
                },
                gridLineColor: '#fff',
                reversed: false
            },
            yAxis: {
                //min: 0,
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    var netbackValue = 0;
                    for (var i in this.points) {
                        if (this.points[i].series.name.trim().indexOf('invisible')) {
                            tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';
                        }
                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';
                        if (data.AsNetbackChart) {
                            if (i == 0) {
                                netbackValue += this.points[i].y;
                            } else {
                                if (this.points[i].series.name.trim().indexOf('invisible')) {
                                    netbackValue -= this.points[i].y;
                                }
                            }

                            if (i == this.points.length - 2) {
                                //tooltip += '<strong>Net back value : ' + netbackValue.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                tooltip += '<strong>' + this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<strong><br/>';
                            }
                        } else {
                            if (typeof this.points[i].series.stackKey !== 'undefined') {
                                //total in process
                                //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                totalInProcess += this.points[i].y;
                                //}

                                if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                    (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                    tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                    //totalInProcess = 0;
                                }
                                if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                    totalInProcess = 0;
                                }
                            }

                        }


                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            //tooltip: {
            //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            //        '<td style="padding:0"><b>{point.y:.1f} ' + data.BarChart.ValueAxisTitle + '</b></td></tr>',
            //    footerFormat: '</table>',
            //    shared: true,
            //    useHTML: true
            //},
            plotOptions: {
                bar: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: data.BarChart.Series
        });
    };

    artifactDesigner._displayMultistacksBarhorizontalChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'bar',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    "color": '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    "color": '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
                reverse: false
            },
            yAxis: {
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //total in process
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>' +
            //            'Total: ' + this.point.stackTotal.format(2) + ' ' + data.BarChart.ValueAxisTitle;
            //    }
            //},
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                bar: {
                    stacking: 'normal'
                }
            },
            series: data.BarChart.Series
        });
    };

    artifactDesigner._displayMultistacksGroupedBarhorizontalChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'bar',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.BarChart.Title,
                style: {
                    "color": '#fff'
                }
            },
            subtitle: {
                text: data.BarChart.Subtitle,
                style: {
                    "color": '#fff'
                }
            },
            xAxis: {
                categories: data.BarChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
                reversed: false
            },
            yAxis: {
                //min: 0,
                title: {
                    text: data.BarChart.ValueAxisTitle,
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        "color": '#fff',
                        fontSize: '12px'
                    }
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    //console.log(data);
                    var tooltip = '';
                    if (data.BarChart.Periodes.length == 1) {
                        tooltip += '<b>' + data.BarChart.Subtitle + '</b><br/>';
                    } else if (data.TimePeriodes.length) {
                        tooltip += '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    } else {

                        tooltip += '<b>' + data.BarChart.Subtitle + '</b><br/>';
                    }
                    var totalInProcess = 0;
                    var netbackValue = 0;
                    for (var i in this.points) {
                        if (this.points[i].series.name.trim().indexOf('invisible')) {
                            tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>';
                        }
                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';
                        if (data.AsNetbackChart) {
                            if (i == 0) {
                                netbackValue += this.points[i].y;
                            } else {
                                if (this.points[i].series.name.trim().indexOf('invisible')) {
                                    netbackValue -= this.points[i].y;
                                }
                            }

                            if (i == this.points.length - 2) {
                                //console.log(this.points[i].series.name);
                                //tooltip += '<strong>Net back value : ' + netbackValue.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                tooltip += '<strong>' + this.points[i].series.name.trim() + ': ' + this.points[i].y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<strong><br/>';
                            }
                        } else {
                            if (typeof this.points[i].series.stackKey !== 'undefined') {
                                //total in process
                                //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                totalInProcess += this.points[i].y;
                                //}

                                if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                    (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                    tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.BarChart.ValueAxisTitle + '</strong><br>';
                                    //totalInProcess = 0;
                                }
                                if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                    totalInProcess = 0;
                                }
                            }
                        }
                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights !== null && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.BarChart.ValueAxisTitle + '<br/>' +
            //            'Total: ' + this.point.stackTotal.format(2) + ' ' + data.BarChart.ValueAxisTitle;
            //    }
            //},
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.BarChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                bar: {
                    stacking: 'normal'
                }
            },
            series: data.BarChart.Series
        });
    };

    //line chart
    artifactDesigner._setupCallbacks.line = function () {
        var removeSeriesOrStack = function () {
            $('.series-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
        }
        var addSeries = function () {
            //console.log('add-series');
            var seriesCount = $('#series-holder').find('.series-template').length + 1;
            $('#add-series').click(function (e) {
                //console.log('series-click');
                e.preventDefault();
                var seriesTemplate = $('.series-template.original').clone(true);

                Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LineChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'ValueAxis', 'Color', 'MarkerColor', 'LineType'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('#LineChart_Series_0__' + field).attr('name', 'LineChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass($('#bar-value-axis').val());
                seriesTemplate.addClass($('#graphic-type').val());
                $('#series-holder').append(seriesTemplate);
                seriesCount++;
            });
        };
        removeSeriesOrStack();
        addSeries();
    };

    artifactDesigner._previewCallbacks.line = function (data, container) {
        container.highcharts({
            chart: {
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.LineChart.Title,
                //x: -20, //center
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.LineChart.Subtitle,
                //x: -20,
                style: {
                    color: '#fff'
                }
            },
            plotOptions: {
                line: {
                    marker: {
                        enabled: false,
                        //radius:2,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                }
            },
            xAxis: {
                categories: data.LineChart.Periodes,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                }
            },
            yAxis: {
                title: {
                    text: data.LineChart.ValueAxisTitle,
                    style: {
                        "color": '#fff'
                    }
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }],
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        "color": '#fff'
                    }
                }
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.LineChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                                this.yAxis[0].axisTitle.attr({
                                    "font-size": '12px'
                                });

                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.LineChart.ValueAxisTitle + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        //total in process
                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.LineChart.ValueAxisTitle + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.LineChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            series: data.LineChart.Series
        });
    };

    //area chart
    artifactDesigner._setupCallbacks.area = function () {
        var removeSeriesOrStack = function () {
            $('.series-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
            $('.stack-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.stack-template').remove();
            });
        }
        var addSeries = function () {
            //console.log('add-series');
            var seriesCount = $('#series-holder').find('.series-template').length + 1;
            $('#add-series').click(function (e) {
                //console.log('series-click');
                e.preventDefault();
                var seriesTemplate = $('.series-template.original').clone(true);

                Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'AreaChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'Color', 'ValueAxis'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('#AreaChart_Series_0__' + field).attr('name', 'AreaChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass($('#seriesType').val().toLowerCase());
                seriesTemplate.addClass($('#bar-value-axis').val());
                seriesTemplate.addClass($('#graphic-type').val());
                $('#series-holder').append(seriesTemplate);
                seriesCount++;
            });
        };
        var addStack = function () {
            var stackCount = $('#series-holder').find('.stack-template').length + 1;
            $('.add-stack').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var stackTemplate = $('.stack-template.original').clone(true);
                Pear.Artifact.Designer._kpiAutoComplete(stackTemplate);
                Pear.Artifact.Designer._colorPicker(stackTemplate);
                stackTemplate.removeClass('original');
                var seriesPos = $this.closest('.series-template').data('series-pos');
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'AreaChart.Series[' + seriesPos + '].Stacks.Index',
                    value: stackCount
                }).appendTo(stackTemplate);
                var fields = ['Label', 'KpiId', 'ValueAxis', 'Color'];
                for (var i in fields) {
                    var field = fields[i];
                    stackTemplate.find('#AreaChart_Series_0__Stacks_0__' + field).attr('name', 'AreaChart.Series[' + seriesPos + '].Stacks[' + stackCount + '].' + field);
                }
                $this.closest('.stacks-holder').append(stackTemplate);
                stackCount++;
            });
        };
        removeSeriesOrStack();
        addSeries();
        addStack();
    };

    artifactDesigner._previewCallbacks.area = function (data, container) {
        if (data.AreaChart.SeriesType == "single-stack") {
            Pear.Artifact.Designer._displayBasicAreaChart(data, container);
        } else {
            Pear.Artifact.Designer._displayMultistacksAreaChart(data, container);
        }
    };

    artifactDesigner._displayBasicAreaChart = function (data, container) {
        container.highcharts({
            chart: {
                type: 'area',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.AreaChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.AreaChart.Subtitle,
                style: {
                    color: '#fff'
                }
                //x: -20
            },
            xAxis: {
                //allowDecimals: false,
                labels: {
                    formatter: function () {
                        return this.value; // clean, unformatted number for year
                    },
                    style: {
                        color: '#fff'
                    }
                },
                gridLineColor: '#fff',
                categories: data.AreaChart.Periodes
            },
            yAxis: {
                title: {
                    text: data.AreaChart.ValueAxisTitle,
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    },
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        //total in process
                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.AreaChart.ValueAxisTitle;
            //    }
            //},
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.AreaChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                area: {
                    lineColor: '#666666',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#666666',
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                }
            },
            series: data.AreaChart.Series
        });
    };

    artifactDesigner._displayMultistacksAreaChart = function (data, container) {
        data.AreaChart.Series = data.AreaChart.Series.reverse();
        //console.log(data);
        container.highcharts({
            chart: {
                type: 'area',
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.AreaChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.AreaChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            xAxis: {
                labels: {
                    formatter: function () {
                        return this.value; // clean, unformatted number for year
                    },
                    style: {
                        color: '#fff'
                    }
                },
                categories: data.AreaChart.Periodes,
                gridLineColor: '#fff'
            },
            yAxis: {
                title: {
                    text: data.AreaChart.ValueAxisTitle,
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                labels: {
                    style: {
                        color: '#fff'
                    }
                }
            },
            //tooltip: {
            //    shared: true,
            //    valueSuffix: ' ' + data.AreaChart.ValueAxisTitle
            //},
            //tooltip: {
            //    formatter: function () {
            //        return '<b>' + this.x + '</b><br/>' +
            //            this.series.name + ': ' + this.y.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '<br/>' +
            //            'Total: ' + this.point.stackTotal.format(2) + ' ' + data.AreaChart.ValueAxisTitle;
            //    }
            //},
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        //total in process
                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + data.AreaChart.ValueAxisTitle + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            plotOptions: {
                area: {
                    stacking: 'normal',
                    lineColor: '#fff',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#fff',
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                }
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.AreaChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            series: data.AreaChart.Series
        });
    };

    //speedometer
    artifactDesigner._setupCallbacks.speedometer = function () {
        var removePlot = function () {
            $('.plot-band-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.plot-band-template').remove();
            });
        };

        var addPlot = function () {
            var plotPos = $('#plot-bands-holder').find('.plot-band-template').length + 1;
            $('#add-plot').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var plotBandTemplate = $('.plot-band-template.original').clone(true);
                plotBandTemplate.removeClass('original');
                Pear.Artifact.Designer._kpiAutoComplete(plotBandTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'SpeedometerChart.PlotBands.Index',
                    value: plotPos
                }).appendTo(plotBandTemplate);
                if (plotPos !== 0) {
                    var fields = ['From', 'To', 'Color'];
                    for (var i in fields) {
                        var field = fields[i];
                        plotBandTemplate.find('#SpeedometerChart_PlotBands_0__' + field).attr('name', 'SpeedometerChart.PlotBands[' + plotPos + '].' + field).attr('id', 'plot-bands-' + i);
                    }
                }
                Pear.Artifact.Designer._colorPicker(plotBandTemplate);
                $('#plot-bands-holder').append(plotBandTemplate);
                plotPos++;
            });
        };



        var rangeDatePicker = function () {
            $('.datepicker').datetimepicker({
                format: "MM/DD/YYYY hh:00 A"
            });
            $('.datepicker').change(function (e) {
                //console.log(this);
            });
            $('#SpeedometerChart_PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                var clearValue = $('.datepicker').each(function (i, val) {
                    $(val).val('');
                    $(val).data("DateTimePicker").destroy();
                });
                switch ($this.val().toLowerCase().trim()) {
                    case 'hourly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY hh:00 A"
                        });
                        break;
                    case 'daily':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY"
                        });
                        break;
                    case 'weekly':
                        $('.datepicker').datetimepicker({
                            format: "MM/DD/YYYY",
                            daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                        });
                        break;
                    case 'monthly':
                        $('.datepicker').datetimepicker({
                            format: "MM/YYYY"
                        });
                        break;
                    case 'yearly':
                        $('.datepicker').datetimepicker({
                            format: "YYYY"
                        });
                        break;
                    default:

                }
            });
        };
        var rangeControl = function () {
            $('#SpeedometerChart_RangeFilter').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                $('#range-holder').prop('class', $this.val().toLowerCase().trim());
            });
            var original = $('#SpeedometerChart_RangeFilter').clone(true);
            var rangeFilterSetup = function (periodeType) {
                var toRemove = {};
                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD'];
                toRemove.daily = ['CurrentHour', 'CurrentYear', 'DTD', 'YTD'];
                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD'];
                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD'];
                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD'];
                var originalClone = original.clone(true);
                originalClone.find('option').each(function (i, val) {
                    if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                        originalClone.find(val).remove();
                    }
                });
                $('#SpeedometerChart_RangeFilter').replaceWith(originalClone);
            };

            rangeFilterSetup($('#SpeedometerChart_PeriodeType').val().toLowerCase().trim());
            $('#SpeedometerChart_PeriodeType').change(function (e) {
                e.preventDefault();
                var $this = $(this);
                rangeFilterSetup($this.val().toLowerCase().trim());
                $('#range-holder').removeAttr('class');
            });

        };

        Pear.Artifact.Designer._kpiAutoComplete($('#graphic-settings'));
        removePlot();
        addPlot();
        //rangeControl();
        //rangeDatePicker();
    };

    artifactDesigner._previewCallbacks.speedometer = function (data, container) {
        container.highcharts({
            chart: {
                type: 'gauge',
                plotBackgroundColor: null,
                plotBackgroundImage: null,
                plotBorderWidth: 0,
                plotShadow: false,
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },

            title: {
                text: data.SpeedometerChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.SpeedometerChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            pane: {
                startAngle: -150,
                endAngle: 150,
                background: [{
                    backgroundColor: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, '#FFF'],
                            [1, '#333']
                        ]
                    },
                    borderWidth: 0,
                    outerRadius: '109%'
                }, {
                    backgroundColor: {
                        linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                        stops: [
                            [0, '#333'],
                            [1, '#FFF']
                        ]
                    },
                    borderWidth: 1,
                    outerRadius: '107%'
                }, {

                    // default background
                }, {
                    backgroundColor: '#DDD',
                    borderWidth: 0,
                    outerRadius: '105%',
                    innerRadius: '103%'
                }]
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.SpeedometerChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            // the value axis
            yAxis: {
                min: data.SpeedometerChart.PlotBands[0].from,
                max: data.SpeedometerChart.PlotBands[data.SpeedometerChart.PlotBands.length - 1].to,

                minorTickInterval: 'auto',
                minorTickWidth: 1,
                minorTickLength: 10,
                minorTickPosition: 'inside',
                minorTickColor: '#666',

                tickPixelInterval: 30,
                tickWidth: 2,
                tickPosition: 'inside',
                tickLength: 10,
                tickColor: '#666',
                labels: {
                    step: 2,
                    rotation: 'auto'
                },
                title: {
                    text: data.SpeedometerChart.ValueAxisTitle,
                    style: {
                        color: '#fff'
                    }
                },
                plotBands: data.SpeedometerChart.PlotBands
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            series: [{
                name: data.SpeedometerChart.Series.name,
                data: data.SpeedometerChart.Series.data,
                tooltip: {
                    valueSuffix: ' ' + data.SpeedometerChart.ValueAxisTitle
                }
            }]
        });
    };

    //tabular
    artifactDesigner._setupCallbacks.tabularrow = {};

    artifactDesigner._setupCallbacks.tabularrow.rangeControl = function (context) {
        context.find('.range-filter').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            context.find('#range-holder').prop('class', $this.val().toLowerCase().trim());
        });
        var original = context.find('.range-filter').clone(true);
        var rangeFilterSetup = function (periodeType) {
            var toRemove = {};

            toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
            toRemove.daily = ['CurrentHour', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
            toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
            toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
            toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];

            var originalClone = original.clone(true);
            originalClone.find('option').each(function (i, val) {
                if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                    originalClone.find(val).remove();
                }
            });
            context.find('.range-filter').replaceWith(originalClone);
            debugger;
            switch (context.find('.periode-type').val().toLowerCase().trim()) {
                case 'daily':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY"
                    });
                    break;
                case 'monthly':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/YYYY"
                    });
                    break;
                case 'yearly':
                    context.find('.datepicker').datetimepicker({
                        format: "YYYY"
                    });
                    break;
            }
        };

        rangeFilterSetup(context.find('.periode-type').val().toLowerCase().trim());
        context.find('.periode-type').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            rangeFilterSetup($this.val().toLowerCase().trim());
            context.find('#range-holder').removeAttr('class');

        });
    };

    artifactDesigner._setupCallbacks.tabularrow.specificDate = function (context) {
        $(".datepicker").on("dp.change", function (e) {
            if (context.find('.range-filter').val().toLowerCase().indexOf('specific') > -1) {//&& e.target.class === 'start-in-display') {
                context.find('.end-in-display').val(context.find('.start-in-display').val());
            }
        });
    };

    artifactDesigner._setupCallbacks.tabularrow.rangeDatePicker = function (context) {

        context.find('.datepicker').change(function (e) {
            //console.log(this);
        });
        context.find('.periode-type').change(function (e) {

            e.preventDefault();
            var $this = $(this);
            var clearValue = context.find('.datepicker').each(function (i, val) {
                $(val).val('');
                if ($(val).data("DateTimePicker") != undefined) {
                    $(val).data("DateTimePicker").destroy();
                }

            });
            switch ($this.val().toLowerCase().trim()) {
                case 'hourly':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY hh:00 A"
                    });
                    break;
                case 'daily':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY"
                    });
                    break;
                case 'weekly':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY",
                        daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                    });
                    break;
                case 'monthly':
                    context.find('.datepicker').datetimepicker({
                        format: "MM/YYYY"
                    });
                    break;
                case 'yearly':
                    context.find('.datepicker').datetimepicker({
                        format: "YYYY"
                    });
                    break;
                default:
            }
        });
    };

    artifactDesigner._setupCallbacks.tabular = function () {
        var removeRow = function () {
            $('.row-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.row-template').remove();
            });
        };
        var addRow = function () {
            var rowCount = $('#rows-holder').find('.row-template').length + 1;
            $('#add-row').click(function (e) {
                e.preventDefault();
                var rowTemplate = $('.row-template.original').clone(true);
                Pear.Artifact.Designer._kpiAutoComplete(rowTemplate, false);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'Tabular.Rows.Index',
                    value: rowCount
                }).appendTo(rowTemplate);
                rowTemplate.removeClass('original');
                rowTemplate.attr('data-row-pos', rowCount);
                //if (seriesCount !== 0) {
                var fields = ['PeriodeType', 'KpiId', 'RangeFilter', 'StartInDisplay', 'EndInDisplay'];
                for (var i in fields) {
                    var field = fields[i];
                    rowTemplate.find('#Tabular_Rows_0__' + field).attr('name', 'Tabular.Rows[' + rowCount + '].' + field);
                }
                //}
                //seriesTemplate.addClass($('#seriesType').val().toLowerCase());
                //seriesTemplate.addClass($('#bar-value-axis').val());
                $('#rows-holder').append(rowTemplate);
                artifactDesigner._setupCallbacks.tabularrow.rangeDatePicker(rowTemplate);
                artifactDesigner._setupCallbacks.tabularrow.rangeControl(rowTemplate);
                artifactDesigner._setupCallbacks.tabularrow.specificDate(rowTemplate);
                rowCount++;
            });
        };

        addRow();
        removeRow();
        $('#general-graphic-settings').css('display', 'none');
        $('.form-measurement').css('display', 'none');
    };

    artifactDesigner._previewCallbacks.tabular = function (data, container) {
        var tableUniqueClass = "table-" + Math.floor((Math.random() * 100) + 1);
        var wrapper = $('<div>');
        wrapper.addClass('tabular-wrapper');
        var btn = '<div class="btn-group chart-button">' +
            '<a class="tabular-wrapper-button dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><img src="/Content/img/printer_3.png" width="32" height="32" ></img></a>' +
            '<ul class="dropdown-menu pull-right">';
        if ($('#user-profile-session-data').length > 0) {
            if ($('#user-profile-session-data').data('issuperadmin') == true) {
                btn += '<li><a href="/Artifact/Edit/' + data.Id + '" target="_blank">Edit Chart</a></li>';
            }
            btn +=
                '<li><a href="javascript:;" class="tabular-export">Export To Excel</a></li>';
        } else {
            btn += '<li><a href="javascript:;" class="tabular-export-preview">Export To Excel</a></li>';
        }
        btn +=
            '</ul>' +
            '</div >';
        var title = data.Tabular.Title;
        wrapper.append(btn);
        

        wrapper.append($('<h3>').html(title));

        //container for scrolling table
        var tableScrollContainer = $('<div>');
        tableScrollContainer.addClass('table-scrolling-container');
        tableScrollContainer.addClass('perfect-scrollbar');
        tableScrollContainer.css('width', '100%');
        tableScrollContainer.css('max-height', '270px');
        tableScrollContainer.css('overflow-y', 'auto');

        var panel = $('<div>');
        panel.addClass('panel panel-default tabular-panel ');

        var $table = $('<table>');
        //$table.attr('id', 'table');
        $table.addClass('tabular');
        $table.addClass('table-bordered');
        $table.addClass('table2');
        $table.addClass(tableUniqueClass);
        var arrWidth = ['40%', '15%'];
        var tHead = $('<thead>');
        var rowHeader = $('<tr>');
        rowHeader.append($('<th style="width:' + arrWidth[0] + '">').html('KPI Name'));
        rowHeader.append($('<th style="width:' + arrWidth[1] + '">').html('Periode'));
        var counter = 2;
        var additionalField = 0;
        if (data.Tabular.Actual)
            additionalField += 1;

        if (data.Tabular.Target)
            additionalField += 1;

        if (data.Tabular.Remark)
            additionalField += 1;

        var thWidth = 50 / additionalField;
        arrWidth.push(thWidth);

        if (data.Tabular.Actual) {
            rowHeader.append($('<th style="width:' + arrWidth[2] + '%">').html('Actual'));
            counter += 1;
        }
        if (data.Tabular.Target) {
            rowHeader.append($('<th style="width:' + arrWidth[2] + '%">').html('Target'));
            counter += 1;
        }
        if (data.Tabular.Remark) {
            rowHeader.append($('<th style="width:' + arrWidth[2] + '%">').html('Remark'));
            counter += 1;
        }

        tHead.append(rowHeader);
        $table.append(tHead);
        for (var i in data.Tabular.Rows) {
            var dataRow = data.Tabular.Rows[i];
            var row = $('<tr>');
            row.append($('<td class="tabular-kpi-name">').html(dataRow.KpiName + ' (' + dataRow.Measurement + ')'));
            row.append($('<td class="tabular-actual">').html(dataRow.Periode));
            if (data.Tabular.Actual) {
                row.append($('<td class="tabular-actual">').html(dataRow.Actual == null ? '-' : dataRow.Actual.format(2)));
            }
            if (data.Tabular.Target) {
                row.append($('<td class="tabular-target">').html(dataRow.Target == null ? '-' : dataRow.Target.format(2)));
            }
            if (data.Tabular.Remark) {
                row.append($('<td>').html(dataRow.Remark));
            }
            $table.append(row);
        }

        tableScrollContainer.append(panel);
        panel.append($table);
        wrapper.append(tableScrollContainer);

        container.html(wrapper);

        $('.table2 tbody').perfectScrollbar();

        var resizeTabular = function () {
            for (var j = 1; j <= counter; j++) {
                var width = $('.' + tableUniqueClass + ' thead th:nth-child(' + j + ')').outerWidth();
                $('.' + tableUniqueClass + ' tbody td:nth-child(' + j + ')').outerWidth(width);
            }
        };

        setTimeout(function () {
            resizeTabular();
        }, 500);

        $('.left-content-toggle').click(function () {
            setTimeout(function () {
                resizeTabular();
            }, 500);

        });

    };

    //trafficlight
    artifactDesigner._setupCallbacks.trafficlight = function () {
        var removePlot = function () {
            $('.plot-band-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.plot-band-template').remove();
            });
        };

        var addPlot = function () {
            var plotPos = $('#plot-bands-holder').find('.plot-band-template').length + 1;
            $('#add-plot').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var plotBandTemplate = $('.plot-band-template.original').clone(true);
                plotBandTemplate.removeClass('original');
                Pear.Artifact.Designer._kpiAutoComplete(plotBandTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'TrafficLightChart.PlotBands.Index',
                    value: plotPos
                }).appendTo(plotBandTemplate);
                if (plotPos !== 0) {
                    var fields = ['From', 'To', 'Color', 'Label'];
                    for (var i in fields) {
                        var field = fields[i];
                        plotBandTemplate.find('#TrafficLightChart_PlotBands_0__' + field).attr('name', 'TrafficLightChart.PlotBands[' + plotPos + '].' + field).attr('id', 'plot-bands-' + i);
                    }
                }
                Pear.Artifact.Designer._colorPicker(plotBandTemplate);
                $('#plot-bands-holder').append(plotBandTemplate);
                plotPos++;
            });
        };



        Pear.Artifact.Designer._kpiAutoComplete($('#graphic-settings'));
        removePlot();
        addPlot();
    };

    artifactDesigner._previewCallbacks.trafficlight = function (data, container) {
        container.trafficlight(data.TrafficLightChart);
    };

    //tank
    artifactDesigner._setupCallbacks.tank = function () {
        Pear.Artifact.Designer._colorPicker($('#graphic-settings'));
        /*$('#graphic-settings').find('.colorpicker').each(function (i, val) {
            console.log(val);
            Pear.Artifact.Designer._colorPicker($(val));
        });*/
        $('.main-value-axis').css('display', 'none');
        $('.form-measurement').css('display', 'none');
        Pear.Artifact.Designer._kpiAutoComplete($('#graphic-settings'), false);
    };
    artifactDesigner._previewCallbacks.tank = function (data, container) {
        data.Tank.ArtifactId = data.Id;
        setTimeout(function () {
            container.tank(data.Tank, {
                height: container.height(),
                width: container.width()
            });

            var btn = '<div class="btn-group chart-button">' +
                '<a class="tabular-wrapper-button dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><img src="/Content/img/printer_3.png" width="32" height="32" ></img></a>' +
                '<ul class="dropdown-menu pull-right">';
            if ($('#user-profile-session-data').length > 0) {
                btn += '<li><a class="tank-subtitle" href="javascript:;">Change Periode</a></li>';
                btn += '<li><a class="tank-export" href="javascript:;">Export To Excel</a></li>';
            } else {
                btn += '<li><a class="tank-export-preview" href="javascript:;">Export To Excel</a></li>';
            }

            if ($('#user-profile-session-data').data('issuperadmin') == true) {
                btn += '<li><a href="/Artifact/Edit/' + data.Id + '" target="_blank">Edit Chart</a></li>';
            }
            btn += '</ul>' +
                '</div >';
            container.css('position', 'relative');
            container.append(btn);
        }, 500);
    };

    //mutliaxis
    artifactDesigner.Multiaxis._setupCallbacks = {};
    artifactDesigner.Multiaxis._setupCallbacks.bar = function (context, prefix) {
        var prefix = prefix || 'MultiaxisChart';
        var removeSeriesOrStack = function () {
            context.find('.series-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
            context.find('.stack-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.stack-template').remove();
            });
        }
        var addSeries = function () {
            var seriesCount = context.find('.series-holder').find('.series-template').length + 1;
            var chartPost = context.data('chart-pos');
            context.find('#add-series').click(function (e) {
                e.preventDefault();
                var seriesTemplate = context.find('.series-template.original').clone(true);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: prefix + '.Charts[' + chartPost + '].BarChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'ValueAxis', 'Color', 'PreviousColor'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('[id$=BarChart_Series_0__' + field + ']').attr('name', prefix + '.Charts[' + chartPost + '].BarChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass(context.find('.series-type').val().toLowerCase());
                seriesTemplate.addClass(context.find('.value-axis-opt').val());
                seriesTemplate.addClass(context.find('.multiaxis-graphic-type').val());
                context.find('.series-holder').append(seriesTemplate);
                if (prefix == 'MultiaxisChart') {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate, true, seriesTemplate.closest('.chart-template'));
                } else {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                }
                seriesCount++;
            });
        };
        var addStack = function () {
            var stackCount = context.find('.series-holder').find('.stack-template').length + 1;
            var chartPost = context.data('chart-pos');
            context.find('.add-stack').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var stackTemplate = context.find('.stack-template.original').clone(true);
                //console.log(stackTemplate.closest('.chart-template'));

                Pear.Artifact.Designer._colorPicker(stackTemplate);
                stackTemplate.removeClass('original');
                var seriesPos = $this.closest('.series-template').data('series-pos');
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: prefix + '.Charts[' + chartPost + '].BarChart.Series[' + seriesPos + '].Stacks.Index',
                    value: stackCount
                }).appendTo(stackTemplate);
                var fields = ['Label', 'KpiId', 'ValueAxis', 'Color'];
                for (var i in fields) {
                    var field = fields[i];
                    stackTemplate.find('[id$=BarChart_Series_0__Stacks_0__' + field + ']').attr('name', prefix + '.Charts[' + chartPost + '].BarChart.Series[' + seriesPos + '].Stacks[' + stackCount + '].' + field);
                }
                $this.closest('.stacks-holder').append(stackTemplate);
                if (prefix == 'MultiaxisChart') {
                    Pear.Artifact.Designer._kpiAutoComplete(stackTemplate, true, stackTemplate.closest('.chart-template'));
                } else {
                    Pear.Artifact.Designer._kpiAutoComplete(stackTemplate);
                }
                stackCount++;
            });
        };

        removeSeriesOrStack();
        addSeries();
        addStack();
    };
    artifactDesigner.Multiaxis._setupCallbacks.baraccumulative = function (context) {
        Pear.Artifact.Designer.Multiaxis._setupCallbacks.bar(context);
    };
    artifactDesigner.Multiaxis._setupCallbacks.barachievement = function (context) {
        context.find('.value-axis-opt').val('KpiActual');
        context.find('.value-axis-holder').css('display', 'none');
        Pear.Artifact.Designer.Multiaxis._setupCallbacks.bar(context);
    };
    artifactDesigner.Multiaxis._setupCallbacks.line = function (context, prefix) {
        var prefix = prefix || "MultiaxisChart";
        var removeSeriesOrStack = function () {
            context.find('.series-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
        }
        var addSeries = function () {
            var seriesCount = context.find('.series-holder').find('.series-template').length + 1;
            var chartPost = context.data('chart-pos');
            context.find('#add-series').click(function (e) {
                e.preventDefault();
                var seriesTemplate = context.find('.series-template.original').clone(true);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: prefix + '.Charts[' + chartPost + '].LineChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'ValueAxis', 'Color', 'MarkerColor', 'LineType'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('[id$=LineChart_Series_0__' + field + ']').attr('name', prefix + '.Charts[' + chartPost + '].LineChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass(context.find('.value-axis-opt').val());
                seriesTemplate.addClass(context.find('.multiaxis-graphic-type').val());
                context.find('.series-holder').append(seriesTemplate);
                if (prefix == "MultiaxisChart") {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate, true, seriesTemplate.closest('.chart-template'));
                } else {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                }
                seriesCount++;
            });
        };
        removeSeriesOrStack();
        addSeries();
    };
    artifactDesigner.Multiaxis._setupCallbacks.area = function (context, prefix) {
        var prefix = prefix || "MultiaxisChart";
        var removeSeriesOrStack = function () {
            context.find('.series-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
            context.find('.stack-template > .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.stack-template').remove();
            });
        }
        var addSeries = function () {
            var seriesCount = context.find('.series-holder').find('.series-template').length + 1;
            var chartPost = context.data('chart-pos');
            context.find('#add-series').click(function (e) {
                e.preventDefault();
                var seriesTemplate = context.find('.series-template.original').clone(true);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: prefix + '.Charts[' + chartPost + '].AreaChart.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'Color', 'ValueAxis'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('[id$=AreaChart_Series_0__' + field + ']').attr('name', prefix + '.Charts[' + chartPost + '].AreaChart.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass(context.find('.series-type').val().toLowerCase());
                seriesTemplate.addClass(context.find('.value-axis-opt').val());
                seriesTemplate.addClass(context.find('.multiaxis-graphic-type').val());
                context.find('.series-holder').append(seriesTemplate);
                if (prefix == "MultiaxisChart") {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate, true, seriesTemplate.closest('.chart-template'));
                } else {
                    Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                }
                seriesCount++;
            });
        };
        var addStack = function () {
            var stackCount = context.find('.series-holder').find('.stack-template').length + 1;
            var chartPost = context.data('chart-pos');
            context.find('.add-stack').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                var stackTemplate = context.find('.stack-template.original').clone(true);
                //console.log(stackTemplate.closest('.chart-template'));

                Pear.Artifact.Designer._colorPicker(stackTemplate);
                stackTemplate.removeClass('original');
                var seriesPos = $this.closest('.series-template').data('series-pos');
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: prefix + '.Charts[' + chartPost + '].AreaChart.Series[' + seriesPos + '].Stacks.Index',
                    value: stackCount
                }).appendTo(stackTemplate);
                var fields = ['Label', 'KpiId', 'ValueAxis', 'Color'];
                for (var i in fields) {
                    var field = fields[i];
                    stackTemplate.find('[id$=AreaChart_Series_0__Stacks_0__' + field + ']').attr('name', prefix + '.Charts[' + chartPost + '].AreaChart.Series[' + seriesPos + '].Stacks[' + stackCount + '].' + field);
                }
                $this.closest('.stacks-holder').append(stackTemplate);
                if (prefix == 'MultiaxisChart') {
                    Pear.Artifact.Designer._kpiAutoComplete(stackTemplate, true, stackTemplate.closest('.chart-template'));
                } else {
                    Pear.Artifact.Designer._kpiAutoComplete(stackTemplate);
                }
                stackCount++;
            });
        };

        removeSeriesOrStack();
        addSeries();
        addStack();
    };
    artifactDesigner.Multiaxis._loadGraph = function (url, type, context, customCallback) {
        var callback = Pear.Artifact.Designer.Multiaxis._setupCallbacks;
        if (typeof customCallback !== 'undefined') {
            callback = customCallback;
        }
        $.ajax({
            url: url,
            data: 'type=' + type,
            cache: true,
            method: 'GET',
            success: function (data) {
                context.find('.chart-settings').html(data);
                var $hiddenFields = context.find('.hidden-fields');
                context.find('.hidden-fields-holder').html($hiddenFields.html());
                $hiddenFields.remove();
                context.find('.graphic-properties').each(function (i, val) {
                    $(val).html('');
                });
                context.find('.value-axis-holder').css('display', 'block');
                if (callback.hasOwnProperty(type)) {
                    callback[type](context);
                }
            }
        });
    };
    artifactDesigner._setupCallbacks.multiaxis = function () {
        $('.main-value-axis').css('display', 'none');
        $('.form-measurement').css('display', 'none');
        var removeChart = function () {
            $('.chart-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.chart-template').remove();
            });
        };
        var addChart = function () {
            var chartCount = $('#charts-holder').find('.chart-template').length + 1;
            $('#add-chart').click(function (e) {
                e.preventDefault();
                var chartTemplate = $('.chart-template.original').clone(true);
                Pear.Artifact.Designer._colorPicker(chartTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'MultiaxisChart.Charts.Index',
                    value: chartCount
                }).appendTo(chartTemplate);
                chartTemplate.removeClass('original');
                chartTemplate.attr('data-chart-pos', chartCount);
                var fields = ['ValueAxis', 'GraphicType', 'MeasurementId', 'ValueAxisTitle', 'ValueAxisColor', 'IsOpposite', 'FractionScale', 'MaxFractionScale'];
                for (var i in fields) {
                    var field = fields[i];
                    chartTemplate.find('#MultiaxisChart_Charts_0__' + field).attr('name', 'MultiaxisChart.Charts[' + chartCount + '].' + field);
                }
                $('#charts-holder').append(chartTemplate);
                chartTemplate.find('.multiaxis-graphic-type').change(function (e) {
                    e.preventDefault();
                    var $this = $(this);
                    artifactDesigner.Multiaxis._loadGraph($this.data('graph-url'), $this.val(), chartTemplate);
                });
                var initialGraphicType = chartTemplate.find('.multiaxis-graphic-type');
                artifactDesigner.Multiaxis._loadGraph(initialGraphicType.data('graph-url'), initialGraphicType.val(), chartTemplate);
                chartCount++;
            });
        };
        addChart();
        removeChart();
    };
    artifactDesigner._previewCallbacks.multiaxis = function (data, container) {
        var yAxes = [];
        var seriesNames = [];
        var chartTypeMap = {
            bar: 'column',
            line: 'line',
            area: 'area',
            barachievement: 'column',
            baraccumulative: 'column'
        };
        var plotOptions = {
            series: {
                animation: false
            }
        };
        var series = [];
        for (var i in data.MultiaxisChart.Charts) {

            //console.log(data.MultiaxisChart.Charts[i].SeriesType);
            yAxes.push({
                labels: {
                    //  format: '{value} ' + data.MultiaxisChart.Charts[i].Measurement,
                    formatter: function () {
                        if (this.value < 1000 && this.value % 1 != 0) {
                            return this.value.format(2);
                        } else {
                            return this.axis.defaultLabelFormatter.call(this)
                        }
                    },
                    style: {
                        color: '#fff',
                        //color: data.MultiaxisChart.Charts[i].ValueAxisColor
                    }
                },
                title: {
                    text: data.MultiaxisChart.Charts[i].Measurement, //data.MultiaxisChart.Charts[i].ValueAxisTitle + ' (' + data.MultiaxisChart.Charts[i].Measurement + ')',
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                        //color: data.MultiaxisChart.Charts[i].ValueAxisColor
                    }
                },
                opposite: data.MultiaxisChart.Charts[i].IsOpposite,
                tickInterval: data.MultiaxisChart.Charts[i].FractionScale == 0 ? null : data.MultiaxisChart.Charts[i].FractionScale,
                max: data.MultiaxisChart.Charts[i].MaxFractionScale == 0 ? null : data.MultiaxisChart.Charts[i].MaxFractionScale
            });
            if (chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType] == 'line') {
                plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]] = {
                    marker: {
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                };
            } else if (chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType] == 'area' && data.MultiaxisChart.Charts[i].SeriesType == 'multi-stack') {

                plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]] = {
                    stacking: 'normal',
                    lineColor: '#666666',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#666666',
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                };
            } else {
                plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]] = { stacking: 'normal' };
            }
            for (var j in data.MultiaxisChart.Charts[i].Series) {
                if (seriesNames.indexOf(data.MultiaxisChart.Charts[i].Series[j].name) < 0) {
                    seriesNames.push(data.MultiaxisChart.Charts[i].Series[j].name);
                } else {
                    data.MultiaxisChart.Charts[i].Series[j].showInLegend = false;
                }
                data.MultiaxisChart.Charts[i].Series[j].type = chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType];
                if (data.MultiaxisChart.Charts[i].Series[j].type != 'spline' && data.MultiaxisChart.Charts[i].SeriesType == 'single-stack') {
                    data.MultiaxisChart.Charts[i].Series[j].stack = data.MultiaxisChart.Charts[i].Series[j].name;
                }
                data.MultiaxisChart.Charts[i].Series[j].yAxis = parseInt(i);// + 1;
                data.MultiaxisChart.Charts[i].Series[j].tooltip = {
                    valueSuffix: ' ' + data.MultiaxisChart.Charts[i].Measurement
                }
                series.push(data.MultiaxisChart.Charts[i].Series[j]);
            }
        }
        var h = container.highcharts({
            chart: {
                zoomType: 'xy',
                alignTicks: false,
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.MultiaxisChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.MultiaxisChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.MultiaxisChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            plotOptions: plotOptions,
            xAxis: [{
                categories: data.MultiaxisChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
            }],
            yAxis: yAxes,
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    //console.log(this.points);
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        //total in process
                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //console.log(
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //    console.log(totalInProcess);
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '</strong><br>';
                                //console.log('ini pas total : ' + totalInProcess);
                            }

                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if (typeof this.points[i].total !== 'undefined') {
                        //    if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //        (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //        tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '<br>';
                        //    }
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            series: series
        })

    };


    //combination chart
    artifactDesigner.Combo = {};
    artifactDesigner.Combo._setupCallbacks = {};
    artifactDesigner.Combo._setupCallbacks.bar = function (context) {
        Pear.Artifact.Designer.Multiaxis._setupCallbacks.bar(context, "ComboChart");
    };
    artifactDesigner.Combo._setupCallbacks.baraccumulative = function (context) {
        Pear.Artifact.Designer.Combo._setupCallbacks.bar(context);
    };
    artifactDesigner.Combo._setupCallbacks.barachievement = function (context) {
        context.find('.value-axis-opt').val('KpiActual');
        context.find('.value-axis-holder').css('display', 'none');
        Pear.Artifact.Designer.Combo._setupCallbacks.bar(context);
    };
    artifactDesigner.Combo._setupCallbacks.line = function (context) {
        Pear.Artifact.Designer.Multiaxis._setupCallbacks.line(context, "ComboChart");
    };
    artifactDesigner.Combo._setupCallbacks.area = function (context) {
        Pear.Artifact.Designer.Multiaxis._setupCallbacks.area(context, "ComboChart");
    };
    artifactDesigner.Combo._loadGraph = function (url, type, context) {
        var callback = Pear.Artifact.Designer.Combo._setupCallbacks;
        Pear.Artifact.Designer.Multiaxis._loadGraph(url, type, context, callback);
    };
    artifactDesigner._setupCallbacks.combo = function () {
        $('.main-value-axis').css('display', 'none');
        var removeChart = function () {
            $('.chart-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.chart-template').remove();
            });
        };
        var addChart = function () {
            var chartCount = $('#charts-holder').find('.chart-template').length + 1;
            $('#add-chart').click(function (e) {
                e.preventDefault();
                var chartTemplate = $('.chart-template.original').clone(true);
                Pear.Artifact.Designer._colorPicker(chartTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'ComboChart.Charts.Index',
                    value: chartCount
                }).appendTo(chartTemplate);
                chartTemplate.removeClass('original');
                chartTemplate.attr('data-chart-pos', chartCount);
                var fields = ['ValueAxis', 'GraphicType'];
                for (var i in fields) {
                    var field = fields[i];
                    chartTemplate.find('#ComboChart_Charts_0__' + field).attr('name', 'ComboChart.Charts[' + chartCount + '].' + field);
                }
                $('#charts-holder').append(chartTemplate);
                chartTemplate.find('.multiaxis-graphic-type').change(function (e) {
                    e.preventDefault();
                    var $this = $(this);
                    artifactDesigner.Combo._loadGraph($this.data('graph-url'), $this.val(), chartTemplate);
                });
                var initialGraphicType = chartTemplate.find('.multiaxis-graphic-type');
                artifactDesigner.Combo._loadGraph(initialGraphicType.data('graph-url'), initialGraphicType.val(), chartTemplate);
                chartCount++;
            });
        };
        addChart();
        removeChart();
    };
    artifactDesigner._previewCallbacks.combo = function (data, container) {
        var yAxes = [];
        var seriesNames = [];
        var chartTypeMap = {
            bar: 'column',
            line: 'line',
            area: 'area',
            barachievement: 'column',
            baraccumulative: 'column'
        };
        var plotOptions = {};
        var series = [];
        for (var i in data.ComboChart.Charts) {
            if (chartTypeMap[data.ComboChart.Charts[i].GraphicType] == 'line') {
                plotOptions[chartTypeMap[data.ComboChart.Charts[i].GraphicType]] = {
                    marker: {
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                };
            } else if (chartTypeMap[data.ComboChart.Charts[i].GraphicType] == 'area' && data.ComboChart.Charts[i].SeriesType == 'multi-stack') {

                plotOptions[chartTypeMap[data.ComboChart.Charts[i].GraphicType]] = {
                    stacking: 'normal',
                    lineColor: '#666666',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#666666',
                        enabled: false,
                        states: {
                            hover: {
                                radius: 4
                            },
                            select: {
                                radius: 4
                            }
                        }
                    }
                };
            } else {
                plotOptions[chartTypeMap[data.ComboChart.Charts[i].GraphicType]] = { stacking: 'normal' };
            }
            for (var j in data.ComboChart.Charts[i].Series) {
                if (seriesNames.indexOf(data.ComboChart.Charts[i].Series[j].name) < 0) {
                    seriesNames.push(data.ComboChart.Charts[i].Series[j].name);
                } else {
                    data.ComboChart.Charts[i].Series[j].showInLegend = false;
                }
                data.ComboChart.Charts[i].Series[j].type = chartTypeMap[data.ComboChart.Charts[i].GraphicType];
                if (data.ComboChart.Charts[i].Series[j].type != 'spline' && data.ComboChart.Charts[i].SeriesType == 'single-stack') {
                    data.ComboChart.Charts[i].Series[j].stack = data.ComboChart.Charts[i].Series[j].name;
                }
                data.ComboChart.Charts[i].Series[j].tooltip = {
                    valueSuffix: ' ' + data.ComboChart.Measurement
                }
                series.push(data.ComboChart.Charts[i].Series[j]);
            }
        }
        container.highcharts({
            chart: {
                zoomType: 'xy',
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
            },
            title: {
                text: data.ComboChart.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.ComboChart.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            plotOptions: plotOptions,
            xAxis: [{
                categories: data.ComboChart.Periodes,
                crosshair: true,
                gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
            }],
            yAxis: {
                title: {
                    text: data.ComboChart.Measurement,
                    style: {
                        color: '#fff',
                        fontSize: '12px'
                    }
                },
                tickInterval: data.FractionScale == 0 ? null : data.FractionScale,
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                //gridLineColor: '#fff',
                labels: {
                    style: {
                        "color": '#fff'
                    }
                },
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.ComboChart.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    var tooltip = '<b>' + artifactDesigner._toJavascriptDate(data.TimePeriodes[this.points[0].point.index], data.PeriodeType) + '</b><br/>';
                    var totalInProcess = 0;
                    for (var i in this.points) {
                        tooltip += this.points[i].series.name + ': ' + this.points[i].y.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '<br/>';

                        var prev = (parseInt(i) - 1);
                        var next = (parseInt(i) + 1);
                        var nextExist = typeof this.points[next] !== 'undefined';
                        var prevExist = typeof this.points[prev] !== 'undefined';

                        //total in process
                        if (typeof this.points[i].series.stackKey !== 'undefined') {
                            //if ((!prevExist && nextExist && this.points[next].series.stackKey == this.points[i].series.stackKey) || (prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                            totalInProcess += this.points[i].y;
                            //}

                            if ((nextExist && prevExist && this.points[next].series.stackKey != this.points[i].series.stackKey && this.points[prev].series.stackKey == this.points[i].series.stackKey) ||
                                (!nextExist && prevExist && this.points[prev].series.stackKey == this.points[i].series.stackKey)) {
                                tooltip += '<strong>Total: ' + totalInProcess.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '</strong><br>';
                                //totalInProcess = 0;
                            }
                            if (nextExist && this.points[next].series.stackKey != this.points[i].series.stackKey) {
                                totalInProcess = 0;
                            }
                        }

                        //if ((!nextExist && prevExist && this.points[prev].total == this.points[i].total) ||
                        //    (nextExist && prevExist && this.points[next].total != this.points[i].total && this.points[prev].total == this.points[i].total)) {
                        //    tooltip += 'Total: ' + this.points[i].total.format(2) + ' ' + this.points[i].series.options.tooltip.valueSuffix + '<br>';
                        //}
                        if (data.Highlights != null && data.Highlights != undefined) {
                            if (!nextExist && data.Highlights[this.points[i].point.index] != null) {
                                tooltip += '<b>Highlight : ' + data.Highlights[this.points[i].point.index].Title + '</b><br>';
                                tooltip += '<p>' + data.Highlights[this.points[i].point.index].Message + '</p>';
                            }
                        }
                    }
                    return tooltip;
                },
                shared: true
            },
            series: series
        });
    };

    //pie chart
    artifactDesigner._setupCallbacks.pie = function () {
        var removeSeriesOrStack = function () {
            $('.series-template .remove').click(function (e) {
                e.preventDefault();
                var $this = $(this);
                $this.closest('.series-template').remove();
            });
        };
        var addSeries = function () {
            //console.log('add-series');
            var seriesCount = $('#series-holder').find('.series-template').length + 1;
            $('#add-series').click(function (e) {
                //console.log('series-click');
                e.preventDefault();
                var seriesTemplate = $('.series-template.original').clone(true);

                Pear.Artifact.Designer._kpiAutoComplete(seriesTemplate);
                Pear.Artifact.Designer._colorPicker(seriesTemplate);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'Pie.Series.Index',
                    value: seriesCount
                }).appendTo(seriesTemplate);
                seriesTemplate.removeClass('original');
                seriesTemplate.attr('data-series-pos', seriesCount);
                if (seriesCount !== 0) {
                    var fields = ['Label', 'KpiId', 'ValueAxis', 'Color'];
                    for (var i in fields) {
                        var field = fields[i];
                        seriesTemplate.find('#Pie_Series_0__' + field).attr('name', 'Pie.Series[' + seriesCount + '].' + field);
                    }
                }
                seriesTemplate.addClass($('#bar-value-axis').val());
                seriesTemplate.addClass($('#graphic-type').val());
                $('#series-holder').append(seriesTemplate);
                seriesCount++;
            });
        };
        removeSeriesOrStack();
        addSeries();
    };

    artifactDesigner._previewCallbacks.pie = function (data, container) {
        container.highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: data.Pie.Is3D,
                    alpha: 60,
                    beta: 0
                },
                backgroundColor: 'transparent',
                events: {
                    beforePrint: function () {
                        this.chartBackground.attr({
                            fill: '#006ba1'
                        });
                    },
                    afterPrint: function () {
                        this.chartBackground.attr({
                            fill: 'transparent'
                        });
                    }
                }
                //margin: [0, 0, 0, 0],
                //spacingTop: 0,
                //spacingBottom: 0,
                //spacingLeft: 0,
                //spacingRight: 0
            },
            title: {
                text: data.Pie.Title,
                style: {
                    color: '#fff'
                }
            },
            subtitle: {
                text: data.Pie.Subtitle,
                style: {
                    color: '#fff'
                }
            },
            exporting: {
                buttons: {
                    contextButton: {
                        symbol: 'url(/Content/img/printer_3.png)',
                        symbolStrokeWidth: 1,
                        symbolFill: '#a4edba',
                        symbolStroke: '#330033'
                    }
                },
                //url: '/Chart/Export',
                filename: data.Pie.Title,
                width: 1200,
                chartOptions: {
                    chart: {
                        events: {
                            load: function () {
                                this.setTitle(null, { style: { fontSize: '12px' } });
                                this.chartBackground.attr({
                                    fill: '#006ba1'
                                });
                            }

                        }
                    }
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    slicedOffset: 30,
                    allowPointSelect: true,
                    cursor: 'pointer',
                    /*dataLabels: {
                        enabled: true,
                        formatter: function () {
                            return '<b>' + this.point.name + '</b>: ' + this.percentage.toFixed(2) + ' %';
                        }
                    }*/
                    dataLabels: {
                        enabled: true,
                        distance: 2,
                        color: '#fff',
                        formatter: function () {
                            return '<b>' + this.point.name + '</b>: <br/> ' + this.percentage.toFixed(2) + ' %';
                        },
                        shadow: false,
                        style: {
                            textShadow: false
                        }
                    },
                    showInLegend: data.Pie.ShowLegend,
                    //innerSize: '40%',
                    size: '75%',
                    shadow: false,
                    depth: 45,
                }
            },
            legend: {
                itemStyle: {
                    "color": '#fff'
                },
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            navigation: {
                buttonOptions: {
                    symbolStroke: '#fff',
                    symbolFill: '#fff',
                    theme: {
                        'stroke-width': 0,
                        stroke: 'silver',
                        fill: 'transparent',
                        r: 0,
                        states: {
                            hover: {
                                fill: 'transparent'
                            },
                            select: {
                                fill: 'transparent'
                            }
                        }
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.point.name + '</b>: ' + this.y.format(2) + ' ' + this.point.measurement + '<br/>' +
                        '<b>Total</b>: ' + this.total.format(2) + ' ' + this.point.measurement;
                }
            },
            series: [{
                type: 'pie',
                name: 'Current selection',
                data: data.Pie.SeriesResponses
            }]
        });
    };

    var templateEditor = Pear.Template.Editor;

    templateEditor._artifactSelectField = function (context) {
        context.find('.artifact-list').select2({
            ajax: {
                url: $('#hidden-fields-holder').data('artifact-url'),
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data, page) {
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        }).on('select2:select', function (e) {
            var link = $(this).parent().find('a')[0];
            var id = e.params.data.id;
            var cuttedLink = $(link).attr('href').substr(0, $(link).attr('href').lastIndexOf('/'));
            $(link).attr('href', cuttedLink + '/' + id);
            $(link).css('visibility', 'visible');
        });
    };


    templateEditor.LayoutSetup = function () {
        $('.column-width').change(function () {
            var colWidth = $(this).val();
            var $column = $(this).closest('.layout-column');
            var $row = $(this).closest('.layout-row');
            var $currentCols = $row.children('.layout-column');
            $column.css('width', colWidth + '%');
            var colIndex = $row.find('.layout-column').index($column);
            var remainWidth = 100;
            var remainLength = $currentCols.length - colIndex - 1;
            $currentCols.each(function (i, val) {
                if (i <= colIndex) {
                    remainWidth -= parseFloat($(val)[0].style.width.replace('%', ''));
                } else {
                    $(val).css('width', (remainWidth / remainLength) + '%');
                }
            });
        });
        var addColumn = function () {
            var columnCount = 2;
            $('.add-column').click(function () {
                var $this = $(this);
                var $row = $(this).parent().find('.layout-row');
                var currentCols = $row.children('.layout-column').length;
                var newWidth = 100 / (currentCols + 1);
                $row.children('.layout-column').each(function (i, val) {
                    $(val).css('width', newWidth + '%');
                });
                var newColumn = $('.layout-column.original').clone(true);
                newColumn.removeClass('original');
                newColumn.css('width', newWidth + '%');
                Pear.Template.Editor._artifactSelectField(newColumn);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns.Index',
                    value: columnCount
                }).prependTo(newColumn);
                newColumn.find('.column-width').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].Width');
                newColumn.find('.column-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].ColumnType');
                newColumn.find('.artifact-list').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].ArtifactId');
                newColumn.find('.periode-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].HighlightPeriodeType');
                newColumn.find('.highlight-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].HighlightTypeId');
                $row.append(newColumn);
                columnCount++;
            });
        };

        var addRow = function () {

            var rowCount = 1;
            $('.add-row').click(function () {

                var row = $('.layout-row-wrapper.original').clone(true);
                row.removeClass('original');
                row.find('.layout-column.original').removeClass('original');
                row.find('.layout-row').attr('data-row-pos', rowCount);
                Pear.Template.Editor._artifactSelectField(row);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows.Index',
                    value: rowCount
                }).prependTo(row.find('.layout-row'));

                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows[' + rowCount + '].LayoutColumns.Index',
                    value: 1
                }).prependTo(row.find('.layout-column'));

                row.find('.column-width').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].Width');
                row.find('.column-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].ColumnType');
                row.find('.artifact-list').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].ArtifactId');
                row.find('.periode-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].HighlightPeriodeType');
                row.find('.highlight-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].HighlightTypeId');
                $('#rows-holder').append(row);
                rowCount++;
            });

        };
        $('.remove-row').click(function () {
            $(this).closest('.layout-row-wrapper').remove();
        });
        $('.remove-column').click(function () {
            var $this = $(this);
            var $column = $(this).closest('.layout-column');
            var $row = $(this).closest('.layout-row');
            var currentCols = $row.children('.layout-column').length;
            var newWidth = 100 / (currentCols - 1);
            $column.remove();
            $row.children('.layout-column').each(function (i, val) {
                $(val).css('width', newWidth + '%');
            });
        });

        $('#graphic-preview-btn').click(function (e) {
            e.preventDefault();
            var $this = $(this);
            $.ajax({
                url: $this.data('preview-url'),
                data: $this.closest('form').serialize(),
                method: 'POST',
                success: function (data) {
                    $('#container').html(data);
                    templateEditor.ViewSetup();
                    $('#graphic-preview').modal('show');
                }
            });
        });
        $('.column-type').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            var $parent = $this.closest('.column-properties');
            if ($this.val() == "Artifact") {
                $parent.find('.column-as-artifact').show();
                $parent.find('.column-as-highlight').hide();
            } else {
                $parent.find('.column-as-artifact').hide();
                $parent.find('.column-as-highlight').show();
            }
        });
        $('#graphic-preview').on('show.bs.modal', function () {
            $('#container').css('visibility', 'hidden');
        });
        $('#graphic-preview').on('shown.bs.modal', function () {
            $('#container').css('visibility', 'visible');
        });

        addRow();
        addColumn();
    };
    templateEditor.ViewSetup = function () {
        var wHeight = parseInt($(window).height());
        var height = wHeight * 50 / 100;
        $('.artifact-holder').each(function (i, val) {
            var $holder = $(val);
            if (wHeight > 667) {
                $holder.css('height', height + 'px');
            }
            Pear.Loading.Show($(val));
            var url = $holder.data('artifact-url');
            var callback = Pear.Artifact.Designer._previewCallbacks;
            $.ajax({
                url: url,
                method: 'GET',
                success: function (data) {
                    if (callback.hasOwnProperty(data.GraphicType)) {
                        Pear.Loading.Stop($(val));
                        callback[data.GraphicType](data, $holder);
                    }
                }
            });
        });
        $('.highlight-holder').each(function (i, val) {
            var $holder = $(val);
            if (wHeight > 667) {
                $holder.css('height', height + 'px');
            }
            Pear.Loading.Show($(val));
            var url = $holder.data('artifact-url');
            $.ajax({
                url: url,
                method: 'GET',
                success: function (data) {
                    var isSuperAdmin = $('#user-profile-session-data').data('issuperadmin') == true;
                    $holder.attr('data-highlight-id', data.Id);
                    var editUrl = $('.highlight-holder').data('artifact-edit-url');
                    if (editUrl != undefined) {
                        editUrl = editUrl.replace('_id_', data.Id);
                    }

                    var title = data.Title == null || data.Title == "" ? data.Type : data.Title;
                    var $title = $('<h4/>').html(title);
                    var $periode = $('<div />').append(Pear.Artifact.Designer._toJavascriptDate(data.Date, data.SPeriodeType));
                    var $message = $('<div />').append(data.Message);
                    $message.addClass('dashboard-highlight-message');
                    $title.addClass('dashboard-highlight-title');
                    $periode.addClass('dashboard-highlight-periode');
                    $periode.attr('data-periode-type', data.SPeriodeType);
                    $holder.css('background', 'none');
                    $holder.append($title);
                    $holder.append($periode);
                    $holder.append($message);

                    var btn = '<div class="btn-group chart-button">' +
                        '<a class="tabular-wrapper-button dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><img src="/Content/img/printer_3.png" width="32" height="32" ></img></a>' +
                        '<ul class="dropdown-menu pull-right">' +
                        '<li><a class="highlight-change-periode" href="javascript:;">Change Periode</a></li>';
                    if (isSuperAdmin) {
                        btn +=
                            '<li><a href="' + editUrl + '" target="_blank">Edit Highlight</a></li>';
                    }
                    btn += '</ul>' +
                        '</div >';
                    $holder.css('position', 'relative');
                    $holder.append(btn);

                }
            });
        });
    };
    templateEditor.EditSetup = function () {
        var addRow = function () {
            var rowCount = $('.template-edit').attr('data-row-count');
            $('.add-row').click(function () {
                var row = $('.layout-row-wrapper.original').clone(true);
                row.removeClass('original');
                row.find('.layout-column.original').removeClass('original');
                row.find('.layout-row').attr('data-row-pos', rowCount);
                Pear.Template.Editor._artifactSelectField(row);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows.Index',
                    value: rowCount
                }).prependTo(row.find('.layout-row'));

                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows[' + rowCount + '].LayoutColumns.Index',
                    value: 1
                }).prependTo(row.find('.layout-column'));

                row.find('.column-width').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].Width').val(100);
                row.find('.artifact-list').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].ArtifactId');
                row.find('.column-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].ColumnType');
                row.find('.periode-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].HighlightPeriodeType');
                row.find('.highlight-type').attr('name', 'LayoutRows[' + rowCount + '].LayoutColumns[1].HighlightTypeId');
                $('#rows-holder').append(row);
                rowCount++;
            });
        };

        var addColumn = function () {
            var columMinWidth = 10;
            var columnCount = 2;
            $('.add-column').click(function () {
                var $this = $(this);
                var $row = $(this).parent().find('.layout-row');

                //alert error if total colum width execeed (> 100%)
                var currentWidth = 0;
                $row.find('.column-width').each(function () {
                    currentWidth += $(this).val() ? parseFloat($(this).val()) : 0;
                });
                if (currentWidth >= 100) {
                    alert('Total column width exceeded. Can not create more column.');
                    return;
                }

                //var currentCols = $row.children('.layout-column').length;
                //var newWidth = 100 / (currentCols + 1);
                //$row.children('.layout-column').each(function(i, val) {
                //    $(val).css('width', newWidth + '%');
                //});
                var newColumn = $('.layout-column.original').clone(true);
                var newWidth = 100 - currentWidth;
                newColumn.removeClass('original');
                newColumn.css('width', newWidth + '%');
                Pear.Template.Editor._artifactSelectField(newColumn);
                $('<input>').attr({
                    type: 'hidden',
                    id: 'foo',
                    name: 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns.Index',
                    value: columnCount
                }).prependTo(newColumn);
                newColumn.find('.column-width').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].Width').val(newWidth);
                newColumn.find('.artifact-list').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].ArtifactId');
                newColumn.find('.column-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].ColumnType');
                newColumn.find('.periode-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].HighlightPeriodeType');
                newColumn.find('.highlight-type').attr('name', 'LayoutRows[' + $row.data('row-pos') + '].LayoutColumns[' + columnCount + '].HighlightTypeId');
                $row.append(newColumn);
                columnCount++;
            });
        };

        $('.remove-row').click(function () {
            $(this).closest('.layout-row-wrapper').remove();
        });
        $('.remove-column').click(function () {
            var $this = $(this);
            var $column = $(this).closest('.layout-column');
            var $row = $(this).closest('.layout-row');
            //var currentCols = $row.children('.layout-column').length;
            //var newWidth = 100 / (currentCols - 1);
            $column.remove();
            //$row.children('.layout-column').each(function (i, val) {
            //    $(val).css('width', newWidth + '%');
            //});
        });

        $('#graphic-preview-btn').click(function (e) {
            e.preventDefault();
            var $this = $(this);
            $.ajax({
                url: $this.data('preview-url'),
                data: $this.closest('form').serialize(),
                method: 'POST',
                success: function (data) {
                    $('#container').html(data);
                    templateEditor.ViewSetup();
                    $('#graphic-preview').modal('show');
                }
            });
        });
        $('#graphic-preview').on('show.bs.modal', function () {
            $('#container').css('visibility', 'hidden');
        });
        $('#graphic-preview').on('shown.bs.modal', function () {
            $('#container').css('visibility', 'visible');
        });
        var currentColWidth = 0;
        $('.column-width').click(function () {
            currentColWidth = $(this).val();
        });
        $('.column-width').change(function () {
            //alert($(this).val());
            var totalWidth = 0;
            $(this).parents('.layout-row-wrapper').find('.column-width').each(function () {
                totalWidth += $(this).val() ? parseFloat($(this).val()) : 0;
            });
            if (totalWidth > 100) {
                alert('Total column width max is 100');
                $(this).val(currentColWidth);

                return false;
            }
            $(this).parents('.layout-column').css('width', $(this).val() + '%');
        });
        $('.column-type').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            var $parent = $this.closest('.column-properties');
            if ($this.val() == "Artifact") {
                $parent.find('.column-as-artifact').show();
                $parent.find('.column-as-highlight').hide();
            } else {
                $parent.find('.column-as-artifact').hide();
                $parent.find('.column-as-highlight').show();
            }
        });
        addRow();
        addColumn();
        templateEditor._artifactSelectField($('.template-edit'));
    };

    Pear.Highlight.EditSetup = function () {
        switch ($('#PeriodeType').val().toLowerCase().trim()) {
            case 'hourly':
                $('.datepicker').datetimepicker({
                    format: "MM/DD/YYYY hh:00 A"
                });
                break;
            case 'daily':
                $('.datepicker').datetimepicker({
                    format: "MM/DD/YYYY"
                });
                break;
            case 'weekly':
                $('.datepicker').datetimepicker({
                    format: "MM/DD/YYYY",
                    daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                });
                break;
            case 'monthly':
                $('.datepicker').datetimepicker({
                    format: "MM/YYYY"
                });
                break;
            case 'yearly':
                $('.datepicker').datetimepicker({
                    format: "YYYY"
                });
                break;
            default:
        }
        $('#PeriodeType').change(function (e) {
            e.preventDefault();
            var $this = $(this);
            var clearValue = $('.datepicker').each(function (i, val) {
                $(val).val('');
                $(val).data("DateTimePicker").destroy();
            });
            switch ($this.val().toLowerCase().trim()) {
                case 'hourly':
                    $('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY hh:00 A"
                    });
                    break;
                case 'daily':
                    $('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY"
                    });
                    break;
                case 'weekly':
                    $('.datepicker').datetimepicker({
                        format: "MM/DD/YYYY",
                        daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                    });
                    break;
                case 'monthly':
                    $('.datepicker').datetimepicker({
                        format: "MM/YYYY"
                    });
                    break;
                case 'yearly':
                    $('.datepicker').datetimepicker({
                        format: "YYYY"
                    });
                    break;
                default:
            }
        });
        var $messageHolder = $('.message-holder');
        var $messageHolderClone = $messageHolder.clone(true);
        //console.log($messageHolderClone);
        $messageHolder.html('');
        var onceChanged = false;
        $('#TypeId').change(function (e) {
            //console.log($(this).val());
            e.preventDefault();
            var $this = $(this);
            $('#Title').val($this.find('option:selected').text());
            var url = $this.data('url');
            $.get(url, 'value=' + $this.val(), function (data) {
                if (data.length) {
                    var select = $messageHolderClone.find('.alert-condition-options select');
                    select.find('option')
                        .remove()
                        .end();
                    $.each(data, function (key, opt) {
                        select
                            .append($("<option></option>")
                                .attr("value", opt.Value.trim())
                                .text(opt.Text.trim()));
                    });

                    $messageHolder.html($messageHolderClone.find('.alert-condition-options').html());
                    if (onceChanged == false) {
                        $messageHolder.find('select').val($messageHolderClone.find('.message-text-area textarea').val().trim());
                        onceChanged = true;
                    }

                } else {
                    if (!$messageHolder.find('.message-text-area').length) {
                        $messageHolder.html($messageHolderClone.find('.message-text-area').html());

                        tinyMCE.init({
                            selector: ".highlight-message",
                            plugins: [
                                "advlist autolink lists link image charmap print preview anchor",
                                "searchreplace visualblocks code fullscreen",
                                "insertdatetime media table contextmenu paste"
                            ],
                            style_formats: [
                                { title: 'Line height 20px', selector: 'p,div,h1,h2,h3,h4,h5,h6', styles: { lineHeight: '20px' } },
                                { title: 'Line height 30px', selector: 'p,div,h1,h2,h3,h4,h5,h6', styles: { lineHeight: '30px' } }
                            ],
                            force_br_newlines: true,
                            force_p_newlines: false,
                            //forced_root_block : '',
                            toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
                        });
                    }
                }
            });

        });
        $('#TypeId').change();
    };

    Pear.VesselSchedule._autocomplete = function (fieldId) {
        var url = $(fieldId).data('url');
        $(fieldId).select2({
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term, // search term
                    };
                },
                processResults: function (data, page) {
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };

    Pear.VesselSchedule.FormSetup = function () {
        Pear.VesselSchedule._autocomplete('#VesselId');
        Pear.VesselSchedule._autocomplete('#BuyerId');
        $('.datepicker').datetimepicker({
            format: "MM/DD/YYYY"
        });
    }

    Pear.NLS.FormSetup = function () {
        Pear.VesselSchedule._autocomplete('#VesselScheduleId');
        $('.datepicker').datetimepicker({
            format: "MM/DD/YYYY"
        });
    }

    Pear.ConstantUsage._autocomplete = function ($field) {
        var url = $field.data('url');
        $field.select2({
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term, // search term
                    };
                },
                processResults: function (data, page) {
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };

    Pear.ConstantUsage.FormSetup = function () {
        var length = $('.constants-holder').find('.constant-template').length + 1;
        if (length > 1) {
            $('.constants-holder .constant-template .constant').each(function (i, val) {
                Pear.ConstantUsage._autocomplete($(val));
            });
        }
        $('.add-constant').click(function (e) {
            e.preventDefault();
            var constantTemplate = $('.constant-template.original').clone(true);
            constantTemplate.removeClass('original');
            $('<input>').attr({
                type: 'hidden',
                id: 'foo',
                name: 'Constants.Index',
                value: length
            }).prependTo(constantTemplate);
            Pear.ConstantUsage._autocomplete(constantTemplate.find('.constant').attr('name', 'Constants[' + length + '].Id'));
            var holder = $('.constants-holder');
            holder.append(constantTemplate);
            length++;
        });
        $('.constant-template .remove').click(function (e) {
            e.preventDefault();
            $(this).closest('.constant-template').remove();
        });
    };

    Pear.OutputConfig.FormSetup = function () {
        //Pear.OutputConfig._autocomplete($('.key-assumption-options'));
        //Pear.OutputConfig._autocomplete($('.kpi-options'));
        var kpiParam = $('.kpi-param');
        var assumptionParam = $('.assumption-param');
        var excludeValue = $('.exclude-value');
        var paramsHolder = $('.params-holder');

        paramsHolder.find('.kpi-options, .key-assumption-options').each(function (i, val) {
            var select = $(val);
            Pear.OutputConfig._autocomplete(select);
        });

        var addKpi = function (e) {
            e.preventDefault();
            var $this = $(this);
            var isAdd = $this.find('i').hasClass('fa-plus');
            if (isAdd) {
                var newKpi = kpiParam.clone(true);
                newKpi.addClass('dynamic-kpi');
                Pear.OutputConfig._autocomplete(newKpi.find('select'));
                var remove = $('<a />');
                remove.append($('<i />').addClass('fa fa-minus'));
                remove.addClass('btn btn-default');
                remove.click(function (e) {
                    var $thisRemove = $(this);
                    removeKpi($thisRemove);
                });
                newKpi.append(remove);
                newKpi.append($this.clone(true));
                paramsHolder.append(newKpi.show());
                if ($this.prev().hasClass('btn')) {
                    $this.remove();
                } else {
                    $this.find('i').removeClass('fa-plus');
                    $this.find('i').addClass('fa-minus');
                }
            } else {
                removeKpi($this);
            }
        };

        var buttonAdd = $('<a />');
        buttonAdd.append($('<i />').addClass('fa fa-plus'));
        buttonAdd.addClass('btn btn-default');
        buttonAdd.click(addKpi);

        var removeKpi = function ($this) {
            $this.closest('div').remove();
            if (paramsHolder.find('.dynamic-kpi').length == 1) {
                var firstDyn = $(paramsHolder.find('.dynamic-kpi')[0]);
                firstDyn.children('a').each(function (i, val) {
                    $(val).remove();
                });
                firstDyn.append(buttonAdd.clone(true));
            }
            if (!paramsHolder.find('.dynamic-kpi .fa-plus').length) {
                paramsHolder.children('.dynamic-kpi:last-child').append(buttonAdd.clone(true));
            }
        }

        $('.remove-kpi').click(function (e) {
            e.preventDefault();
            removeKpi($(this));
        });

        $('.add-kpi').click(addKpi);

        $('.output-formula').change(function (e) {
            var $this = $(this);
            var val = $this.val();
            paramsHolder.html('');
            switch (val) {
                case "AVERAGE":
                case "MIN":
                case "MINDATE":
                case "BREAKEVENTYEAR":
                case "PAYBACK":
                case "PROJECTIRR":
                case "EQUITYIRR":
                case "PROFITINVESTMENTRATIO":
                case "SUM":
                    var start = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(start.find('select'));
                    start.find('label').html('Start');
                    paramsHolder.append(start.show());
                    var end = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(end.find('select'));
                    end.find('label').html('End');
                    paramsHolder.append(end.show());
                    if (val == 'MIN' || val == 'MINDATE') {
                        var exclude = excludeValue.clone(true);
                        paramsHolder.append(exclude.show());
                    }
                    if (val == 'PAYBACK') {
                        //var operationStart = assumptionParam.clone(true);
                        //Pear.OutputConfig._autocomplete(operationStart.find('select'));
                        //operationStart.find('label').html('Operation Start');
                        //paramsHolder.append(operationStart.show());
                        var commercialDate = assumptionParam.clone(true);
                        Pear.OutputConfig._autocomplete(commercialDate.find('select'));
                        commercialDate.find('label').html('Commercial Date');
                        paramsHolder.append(commercialDate.show());
                    }
                    var kpi = kpiParam.clone(true);
                    Pear.OutputConfig._autocomplete(kpi.find('select'));
                    paramsHolder.append(kpi.show());
                    if (val == "PROFITINVESTMENTRATIO") {
                        kpi.find('label').html('Ultimate FCF - Project');
                        var projectCost = kpiParam.clone(true);
                        Pear.OutputConfig._autocomplete(projectCost.find('select'));
                        projectCost.find('label').html('Project Cost');
                        paramsHolder.append(projectCost.show());
                    }
                    break;
                case "COMPLETIONDATE":
                    var start = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(start.find('select'));
                    start.find('label').html('Completion Date');
                    paramsHolder.append(start.show());
                    var kpi = kpiParam.clone(true);
                    Pear.OutputConfig._autocomplete(kpi.find('select'));
                    paramsHolder.append(kpi.show());
                    break;
                case "GROSSPROFIT":
                    var start = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(start.find('select'));
                    start.find('label').html('Start');
                    paramsHolder.append(start.show());
                    var end = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(end.find('select'));
                    end.find('label').html('End');
                    paramsHolder.append(end.show());
                    for (var i = 0; i < 4; i++) {
                        var kpi = kpiParam.clone(true);
                        Pear.OutputConfig._autocomplete(kpi.find('select'));
                        paramsHolder.append(kpi.show());
                    }
                    break;
                case "NETBACKVALUE":
                    var start = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(start.find('select'));
                    start.find('label').html('Start');
                    paramsHolder.append(start.show());
                    var end = assumptionParam.clone(true);
                    Pear.OutputConfig._autocomplete(end.find('select'));
                    end.find('label').html('End');
                    paramsHolder.append(end.show());
                    for (var i = 0; i < 2; i++) {
                        var kpi = kpiParam.clone(true);
                        if (i == 1) {
                            kpi.addClass('dynamic-kpi');

                            kpi.append(buttonAdd.clone(true));
                        }
                        Pear.OutputConfig._autocomplete(kpi.find('select'));
                        paramsHolder.append(kpi.show());
                    }
                    break;
            }
        });
    }

    Pear.OutputConfig._autocomplete = function ($field) {
        var url = $field.data('url');
        $field.select2({
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term, // search term
                    };
                },
                processResults: function (data, page) {
                    //console.log(data);
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };

    Pear.EconomicSummary._autocomplete = function ($field) {
        var url = $field.data('url');
        $field.select2({
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term, // search term
                    };
                },
                processResults: function (data, page) {
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };

    Pear.EconomicSummary.FormSetup = function () {
        var length = $('.scenarios-holder').find('.scenario-template').length + 1;
        if (length > 1) {
            $('.scenarios-holder .scenario-template .scenario').each(function (i, val) {
                Pear.EconomicSummary._autocomplete($(val));
            });
        }
        $('.add-scenario').click(function (e) {
            e.preventDefault();
            var scenarioTemplate = $('.scenario-template.original').clone(true);
            scenarioTemplate.removeClass('original');
            $('<input>').attr({
                type: 'hidden',
                id: 'foo',
                name: 'Scenarios.Index',
                value: length
            }).prependTo(scenarioTemplate);
            Pear.EconomicSummary._autocomplete(scenarioTemplate.find('.scenario').attr('name', 'Scenarios[' + length + '].Id'));
            var holder = $('.scenarios-holder');
            holder.append(scenarioTemplate);
            length++;
        });
        $('.scenario-template .remove').click(function (e) {
            e.preventDefault();
            $(this).closest('.scenario-template').remove();
        });
    };

    Pear.OperationConfig.FormSetup = function () {
        Pear.VesselSchedule._autocomplete('#KpiId');
        /*var kpiParam = $('.kpi-param');
        var assumptionParam = $('.assumption-param');
        var excludeValue = $('.exclude-value');
        var paramsHolder = $('.params-holder');
    
        paramsHolder.find('.kpi-options, .key-assumption-options').each(function(i, val) {
            var select = $(val);
            Pear.OperationConfig._autocomplete(select);
        });
    
        $('.output-formula').change(function(e) {
            var $this = $(this);
            var val = $this.val();
            paramsHolder.html('');
            var kpi = kpiParam.clone(true);
            Pear.OperationConfig._autocomplete(kpi.find('select'));
            paramsHolder.append(kpi.show());
        });*/
    };

    Pear.OperationConfig._autocomplete = function ($field) {
        var url = $field.data('url');
        $field.select2({
            ajax: {
                url: url,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term, // search term
                    };
                },
                processResults: function (data, page) {
                    //console.log(data);
                    return data;
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 1,
            templateResult: Pear.Artifact.Designer._formatKpi, // omitted for brevity, see the source of this page
            templateSelection: Pear.Artifact.Designer._formatKpiSelection // omitted for brevity, see the source of this page
        });
    };

    $(document).ready(function () {
        if ($('.artifact-designer').length) {
            Pear.Artifact.Designer.GraphicSettingSetup();
            Pear.Artifact.Designer.Preview();
        }
        if ($('.artifact-edit').length) {
            Pear.Artifact.Designer.EditSetup();
        }
        if ($('.artifact-list').length) {
            Pear.Artifact.Designer.ListSetup();
        }
        if ($('.template-editor').length) {
            Pear.Template.Editor.LayoutSetup();
        }
        if ($('.template-view').length) {
            Pear.Template.Editor.ViewSetup();
        }
        if ($('.template-edit').length) {
            Pear.Template.Editor.EditSetup();
        }
        if ($('.highlight-save').length) {
            Pear.Highlight.EditSetup();
        }
        if ($('.vessel-schedule-save').length) {
            Pear.VesselSchedule.FormSetup();
        }
        if ($('.nls-save').length) {
            Pear.NLS.FormSetup();
        }
        if ($('.constant-usage-save').length) {
            Pear.ConstantUsage.FormSetup();
        }

        if ($('.calculator').length) {
            Pear.PricingCalculator.Init();
            Pear.ProductionYieldCalculator.Init();
            Pear.StandardCalculator.Init();
            Pear.PlantAvailabilityCalculator.Init();
            Pear.CalculatorConstant.Init();
        }

        if ($('.output-config-save').length) {
            Pear.OutputConfig.FormSetup();
        }

        $('.see-more').click(function () {
            var modalHeader = $('<div/>');
            modalHeader.addClass('modal-header');
            modalHeader.html('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>');
            modalHeader.append($('<div/>').addClass('row').addClass('modal-header-detail').html('<div class="title">' + $(this).closest('td').find('.highlight-title').html() + '</div>'));
            var modalBody = $('<div/>');
            modalBody.addClass('modal-body');
            modalBody.html($(this).closest('td').find('.full-string').clone());
            modalBody.find('.full-string').show();
            var more = $('#modalDialog .modal-content');
            more.html('');
            more.append(modalHeader);
            more.append(modalBody);
            $('#modalDialog .modal-content').removeClass('ajax-loading');
        });
        $('.nls-see-all').click(function (e) {
            e.preventDefault();
            $.get($(this).attr('href'), function (data) {
            });
        });
        $('.highlight-order').keyup(function () {
            var form = $(this).closest('form');
            $.post(form.attr('action'), form.serialize().replace(/item\./g, ''), function (data) {
            });
        });
        $('.select-form #ParentId').change(function () {
            var $this = $(this);
            var url = $this.data('url');
            var val = $this.val();
            if (parseInt(val) == 0) {
                $('.parent-options').hide();
            } else {
                $('#ParentOptionId')
                    .find('option')
                    .remove()
                    .end();
                $.get(url, 'id=' + val, function (data) {
                    $.each(data, function (key, opt) {

                        $('#ParentOptionId')
                            .append($("<option></option>")
                                .attr("value", opt.Id)
                                .text(opt.Text));
                    });
                    $('.parent-options').show();
                });
            }
        });
        if ($('.highlight-display').length) {
            $('.datepicker').datetimepicker({
                format: "MM/DD/YYYY",
                focusOnShow: false,
            }).on('dp.change', function (e) {
                if (e.oldDate == null || e.date.format("MM/DD/YYYY") == e.oldDate.format("MM/DD/YYYY")) {
                    return;
                }
                var href = $('.nav-tabs .active a').attr('href');
                var s = href + '&Periode=' + encodeURIComponent(e.date.format("MM/DD/YYYY"));
                window.location = s;

            });

            $('.monthpicker').datetimepicker({
                format: "MM/YYYY"
            }).on('dp.change', function (e) {
                if (e.oldDate == null || e.date.format("MM/YYYY") == e.oldDate.format("MM/YYYY")) {
                    return;
                }
                var href = $('.nav-tabs .active a').attr('href');
                var s = href + '&Periode=' + encodeURIComponent(e.date.format("MM/YYYY"));
                window.location = s;
            });

            $('.yearpicker').datetimepicker({
                format: "YYYY"
            }).on('dp.change', function (e) {
                if (e.oldDate == null || e.date.format("YYYY") == e.oldDate.format("YYYY")) {
                    return;
                }
                var href = $('.nav-tabs .active a').attr('href');
                var s = href + '&Periode=' + encodeURIComponent(e.date.format("YYYY"));
                window.location = s;
            });
        }

        if ($('.economic-summary-save').length) {
            Pear.EconomicSummary.FormSetup();
        }

        if ($('.operation-config-save').length) {
            Pear.OperationConfig.FormSetup();
        }


    });
    window.Pear = Pear;

    var _artifactHolder;
    var _highchartsContainer;
    var _latestDateTimeFormat;
    var _highlightHolder;
    var _configs = [];
    var _highlightConfigs = [];
    var searchArtifactConfig = function (configs, artifactId) {
        var result = { isExisted: false, config: {}, index: -1 };
        if (configs.length > 0) {
            for (var i = 0; i < configs.length; i++) {
                if (configs[i].Id == artifactId) {
                    result.isExisted = true;
                    result.config = configs[i];
                    result.index = i;
                    result.dateformat = configs[i].dateformat;
                    return result;
                }
            }
        }

        return result;
    }
    var searchHighlightConfig = function (configs, highlightId) {
        var result = { isExisted: false, config: {}, index: -1 };
        if (configs.length > 0) {
            for (var i = 0; i < configs.length; i++) {
                if (configs[i].Id == highlightId) {
                    result.isExisted = true;
                    result.config = configs[i];
                    result.index = i;
                    result.dateformat = configs[i].dateformat;
                    return result;
                }
            }
        }

        return result;
    }
    var getDateFormat = function (periodeType) {
        if (periodeType == undefined) return;
        switch (periodeType.toLowerCase()) {
            case "yearly":
                return 'YYYY';
            case "monthly":
                return 'MM/YYYY';
            case "weekly":
                return 'MM/DD/YYYY';
            case "daily":
                return 'MM/DD/YYYY';
            case "hourly":
                return 'MM/DD/YYYY hh:00 A';
        }
    }
    var getGraphicSetting = function (el) {
        var artifactHolder = el.closest('.artifact-holder');
        _artifactHolder = artifactHolder;
        _highchartsContainter = el.closest('.highcharts-container');


        var artifactId = artifactHolder.attr('data-artifact-id');
        var chart = artifactHolder.highcharts();
        var url = '/Artifact/GraphicSetting/' + artifactId;

        $('body').append($('<div/>').addClass('modal-loader').css({
            position: 'fixed',
            top: 0,
            bottom: 0,
            right: 0,
            left: 0,
            backgroundColor: 'rgba(0,0,0,.5)',
            backgroundImage: 'url("/Content/img/ajax-loader2.gif")',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat',
            zIndex: 1000,
        }));

        $.ajax({
            url: url,
            method: 'GET',
            success: function (data) {
                $('.graphic-setting-content').hide();
                $('.graphic-setting-content').html(data);
                $('.graphic-setting-content').append('<input type="hidden" value="' + artifactId + '" name="Id" />');

                $('.graphic-setting-content').show();
                $('#graphic-setting').modal('show');

                function rangeDatePicker() {
                    var format = $('#datetime-attr').attr('data-datepickerformat');
                    $('.datepicker').datetimepicker({
                        format: format,
                    });
                    $('.datepicker').change(function (e) {
                    });
                    $('#PeriodeType').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        var clearValue = $('.datepicker').each(function (i, val) {
                            $(val).val('');
                            if ($(val).data("DateTimePicker") !== undefined) {
                                $(val).data("DateTimePicker").destroy();
                            }
                        });
                        switch ($this.val().toLowerCase().trim()) {
                            case 'hourly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY hh:00 A"
                                });
                                break;
                            case 'daily':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY"
                                });
                                break;
                            case 'weekly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY",
                                    daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                                });
                                break;
                            case 'monthly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/YYYY"
                                });
                                break;
                            case 'yearly':
                                $('.datepicker').datetimepicker({
                                    format: "YYYY"
                                });
                                break;
                            default:

                        }
                    });
                };
                function rangeControl() {
                    $('#general-graphic-settings').on('change', '#RangeFilter', function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        $('#range-holder').prop('class', $this.val().toLowerCase().trim());
                    });
                    var original = $('#RangeFilter').clone(true);
                    var rangeFilterSetup = function (periodeType) {
                        var graphicType = $('#graphic-type').val();
                        var toRemove = {};
                        switch (graphicType) {
                            case "tabular":
                            case "tank":
                            case "speedometer":
                            case "traffic":
                            case "pie":
                                toRemove.hourly = ['AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'AllExistingYears'];
                                break;
                            default:
                                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                break;
                        }
                        var originalClone = original.clone(true);
                        originalClone.find('option').each(function (i, val) {
                            if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                                originalClone.find(val).remove();
                            }
                        });
                        $('#RangeFilter').replaceWith(originalClone);
                    };

                    rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
                    $('#general-graphic-settings').on('change', '#PeriodeType', function (e) {
                        e.preventDefault();
                        var $this = $(this);

                        rangeFilterSetup($this.val().toLowerCase().trim());
                        $('.graphic-setting-content #RangeFilter option:eq(0)').attr('selected', 'selected');
                        $('#range-holder').removeAttr('class');
                    });

                };
                function specificDate() {
                    $(".datepicker").on("dp.change", function (e) {
                        if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                            $('#EndInDisplay').val($('#StartInDisplay').val());
                        }
                    });
                };
                rangeControl();
                rangeDatePicker();
                specificDate();
                var search = searchArtifactConfig(_configs, artifactId);
                if (search.isExisted == true) {
                    $('.graphic-setting-content #PeriodeType').val(search.config.PeriodeType).change();
                    $('.graphic-setting-content #RangeFilter').val(search.config.RangeFilter).change();
                    $('.datepicker').datetimepicker({
                        format: search.dateformat,
                    });
                    $('.graphic-setting-content #StartInDisplay').val(search.config.StartInDisplay);
                    $('.graphic-setting-content #EndInDisplay').val(search.config.EndInDisplay);
                    $('#range-holder').removeClass();
                    $('#range-holder').addClass(search.config.RangeFilter.toLowerCase());
                }
            }
        });

    }
    var getHighlightSetting = function (el) {
        var highlightHolder = el.closest('.highlight-holder');
        _highlightHolder = highlightHolder;
        _highchartsContainter = el.closest('.highcharts-container');

        var highlightId = highlightHolder.attr('data-highlight-id');
        //var chart = artifactHolder.highcharts();
        var url = '/Artifact/HighlightSetting/' + highlightId;
        //_highchartsContainter.hide();

        $('body').append($('<div/>').addClass('modal-loader').css({
            position: 'fixed',
            top: 0,
            bottom: 0,
            right: 0,
            left: 0,
            backgroundColor: 'rgba(0,0,0,.5)',
            backgroundImage: 'url("/Content/img/ajax-loader2.gif")',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat',
            zIndex: 1000,
        }));

        $.ajax({
            url: url,
            method: 'GET',
            success: function (data) {
                $('.highlight-setting-content').hide();
                $('.highlight-setting-content').html(data);
                $('.highlight-setting-content').append('<input type="hidden" value="' + highlightId + '" name="Id" />');

                $('.highlight-setting-content').show();
                $('#highlight-setting').modal('show');

                function rangeDatePicker() {
                    var format = $('#datetime-attr').attr('data-datepickerformat');
                    $('.datepicker').datetimepicker({
                        format: format,
                    });
                    $('.datepicker').change(function (e) {
                    });
                    $('#PeriodeType').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        var clearValue = $('.datepicker').each(function (i, val) {
                            $(val).val('');
                            if ($(val).data("DateTimePicker") !== undefined) {
                                $(val).data("DateTimePicker").destroy();
                            }
                        });
                        switch ($this.val().toLowerCase().trim()) {
                            case 'hourly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY hh:00 A"
                                });
                                break;
                            case 'daily':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY"
                                });
                                break;
                            case 'weekly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY",
                                    daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                                });
                                break;
                            case 'monthly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/YYYY"
                                });
                                break;
                            case 'yearly':
                                $('.datepicker').datetimepicker({
                                    format: "YYYY"
                                });
                                break;
                            default:

                        }
                    });
                };
                function rangeControl() {
                    $('#general-graphic-settings').on('change', '#RangeFilter', function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        $('#range-holder').prop('class', $this.val().toLowerCase().trim());
                    });
                    var original = $('#RangeFilter').clone(true);
                    var rangeFilterSetup = function (periodeType) {
                        var graphicType = $('#graphic-type').val();
                        var toRemove = {};
                        switch (graphicType) {
                            case "tabular":
                            case "tank":
                            case "speedometer":
                            case "traffic":
                            case "pie":
                                toRemove.hourly = ['AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'AllExistingYears'];
                                break;
                            case "highlight":
                                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'CurrentYear', 'AllExistingYears'];
                                break;
                            default:
                                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                break;
                        }
                        var originalClone = original.clone(true);
                        originalClone.find('option').each(function (i, val) {
                            if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                                originalClone.find(val).remove();
                            }
                        });
                        $('#RangeFilter').replaceWith(originalClone);
                    };

                    rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
                    $('#general-graphic-settings').on('change', '#PeriodeType', function (e) {
                        e.preventDefault();
                        var $this = $(this);

                        rangeFilterSetup($this.val().toLowerCase().trim());
                        $('.highlight-setting-content #RangeFilter option:eq(0)').attr('selected', 'selected');
                        $('.highlight-setting-content #RangeFilter').trigger('change');
                        //$('#range-holder').removeAttr('class');
                    });

                };
                function specificDate() {
                    $(".datepicker").on("dp.change", function (e) {
                        if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                            $('#EndInDisplay').val($('#StartInDisplay').val());
                        }
                    });
                };
                rangeControl();
                rangeDatePicker();
                specificDate();

                var search = searchHighlightConfig(_highlightConfigs, highlightId);
                if (search.isExisted == true) {
                    $('.highlight-setting-content #PeriodeType').val(search.config.PeriodeType).change();
                    $('.highlight-setting-content #RangeFilter').val(search.config.RangeFilter).change();
                    $('.datepicker').datetimepicker({
                        format: search.dateformat,
                    });
                    $('.highlight-setting-content #StartInDisplay').val(search.config.StartInDisplay);
                    $('.highlight-setting-content #EndInDisplay').val(search.config.EndInDisplay);
                    $('#range-holder').removeClass();
                    $('#range-holder').addClass(search.config.RangeFilter.toLowerCase());
                } else {
                    var currentPeriodeType = $('#general-graphic-settings #PeriodeType').val();
                    var currentRangeFilter = $('#general-graphic-settings #RangeFilter').val().toLowerCase().trim();
                    $('#general-graphic-settings #range-holder').prop('class', currentRangeFilter);
                }
                $('#general-graphic-settings #range-filter-holder').hide();
            }
        });
    }
    var getExportChart = function (el) {
        var artifactHolder = el.closest('.artifact-holder');
        _artifactHolder = artifactHolder;
        _highchartsContainter = el.closest('.highcharts-container');


        var artifactId = artifactHolder.attr('data-artifact-id');
        var chart = artifactHolder.highcharts();
        var url = '/Artifact/ExportSetting/' + artifactId;

        $('body').append($('<div/>').addClass('modal-loader').css({
            position: 'fixed',
            top: 0,
            bottom: 0,
            right: 0,
            left: 0,
            backgroundColor: 'rgba(0,0,0,.5)',
            backgroundImage: 'url("/Content/img/ajax-loader2.gif")',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat',
            zIndex: 1000,
        }));

        $.ajax({
            url: url,
            method: 'GET',
            cached: false,
            success: function (data) {
                console.log(data);
                $('.export-setting-content').hide();
                $('.export-setting-content').empty().html(data);
                
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
                $('.export-setting-content').append('<input type="hidden" value="' + artifactId + '" name="Id" />');

                $('.export-setting-content').show();
                $('#export-setting').modal('show');

                function rangeDatePicker() {
                    var format = $('#datetime-attr').attr('data-datepickerformat');
                    $('.datepicker').datetimepicker({
                        format: format,
                    });
                    $('.datepicker').change(function (e) {
                    });
                    $('#PeriodeType').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        var clearValue = $('.datepicker').each(function (i, val) {
                            $(val).val('');
                            if ($(val).data("DateTimePicker") !== undefined) {
                                $(val).data("DateTimePicker").destroy();
                            }
                        });
                        switch ($this.val().toLowerCase().trim()) {
                            case 'hourly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY hh:00 A"
                                });
                                break;
                            case 'daily':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY"
                                });
                                break;
                            case 'weekly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY",
                                    daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                                });
                                break;
                            case 'monthly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/YYYY"
                                });
                                break;
                            case 'yearly':
                                $('.datepicker').datetimepicker({
                                    format: "YYYY"
                                });
                                break;
                            default:

                        }
                    });
                };
                function rangeControl() {
                    $('#general-graphic-settings').on('change', '#RangeFilter', function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        $('#range-holder').prop('class', $this.val().toLowerCase().trim());
                    });
                    var original = $('#RangeFilter').clone(true);
                    var rangeFilterSetup = function (periodeType) {
                        var graphicType = $('#graphic-type').val();
                        var toRemove = {};
                        switch (graphicType) {
                            case "tabular":
                            case "tank":
                            case "speedometer":
                            case "traffic":
                            case "pie":
                                toRemove.hourly = ['AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'AllExistingYears'];
                                break;
                            default:
                                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                break;
                        }
                        var originalClone = original.clone(true);
                        originalClone.find('option').each(function (i, val) {
                            if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                                originalClone.find(val).remove();
                            }
                        });
                        $('#RangeFilter').replaceWith(originalClone);
                    };

                    rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
                    $('#general-graphic-settings').on('change', '#PeriodeType', function (e) {
                        e.preventDefault();
                        var $this = $(this);

                        rangeFilterSetup($this.val().toLowerCase().trim());
                        $('.graphic-setting-content #RangeFilter option:eq(0)').attr('selected', 'selected');
                        $('#range-holder').removeAttr('class');
                    });

                };
                function specificDate() {
                    $(".datepicker").on("dp.change", function (e) {
                        if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                            $('#EndInDisplay').val($('#StartInDisplay').val());
                        }
                    });
                };
                rangeControl();
                rangeDatePicker();
                specificDate();

                if ($('.filename').val().length < 7) {
                    $('.export').prop('disabled', true);
                }

                $('.filename').keyup(function () {
                    if ($(this).val().length >= 7) {
                        $('.export').prop('disabled', false);
                    } else {
                        $('.export').prop('disabled', true);
                    }
                });

                setTimeout(function () {
                    var search = searchArtifactConfig(_configs, artifactId);
                    if (search.isExisted == true) {
                        var currentTime = new Date(2018, 9, 28);
                        var currentDay = currentTime.getDate()
                        var currentMonth = ("0" + (currentTime.getMonth() + 1)).slice(-2);
                        var currentYear = currentTime.getFullYear();
                        var firstDayOfWeek = currentTime.getDate() - currentTime.getDay();
                        var lastDayOfWeek = firstDayOfWeek + 6;

                        var startOfWeek = moment().startOf('week');
                        var endOfWeek = moment().endOf('week');

                        var startOfMonth = moment().startOf('month');
                        var endOfMonth = moment().endOf('month');

                        var current = moment();

                        var startOfYear = moment().startOf('year');
                        var endOfYear = moment().endOf('year');

                        var startInDisplay = '';
                        var endInDisplay = '';
                        var startAfterParsed = '';
                        var endAfterParsed = '';
                        
                        switch (search.config.PeriodeType.toLowerCase()) {
                            case "daily":
                                var formatDaily = "MM/DD/YYYY";
                                if (search.config.RangeFilter.toLowerCase() == "currentweek") {
                                    startInDisplay = startOfWeek.format(formatDaily);
                                    endInDisplay = endOfWeek.format(formatDaily);
                                } else if (search.config.RangeFilter.toLowerCase() == "currentmonth") {
                                    startInDisplay = startOfMonth.format(formatDaily);
                                    endInDisplay = endOfMonth.format(formatDaily);
                                } else if (search.config.RangeFilter.toLowerCase() == "mtd") {
                                    startInDisplay = startOfMonth.format(formatDaily);
                                    endInDisplay = current.format(formatDaily);
                                } else if (search.config.RangeFilter.toLowerCase() == "ytd") {
                                    startInDisplay = startOfYear.format(formatDaily);
                                    endInDisplay = current.format(formatDaily);
                                } else if (search.config.RangeFilter.toLowerCase() == "specificday") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                } else if (search.config.RangeFilter.toLowerCase() == "interval") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                }
                                startAfterParsed = startInDisplay;
                                endAfterParsed = endInDisplay;
                                break;
                            case "monthly":
                                var formatMonthly = "MM/YYYY";
                                if (search.config.RangeFilter.toLowerCase() == "currentyear") {
                                    startInDisplay = startOfYear.format(formatMonthly);
                                    endInDisplay = endOfYear.format(formatMonthly);
                                } else if (search.config.RangeFilter.toLowerCase() == "ytd") {
                                    startInDisplay = startOfYear.format(formatMonthly);
                                    endInDisplay = current.format(formatMonthly);
                                } else if (search.config.RangeFilter.toLowerCase() == "specificmonth") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                } else if (search.config.RangeFilter.toLowerCase() == "interval") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                }
                                startAfterParsed = startInDisplay.split('/')[0] + "/01/" + startInDisplay.split('/')[1];
                                endAfterParsed = endInDisplay.split('/')[0] + "/01/" + endInDisplay.split('/')[1];
                                break;
                            case "yearly":
                                var formatYearly = "YYYY";
                                if (search.config.RangeFilter.toLowerCase() == "currentyear") {
                                    startInDisplay = startOfYear.format(formatYearly);
                                    endInDisplay = endOfYear.format(formatYearly);
                                } else if (search.config.RangeFilter.toLowerCase() == "specificyear") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                } else if (search.config.RangeFilter.toLowerCase() == "interval") {
                                    startInDisplay = search.config.StartInDisplay;
                                    endInDisplay = search.config.EndInDisplay;
                                }
                                startAfterParsed = "01/01/" + startInDisplay;
                                endAfterParsed = "01/01/" + endInDisplay;
                                break;
                        }
                        $('.export-setting-content #PeriodeType').val(search.config.PeriodeType).change();
                        $('.export-setting-content #RangeFilter').val(search.config.RangeFilter).change();
                        $('.datepicker').datetimepicker({
                            format: search.dateformat                         
                        });
                                              
                        $('#StartInDisplay').data("DateTimePicker").minDate(moment(startAfterParsed));
                        $('#StartInDisplay').data("DateTimePicker").maxDate(moment(endAfterParsed));
                        $('#EndInDisplay').data("DateTimePicker").minDate(moment(startAfterParsed));
                        $('#EndInDisplay').data("DateTimePicker").maxDate(moment(endAfterParsed));
                        $('#StartInDisplay.datepicker').on('dp.change', function (e) {
                            $('#EndInDisplay').data("DateTimePicker").minDate(e.date);
                        });
                        

                        $('.export-setting-content #StartInDisplay').val(startInDisplay);
                        $('.export-setting-content #EndInDisplay').val(endInDisplay);

                        $('#range-holder').removeClass();
                        $('#range-holder').addClass(search.config.RangeFilter.toLowerCase());
                    }

                }, 1000);


            }
        });
    }

    var getExportChartPreview = function (el) {
        var artifactHolder = el.closest('.artifact-holder');
        _artifactHolder = artifactHolder;
        _highchartsContainter = el.closest('.highcharts-container');


        var artifactId = artifactHolder.attr('data-artifact-id');
        var chart = artifactHolder.highcharts();
        var url = '/Artifact/ExportSettingPreview/';

        $('body').append($('<div/>').addClass('modal-loader').css({
            position: 'fixed',
            top: 0,
            bottom: 0,
            right: 0,
            left: 0,
            backgroundColor: 'rgba(0,0,0,.5)',
            backgroundImage: 'url("/Content/img/ajax-loader2.gif")',
            backgroundPosition: 'center',
            backgroundRepeat: 'no-repeat',
            zIndex: 9000,
        }));

        $.ajax({
            url: url,
            data: $('.artifact-form').serialize(),
            method: 'POST',
            success: function (data) {
                $('.export-setting-content').hide();
                $('.export-setting-content').html(data);
                $("form").removeData("validator");
                $("form").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("form");
                $('.export-setting-content').append('<input type="hidden" value="' + artifactId + '" name="Id" />');

                $('.export-setting-content').show();
                $('#export-setting').modal('show');

                function rangeDatePicker() {
                    var format = $('#datetime-attr').attr('data-datepickerformat');
                    $('.datepicker').datetimepicker({
                        format: format,
                    });
                    $('.datepicker').change(function (e) {
                    });
                    $('#PeriodeType').change(function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        var clearValue = $('.datepicker').each(function (i, val) {
                            $(val).val('');
                            if ($(val).data("DateTimePicker") !== undefined) {
                                $(val).data("DateTimePicker").destroy();
                            }
                        });
                        switch ($this.val().toLowerCase().trim()) {
                            case 'hourly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY hh:00 A"
                                });
                                break;
                            case 'daily':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY"
                                });
                                break;
                            case 'weekly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/DD/YYYY",
                                    daysOfWeekDisabled: [0, 2, 3, 4, 5, 6]
                                });
                                break;
                            case 'monthly':
                                $('.datepicker').datetimepicker({
                                    format: "MM/YYYY"
                                });
                                break;
                            case 'yearly':
                                $('.datepicker').datetimepicker({
                                    format: "YYYY"
                                });
                                break;
                            default:

                        }
                    });
                };
                function rangeControl() {
                    $('#general-graphic-settings').on('change', '#RangeFilter', function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        $('#range-holder').prop('class', $this.val().toLowerCase().trim());
                    });
                    var original = $('#RangeFilter').clone(true);
                    var rangeFilterSetup = function (periodeType) {
                        var graphicType = $('#graphic-type').val();
                        var toRemove = {};
                        switch (graphicType) {
                            case "tabular":
                            case "tank":
                            case "speedometer":
                            case "traffic":
                            case "pie":
                                toRemove.hourly = ['AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentWeek', 'CurrentYear', 'DTD', 'MTD', 'YTD', 'CurrentMonth', 'YTD', 'Interval', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'DTD', 'MTD', 'CurrentYear', 'YTD', 'Interval', 'SpecificDay', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'Interval', 'SpecificDay', 'SpecificMonth', 'AllExistingYears'];
                                break;
                            default:
                                toRemove.hourly = ['CurrentWeek', 'CurrentMonth', 'CurrentYear', 'YTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.daily = ['CurrentHour', 'CurrentDay', 'CurrentYear', 'DTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.weekly = ['CurrentHour', 'CurrentDay', 'DTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.monthly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                toRemove.yearly = ['CurrentHour', 'CurrentDay', 'CurrentWeek', 'CurrentMonth', 'DTD', 'MTD', 'YTD', 'SpecificDay', 'SpecificMonth', 'SpecificYear', 'AllExistingYears'];
                                break;
                        }
                        var originalClone = original.clone(true);
                        originalClone.find('option').each(function (i, val) {
                            if (toRemove[periodeType].indexOf(originalClone.find(val).val()) > -1) {
                                originalClone.find(val).remove();
                            }
                        });
                        $('#RangeFilter').replaceWith(originalClone);
                    };

                    rangeFilterSetup($('#PeriodeType').val().toLowerCase().trim());
                    $('#general-graphic-settings').on('change', '#PeriodeType', function (e) {
                        e.preventDefault();
                        var $this = $(this);

                        rangeFilterSetup($this.val().toLowerCase().trim());
                        $('.graphic-setting-content #RangeFilter option:eq(0)').attr('selected', 'selected');
                        $('#range-holder').removeAttr('class');
                    });

                };
                function specificDate() {
                    $(".datepicker").on("dp.change", function (e) {
                        if ($('#RangeFilter').val().toLowerCase().indexOf('specific') > -1 && e.target.id === 'StartInDisplay') {
                            $('#EndInDisplay').val($('#StartInDisplay').val());
                        }
                    });
                };
                rangeControl();
                rangeDatePicker();
                specificDate();

                if ($('.filename').val().length < 7) {
                    $('.export').prop('disabled', true);
                }

                $('.filename').keyup(function () {
                    if ($(this).val().length >= 7) {
                        $('.export').prop('disabled', false);
                    } else {
                        $('.export').prop('disabled', true);
                    }
                });

                //$('.export-setting-content #PeriodeType').val(search.config.PeriodeType).change();
                //$('.export-setting-content #RangeFilter').val(search.config.RangeFilter).change();
                //$('.datepicker').datetimepicker({
                //    format: search.dateformat,
                //});

                //$('.export-setting-content #StartInDisplay').val(startInDisplay);
                //$('.export-setting-content #EndInDisplay').val(endInDisplay);

                //$('#range-holder').removeClass();
                //$('#range-holder').addClass(search.config.RangeFilter.toLowerCase());




            }
        });
    }

    $('.artifact-holder').on('click', '.tank-subtitle', function () {
        getGraphicSetting($(this));
    });

    $('.artifact-holder').on('click', '.tank-export', function () {
        getExportChart($(this));
    });

    $('#container').on('click', '.tank-export-preview', function () {
        getExportChartPreview($(this));
    });

    $('#container').on('click', '.tabular-export-preview', function () {
        getExportChartPreview($(this));
    });


    $('.artifact-holder').on('click', '.tabular-export', function () {
        getExportChart($(this));
    });

    $('.highlight-holder').on('click', '.highlight-change-periode', function () {
        getHighlightSetting($(this));
    });

    $('#graphic-setting').on('show.bs.modal', function () {
        $('body').children('.modal-loader').remove();
    });

    $('#highlight-setting').on('show.bs.modal', function () {
        $('body').children('.modal-loader').remove();
    });

    $('#export-setting').on('show.bs.modal', function () {
        $('body').children('.modal-loader').remove();
    });

    $('#graphic-setting').on('submit', '#graphic-setting-form', function (e) {
        e.preventDefault();
        $.ajax({
            url: $('#graphic-setting-form').attr('action'),
            data: $('#graphic-setting-form').serialize(),
            method: 'POST',
            success: function (data2) {
                var callback = Pear.Artifact.Designer._previewCallbacks;
                var result = {};
                $.each($('#graphic-setting-form').serializeArray(), function (key, value) {
                    result[this.name] = this.value;
                });
                result['dateformat'] = getDateFormat(result["PeriodeType"]);
                var search = searchArtifactConfig(_configs, result["Id"]);
                (search.isExisted) ? _configs[search.index] = result : _configs.push(result);
                callback[data2.GraphicType](data2, _artifactHolder);
                $('#graphic-setting').modal('hide');
            }
        })
    });

    $('.btn-export').click(function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            cache: false,
            url: '/Artifact/ExportSetting',
            data: $('#export-setting-form').serialize(),
            success: function (data) {
                if (data.IsSuccess) {
                    window.location = '/Artifact/DownloadExportToExcel?fileGuid=' + data.FileGuid
                        + '&filename=' + data.FileName;
                    $('#export-setting').modal('hide');
                } else {
                    alert("Download Failed : " + data.Message);
                }
            }
        });
    });

    $('#highlight-setting').on('submit', '#highlight-setting-form', function (e) {
        e.preventDefault();
        $.ajax({
            url: $('#highlight-setting-form').attr('action'),
            data: $('#highlight-setting-form').serialize(),
            method: 'POST',
            success: function (data2) {
                var result = {};
                result['dateformat'] = getDateFormat(result["PeriodeType"]);
                $.each($('#highlight-setting-form').serializeArray(), function (key, value) {
                    result[this.name] = this.value;
                });
                var search = searchHighlightConfig(_highlightConfigs, result["Id"]);
                (search.isExisted) ? _highlightConfigs[search.index] = result : _highlightConfigs.push(result);
                _highlightHolder.find('.dashboard-highlight-periode:first').html(Pear.Artifact.Designer._toJavascriptDate(data2.Date, data2.SPeriodeType));
                _highlightHolder.find('.dashboard-highlight-message:first').html(data2.Message);
                $('#highlight-setting').modal('hide');
            }
        })
    })

    $('#graphic-setting').on('click', '#backToDefault', function (e) {
        e.preventDefault();
        getGraphicSetting(_artifactHolder, false);
        var search = searchArtifactConfig(_configs, _artifactHolder.data('artifact-id'));
        if (search.isExisted) {
            _configs.splice(search.index, 1);
        }
    });

    $('#highlight-setting').on('click', '#backToDefault', function (e) {
        e.preventDefault();
        getHighlightSetting(_highlightHolder, false);
        var search = searchHighlightConfig(_highlightConfigs, _highlightHolder.data('highlight-id'));
        if (search.isExisted) {
            _highlightConfigs.splice(search.index, 1);
        }
    });

    $('#graphic-setting').on('hidden.bs.modal', function () {
        $('.graphic-setting-content').html('');
    });

    $('#highlight-setting').on('hidden.bs.modal', function () {
        $('.highlight-setting-content').html('');
    });

    $('#highlight-setting').on('hidden.bs.modal', function () {
        $('.highlight-setting-content').html('');
    });


    if ($('#user-profile-session-data').length > 0) {
        Highcharts.getOptions().exporting.buttons.contextButton.menuItems.push({
            text: "Change Periode",
            onclick: function () {
                getGraphicSetting($(this.renderTo));
            }
        });

        Highcharts.getOptions().exporting.buttons.contextButton.menuItems.push({
            text: "Export To Excel",
            onclick: function () {
                getExportChart($(this.renderTo));
            }
        });
    } else {
        Highcharts.getOptions().exporting.buttons.contextButton.menuItems.push({
            text: "Export To Excel",
            onclick: function () {
                getExportChartPreview($(this.renderTo));
            }
        });
    }

    if ($('#user-profile-session-data').length > 0 && $('#user-profile-session-data').data('issuperadmin') == true) {
        Highcharts.getOptions().exporting.buttons.contextButton.menuItems.push({
            text: "Edit Chart",
            onclick: function (e) {
                var id = $(this.renderTo).data('artifact-id');
                window.open('/Artifact/Edit/' + id, '_blank');
            }
        });
    }
}(window, jQuery, undefined));