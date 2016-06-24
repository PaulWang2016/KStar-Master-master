(function($)
{
    /**
     * Auto-growing textareas; technique ripped from Facebook
     * 
     * 
     * http://github.com/jaz303/jquery-grab-bag/tree/master/javascripts/jquery.autogrow-textarea.js
     */
    $.fn.autogrow = function (options) {
        return this.filter('textarea').each(function () {
            var self = this;
            var $self = $(self);
            $self.css("overflow-y", "hidden");
            var analog = function (event) {
                if ($self.val() == "") return;
                var shadow = $('<div></div>').css({
                    width: $self.outerWidth(),
                    fontSize: $self.css('fontSize'),
                    fontFamily: $self.css('fontFamily'),
                    fontWeight: $self.css('fontWeight'),
                    lineHeight: $self.css('lineHeight'),
                    padding: $self.css('padding'),
                    margin: $self.css("margin"),
                    'word-wrap': 'break-word',
                    display: 'none'
                }).appendTo(document.body);
                var times = function (string, number) {
                    for (var i = 0, r = ''; i < number; i++) r += string;
                    return r;
                };
                var val = $self.val().replace(/</g, '&lt;')
                                 .replace(/>/g, '&gt;')
                                 .replace(/&/g, '&amp;')
                                 .replace(/\n$/, '<br/>&nbsp;')
                                 .replace(/\n/g, '<br/>')
                                 .replace(/ {2,}/g, function (space) { return times('&nbsp;', space.length - 1) + ' ' });

                shadow.html(val);
                if (val.trim() == "") {
                    //$self.height(Math.max(shadow.outerHeight(), $self.height()));
                } else {
                    $self.height(Math.max(shadow.outerHeight(),34));
                }
               
                shadow.remove();
            }
            analog();
            $self.change(analog).keyup(analog).keydown({ event: 'keydown' }, analog);
            $(window).resize(analog);
        
        });

    };
})(jQuery);
