// jQuery Functions
(function ($) {

    $.fn.showModal = function (options, callback) {

        var defaults = {
            title: 'Thông báo',
            message: ''
        };

        options = $.extend(defaults, options);

        var modal = $(this);

        modal.find('div.header').html(options.title);
        modal.find('div.content').html(options.message);

        if (typeof callback == "function") {
            modal
                .modal({
                    closable: false,
                    onDeny: function () {
                        callback(false);
                        return true;
                    },
                    onApprove: function () {
                        callback(true);
                    }
                })
                .modal('show');
        }
        else
            modal.modal('show');
    };

    $.fn.hideModal = function() {
        $(this).modal('hide');
    };

})(jQuery);