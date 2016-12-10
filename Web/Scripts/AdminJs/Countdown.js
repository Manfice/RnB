function Countdown(end,elements, callback) {
    var sec = 1000,
        min = sec * 60,
        hour = min * 60,
        day = hour * 24,
        exp = new Date(end),
        timer,
        culculate = function() {
            var now = new Date(),
                remaining =  exp.getTime() - now.getTime(),
                data;

            if (isNaN(exp)) {
                console.log("Err NaN");
                return;
            }

            if (remaining <= 0) {
                clearInterval(timer);
                var cd = $("#countdownBlock");
                cd.html("<h3>Время регистрации истекло</h3>");
                if (typeof callback === "function") {
                    callback();
                }
            } else {
                if (!timer) {
                    timer = setInterval(culculate, sec);
                }
                data = {
                    'days': Math.floor(remaining / day),
                    'hours': Math.floor((remaining % day) / hour),
                    'minutes': Math.floor((remaining % hour) / min),
                    'seconds': Math.floor((remaining % min) / sec)
                }
                if (elements.length) {
                    for (x in elements) {
                        if (elements.hasOwnProperty(x)) {
                            var e = elements[x];
                            data[e] = ("00" + data[e]).slice(-2);
                            document.getElementById(e).innerText = data[e];
                        }
                    }
                }
            }
        };

    culculate();
}