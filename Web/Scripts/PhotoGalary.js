var Galary = function () {
    /*models*/
    var model = {
        all: ko.observableArray([]),
        pagedAlboms: observableArray([]),
        currentPage: ko.observable(0),
        pageSize: ko.observable(9),
        pageCount: ko.observable()
    }
    /*Methods*/

    /*init function*/
    var init = function () {
        ko.applyBindings(model, document.getElementById("pt"));
    };
    $(init);

}();