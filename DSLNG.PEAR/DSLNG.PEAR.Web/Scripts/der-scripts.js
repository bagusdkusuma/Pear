﻿// Common
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

/**
 * Number.prototype.format(n, x)
 * 
 * @param integer n: length of decimal
 * @param integer x: length of sections
 */
Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};

(function window(window,$,undefined){
    var Der = {};
    Der.Artifact = {};
    Der.Artifact.line = function(data,container){
        container.highcharts({
            chart: {
                zoomType: 'xy',
                backgroundColor: 'transparent',
                height: 200,
                spacingBottom: 5,
                spacingTop: 5,
                spacingLeft: 0,
                spacingRight: 0,
            },
            title: {
                text: data.LineChart.Title,
                style:{
                    fontSize: '11px',
                    fontWeight:'bold'
                }
            },
            subtitle: {
                text: data.LineChart.Subtitle,
                style: {
                    fontSize: '10px',
                    display:'none'
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
                    },
                    animation: false
                }
            },
            xAxis: {
                categories: data.LineChart.Periodes,
                labels: {
                    style: {
                        fontSize: '7px'
                    }
                }
            },
            yAxis: {
                title: {
                    text: data.LineChart.ValueAxisTitle,
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
                        fontSize: '7px'
                    }
                }
            },
            exporting: {
                url: '/Chart/Export',
                filename: 'MyChart',
                width: 1200
            },
            credits: {
                enabled: false
            },
            legend: {
                enabled:false,
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            series: data.LineChart.Series
        });
    }
    Der.Artifact.multiaxis =  function (data, container) {
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
            yAxes.push({
                title: {
                    text: data.MultiaxisChart.Charts[i].Measurement, //data.MultiaxisChart.Charts[i].ValueAxisTitle + ' (' + data.MultiaxisChart.Charts[i].Measurement + ')',
                },
                opposite: data.MultiaxisChart.Charts[i].IsOpposite,
                tickInterval: data.MultiaxisChart.Charts[i].FractionScale == 0 ? null : data.MultiaxisChart.Charts[i].FractionScale,
                max: data.MultiaxisChart.Charts[i].MaxFractionScale == 0 ? null : data.MultiaxisChart.Charts[i].MaxFractionScale,
                labels: {
                    style: {
                        fontSize: '7px'
                    }
                }
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
        container.highcharts({
            chart: {
                zoomType: 'xy',
                alignTicks: false,
                backgroundColor: 'transparent',
                height: 200,
                spacingBottom: 5,
                spacingTop: 5,
                spacingLeft: 0,
                spacingRight: 0,
            },
            title: {
                text: data.MultiaxisChart.Title,
                style: {
                    fontSize: '11px',
                    fontWeight: 'bold'
                }
            },
            subtitle: {
                text: data.MultiaxisChart.Subtitle,
                style: {
                    fontSize: '10px',
                    display: 'none'
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: plotOptions,
            xAxis: [{
                categories: data.MultiaxisChart.Periodes,
                crosshair: true,
                labels: {
                    style: {
                        fontSize: '7px'
                    }
                }
            }],
            yAxis: yAxes,
            legend: {
                itemStyle: {
                    fontSize : '7px'
                },

            },
            series: series
        });
    };
    Der.Artifact.speedometer = function (data, container) {
        var stops = [];
        var last = data.SpeedometerChart.PlotBands.length-1;
        for ( var i in data.SpeedometerChart.PlotBands){
            stops.push([data.SpeedometerChart.PlotBands[i].from / data.SpeedometerChart.PlotBands[last].to, data.SpeedometerChart.PlotBands[i].color]);
        }
        console.log(stops);
        container.highcharts({
            chart: {
                type: 'solidgauge',
                height: 60,
                width: 200,
                margin: [12, 0, 6, 0],
                spacingTop: 0,
                spacingBottom: 0,
                spacingLeft: 0,
                spacingRight: 0,
            },

            title: {
                text: data.SpeedometerChart.Title,
                style: {
                    fontSize:'8px'
                }
            },
            //subtitle: {
            //    text: data.SpeedometerChart.Subtitle,
            //},
            subtitle: {
                text: "MCHE Rundown",
                style: {
                    fontSize: '10px',
                    color:'#000'
                }
            },
            pane: {
                center: ['50%', '85%'],
                size: '140%',
                startAngle: -90,
                endAngle: 90,
                background: {
                    backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
                    innerRadius: '60%',
                    outerRadius: '100%',
                    shape: 'arc'
                }
            },
            plotOptions: {
                solidgauge: {
                    dataLabels: {
                        y: 5,
                        borderWidth: 0,
                        useHTML: true,
                        enabled:false
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

                stops: stops,
                lineWidth: 0,
                minorTickInterval: null,
                tickPixelInterval: 400,
                tickWidth: 0,
                title: {
                    y: -70
                },
                labels: {
                    y: 10,
                    distance: -30,
                    //style:{
                    //    margin:'0 30px'
                    //}
                    //x: -20,
                    enabled:false
                },

                title: {
                    text: data.SpeedometerChart.ValueAxisTitle,
                },
                //plotBands: data.SpeedometerChart.PlotBands
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
    Der.Artifact.barmeter = function (data, container) {
        var $wrapper = $('<div />');
        $wrapper.addClass('barmeter-wrapper');
        var $this = container;
        var $canvas = $('<canvas />');
        var $label = $this.find('label').clone();
        if ($label.length) {
            $this.find('label').css('display', 'none');
            console.log(data);
            $label.append('<span style="margin-left:30px">' + data['SpeedometerChart'].Series.data[0] + '</span>');
            $wrapper.append($label);
        } else {
            $wrapper.append('<label>' + data['SpeedometerChart'].Series.data[0] + '</label>');
        }
        var config = data['SpeedometerChart'];
        $canvas.css({
            width: $this.width() + 'px',
            height: $this.height() + 'px'
        });
       
        $this.append($wrapper.append($canvas))

        var canvas = $this.find('canvas')[0];
        if (canvas.getContext) {
            var ctx = canvas.getContext("2d");
            gradient = ctx.createLinearGradient(0, 0, canvas.width, 0);
            var last = config.PlotBands.length -1;
            for (var i in config.PlotBands) {
                gradient.addColorStop(config.PlotBands[i].from / config.PlotBands[last].from, config.PlotBands[i].color);
            }
            ctx.fillStyle = gradient;
            ctx.fillRect(3, 0, canvas.width-3, canvas.height);
            ctx.fillStyle = "rgb(0,0,0)";
            var point = config.Series.data[0] / config.PlotBands[last].from * (canvas.width - 6) + 3;
            ctx.fillRect(point-3, 0, 6, canvas.height - 30);

            // the triangle
            ctx.beginPath();
            ctx.moveTo(point-3, canvas.height - 30);
            ctx.lineTo(point, canvas.height);
            ctx.lineTo(point + 3, canvas.height - 30);
            ctx.closePath();
            ctx.fillStyle = "rgb(0,0,0)";
            ctx.fill();
        }
    }
    Der.Artifact.termometer = function (data, container) {
        var $this = container;
        var $canvas = $('<canvas/>');
        $canvas.css({
            width: '100%',
            height: '100%'
        });
        $this.append($canvas);
        var canvas = $this.find('canvas')[0];
        if (canvas.getContext) {
            var ctx = canvas.getContext("2d");
            var start = (100 - data.Value) * canvas.height / 100;
            var end = canvas.height;
            gradient = ctx.createLinearGradient(0, start, 0, end);
            gradient.addColorStop("0", "#17375E");
            gradient.addColorStop("1.0", "#8EB4E3");
            ctx.fillStyle = gradient;
            ctx.fillRect(0, start, canvas.width, data.Value * canvas.height / 100);
        }
    }
    Der.Artifact.pie = function (data, container) {
        var $title = $('<div />');
        $title.addClass('pie-title');
        $title.html(data.Pie.Title);
        container.highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: data.Pie.Is3D,
                    alpha: 60,
                    beta: 0
                },
                backgroundColor: 'transparent',
                margin: [0, 0, 0, 0],
                spacingTop: 0,
                spacingBottom: 0,
                spacingLeft: 0,
                spacingRight: 0,
                height:170
            },
            title: {
                text: data.Pie.Title,
                style: {
                    fontSize: '11px',
                    fontWeight: 'bold',
                    display:'none'
                }
            },
            subtitle: {
                text: data.Pie.Subtitle,
                style: {
                    color: '#fff',
                    display: 'none'
                }
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    size: '100%',
                    slicedOffset: 30,
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        distance: 2,
                        formatter: function () {
                            return this.point.name + ': <br/> ' + this.percentage.toFixed(2) + ' %';
                        },
                        shadow: false,
                        style: {
                            textShadow: false,
                            fontSize: '7px',
                            fontWeight: 'normal'
                        }
                    },
                    showInLegend: data.Pie.ShowLegend,
                    //innerSize: '40%',
                    size: '75%',
                    shadow: false,
                    depth: 45,
                    animation:false
                }
            },
            legend: {
                itemStyle: {
                    fontSize: '7px'
                },
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
        container.append($title);
    };
    Der.Artifact.tank = function (data, container) {
        container.tank(data.Tank, {
            height: container.height(),
            width: container.width()
        });
    };
    window.Der = Der;
}(window,jQuery));