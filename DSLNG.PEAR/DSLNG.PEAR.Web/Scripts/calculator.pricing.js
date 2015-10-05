(function (window, $, undefined) {
    var pear = window.Pear;
    pear.Pricing = {};
    pear.Pricing.Init = function () {
        console.log('calculator-init');
        $('#MainInput').change(function (e) {
            e.preventDefault();
        });
    };
}(window, jQuery));
    
