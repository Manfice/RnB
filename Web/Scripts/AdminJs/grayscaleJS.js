function grayscale(src) {
    var canvas = document.createElement("canvas");
    var context = canvas.getContext("2d");
    var imageObject = new Image;
    imageObject.src = src;

    canvas.width = imageObject.width;
    canvas.height = imageObject.height;

    context.drawImage(imageObject, 0, 0);

    var imagePixels = context.getImageData(0, 0, canvas.width, canvas.height);

    for (var y = 0; y < imagePixels.height; y++) {
        for (var x = 0; x < imagePixels.width; x++) {
            var i = (y * 4) * imagePixels.width + x * 4;
            var avg = (imagePixels.data[i] + imagePixels.data[i + 1] + imagePixels.data[i + 2]) / 3;
            imagePixels.data[i] = avg;
            imagePixels.data[i+1] = avg;
            imagePixels.data[i+2] = avg;
        }
    }
    context.putImageData(imagePixels, 0, 0, 0, 0, imagePixels.width, imagePixels.height);
    return canvas.toDataURL();
}

$(window).load(function() {
    $(".grayItem img").fadeIn(500);

    $(".grayItem img").each(function() {
        var el = $(this);
        el.css({"position":"relative", "width":"100%"}).wrap('<div class="img_wrapper" style="display: inline-block">').clone().addClass('img_gray').css({"position":"absolute","z-index":"900","opacity":"0", "width":"100%"}).insertBefore(el).queue(function() {
            var el = $(this);
            //el.parent().css({ "width": "260px" });
            el.dequeue();
        });
        this.src = grayscale(this.src);
    });

    $(".grayItem img").mouseover(function() {
        $(this).parent().find('img:first').stop().animate({ opacity: 1 }, 1000);

    });

    $(".img_gray").mouseout(function() {
        $(this).stop().animate({ opacity: 0 }, 1000);
    });
});