var Paty = function () {
    function report(data) {
        console.log("report >>>>" + ko.toJSON(data));
    }
    function clearInputField(id) {
        console.log(document.getElementById(id).innerHTML);

        document.getElementById(id).innerHTML = document.getElementById(id).innerHTML;
    }

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
                success: function (data) {
                    callback(data);
                }

            });
        };
        var delCat = function(data, callback) {
            $.ajax({
                type: "GET",
                url: "/Paty/DeleteCategory/"+data.id(),
                success: function(result) {
                    callback(data, result);
                }
            });
        }
        return {
            getCategorys: getCategorys, saveRootCat: saveRootCat, delCat: delCat
        };
    };

    var pClient = patyClient();

    var viewmodel = {
        currView: ko.observable("CATEGORYS"),
        addRoot: ko.observable(false),
        newPhoto: ko.observable(null),
        editPhoto: ko.observable(false),
        haveAvatar: ko.observable(false),
        editCategory:ko.observable()
    };

    var model = {
        categorys: {
            all: ko.observableArray([]),
            current: ko.observable(null),
            root: ko.observableArray([]),
            sub: ko.observableArray()
        },
        events: {
            all: ko.observableArray([]),
            active: ko.observableArray([]),
            current:ko.observable(null)
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
    var paty = function(data, mode) {
        var self = this;
        self.info = {};
        self.info.Id = ko.observable(data.Id);
        self.info.ExpDate = ko.observable(data.PatyDate);
        self.info.Title = ko.observable(data.Title);
        self.info.Description = ko.observable(data.Descr);
        self.info.Guests = ko.observable(data.MaxGuests);
        self.info.Price = ko.observable(data.Price);

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
    };
    var saveRootCallback = function (item) {
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
        }        //pClient.getCategorys(getCatCallback);
        model.categorys.all.push(new category(dt, displayMode.view));
        if (dt.ParentCategory !== "") {
            model.categorys.sub.push(new category(dt, displayMode.view));
        } else {
            model.categorys.root.push(new category(dt, displayMode.view));
        }
        viewmodel.editCategory().mode(displayMode.view);
        clearInputField("upimgInput");
        alert(ko.toJSON(item));
    };
    var deleteCatCb = function (data, result) {
        if (data.parentCat() !== "") {
            model.categorys.sub.remove(data);
        } else {
            model.categorys.root.remove(data);
        }
        alert("Категория удалена: "+result.Title);
    }
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
        previewUpdImg(this);
        viewmodel.newPhoto(true);
        viewmodel.haveAvatar(true);
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
        viewmodel.editCategory(item);
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
    var deleteCategory = function(data) {
        if (confirm("Удалить категорию: " + data.title())) {
            pClient.delCat(data, deleteCatCb);
        } else {
            alert("Удаление отменено");
        }
    }
    var addRoot = function() {
        var dt = new catData();
        dt.Id = 0;
        dt.ParentCategory = 0;
        viewmodel.editCategory(new category(dt,displayMode.edit));
        viewmodel.newPhoto(null);
        viewmodel.addRoot(true);
    }
    var addSubRoot = function() {
        var dt = new catData();
        dt.Id = 0;
        dt.ParentCategory = model.categorys.current().id();
        viewmodel.editCategory(new category(dt, displayMode.edit));
        viewmodel.newPhoto(null);
        viewmodel.haveAvatar(false);
        viewmodel.addRoot(true);
    }
    var init = function () {
        var dt = new catData();
        dt.Id = 0;
        model.categorys.current(new category(dt));
        viewmodel.editCategory(new category(dt, displayMode.view));
        pClient.getCategorys(getCatCallback);
        ko.applyBindings(model, document.getElementById("patySection"));
    };

    $(init);
    return {
        viewmodel: viewmodel, submitNewRoot: submitNewRoot, setCurrCat: setCurrCat,
        setWorkDirectory: setWorkDirectory, addRoot: addRoot, addSubRoot: addSubRoot,
        deleteCategory: deleteCategory
    };
}();

