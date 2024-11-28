(function () {
    'use strict';
    var $isMicro = () => $(window).width() < '650';
    $(window).on("resize", () => {
        $isMicro();
    });
    $('.footer .copyright').appendTo($('.hTwoLevelNavigation:last'));

    if ($isMicro()) {
        $('.header__content._mobile .hIconLinkBlock').prependTo($('.header__menu._mobile'));
    }


    if ($('.loadInput').length > 0) {

        $('.loadInput').change(function (e) {
            console.log(e.target)
            const dt = new DataTransfer();
            for (var i = 0; i < this.files.length; i++) {
                let fileBloc = $('<div/>', { class: 'file-item' }),
                    fileName = $('<span/>', { class: 'file-label', text: this.files.item(i).name });
                fileBloc.append('<span class="file-delete">&#10006;</span>').append(fileName);
                $(this).siblings(".files-names").append(fileBloc);
            };
            for (let file of this.files) {
                dt.items.add(file);
            }
            this.files = dt.files;
            $('span.file-delete').click(function () {
                let name = $(this).next('.file-label').text();
                $(this).parent().remove();
                for (let i = 0; i < dt.items.length; i++) {
                    if (name === dt.items[i].getAsFile().name) {
                        dt.items.remove(i);
                        continue;
                    }
                }
                e.target.files = dt.files;
                console.log(e.target.files)
            });
        });
    }


    if ($(".advancedForm__tabs").length > 0) {
        $(".tab__body").hide();
        $(".tab__body:first").show();
        $(".tabs__control:first").addClass("current");
        $(".tabs__control").click(function () {
            $(".tab__body").hide();
            var activeTab = $(this).attr("data-rel");
            $("#" + activeTab).fadeIn();

            $(".tabs__control").removeClass("current");
            $(this).addClass("current");

        })

    }

    if ($('.videoAndText__wrapper').length > 0) {
        $('.videoAndText__wrapper').each(function (index, item) {

            const videoVrapper = $('.videoAndText__videoCover', $(item));
            const videoFrame = $('iframe', $(item))[0];
            const videoFrameSrc = $('iframe', $(item))[0].src;
            //console.log(videoFrame)
            $(item).on('click', function () {
                videoVrapper.remove();
                if (videoFrameSrc.split("/").includes("vk.com")) {
                    // const fraime = this.contentContainer[0].childNodes[0].lastElementChild;
                    const player = VK.VideoPlayer(videoFrame);
                    player.play();
                }
            })
        })
    }
    if (videoSlider !== null) {
        let personSlider = document.querySelector('.videoSlider').swiper;
        if (window.innerWidth >= 577) {
            personSlider.init();
            personSlider.updateSlides();
        } else {
            personSlider.destroy(true, true);
        }
    }

    const caseSlider = new Swiper('.case__slider', {
        slidesPerView: 1.1,
        spaceBetween: 10,
        slidesOffsetBefore: 0,
        observer: true,
        //loop: true,
        //centeredSlides: false,
        navigation: {
            nextEl: ".case__arrow-next",
            prevEl: ".case__arrow-prev",
        },
        breakpoints: {
            576: {
                slidesPerView: 1,
                spaceBetween: 10,
            },
        }
    })

})();