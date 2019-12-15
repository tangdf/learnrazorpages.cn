// Write your Javascript code.
$(function() {
    $('ul.level1').show();
    console.log(window.location.pathname)
    var link = $('nav.main-nav a').filter(function() {
        return $(this).attr('href') == window.location.pathname;
    });
    $(link).parents('ul').show();
    $(link).next('ul').show();
    $(link).css('color' , 'black').css('background-color' , '#ffeee1');

    function fixDiv() {
        var $docOutline = $('#doc-outline');
        var $window = $(window);
        if ($window.scrollTop() > 100 && $window.width() > 500)
            $docOutline.css({
                    'position' : 'fixed' ,
                    'top' : '0px' ,
                    'max-width' : '270px'
                });
        else
            $docOutline.css({
                    'position' : 'relative' ,
                    'top' : 'auto' ,
                    'max-width' : '270px'
                });

    }

    $(window).scroll(fixDiv);
    fixDiv();

    $('a.page-scroll').bind('click' ,
        function(event) {
            var $anchor = $(this);
            $('html, body').stop().animate({
                    scrollTop : $($anchor.attr('href')).offset().top
                } ,
                1500 ,
                'easeInOutExpo');
            event.preventDefault();
        });


});