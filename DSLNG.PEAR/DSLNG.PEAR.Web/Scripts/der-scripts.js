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
//Array.prototype.max = function () {
//    return Math.max.apply(null, this);
//};

//Array.prototype.min = function () {
//    return Math.min.apply(null, this);
//};

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

(function window(window, $, undefined) {

    Highcharts.setOptions({
        lang: {
            decimalPoint: '.',
            thousandsSep: ','
        }
    });
    var Der = {};
    Der.Artifact = {};
    Der.Helper = {};
    Der.Artifact.line = function (data, container) {
        //console.log(data);
        var symbol = 'circle';
        var fillColor = 'red';
        if (data.LineChart.Title.toLowerCase().indexOf('cds') > -1) {
            symbol = 'triangle';
        } else if (data.LineChart.Title.toLowerCase().indexOf('thermal') > -1) {
            symbol = 'square';
        } else if (data.LineChart.Title.toLowerCase().indexOf('loss') > -1) {
            fillColor = '#403152';
        }
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
                style: {
                    fontSize: '11px',
                    fontWeight: 'bold'
                }
            },
            subtitle: {
                text: data.LineChart.Subtitle,
                style: {
                    fontSize: '10px',
                    display: 'none'
                }
            },

            plotOptions: {
                line: {
                    marker: {
                        enabled: true,
                        symbol: symbol,
                        fillColor: fillColor,
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
                    align: 'high',
                    rotation: 0,
                    y: -10,
                    offset: 15,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'bold'
                    }
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }],
                tickInterval: parseFloat(container.data('fraction')),
                max: data.MaxFractionScale == 0 ? null : data.MaxFractionScale,
                min: container.data('min'),
                labels: {
                    style: {
                        fontSize: '7px'
                    },
                    formatter: function () {
                        var x = this.value;
                        return x.format();
                    }
                },
                lineWidth: 1
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
                enabled: false,
                itemHoverStyle: {
                    color: '#FF0000'
                }
            },
            series: data.LineChart.Series
        });
        //setTimeout(function () {
        //var svg = container.find('.highcharts-container').html();
        ////console.log(svg);   
        //var $canvas = $('<canvas />');
        //$canvas.width(container.width());
        //$canvas.height(container.height());
        //container.hide();
        //container.parent().append($canvas);
        //console.log(container.parent().find('canvas')[0]);
        //canvg(container.parent().find('canvas')[0], svg);
        //var canvas = container.parent().find('canvas')[0];
        //var $img = $('<img />');
        //$img.attr('src', canvas.toDataURL());
        //container.parent().find('canvas').replaceWith($img);
        //container.remove();
        //}, 1000);

    }
    Der.Artifact.multiaxis = function (data, container, opt, type) {
        var symbol = i % 2 == 0 ? 'triangle' : 'square';
        var fillColor = i % 2 == 0 ? '#31587f' : '#fff000';
        var converted = false;
        var decimal = 0;
        var title = data.MultiaxisChart.Title.toLowerCase();
        var options = {
            legend: {
                enabled: false,
                itemStyle: { "fontSize": "9px", "fontWeight": "normal" },
                verticalAlign: "bottom" 
            },
            gridLineColor: '#e6e6e6'
        };
        var plotOpt = {};
        if (opt !== undefined) {
            if (opt.hasOwnProperty('legend')) {
                Object.assign(options.legend, opt.legend);
            }
        }
        
        if (container.hasClass('row1col0')) {
            if (options.legend.hasOwnProperty('enabled')) {
                options.legend.enabled = true;
            }
            decimal = 0;            
        }
        else if (container.hasClass('row1col1')) {            
            decimal = 0;
        }
        else if (container.hasClass('row1col2')) {
            decimal = 1;
        }
        else if (container.hasClass('row1col3')) {
            symbol = 'triangle';
            converted = true;
            decimal = 2;
        }
        else if (container.hasClass('row1col4')) {
            symbol = 'square';
            converted = true;
            decimal = 2;
        }
        else if (container.hasClass('row1col5')) {
            fillColor = '#403152';
            converted = true;
            decimal = 2;
        }
        else if (container.hasClass('row5col2')) {
            options.gridLineColor = 'transparent';
            decimal = 1;
            plotOpt['line'] = {
                dataLabels: {
                    enabled: true,
                    format: '{y:.2f}',
                    style: {
                        fontWeight: 'normal',
                        fontSize: '9px'
                    },
                    overflow: 'none',
                    crop: false
                }
            }
        }
        else if (container.hasClass('row5col3')) {
            options.gridLineColor = 'transparent';
            decimal = 1;
            plotOpt['line'] = {
                dataLabels: {
                    enabled: true,
                    format: '{y:.2f}',
                    style: {
                        fontWeight: 'normal',
                        fontSize: '9px'
                    },
                    overflow: 'none',
                    crop: false
                }
            }
        }
        else if (container.hasClass('row5col4')) {
            options.gridLineColor = 'transparent';      
            decimal = 1;
            plotOpt['line'] = {
                dataLabels: {
                    enabled: true,
                    format: '{y:.2f}',
                    style: {
                        fontWeight: 'normal',
                        fontSize: '9px'
                    },
                    overflow: 'none',
                    crop: false
                }
            }
        }
        else if (container.hasClass('row15col4')) {
            options.gridLineColor = 'transparent';
            decimal = 1;
            plotOpt['line'] = {
                dataLabels: {
                    enabled: true,
                    format: '{y:.1f}',
                    style: {
                        fontWeight: 'normal',
                        fontSize: '9px'
                    },
                    overflow: 'none',
                    crop: false
                }
            }
        }
        else if (container.hasClass('row15col5')) {
            options.gridLineColor = 'transparent';
            decimal = 2;
        }

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
        var getMarkerConfig = function (j) {            
            var marker = {
                symbol: 'square',
                fillColor: '#ffc000',
                enabled: true,
                radius: 3
            };
            if (container.hasClass('row1col0')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    fillColor: '#fbf405',
                    symbol: 'square'
                };
            }
            else if (container.hasClass('row1col1')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'triangle',
                    fillColor: '#fbf405'
                };
            }
            else if (container.hasClass('row1col2')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'square',
                    fillColor: '#fbf405'
                };
            }
            else if (container.hasClass('row1col3')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'square',
                    fillColor: '#401352'
                };
            }
            else if (container.hasClass('row5col2')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'triangle',
                    fillColor: '#7e131b'
                };
            }
            else if (container.hasClass('row5col3')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'square',
                    fillColor: '#ffc000'
                };
            }
            else if (container.hasClass('row5col4')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'circle',
                    fillColor: '#048304'
                };
            }
            else if (container.hasClass('row15col4')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: 'triangle',
                    fillColor: '#ffc000'
                };
            }
            else if (container.hasClass('row15col5')) {
                marker = {
                    enabled: true,
                    radius: 3,
                    symbol: j == 0 ? 'triangle' : 'diamond',
                    fillColor: j == 0 ? '#f90406' : '#002060'
                };
            }

            return marker;
        }
        var series = [];
        for (var i in data.MultiaxisChart.Charts) {
            yAxes.push({
                gridLineWidth: container.hasClass('row15col4') ? 0 : 1,
                minorGridLineWidth: container.hasClass('row15col4') ? 0 : 1,
                minorGridLineWidth: container.hasClass('row15col4') ? 0 : 1,
                lineColor: container.hasClass('row15col4') ? 'transparent' : '#ccd6eb',
                visble:false,
                title: {
                    enabled: container.is('.row15col4') ? false : true,       
                    text: container.is('.row1col1, .row1col2, .row1col3, .row1col0') ? data.MultiaxisChart.Charts[i].Measurement : null,
                    align: 'high',
                    rotation: 0,
                    y: -10,
                    x: container.is('.row1col0') ? 15 : 0,
                    offset: 15,
                    style: {
                        fontSize: '9px',
                        fontWeight: 'bold'
                    }
                    //data.MultiaxisChart.Charts[i].ValueAxisTitle + ' (' + data.MultiaxisChart.Charts[i].Measurement + ')',
                },
                opposite: data.MultiaxisChart.Charts[i].IsOpposite,
                tickInterval: data.MultiaxisChart.Charts[i].FractionScale == 0 ? container.data('fraction') : data.MultiaxisChart.Charts[i].FractionScale,
                max: data.MultiaxisChart.Charts[i].MaxFractionScale == 0 ? null : data.MultiaxisChart.Charts[i].MaxFractionScale,
                labels: {
                    enabled: container.is('.row15col4') ? false : true,                    
                    style: {
                        fontSize: '7px',
                        color: container.is('.row5col3, .row5col4, .row5col2') ? '#fff' : '#666666'
                    },
                    formatter: function () {
                        var x = this.value;
                        if (decimal > 0) {
                            return x.format(decimal, '{point.y}');
                        } else {
                            return x.format();
                        }

                        //var x = this.value
                        //return x.format();
                    }
                },
                min: container.data('min') != '' && container.data('min') != null ? container.data('min') : null,
                lineWidth: 1,
                gridLineColor: options.gridLineColor,
            });
            if (chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType] === 'line') {
                //var symbol = i % 2 == 0 ? 'triangle' : 'square';
                plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]] = {
                    marker: {
                        enabled: true,
                        symbol: symbol,
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
                Object.assign(plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]], plotOpt[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]]);
            } else if (chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType] === 'area' && data.MultiaxisChart.Charts[i].SeriesType === 'multi-stack') {
                plotOptions[chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType]] = {
                    stacking: 'normal',
                    lineColor: '#666666',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#666666',
                        enabled: true,
                        symbol: 'triangle',
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
                    if (converted === false) {
                        seriesNames.push(data.MultiaxisChart.Charts[i].Series[j].name);
                    } else {
                        data.MultiaxisChart.Charts[i].Series[j].showInLegend = false;
                    }

                } else {
                    data.MultiaxisChart.Charts[i].Series[j].showInLegend = false;
                }
                data.MultiaxisChart.Charts[i].Series[j].type = chartTypeMap[data.MultiaxisChart.Charts[i].GraphicType];
                //data.MultiaxisChart.Charts[i].Series[j].marker = {
                //    enabled: true,
                //    radius: 3,
                //    symbol: getSymbol(container, j),
                //    fillColor: fillColor
                //};
                data.MultiaxisChart.Charts[i].Series[j].marker = getMarkerConfig(j);
                if (data.MultiaxisChart.Charts[i].Series[j].type != 'spline' && data.MultiaxisChart.Charts[i].SeriesType == 'single-stack') {
                    data.MultiaxisChart.Charts[i].Series[j].stack = data.MultiaxisChart.Charts[i].Series[j].name;
                }
                data.MultiaxisChart.Charts[i].Series[j].yAxis = parseInt(i);// + 1;
                data.MultiaxisChart.Charts[i].Series[j].tooltip = {
                    valueDecimals: decimal,
                    valueSuffix: ' ' + data.MultiaxisChart.Charts[i].Measurement
                }
                //data.MultiaxisChart.Charts[i].Series[j].dataLabels = { enabled: showLegends, style: { "fontSize": "9px", "fontWeight": "normal" }, verticalAlign: "bottom" };
                series.push(data.MultiaxisChart.Charts[i].Series[j]);
            }
        }
        container.highcharts({
            chart: {
                zoomType: 'xy',
                alignTicks: false,
                backgroundColor: 'transparent',
                height: container.is('.row15col4, .row15col5') ? 140 : 200,
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
                    fontSize: '7px'
                },

            },
            legend: options.legend,
            series: series,
            
        });
    };
    Der.Artifact.jccMonthlyTrend = function (data, container) {
        var options = {}
        options.legend = {
            layout: 'horizontal',
            align: 'left',
            x: 30,
            verticalAlign: 'bottom',
            y: -15,
            floating: true,
            enabled: true,
        };
        Der.Artifact.multiaxis(data, container, options, 'jcc-monthly-trend');
        var chart = $('.row15col5').highcharts();
        if (chart.yAxis.length > 1) {
            chart.yAxis[1].update({
                labels: {
                    enabled: false
                },
                title: {
                    text: null
                }
            });
        }        
    };
    Der.Artifact.speedometer = function (data, container) {
        //console.log(data.SpeedometerChart);
        var $this = container;
        $this.append('<h5>MCHE Rundown</h5>');
        var $canvas = $('<canvas />');
        $canvas.css({
            width: '100px',
            height: '40px'
        });
        $canvas.attr('id', 'speedometer_rainbow');
        $this.append($canvas);
        var canvas = new fabric.Canvas('speedometer_rainbow');
        canvas.setHeight(40);
        canvas.setWidth(100);
        canvas.selection = false;
        var thickness = 14;
        function drawMultiRadiantCircle(xc, yc, r, plotBands) {
            //var partLength = (Math.PI) / (radientColors.length - 1);
            var start = Math.PI;
            var gradient = null;
            var startColor = null,
                endColor = null;
            var centerPoint;
            //console.log('speedo bands',plotBands);
            for (var i = 0; i < plotBands.length - 1; i++) {
                startColor = plotBands[i].color;
                endColor = plotBands[(i + 1)].color;
                var partLength = (plotBands[i + 1].from - plotBands[i].from) / plotBands[plotBands.length - 1].from * Math.PI;
                // x start / end of the next arc to draw
                var xStart = Math.cos(start) * r;
                var xEnd = Math.cos(start + partLength) * r;
                // y start / end of the next arc to draw
                var yStart = Math.sin(start) * r;
                var yEnd = Math.sin(start + partLength) * r;

                var circle = new fabric.Circle({
                    radius: r,
                    left: 18,
                    top: 7,
                    angle: 0,
                    startAngle: start,
                    endAngle: start + partLength,
                    stroke: plotBands[i].color,
                    strokeWidth: thickness,
                    fill: ''
                });
                circle.setGradient('stroke', {
                    type: 'linear',
                    x1: xStart,
                    y1: yStart,
                    x2: xEnd,
                    y2: yEnd,
                    colorStops: {
                        0: startColor,
                        1: endColor
                    }
                });
                circle.set('selectable', false)
                canvas.add(circle);
                //ctx.beginPath();

                //gradient = ctx.createLinearGradient(xStart, yStart, xEnd, yEnd);
                //gradient.addColorStop(0, startColor);
                //gradient.addColorStop(1.0, endColor);

                //ctx.strokeStyle = gradient;
                //ctx.arc(xc, yc, r, start, start + partLength);
                //ctx.lineWidth = start + partLength;
                //ctx.stroke();
                //ctx.closePath();
                centerPoint = circle.getCenterPoint();
                start += partLength;
            }

            //var point = Math.PI + (data.SpeedometerChart.Series.data[0] * Math.PI / plotBands[plotBands.length - 1].from);
            var jarum = data.SpeedometerChart.Series.data[0] > 100 ? 99.99 : data.SpeedometerChart.Series.data[0];
            var point = Math.PI + (jarum * Math.PI / plotBands[plotBands.length - 1].from);
            var relateiveR = r - thickness / 2;
            var relativeR2 = r + thickness / 2;
            var xPoint = centerPoint.x + Math.cos(point) * (relateiveR);
            var yPoint = centerPoint.y + Math.sin(point) * (relateiveR);

            var M = 'M ' + xPoint + ' ' + yPoint;
            var L1 = ' L ' + (centerPoint.x + Math.cos(point * 0.98) * (relativeR2)) + ' ' + (centerPoint.y + Math.sin(point * 0.98) * (relativeR2));
            var L2 = ' L ' + (centerPoint.x + Math.cos(point * 1.02) * (relativeR2)) + ' ' + (centerPoint.y + Math.sin(point * 1.02) * (relativeR2));
            var path = new fabric.Path(M + L1 + L2 + ' z');
            //console.log(centerPoint);
            //console.log(M + L1 + L2 + ' z');
            path.set({ fill: 'black' });
            canvas.add(path);
            //var triangle = new fabric.Triangle({
            //    width: 5, height: 14, fill: 'black', left: xPoint, top: yPoint, angle : 180
            //});

            //canvas.add(triangle);
            //var point = Math.PI + (data.SpeedometerChart.Series.data[0] * Math.PI);

            //// the triangle
            //ctx.beginPath();
            //ctx.moveTo(xc + Math.cos(point) * (r - thickness / 2), yc + Math.sin(point) * (r - thickness / 2));
            //ctx.lineTo(xc + Math.cos(point * 0.97) * (r + thickness / 2), yc + Math.sin(point * 0.97) * (r + thickness / 2));
            //ctx.lineTo(xc + Math.cos(point * 1.03) * (r + thickness / 2), yc + Math.sin(point * 1.03) * (r + thickness / 2));
            //ctx.closePath();

            //// the fill color
            //ctx.fillStyle = "#000";
            //ctx.fill();
        }
        drawMultiRadiantCircle(50, 35, 23, data.SpeedometerChart.PlotBands);

        //var zero = new fabric.Text('0%', { left: 7, top: canvas.getHeight() - 10, fontSize: 8, fontWeight:'bold' });
        //canvas.add(zero);
        //var helf = new fabric.Text('50%', { left: canvas.getWidth() / 2 - 5, top: 0, fontSize: 8, fontWeight: 'bold' });
        //canvas.add(helf);
        //var full = new fabric.Text('100%', { left: canvas.getWidth() - 20, top: canvas.getHeight() - 10, fontSize: 8, fontWeight: 'bold' });
        //canvas.add(full);
        //ctx.font = "30px serif";
        //ctx.fillText("0%", 0, canvas.height - 20);
        //ctx.fillText("100%", canvas.width - 45, canvas.height - 20);
        //ctx.fillText("50%", canvas.width / 2 - 20, 20);

        $this.append('<span class="value"><strong>' + data.SpeedometerChart.LabelSeries.value.format(2) + ' ' + data.SpeedometerChart.LabelSeries.name + '</strong></span>');
        var $image = $('<img />');
        $image.attr('src', canvas.toDataURL({
            format: 'png',
            quality: 1
        }));
        $this.find('.canvas-container').html($image);
        $this.find('.canvas-container').append('<span class="zero-val">0%</span>');
        $this.find('.canvas-container').append('<span class="half-val">50%</span>');
        $this.find('.canvas-container').append('<span class="full-val">100%</span>');
    };
    Der.Artifact.altspeedometer = function (data, container) {
        var $this = container;
        $this.append('<h5>MCHE Rundown</h5>');
        var $canvas = $('<canvas />');
        $canvas.css({
            width: '100px',
            height: '40px'
        });
        $this.append($canvas);
        var canvas = $this.find("canvas")[0];
        var ctx = canvas.getContext("2d");
        var thickness = 50;
        function drawMultiRadiantCircle(xc, yc, r, plotBands) {
            //var partLength = (Math.PI) / (radientColors.length - 1);
            var start = Math.PI;
            var gradient = null;
            var startColor = null,
                endColor = null;

            for (var i = 0; i < plotBands.length - 1; i++) {
                startColor = plotBands[i].color;
                endColor = plotBands[(i + 1)].color;
                var partLength = plotBands[i].to / plotBands[plotBands.length - 1].to * Math.PI;
                // x start / end of the next arc to draw
                var xStart = xc + Math.cos(start) * r;
                var xEnd = xc + Math.cos(start + partLength) * r;
                // y start / end of the next arc to draw
                var yStart = yc + Math.sin(start) * r;
                var yEnd = yc + Math.sin(start + partLength) * r;

                ctx.beginPath();

                gradient = ctx.createLinearGradient(xStart, yStart, xEnd, yEnd);
                gradient.addColorStop(0, startColor);
                gradient.addColorStop(1.0, endColor);

                ctx.strokeStyle = gradient;
                ctx.arc(xc, yc, r, start, start + partLength);
                ctx.lineWidth = thickness;
                ctx.stroke();
                ctx.closePath();

                start += partLength;
            }

            var point = Math.PI + (data.SpeedometerChart.Series.data[0] * Math.PI / plotBands[plotBands.length - 1].to);

            //console.log(xc + Math.cos(point) * (r - thickness / 2), yc + Math.sin(point) * (r - thickness / 2));
            // the triangle
            ctx.beginPath();
            ctx.moveTo(xc + Math.cos(point) * (r - thickness / 2), yc + Math.sin(point) * (r - thickness / 2));
            ctx.lineTo(xc + Math.cos(point * 0.97) * (r + thickness / 2), yc + Math.sin(point * 0.97) * (r + thickness / 2));
            ctx.lineTo(xc + Math.cos(point * 1.03) * (r + thickness / 2), yc + Math.sin(point * 1.03) * (r + thickness / 2));
            ctx.closePath();

            // the fill color
            ctx.fillStyle = "#000";
            ctx.fill();
        }
        drawMultiRadiantCircle(canvas.width / 2, canvas.height - 10, canvas.height - 40 - thickness / 2, data.SpeedometerChart.PlotBands);
        ctx.font = "30px serif";
        ctx.fillText("0%", 0, canvas.height - 20);
        ctx.fillText("100%", canvas.width - 45, canvas.height - 20);
        ctx.fillText("50%", canvas.width / 2 - 20, 20);

    };
    Der.Artifact.barmeter = function (data, container) {
        var $wrapper = $('<div />');
        $wrapper.addClass('barmeter-wrapper');
        var $this = container;
        var $canvas = $('<canvas />');
        var $label = $this.find('label').clone();
        if ($label.length) {
            $this.find('label').css('display', 'none');
            if ($this.data('break')) {
                $label.append('<div style="margin-left:15px; text-align:right;">' + data['SpeedometerChart'].Series.data[0].format(2) + '</div>');
            } else {
                $label.append('<span style="margin-left:15px; text-align:right;">' + data['SpeedometerChart'].Series.data[0].format(2) + '</span>');
            }
            $wrapper.append($label);
        } else {
            $wrapper.append('<label>' + data['SpeedometerChart'].Series.data[0].format(2) + '</label>');
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
            var last = config.PlotBands.length - 1;
            for (var i in config.PlotBands) {
                gradient.addColorStop(config.PlotBands[i].from / config.PlotBands[last].to, config.PlotBands[i].color);
            }
            ctx.fillStyle = gradient;
            ctx.fillRect(3, 0, canvas.width - 3, canvas.height);
            ctx.fillStyle = "rgb(0,0,0)";

            /*console.log(container[0]);
            console.log($label.html());
            console.log(config.PlotBands);
            console.log(last);*/
            var point = config.Series.data[0] / config.PlotBands[last].to * (canvas.width - 6) + 3;
            var basePoint = 6;
            var maxPoint = config.PlotBands[last].to * (canvas.width - 6) + 3
            if (point - 3 < 0) {
                point = basePoint;
            } else if (point > maxPoint) {
                point = maxPoint;
            }
            ctx.fillRect(point - 3, 0, 6, canvas.height - 30);


            // the triangle
            ctx.beginPath();
            ctx.moveTo(point - 3, canvas.height - 30);
            ctx.lineTo(point, canvas.height);
            ctx.lineTo(point + 3, canvas.height - 30);
            ctx.closePath();
            ctx.fillStyle = "rgb(0,0,0)";
            ctx.fill();
        }
        var $image = $('<img />');
        $image.attr('src', $this.find('canvas')[0].toDataURL());
        $image.width($this.find('canvas').width());
        $image.height($this.find('canvas').height());
        $this.find('canvas').replaceWith($image);
    }
    Der.Artifact.termometer = function (data, container) {
        var $this = container;
        $this.append('<span style="top:' + (100 - data.Value + 5) + '%' + '" class="termo-label">' + data.Value  + '</span>');
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
        var $image = $('<img />');
        $image.attr('src', $this.find('canvas')[0].toDataURL());
        $image.width($this.find('canvas').width());
        $image.height($this.find('canvas').height());
        $this.find('canvas').replaceWith($image);
        if (!$this.parent().hasClass('preview')) {
            $this.parent().css({ 'display': 'none' })
        }
    }
    Der.Artifact.pie = function (data, container) {
        var $title = $('<div />');
        $title.addClass('pie-title');
        $title.html(data.Pie.Title);

        var maxSeries;
        var values = [];
        //var displayName = {
        //    'LNG/Feed yield': 'LNG',
        //    'CDS/Feed Yield': 'CDS',
        //    'Fuel Gas & Ref Yield': 'Fuel-Refgr',
        //    'Flare Loss' : 'Loss'
        //}
        for (var i in data.Pie.SeriesResponses) {

            if (parseFloat(data.Pie.SeriesResponses[i].y) < 0) {
                continue;
            }
            values.push(parseFloat(data.Pie.SeriesResponses[i].y));
        }
        var maxValue = Math.max.apply(null, values);;
        var minValue = Math.min.apply(null, values);;
        for (var i in data.Pie.SeriesResponses) {
            if (parseFloat(data.Pie.SeriesResponses[i].y) != maxValue) {
                data.Pie.SeriesResponses[i].sliced = true;
                data.Pie.SeriesResponses[i].selected = true;
            };
            //if (parseFloat(data.Pie.SeriesResponses[i].y) == minValue) {
            //    //data.Pie.SeriesResponses[i].sliced = true;
            //    data.Pie.SeriesResponses[i].selected = true;
            //}
            //data.Pie.SeriesResponses[i].name = displayName[data.Pie.SeriesResponses[i].name.trim()];
        }
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
                height: 250
            },
            title: {
                text: data.Pie.Title,
                style: {
                    fontSize: '11px',
                    fontWeight: 'bold',
                    display: 'none'
                },
            },
            subtitle: {
                text: data.Pie.Subtitle,
                style: {
                    color: '#fff',
                    display: 'none'
                },
            },
            credits: {
                enabled: false
            },
            plotOptions: {
                pie: {
                    //size: '70%',
                    //slicedOffset: 30,
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        distance: 20,
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
                    animation: false
                }
            },
            legend: {
                enabled: false,
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
    Der.Helper.DrawInlineSVG = function (container, rawSVG) {
        var id = container.attr('id');
        var width = container.width();
        var height = container.height();
        var newCanvas = $('<canvas/>', { 'id': id })
            .width(width)
            .height(height);
        container.replaceWith(newCanvas);
        canvg(document.getElementById(id), rawSVG, {
            renderCallback: function () {
                var $image = $('<img />');
                $image.attr('src', document.getElementById(id).toDataURL());
                $image.width(width);
                $image.height(height);
                $('#'+id).replaceWith($image);
            }
        })

        //var ctx = document.getElementById(id).getContext("2d");
        ////console.log(rawSVG);
        //var svg = new Blob([rawSVG], { type: "image/svg+xml;charset=utf-8" }),
        //    domURL = self.URL || self.webkitURL || self,
        //    url = domURL.createObjectURL(svg),
        //    img = new Image;

        //img.onload = function () {
        //    ctx.drawImage(this, 0, 0);
        //    domURL.revokeObjectURL(url);
        //    callback(this);
        //};
        ////console.log(url);
        //img.src = url;
    };
    Der.Artifact.tank = function (data, container) {
        //console.log(data);
        if (container.data('type') == 'custom') {
            container.tank(data.Tank, {
                height: container.height(),
                width: container.width()
            });
        } else {
            FusionCharts.ready(function () {
                var fusioncharts = new FusionCharts({
                    type: 'cylinder',
                    dataFormat: 'json',
                    id: 'chart-' + container.attr('id'),
                    renderAt: container.attr('id'),
                    width: container.width(),
                    height: container.height(),
                    dataSource: {
                        "chart": {
                            //"caption" : "Test",
                            "manageresize": "1",
                            "bgcolor": "dbeef4",
                            "bgalpha": "100",
                            "showborder": "0",
                            "lowerlimit": "0",
                            "upperlimit": "100",
                            "showtickmarks": "0",
                            "showtickvalues": "0",
                            "showlimits": "0",
                            "numbersuffix": "%",
                            "decmials": "0",
                            "cylfillcolor": data.Tank.Color.replace('#',''),
                            "basefontcolor": "000",
                            "chartLeftMargin": "1",
                            "chartTopMargin": "1",
                            "chartRightMargin": "1",
                            "chartBottomMargin": "1",
                        },
                        "value": Math.round(data.Tank.VolumeInventory),
                        //"annotations": {
                        //    "groups": [
                        //        {
                        //            "showbelow": "1",
                        //            "items": [
                        //                {
                        //                    "type": "rectangle",
                        //                    "x": "$chartStartX+1",
                        //                    "y": "$chartStartY+1",
                        //                    "tox": "$chartEndX-1",
                        //                    "toy": "$chartEndY-1",
                        //                    "color": "dbeef4",
                        //                    "alpha": "100",
                        //                    "showborder": "0",
                        //                    "bordercolor": "dbeef4",
                        //                    "borderthickness": "0",
                        //                    "radius": "0"
                        //                }
                        //            ]
                        //        }
                        //    ]
                        //}

                    }
                }
            );
                fusioncharts.render();
            });
        }
        //console.log(data);
    };
    //Der.Artifact.tank = function (data, container) {
    //    container.tank(data.Tank, {
    //        height: container.height(),
    //        width: container.width()
    //    });
    //};
    window.Der = Der;
}(window, jQuery));