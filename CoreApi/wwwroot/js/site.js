(function($) {

    $.fn.showDimmer = function () {

        var element = $(this);

        element
            .dimmer({ closable: false })    // Options
            .dimmer('show');                // Show dimmer
    }

    $.fn.hideDimmer = function () {

        var element = $(this);

        element
            .dimmer('hide');                // Hide dimmer
    }

})(jQuery);