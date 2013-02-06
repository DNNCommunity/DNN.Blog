(function ($) {

    $.widget("ui.qaplaceholder", {
        options: {
            TextColor: "#000",
            PlaceHolderColor: "#ccc"
        },

        _create: function () {
            var self = this;
            var el = this.element;
            var defaultValue = $(el).attr("defaultvalue");

            $(el).val(defaultValue);
            $(el).css('color', self.options.PlaceHolderColor);

            $(el).bind("click.dnnqa", function () {
                if ($(el).val() == defaultValue) {
                    $(el).css('color', self.options.TextColor);
                    $(el).val("");
                }
            });

            var isEmpty = function () {
                if ($(el).val() == "") {
                    $(el).val(defaultValue);
                    $(el).css('color', self.options.PlaceHolderColor);
                } else {
                    $(el).css('color', self.options.TextColor);
                }

            };

            $(el).bind("blur.dnnqa", isEmpty);
        },

        _init: function () {

        },

        _setOption: function (key, value) {
            options.key = value;
        },

        destroy: function () {
            $(".qaplaceholder").each(function () {
                $(this).unbind("click.dnnqa").unbind("blur.dnnqa").val("");
            });
        }

    });

})(jQuery);

$(document).ready(function () {
    $(".qaplaceholder").qaplaceholder();
});