var Paty = function () {
    var patyClient = function () {
        var getCategorys = function (callback) {
            $.ajax({
                type: "GET",
                url: "/paty/GetCategorys",
                success: function (data) {
                    callback(data);
                }
            });
        }
        var saveRootCat = function(fData, callback) {
            $.ajax({
                type: "POST",
                url: "/Paty/AddRootCat",
                data: fData,
                contentType: false,
                processData: false,
                success: function () {
                    callback();
                }

            });
        };
        return {
            getCategorys: getCategorys, saveRootCat: saveRootCat
        };
    };

    var pClient = patyClient();

    var viewmodel = {
        currView: ko.observable("CATEGORYS"),
        addRoot: ko.observable(false),
        newPhoto: ko.observable(null),
        editPhoto: ko.observable(false),
        haveAvatar: ko.observable(false)
    };

    var model = {
        categorys: {
            all: ko.observableArray([]),
            current: ko.observable(null),
            root: ko.observableArray([]),
            sub: ko.observableArray()
        },
        workDirectory:ko.observable(null)
    };

    model.categorys.all.subscribe(function(newCategorys) {
        model.categorys.root.removeAll();
        var tempArr = [];
        tempArr.push.apply(tempArr, model.categorys.all().map(function(c) {
            return c;
        }).filter(function (value) {
            return value.parentCat() === "";
        }).sort());
        model.categorys.root(tempArr);
    });
    model.categorys.current.subscribe(function(newValue) {
        model.categorys.sub.removeAll();
        var tempArr = [];
        tempArr.push.apply(tempArr, model.categorys.all().map(function(c) {
            return c;
        }).filter(function (value) {
            return value.parentCat() === newValue.id();
        }).sort());
        model.categorys.sub(tempArr);
    });
    /*Models*/
    var category = function(data, mode) {
        var self = this;
        self.id = ko.observable(data.Id);
        self.title = ko.observable(data.Title);
        self.description = ko.observable(data.Description);
        self.avatar = {}
        self.avatar.id = ko.observable(data.AvatarId);
        self.avatar.path = ko.observable(data.AvatarPath);
        self.parentCat = ko.observable(data.ParentCategory);
        self.mode = ko.observable(mode);
    }
    var catData = function () {
        var self = this;
        self.Id = "";
        self.Title = "";
        self.Description = "";
        self.AvatarId = 0;
        self.AvatarPath = "";
        self.ParentCategory = "";
    }
    var displayMode = {
        edit: "EDIT",
        view:"VIEW"
    }
    /*Callbacks*/
    var getCatCallback = function(data) {
        model.categorys.all.removeAll();
        data.forEach(function(item) {
            var dt = new catData();
            dt.Id = item.Id;
            dt.Title = item.Title;
            dt.Description = item.Description;
            if (item.Avatar !== null) {
                dt.AvatarId = item.Avatar.Id;
                dt.AvatarPath = item.Avatar.Path;
            }
            if (item.ParentCategory !== null) {
                dt.ParentCategory = item.ParentCategory.Id;
            }
            model.categorys.all.push(new category(dt, displayMode.view));
        });
        console.log(ko.toJSON(model.categorys.all));
    };
    var saveRootCallback = function() {
        pClient.getCategorys(getCatCallback);
    };
    function previewUpdImg(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                viewmodel.haveAvatar(true);
                $("#updImg").attr("src", e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    };
    $("#upimgInput").change(function () {
        console.log(ko.toJSON(model));
        previewUpdImg(this);
        viewmodel.newPhoto(true);
    });

    var submitNewRoot = function () {
        var data = new FormData($("#updRoot")[0]);
        var img = $("#upimgInput").get(0).files;
        if (img.length > 0) {
            data.append("UploadedImage", img[0]);
        }
        pClient.saveRootCat(data, saveRootCallback);
    };

    var setCurrCat = function (data) {
        var item = data;
        item.mode(displayMode.edit);
        model.categorys.current(data);
        if (data.avatar.id() !== "") {
            viewmodel.haveAvatar(true);
        } else {
            viewmodel.haveAvatar(false);
        }
        viewmodel.newPhoto(null);
    }
    var setWorkDirectory = function (data) {
        model.categorys.current(data);
    }

    var addRoot = function() {
        var dt = new catData();
        dt.Id = 0;
        dt.ParentCategory = 0;
        model.categorys.current(new category(dt,displayMode.edit));
        viewmodel.newPhoto(null);
        viewmodel.addRoot(true);
        console.log(ko.toJSON(model.categorys.current));
    }
    var addSubRoot = function(data) {
        var dt = new catData();
        dt.Id = 0;
        dt.ParentCategory = data.id;
        model.categorys.current(new category(dt));
        viewmodel.newPhoto(null);
        viewmodel.addRoot(true);
    }
    var init = function () {
        var dt = new catData();
        dt.Id = 0;
        model.categorys.current(new category(dt));
        pClient.getCategorys(getCatCallback);
        ko.applyBindings(model, document.getElementById("patySection"));
    };

    $(init);
    return {
        viewmodel: viewmodel, submitNewRoot: submitNewRoot, setCurrCat: setCurrCat,
        setWorkDirectory: setWorkDirectory, addRoot: addRoot, addSubRoot: addSubRoot
    };
}();

