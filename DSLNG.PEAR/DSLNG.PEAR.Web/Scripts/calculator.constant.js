(function (window, $, undefined) {
    var pear = window.Pear;
    pear.CalculatorConstant = {};
    pear.CalculatorConstant.Init = function () {
        var url = $('#calculator-data').attr('data-changeConstantUrl');
        $('.calculator-constant').change(function () {
            var that = $(this);
            var data = {};
            data.id = that.siblings().val();
            data.value = that.val();            
            $.ajax({
                type: 'POST',
                url: url,
                data: data,
                success:function(res) {
                    if (res.IsSuccess) {
                        var msg = $('<span style="color:#ffffff">' + res.Message + '</span>');
                        that.parent().append(msg);
                        $(msg).delay(1500).fadeOut(function () {
                            $(this).remove();
                        });
                    }
                }
            });
        });
    };
}(window, jQuery));