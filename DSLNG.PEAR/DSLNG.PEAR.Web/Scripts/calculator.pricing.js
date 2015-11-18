(function (window, $, undefined) {
    var pear = window.Pear;
    pear.PricingCalculator = {};
    pear.PricingCalculator.Init = function () {
        /*var constanta1 = parseFloat($('.pricing-constant-1').val());
        var constanta2 = parseFloat($('.pricing-constant-2').val());
        var constanta3 = parseFloat($('.pricing-constant-3').val());
        var constanta4 = parseFloat($('.pricing-constant-4').val());
        var constanta5 = parseFloat($('.pricing-constant-5').val());
        var constanta6 = parseFloat($('.pricing-constant-6').val());
        var constanta7 = parseFloat($('.pricing-constant-7').val());
        var senoroGasPrice = parseFloat($('.senoro-gsa-price').val());
        var matindokGasPrice = parseFloat($('.matindok-gsa-price').val());
        var jccFloor = parseFloat($('.jcc-floor').val());
        var slopeLngPrice = parseFloat($('.slope-lng-price').val());
        var coefficientOfBoilGas = parseFloat($('.coefficient-of-boil-gas').val() /100);
        var coefficientOfLngPlant = parseFloat($('.coefficient-of-lng-plant').val() /100);
        var gasSellersAboveJcc = parseFloat($('.gas-sellers-above-jcc').val() / 100);
        var senoroComposition = parseFloat($('.senoro-composition').val());
        var matindokComposition = parseFloat($('.matindok-composition').val());
        var priceVariable = parseFloat($('.price-variable').val());
        var priceConstanta = parseFloat($('.price-constanta').val());*/
        
        
        $('#JccPrice').keyup(function(e) {
            e.preventDefault();
            var val = parseFloat($('#JccPrice').val());
            changeLngSpaFob(val);
            changeLngSpaDes(val);
            changeSenoroFeedGas(val);
            changeMatindokFeedGas(val);
            changeAverageFeedGas(val);
            changeCdsPrice(val);
        });

        $('.pricing-constanta-wrapper input[type="text"]').change(function () {
            var val = parseFloat($('#JccPrice').val());
            if (val != 0) {
                changeLngSpaFob(val);
                changeLngSpaDes(val);
                changeSenoroFeedGas(val);
                changeMatindokFeedGas(val);
                changeAverageFeedGas(val);
                changeCdsPrice(val);
            }
        });

        var changeLngSpaFob = function (val) {
            var constanta1 = parseFloat($('.pricing-constant-1').val());
            var constanta2 = parseFloat($('.pricing-constant-2').val());
            var constanta3 = parseFloat($('.pricing-constant-3').val());
            var result = constanta1 * val + (constanta2 * val - constanta3);
            $('#lng-spa-fob').val(result.format(2));
        };
        
        var changeLngSpaDes = function (val) {
            var constanta3 = parseFloat($('.pricing-constant-3').val());
            var constanta4 = parseFloat($('.pricing-constant-4').val());
            var constanta5 = parseFloat($('.pricing-constant-5').val());
            var constanta6 = parseFloat($('.pricing-constant-6').val());
            var constanta7 = parseFloat($('.pricing-constant-7').val());
            var result = constanta4 * val - constanta3 + constanta5 + constanta6 * (constanta7 * val);
            $('#lng-spa-des').val(result.format(2));
        };
        
        var changeSenoroFeedGas = function (val) {
            var jccFloor = parseFloat($('.jcc-floor').val());
            var slopeLngPrice = parseFloat($('.slope-lng-price').val());
            var coefficientOfBoilGas = parseFloat($('.coefficient-of-boil-gas').val() / 100);
            var coefficientOfLngPlant = parseFloat($('.coefficient-of-lng-plant').val() / 100);
            var gasSellersAboveJcc = parseFloat($('.gas-sellers-above-jcc').val() / 100);
            var senoroGasPrice = parseFloat($('.senoro-gsa-price').val());
            
            var result = 0;
            if (val > jccFloor) {
                result = slopeLngPrice * (val - jccFloor) * coefficientOfBoilGas * coefficientOfLngPlant * gasSellersAboveJcc;
            }
            result += senoroGasPrice;
            $('#senoro-feed-gas').val(result.format(2));
        };
        
        var changeMatindokFeedGas = function (val) {
            var jccFloor = parseFloat($('.jcc-floor').val());
            var slopeLngPrice = parseFloat($('.slope-lng-price').val());
            var coefficientOfBoilGas = parseFloat($('.coefficient-of-boil-gas').val() / 100);
            var coefficientOfLngPlant = parseFloat($('.coefficient-of-lng-plant').val() / 100);
            var gasSellersAboveJcc = parseFloat($('.gas-sellers-above-jcc').val() / 100);
            var matindokGasPrice = parseFloat($('.matindok-gsa-price').val());
            var result = 0;
            if (val > jccFloor) {
                result = slopeLngPrice * (val - jccFloor) * coefficientOfBoilGas * coefficientOfLngPlant * gasSellersAboveJcc;
            }
            result += matindokGasPrice;
            $('#matindok-feed-gas').val(result.format(2));
        };
        
        var changeAverageFeedGas = function (val) {
            var senoroComposition = parseFloat($('.senoro-composition').val());
            var matindokComposition = parseFloat($('.matindok-composition').val());
            var senoro = ($('#senoro-feed-gas').val() * senoroComposition) / (senoroComposition + matindokComposition);
            var matindok = ($('#matindok-feed-gas').val() * matindokComposition) / (senoroComposition + matindokComposition);
            var result = senoro + matindok;
            $('#average-feed-gas').val(result.format(2));
        };
        
        var changeCdsPrice = function (val) {
            var priceVariable = parseFloat($('.price-variable').val());
            var priceConstanta = parseFloat($('.price-constanta').val());
            var result = (val / priceVariable) + priceConstanta;
            $('#cds-price').val(result.format(2));
        };

        $('.constant-pricing-wrapper').hide();
        $('.show-pricing-constant').click(function () {
            if ($('.constant-pricing-wrapper').is(":visible")) {
                $('.constant-pricing-wrapper').slideUp();
                $('.show-pricing-constant').html('Show Constant');
            } else {
                $('.constant-pricing-wrapper').slideDown();
                $('.show-pricing-constant').html('Hide Constant');
            }
        });

    };
}(window, jQuery));