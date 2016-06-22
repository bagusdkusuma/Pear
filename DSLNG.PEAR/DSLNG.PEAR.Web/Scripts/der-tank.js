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
        

        // variable Tank Chart

        var svgWidth = dimension.width
        var svgHeight = dimension.height
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

        var ellipseTop = s.ellipse(ellipseX, topTank, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: 'transparent',
            strokeWidth: 1
        });
        s.text(ellipseX - 25, topTank, title);
        var ellipseBottom = s.ellipse(ellipseX, bottomTank, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: 'grey',
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

        var heightFill = percentFill * tankHeight /100;
        var yFill = svgHeight - heightFill -( ellipseRY / 2 + marginBottom);
        //console.log(svgHeight - topTank - (ellipseRY / 2 + marginBottom));
        //console.log(options);
        var rect = s.rect(0, yFill, svgWidth, heightFill).attr({
            fill: 'aqua',
            stroke: 'aqua',
            strokeWidth: 0,
        });

        var borderEclipse = s.ellipse(svgWidth/2, yFill, ellipseRX, ellipseRY).attr({
            stroke: 'grey',
            fill: 'aqua',
            strokeWidth: 1,
            strokeDasharray: 1
        });

        if (percentFill != 0) {
            var ellipseBottomFilled = s.ellipse(ellipseX, bottomTank, ellipseRX, ellipseRY).attr({
                stroke: 'aqua',
                fill: 'aqua',
                strokeWidth: 0
            });
            s.text(svgWidth / 2 - 10, yFill, percentFill + '%');
        } else {
            s.text(svgWidth / 2 - 10, bottomTank, percentFill + '%');
        }

        
        return this;
    };
})(jQuery);

