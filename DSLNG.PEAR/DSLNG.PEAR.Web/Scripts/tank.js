(function ($) {
    $.fn.tank = function (options) {
        console.log(options);
        var id = "tank_" + options.Id + Date.now();
        
        this.html('<svg class="svg" id="' + id + '" style="margin:auto;display:block"></svg>');

        var s = Snap('#' + id).attr({
            width: 498,
            height: 290
        });;

        // variable Donggi
        var title = options.Title;
        var subtitle = options.Subtitle;
        var minCapacity = options.MinCapacity;
        var maxCapacity = options.MaxCapacity;
        var volumeInventory = options.VolumeInventory;
        var volumeInventoryUnit = options.VolumeInventoryUnit;
        var daysToTankTop = options.DaysToTankTop;
        var daysToTankTopUnit = options.DaysToTankTopUnit;
        var daysToTankTopTitle = options.DaysToTankTopTitle;

        // var MinCapacity = 7000;
        // var MaxCapacity = 180000;
        // var VolumeInventory = 43976.98;


        // variable Tank Chart

        var svgWidth = 498;
        var svgHeight = 290;

        var percentFill = Math.round((volumeInventory / maxCapacity) * 100);
        var percentMin = Math.round((minCapacity / maxCapacity) * 100);

        var tankHeight = 170;
        var tankWidth = 140;

        // var marginSide = 166;
        var marginSide = (svgWidth - tankWidth) / 2;
        var marginTop = 90;

        var ellipseRY = 14;
        var ellipseRX = tankWidth / 2;
        var tankFullHeight = tankHeight - (ellipseRY * 3);
        var roundMaxY = tankHeight - tankFullHeight;

        var softBlue = options.Color !== undefined ? options.Color : '#3949AB';
        var darkBlue = '#283593';
        var greyBorder = '#BDBDBD';
        var red = '#e43834';
        var yellow = '#fcd734';
        var green = '#429f46';
        var red = "#FF0000";

        var lineMaxColor = greyBorder;
        var lineMinColor = green;

        // function Tank Chart

        var roundBottomY = marginTop + tankHeight;

        var fillHeight = (tankFullHeight / 100 * percentFill);
        var minHeight = (tankFullHeight / 100 * percentMin);

        var roundFillY = marginTop + roundMaxY + (tankFullHeight - fillHeight);
        var lineMinY = marginTop + roundMaxY + (tankFullHeight - minHeight);
        var lineMaxY = marginTop + roundMaxY;


        function calEllipseX(a, b) {
            var x;
            x = (a / 2) + b;
            return (x);
        };

        var ellipseX = calEllipseX(tankWidth, marginSide);


        // Shape

        var tank = s.rect(marginSide, marginTop, tankWidth, tankHeight).attr({
            fill: '#fff',
            stroke: greyBorder,
            strokeWidth: 2
        });


        var roundTop = s.ellipse(ellipseX, marginTop, ellipseRX, ellipseRY).attr({
            fill: '#fff',
            stroke: greyBorder,
            strokeWidth: 2
        });
        var roundBottom = s.ellipse(ellipseX, roundBottomY, ellipseRX, ellipseRY).attr({
            fill: '#fff',
            stroke: greyBorder,
            strokeWidth: 2
        });

        var xx = s.rect(marginSide, roundFillY, tankWidth, fillHeight).attr({
            fill: darkBlue,
            stroke: softBlue,
            strokeWidth: 2
        });
        var roundFillMin = s.ellipse(ellipseX, roundBottomY, ellipseRX, ellipseRY).attr({
            fill: darkBlue,
            stroke: softBlue,
            strokeWidth: 2,
            // strokeDasharray: 2
        });

        var roundFill = s.ellipse(ellipseX, roundFillY, ellipseRX, ellipseRY).attr({
            fill: softBlue,
            stroke: softBlue,
            strokeWidth: 2
        });

        var roundMax = s.ellipse(ellipseX, marginTop + roundMaxY, ellipseRX, ellipseRY).attr({
            fill: 'transparent',
            stroke: greyBorder,
            strokeWidth: 2,
            strokeDasharray: 2
        });

        var roundMin = s.ellipse(ellipseX, lineMinY, ellipseRX, ellipseRY).attr({
            fill: 'transparent',
            stroke: green,
            strokeWidth: 2,
            strokeDasharray: 2
        });

        // Meteran

        var rightLineX = marginSide + tankWidth + 16;

        var leftLineX = marginSide - 16;

        var leftLine = s.line(leftLineX, marginTop + roundMaxY, leftLineX, roundBottomY).attr({
            fill: 'none',
            stroke: greyBorder,
            strokeWidth: 2,
            strokeDasharray: 2
        });

        //////////

        var lineMax = s.line(leftLineX - 4, lineMaxY, leftLineX + 4, lineMaxY).attr({
            stroke: lineMaxColor,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var tMax = s.text(leftLineX - 14, marginTop + roundMaxY + 4, [maxCapacity.format(2), " ", volumeInventoryUnit, " (Max)"]).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#444",
            textAnchor: "end",
        });

        //////////

        var lineMin = s.line(leftLineX - 4, lineMinY, leftLineX + 4, lineMinY).attr({
            stroke: lineMinColor,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var tMin = s.text(leftLineX - 14, lineMinY + 4, [minCapacity.format(2), " ", volumeInventoryUnit, " (Min)"]).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#444",
            textAnchor: "end",
        });

        //////////

        var lineZero = s.line(leftLineX - 4, roundBottomY, leftLineX + 4, roundBottomY).attr({
            stroke: red,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var tZero = s.text(leftLineX, roundBottomY + 20, ["0", " ", volumeInventoryUnit]).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#444",
            textAnchor: "middle",
        });

        //////////

        var rightLine = s.line(rightLineX, marginTop + roundMaxY, rightLineX, roundBottomY).attr({
            fill: 'none',
            stroke: greyBorder,
            strokeWidth: 2,
            strokeDasharray: 2
        });
        var lineMax2 = s.line(rightLineX - 4, lineMaxY, rightLineX + 4, lineMaxY).attr({
            stroke: lineMaxColor,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var lineMin2 = s.line(rightLineX - 4, lineMinY, rightLineX + 4, lineMinY).attr({
            stroke: lineMinColor,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var lineZero = s.line(rightLineX - 4, roundBottomY, rightLineX + 4, roundBottomY).attr({
            stroke: red,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });

        var lineFill = s.line(rightLineX - 4, roundFillY, rightLineX + 4, roundFillY).attr({
            stroke: darkBlue,
            strokeWidth: 3,
            strokeLinecap: "round",
            strokeLinejoin: "round"
        });
        var tFill = s.text(rightLineX + 14, roundFillY + 4, [volumeInventory.format(2), " ", volumeInventoryUnit, " (", percentFill, "%)"]).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#444"
        });

        //////////

        var tDay = s.text(rightLineX, marginTop, [daysToTankTop, " ", daysToTankTopUnit]).attr({
            font: "16px Open Sans, sans-serif",
            fill: darkBlue,
            fontWeight: "bold",
            fontStyle: "italic"
        });
        var tDayKeterangan = s.text(rightLineX, marginTop + 20, daysToTankTopTitle).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#444",
            fontStyle: "italic"
        });

        var tTitle = s.text(svgWidth / 2, 18, title).attr({
            font: "16px Open Sans, sans-serif",
            fill: "#444",
            fontWeight: "bold",
            textAnchor: "middle",
        });

        var tSubtitle = s.text(svgWidth / 2, 44, subtitle).attr({
            font: "14px Open Sans, sans-serif",
            fill: "#666",
            textAnchor: "middle",
        });
        
        return this;
    };
})(jQuery);

