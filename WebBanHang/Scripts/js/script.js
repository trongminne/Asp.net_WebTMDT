// some scripts

// jquery ready start
$(document).ready(function () {
    // jQuery code

    // var html_download = '<a href="http://bootstrap-ecommerce.com/templates.html" class="btn btn-dark rounded-pill" style="font-size:13px; z-index:100; position: fixed; bottom:10px; right:10px;">Download theme</a>';
    //  $('body').prepend(html_download);


    //////////////////////// Prevent closing from click inside dropdown
    $(document).on('click', '.dropdown-menu', function (e) {
        e.stopPropagation();
    });




    //////////////////////// Bootstrap tooltip
    if ($('[data-toggle="tooltip"]').length > 0) {  // check if element exists
        $('[data-toggle="tooltip"]').tooltip()
    } // end if





});
// jquery end

// danh gia

window.addEventListener("DOMContentLoaded", () => {
    const starRating = new StarRating("form");
});

class StarRating {
    constructor(qs) {
        this.ratings = [
            { id: 1, name: "Terrible" },
            { id: 2, name: "Bad" },
            { id: 3, name: "OK" },
            { id: 4, name: "Good" },
            { id: 5, name: "Excellent" }
        ];
        this.rating = null;
        this.el = document.querySelector(qs);

        this.init();
    }
    init() {
        this.el ?.addEventListener("change", this.updateRating.bind(this));

        // stop Firefox from preserving form data between refreshes
        try {
            this.el ?.reset();
        } catch (err) {
            console.error("Element isn’t a form.");
        }
    }
    updateRating(e) {
        // clear animation delays
        Array.from(this.el.querySelectorAll(`[for*="rating"]`)).forEach(el => {
            el.className = "rating__label";
        });

        const ratingObject = this.ratings.find(r => r.id === +e.target.value);
        const prevRatingID = this.rating ?.id || 0;

        let delay = 0;
        this.rating = ratingObject;
        this.ratings.forEach(rating => {
            const { id } = rating;

            // add the delays
            const ratingLabel = this.el.querySelector(`[for="rating-${id}"]`);

            if (id > prevRatingID + 1 && id <= this.rating.id) {
                ++delay;
                ratingLabel.classList.add(`rating__label--delay${delay}`);
            }

            // hide ratings to not read, show the one to read
            const ratingTextEl = this.el.querySelector(`[data-rating="${id}"]`);

            if (this.rating.id !== id)
                ratingTextEl.setAttribute("hidden", true);
            else
                ratingTextEl.removeAttribute("hidden");
        });
    }
}




// Loader

/*--------------------------------------------------
Project:        Markito
Version:        1.0
Author:        Markito
-----------------------------------------------------

JS INDEX
================================================
* preloader js
* One Page Scroll
* slick Menu js
* slick Nav
* Isotope
* Counter
* bottom to top
================================================*/

(function ($) {
    "use strict";
    var $main_window = $(window);
    /*====================================
        preloader js
      ======================================*/
    $main_window.on("load", function () {
        $("#preloader").fadeOut("slow");
    });

    /*====================================
                  One Page Scroll
      ======================================*/

    // Select all »a« elements with a parent class »links« and add a function that is executed on click
    $(".navbar-nav .nav-item a").on("click", function (e) {
        // Define variable of the clicked »a« element (»this«) and get its href value.
        var href = $(this).attr("href");
        // Run a scroll animation to the position of the element which has the same id like the href value.
        $("html, body").animate(
            {
                scrollTop: $(href).offset().top,
            },
            "300"
        );
        // Prevent the browser from showing the attribute name of the clicked link in the address bar
        e.preventDefault();
    });

    /*====================================
        slick slider
    ======================================*/

    $(".home-slider").slick({
        dots: false,
        infinite: true,
        speed: 500,
        fade: true,
        arrows: true,
        cssEase: "linear",
        responsive: [
            {
                breakpoint: 786,
                settings: {
                    arrows: false,
                    dots: false,
                    slidesToScroll: 1,
                    autoplay: true,
                    autoplaySpeed: 2000,
                },
                breakpoint: 600,
                settings: {
                    arrows: false,
                    dots: false,
                    slidesToScroll: 1,
                    autoplay: true,
                    autoplaySpeed: 2000,
                },
            },
        ],
    });

    $(".autoplay").slick({
        slidesToShow: 1,
        arrows: false,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 2000,
        dots: true,
    });
    /*====================================
                slick slider
    =====================================*/
    $(".product-slider").slick({
        slidesToShow: 4,
        arrow: true,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 1500,
        responsive: [
            {
                breakpoint: 1200,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                },
            },
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                },
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                },
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                },
            },
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ],
    });

    $(".one-time").slick({
        dots: true,
        infinite: true,
        arrow: false,
        speed: 300,
        autoplay: false,
        autoplaySpeed: 1400,
        slidesToShow: 1,
        adaptiveHeight: true,
    });

    $(".slider-for").slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        autoplay: true,
        fade: true,
        asNavFor: ".slider-nav",
    });

    $(".slider-nav").slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        asNavFor: ".slider-for",
        dots: true,
        centerMode: true,
        focusOnSelect: true,
    });

    // client-Slider

    $(".client-Slider").slick({
        infinite: true,
        slidesToShow: 5,
        arrow: false,
        dots: true,
        autoplay: false,
        autoplaySpeed: 1400,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                },
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                },
            },
            {
                breakpoint: 450,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                },
            },
            {
                breakpoint: 375,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                },
            },
        ],
    });

    /*====================================
    sticky menu js
    ======================================*/
    var windows = $(window);
    var sticky = $(".header-fixed");
    windows.on("scroll", function () {
        var scroll = windows.scrollTop();
        if (scroll < 50) {
            sticky.removeClass("stick");
        } else {
            sticky.addClass("stick");
        }
    });
    /*====================================
        slick nav
    ======================================*/
    var logo_path = $(".mobile-menu").data("logo");
    $(".navbar-nav").slicknav({
        appendTo: ".mobile-menu",
        removeClasses: true,
        label: "",
        closedSymbol: '<i class="fa fa-angle-right"><i />',
        openedSymbol: '<i class="fa fa-angle-down"><i />',
        brand: '<img src="' + logo_path + '" class="img-fluid" alt="logo">',
    });

    /*------------ Wow-js -------------*/

    new WOW().init();

    /*====================================
              bottom to top
          ======================================*/

    var btn = $("#btn-to-top");
    $(window).on("scroll", function () {
        if ($(window).scrollTop() > 300) {
            btn.addClass("show");
        } else {
            btn.removeClass("show");
        }
    });
    btn.on("click", function (e) {
        e.preventDefault();
        $("html, body").animate(
            {
                scrollTop: 0,
            },
            "300"
        );
    });

    $(document).ready(function () {
        var count = 0;
        $("a.add-to-cart").click(function (event) {
            count++;
            $("a.add-to-cart").addClass("size");
            setTimeout(function () {
                $("a.add-to-cart").addClass("hover");
            }, 200);
            setTimeout(function () {
                $("a.cart > span").addClass("counter");
                $("a.cart > span.counter").text(count);
            }, 400);
            setTimeout(function () {
                $("a.add-to-cart").removeClass("hover");
                $("a.add-to-cart").removeClass("size");
            }, 600);
            event.preventDefault();
        });
    });

    document.addEventListener("touchstart", function () { }, true);
})(jQuery);


