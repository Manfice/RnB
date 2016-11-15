$(document).ready(function () {
    $(".topMenu").click(function () {
        $(".ddMenu").slideToggle();
    });

    var topSlider = $("#topSlider");

    topSlider.owlCarousel({
        items: 1,
        autoplay: true,
        autoplayTimeout: 10000,
        autoplayHoverPause: true,
        nav: false,
        dots: true,
        loop: true,
        autoplaySpeed: 3000,
        dotsContainer: "#topDot",
        autoHeight: false
    });
    topSlider.on("changed.owl.carousel", function (e) {
        console.log(e.item.index);
    });
    var paty = $("#paty");

    paty.owlCarousel({
        items: 3,
        autoplay: false,
        autoplayTimeout: 10000,
        autoplayHoverPause: true,
        nav: false,
        dots: false,
        autoplaySpeed: 3000,
        autoHeight: false,
        margin: 10,
        responsive: {
            0: {
                items:1
            },
            650: {
                items:2
            },
            960: {
                items:3
            }
        }
    });

    var partner = $(".partnersCarousel");
    partner.owlCarousel({
        items: 5,
        autoplay: true,
        autoplayTimeout: 5000,
        autoplayHoverPause: true,
        nav: false,
        dots: false,
        autoplaySpeed: 5000,
        autoHeight: false,
        loop: true,
        margin: 10,
        responsive: {
            0: {
                items: 1
            },
            650: {
                items: 2
            },
            960: {
                items: 5
            }
        }

    });
    $("#prev").click(function () {
        paty.trigger("prev.owl.carousel", [500]);
    });

    $("#next").click(function () {
        paty.trigger("next.owl.carousel", [500]);
    });
    $("#prevPar").click(function () {
        partner.trigger("prev.owl.carousel", [500]);
    });

    $("#nextPar").click(function () {
        partner.trigger("next.owl.carousel", [500]);
    });
    lightbox.option({
        "disableScrolling": true,
        "albumLabel": "@Model.Title"
    });
    function previewUpdImg(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var i = $("#avaImage");
                var f = $(".emptyAvatar");
                $(".avaControls label").css("display", "none");
                $(".avaControls input").css("display", "block");
                f.css("display","none");
                i.attr("src", e.target.result);
                i.css("display","block");
            }
            reader.readAsDataURL(input.files[0]);
        }
    };
    $("#castAva").change(function () {
        previewUpdImg(this);
    });

    
});

var mainJS = function () {

    var modelView = {
        photoView: ko.observable("PHOTO")
    };

    var init = function () {
        var photoBlock = document.getElementById("photoBlock");
        if (photoBlock) {
            ko.applyBindings(modelView, document.getElementById("photoBlock"));
        }
    };
    $(init);
    return {
    };
}();

var popUp = function () {

    var modelPopUp = {
        register: ko.observable(false),
        login: ko.observable(false),
        thankYou: ko.observable(false)
    };
    var registerMe = function () {
        var now = modelPopUp.register();
        modelPopUp.register(!now);
    };
    var loginMe = function () {
        var now = modelPopUp.login();
        modelPopUp.login(!now);
    };
    var thanks = function () {
        var now = modelPopUp.thankYou();
        modelPopUp.thankYou(!now);
    };
    var clReg = function () {
        registerMe();
        thanks();
    };
    var init = function () {
        ko.applyBindings(modelPopUp, document.getElementById("head"));
    };
    $(init);
    return {
        registerMe: registerMe, loginMe: loginMe, thanks: thanks, clReg: clReg
    };
}();