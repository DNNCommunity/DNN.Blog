(function ($) {
    $.fn.qaTooltip = function (options) {
        var opts = $.extend({}, $.fn.qaTooltip.defaultOptions, options),
                   $wrap = this;

        $wrap.each(function () {

            var $this = $(this);

            function hoverOver() {
                if (opts.useParentActiveClass) {
                    $this.addClass(opts.parentActiveClass);
                }
                $this.find(opts.tooltipSelector).show();
            }

            function hoverOut() {
                if (opts.useParentActiveClass) {
                    $this.removeClass(opts.parentActiveClass);
                }
                $this.find(opts.tooltipSelector).hide();
            }

            $this.hoverIntent({
                over: function () {
                    hoverOver();
                },
                out: function () {
                    hoverOut();
                },
                timeout: 200,
                interval: 200
            });

            if (opts.enableTouch) {
                $this.bind("touchstart", hoverOver);
                $this.bind("touchend", hoverOut);
            }

            if (opts.suppressClickSelector) {
                $this.find(opts.suppressClickSelector).click(function (e) {
                    e.preventDefault();
                });
            }
        });

        return $wrap;

    };

    $.fn.qaTooltip.defaultOptions = {
        tooltipSelector: '.tag-menu',
        suppressClickSelector: '',
        parentActiveClass: 'active',
        useParentActiveClass: false,
        enableTouch: false
    };

})(jQuery);