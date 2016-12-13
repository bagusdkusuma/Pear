(function ($) {
    $.fn.tank = function (options, dimension) {
        //console.log(options);
        var id = "tank_" + options.Id + Date.now();
        
        this.html('<svg class="svg" id="' + id + '" style="margin:auto;display:block"></svg>');
        
        var s = Snap('#' + id).attr({
            width: dimension.width,
            height: dimension.height
        });;

        var title = options.Title;
        var subtitle = options.Subtitle;
        var minCapacity = options.MinCapacity;
        var maxCapacity = options.MaxCapacity;
        var volumeInventory = options.VolumeInventory;
        var volumeInventoryUnit = options.VolumeInventoryUnit;
        var daysToTankTop = options.DaysToTankTop;
        var tankColor = '#2197eb';
        
        this.append(title);
        // variable Tank Chart

        var svgWidth = dimension.width;
        var svgHeight = dimension.height;
        var marginTop = 10;
        var marginBottom = 10;

        var percentFill = Math.round((volumeInventory / maxCapacity) * 100);
        var percentMin = Math.round((minCapacity / maxCapacity) * 100);

        //var tankHeight = svgHeight;
        //var tankWidth = svgWidth

        var ellipseRY = 14;
        var ellipseRX = svgWidth / 2;
        var tankFullHeight = tankHeight - (ellipseRY * 3);
        var roundMaxY = tankHeight - tankFullHeight;


        var lineMaxColor = 'red';
        var lineMinColor = 'green';

        // function Tank Chart

        var roundBottomY = tankHeight- (ellipseRY/2) - 8;

        var fillHeight = (tankFullHeight / 100 * percentFill);
        var minHeight = (tankFullHeight / 100 * percentMin);

        var roundFillY = roundMaxY + (tankFullHeight - fillHeight);
        var lineMinY = roundMaxY + (tankFullHeight - minHeight);
        var lineMaxY = roundMaxY;


        function calEllipseX(a, b) {
            var x;
            x = (a / 2) + b;
            return (x);
        };

        var ellipseX = calEllipseX(svgWidth, 0);

        var topTank = ellipseRY / 2 + marginTop;
        var bottomTank = svgHeight - (ellipseRY / 2 + marginBottom);

        var tankHeight = bottomTank - topTank;

        var heightFill = percentFill * tankHeight / 100;
        var yFill = svgHeight - heightFill - (ellipseRY / 2 + marginBottom);
        
        if ($(this).data('type') == 'custom') {
            percentFill = Math.round((daysToTankTop / maxCapacity) * 100);
            var $volume = $('<div />');
            $volume.addClass('volume');
            $volume.html(Math.round(volumeInventory.format(2)) + ' ' + volumeInventoryUnit);
            $(this).append($volume);
            ellipseRX = 14;
            ellipseRY = svgHeight / 2;
            var marginLeft = marginTop;
            var marginRight = marginBottom;
            var ellipseY = calEllipseX(svgHeight, 0);
            var leftTank = ellipseRX / 2 + marginLeft;
            var rightTank = svgWidth - (ellipseRX / 2 + marginRight);
            var tankWidth = rightTank - leftTank;
            var footWidth = 10;
            var chartHeight = svgHeight - 10;
            var ellipseLeft = s.ellipse(leftTank, ellipseY, ellipseRX, ellipseRY).attr({
                fill: '#D7D7D7',
                stroke: 'grey',
                strokeWidth: 1
            });
            var ellipseRight = s.ellipse(rightTank, ellipseY, ellipseRX, ellipseRY).attr({
                fill: '#D7D7D7',
                stroke: 'grey',
                strokeWidth: 1
            });
            var rect = s.rect(leftTank, 0, tankWidth, svgHeight).attr({
                fill: '#D7D7D7',
                strokeWidth: 0,
            });

            var topBorder = s.line(leftTank, 0, rightTank, 0).attr({
                fill: 'none',
                stroke: 'gray',
                strokeWidth: 1,
            });
            var topBorder = s.line(leftTank, svgHeight, rightTank, svgHeight).attr({
                fill: 'none',
                stroke: 'gray',
                strokeWidth: 1,
            });
            var leftFoot = s.line(leftTank, svgHeight, leftTank+ footWidth, svgHeight).attr({
                fill: 'none',
                stroke: 'transparent',
                strokeWidth: 4,
            });
            var rightFoot = s.line(rightTank - footWidth, svgHeight, rightTank, svgHeight).attr({
                fill: 'none',
                stroke: 'transparent',
                strokeWidth: 4,
            });
            //var leftVertical = s.line(leftTank + footWidth, 0, leftTank + footWidth, svgHeight).attr({
            //    fill: 'none',
            //    stroke: 'gray',
            //    strokeWidth: 1,
            //});
            var rightVertical = s.line(rightTank - footWidth, 0, rightTank - footWidth, svgHeight).attr({
                fill: 'none',
                stroke: 'gray',
                strokeWidth: 1,
            });
            var rect = s.rect(rightTank - footWidth - 20, 5, 10, chartHeight).attr({
                fill: 'white',
                stroke: 'white',
                strokeWidth: 0,
            });
            var filledChart = chartHeight * percentFill / 100;
            var yStart = chartHeight - filledChart + 5;
            var rect = s.rect(rightTank - footWidth - 20, 5, 10, chartHeight).attr({
                fill: 'white',
                stroke: 'white',
                strokeWidth: 0,
            });
            var rect = s.rect(rightTank - footWidth - 20, yStart, 10, filledChart).attr({
                fill: '#00FF00',
                stroke: '#00FF00',
                strokeWidth: 0,
            });
            s.text(rightTank - footWidth -50, svgHeight / 2, percentFill + '%');
            return this;
        }

        if (title.toLowerCase().indexOf('cds') > -1) {
            tankColor = 'lawngreen';
        }
        
        //s.text(ellipseX - 25, topTank, title);
        var ellipseBottom = s.ellipse(ellipseX, bottomTank, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: 'grey',
            strokeWidth: 1
        });

        if (percentFill != 0) {
            var ellipseBottomFilled = s.ellipse(ellipseX, bottomTank, ellipseRX, ellipseRY).attr({
                stroke: 'gray',
                fill: tankColor,
                strokeWidth: 1
            });
        }
        

        var rect = s.rect(0, yFill, svgWidth, heightFill).attr({
            fill: tankColor,
            stroke: tankColor,
            strokeWidth: 0,
        });

        var borderEclipse = s.ellipse(svgWidth / 2, yFill, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: tankColor,
            strokeWidth: 1,
            strokeDasharray: 1
        });

        var ellipseTop = s.ellipse(ellipseX, topTank, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: 'none',
            strokeWidth: 1
        });

        var leftBorder = s.line(0, topTank, 0, bottomTank).attr({
            fill: 'none',
            stroke: 'gray',
            strokeWidth: 1,
        });
        var rightBorder = s.line(svgWidth, topTank, svgWidth, bottomTank).attr({
            fill: 'none',
            stroke: 'gray',
            strokeWidth: 1,
        });

        if (percentFill != 0) {
            //var ellipseBottomFilled = s.ellipse(ellipseX, bottomTank, ellipseRX, ellipseRY).attr({
            //    stroke: 'gray',
            //    fill: tankColor,
            //    strokeWidth: 1
            //});
            s.text(svgWidth / 2 - 10, yFill, percentFill + '%');
        } else {
            s.text(svgWidth / 2 - 10, bottomTank, percentFill + '%');
        }

        
        return this;
    };
})(jQuery);

