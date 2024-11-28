'use strict';

/* startup */
$(function () {
    var $menuToggle = $("#menuToggle");
    var $body = $("body");
    var $isTablet = () => $(window).width() < '1100';
    var $isMobile = () => $(window).width() < '768';
    var header = $('.header');
    var scrollPrev = 0;

    $(window).on("resize", () => {
        $isMobile();
        $isTablet();
    });

    (function mobileMenuToggle() {
        $menuToggle.on('click', function () {
            $body.toggleClass("_menu_open");
        });

        // to close mobile menu
        $body.on("click", function (e) {
            if ($body.hasClass('_menu_open') && !e.target.closest('.burger')) {
                if (e.target.closest('a') || e.target.tagName === 'a') {
                    $body.removeClass('_menu_open');
                }
            }
        });

        function closeMenuAndOverlay() {
            if ($body.hasClass('_menu_open')) {
                $body.removeClass('_menu_open');
            }
        }
    })();

    function scrollToCoord(coord) {
        $('html, body').animate({ scrollTop: coord }, '200');
    }

    $('a.navigation__item').on('click', function (e) {
        let id = $(this).attr('href').slice(($(this).attr('href').indexOf('#') + 1));
        let ancorElem = document.getElementById(id);

        if (!$(this).attr('href').includes("popup")) {
            if (ancorElem) scrollToCoord(ancorElem.offsetTop);
        }
    });

    $(window).on("scroll", function () {
        if ($isTablet()) {
            var scrolled = $(window).scrollTop();

            if (scrolled > 100 && scrolled > scrollPrev) {
                $body.addClass('_header_out');
            } else {
                $body.removeClass('_header_out');
            }
            scrollPrev = scrolled;
        }
    });

    var nav = $('header .navigation').clone();
    nav.prependTo($('.header__menu'));


    if (typeof Swiper !== "undefined") {
        $('.swiper-container').each(function (index, e) {
            var $e = $(e);

            $e.addClass("mySwiper" + index);
            $e.find('.swiper-button-next').addClass("button-next" + index);
            $e.find('.swiper-button-prev').addClass("button-prev" + index);
            $e.find('.swiper-pagination').addClass("pagination" + index);

            var delay = parseInt($e.find('.arrowBlock').attr('data-delay'));
            if (isNaN(delay)) { delay = 0; }
            var autoplayVal = delay !== 0


            var swiperOptions = {
                "banner": {
                    slidesPerView: 1,
                    loop: !$e.hasClass('_static'),
                    autoplay: autoplayVal ? {
                        delay: +delay,
                        disableOnInteraction: false,
                        pauseOnMouseEnter: true
                    } : false,
                    pagination: {
                        el: ".pagination" + index,
                        clickable: true
                    },
                    breakpoints: {
                        768: {
                            navigation: {
                                nextEl: ".button-next" + index,
                                prevEl: ".button-prev" + index
                            }
                        }
                    }
                },
                "videoSlider": {
                    slidesPerView: 1,
                    navigation: {
                        nextEl: ".button-next" + index,
                        prevEl: ".button-prev" + index,
                    },
                    pagination: {
                        el: ".pagination" + index,
                        type: "fraction",
                        clickable: true
                    },
                },
                "gallery": {
                    loop: true,
                    spaceBetween: 10,
                    navigation: {
                        nextEl: ".swiper-button-next" + index,
                        prevEl: ".swiper-button-prev" + index,
                    },
                    autoplay: autoplayVal ? {
                        delay: +delay,
                    } : false
                },
                "linear": {
                    slidesPerView: 1.2,
                    loop: true,
                    autoplay: autoplayVal ? {
                        delay: +delay,
                        disableOnInteraction: false,
                        pauseOnMouseEnter: true
                    } : false,
                    speed: 900,
                    spaceBetween: 16,
                    pagination: {
                        el: ".pagination" + index,
                        clickable: true
                    },
                    navigation: {
                        nextEl: '.swiper-button-next',
                        prevEl: '.swiper-button-prev',
                    },
                    breakpoints: {
                        576: {
                            slidesPerView: 2.2
                        },
                        768: {
                            slidesPerView: 3
                        }
                    }
                },
                "cardGallery": {
                    slidesPerView: 1.2,
                    loop: true,
                    autoplay: autoplayVal ? {
                        delay: +delay,
                        disableOnInteraction: false,
                        pauseOnMouseEnter: true
                    } : false,
                    speed: 900,
                    spaceBetween: 16,
                    pagination: {
                        el: ".pagination" + index,
                        clickable: true
                    },
                    breakpoints: {
                        576: {
                            slidesPerView: 2.2
                        },
                        768: {
                            slidesPerView: 3
                        }
                    }
                }
            };

            var options;
            if ($e.hasClass("banner")) {
                options = swiperOptions.banner;
            }
            if ($e.hasClass("gallery")) {
                options = swiperOptions.gallery;
            }
            if ($e.hasClass("linear")) {
                options = swiperOptions.linear;
            }
            if ($e.hasClass("cardGallery")) {
                options = swiperOptions.cardGallery;
            }
            //if ($e.hasClass("videoSlider")) {
            //    options = swiperOptions.videoSlider;
            //}
            var swiper = new Swiper(".mySwiper" + index, options);
        });
    }

    if ($.magnificPopup) {
        $('.popup-inline').magnificPopup({
            removalDelay: 300,
            fixedContentPos: true,
            autoFocusLast: false,
            mainClass: 'mfp-move-vertical',
            callbacks: {
                beforeOpen: function () {
                    $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=2.0');
                },
                beforeClose: function () {
                    $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=1.0');
                }
            },
        })

        $('.popup-gallery').each(function () {
            //console.log($(this))
            $(this).magnificPopup({
                delegate: 'a',
                type: 'image',
                fixedContentPos: true,
                autoFocusLast: false,
                tClose: 'Закрыть (Esc)',
                gallery: {
                    enabled: true,
                    tCounter: '<span class="mfp-counter">%curr% из %total%</span>'
                },
                callbacks: {
                    beforeOpen: function () {
                        $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=2.0');
                    },
                    beforeClose: function () {
                        $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=1.0');
                    }
                },
            });
        })

        $('.videoSlider').magnificPopup({
            delegate: 'a',
            type: 'image',
            //gallery: {
            //    enabled: true,
            //    navigateByImgClick: true,
            //    preload: [0, 1]
            //},
            callbacks: {
                elementParse: function (item) {
                    item.type = 'iframe',
                        item.iframe = {
                            patterns: {
                                youtube: {
                                    index: 'youtube.com/',
                                    id: 'v=',
                                    src: '//www.youtube.com/embed/%id%?autoplay=1'
                                },
                                vimeo: {
                                    index: 'vimeo.com/',
                                    id: '/',
                                    src: '//player.vimeo.com/video/%id%?autoplay=1'
                                },
                                gmaps: {
                                    index: '//google.com/maps/d/',
                                    id: 'mid=',
                                    src: 'www.google.com/maps/d/embed?mid=%id%'
                                }
                            }
                        }

                }
            }
        });

    }

    (function cropReviewDescription() {
        const rewiewCropText = document.querySelectorAll('.reviews-linear .review__text');
        if (rewiewCropText && rewiewCropText.length) {
            rewiewCropText.forEach(cropText => {
                if (cropText.scrollHeight > cropText.offsetHeight) {
                    cropText.classList.add('_cropped');
                }
            });
        }
    })();

    $('.schedule__button').on('click', function () {
        let $buttonTextWrap = $('.btn__caption', this);
        if ($(this).hasClass('schedule_open')) {
            $(this).removeClass('schedule_open');
            $buttonTextWrap.text('Показать расписание');
        } else {
            $(this).addClass('schedule_open');
            $buttonTextWrap.text('Свернуть расписание');
        }
    });


    /*Работа простого аккордиона*/

    const accordionHeaders = document.querySelectorAll('.accordion__header');

    if (accordionHeaders && accordionHeaders.length) {
        accordionHeaders.forEach(title => {
            title.addEventListener('click', () => {
                title.classList.toggle('accordion_active');

                if (title.classList.contains('accordion_active')) {
                    const expBody = title.nextElementSibling;
                    expBody.classList.add('accordion_active');
                } else {
                    const expBody = title.nextElementSibling;
                    expBody.classList.remove('accordion_active');
                }
            })
        })
    }

});

(function () {
    const linkArry = document.querySelectorAll('a');

    linkArry.forEach(link => {
        let href = link.getAttribute('href');
        href = href?.substring(1);
        const popupTarget = document.getElementById(href);
        if (popupTarget && popupTarget.nextElementSibling) {
            link.classList.add('popup-inline');
            if ($(popupTarget.nextElementSibling).hasClass("mfp-hide")) {

                popupTarget.removeAttribute('id');
                popupTarget.nextElementSibling.setAttribute('id', href)
            }

        }
    })
})();

function cropPlanDescription() {
    const descriptions = document.querySelectorAll('.plan__description');

    if (descriptions) {
        descriptions.forEach(desc => {
            const text = desc.querySelector('.text');

            if (text.scrollHeight > text.offsetHeight) {
                const expandBtn = document.createElement('a');
                expandBtn.textContent = "Полное описание";
                expandBtn.classList.add('btn-link', 'btn_expand');
                desc.append(expandBtn);
                expandBtn.addEventListener('click', function () {
                    desc.classList.toggle('desc_open');
                });
            }
        });
    }
}



window.addEventListener('DOMContentLoaded', () => {
    cropPlanDescription();
});

