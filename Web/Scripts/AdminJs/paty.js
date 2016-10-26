var Paty = function () {
    function report(data) {
        console.log("report >>>>" + ko.toJSON(data));
    }
    function clearInputField(id) {
        console.log(document.getElementById(id).innerHTML);

        document.getElementById(id).innerHTML = document.getElementById(id).innerHTML;
    }

    var patyClient = function () {
        var getCategorys = function(callback) {
            $.ajax({
                type: "GET",
                url: "/paty/GetCategorys",
                success: function(data) {
                    callback(data);
                }
            });
        };
        var getPatys = function (callback) {
            $.ajax({
                type: "GET",
                url: "/paty/GetPatys",
                success: function (data) {
                    callback(data);
                }
            });
        };
        var saveRootCat = function (fData, callback) {
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
        var savePaty = function (fData, callback) {
            $.ajax({
                type: "POST",
                url: "/Paty/AddPatyAsync",
                data: fData,
                contentType: false,
                processData: false,
                success: function (data) {
                    callback(data);
                }

            });
        };
        var delCat = function (data, callback) {
            $.ajax({
                type: "GET",
                url: "/Paty/DeleteCategory/"+data.id(),
                success: function(result) {
                    callback(data, result);
                }
            });
        }
        return {
            getCategorys: getCategorys, saveRootCat: saveRootCat, delCat: delCat, savePaty: savePaty,
            getPatys: getPatys
        };
    };

    var pClient = patyClient();

    var viewmodel = {
        currView: ko.observable("PATYS"),
        addRoot: ko.observable(false),
        newPhoto: ko.observable(null),
        editPhoto: ko.observable(false),
        haveAvatar: ko.observable(false),
        editCategory: ko.observable(),
        editPaty:ko.observable(null)
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
        report(model.categorys.all());
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
        self.info.ExpDate = ko.observable(moment(data.ExpDate).format("DD.MM.YYYY HH:mm"));
        self.info.Title = ko.observable(data.Title);
        self.info.Description = ko.observable(data.Description);
        self.info.Place = ko.observable(data.Place);
        self.info.MaxGuests = ko.observable(data.MaxGuests);
        self.info.Price = ko.observable(data.Price);
        self.info.Orders = ko.observableArray(data.Orders);
        self.info.Dres = ko.observable(data.Dres);
        self.info.Avatar = {};
        self.info.Avatar.Id = ko.observable(data.AvatarId);
        self.info.Avatar.Path = ko.observable(data.AvatarPath);
        self.info.Category = ko.observable(data.Category);
        self.info.Rate = ko.observable(data.Rate);
        self.info.PatyInterest = ko.observable(data.PatyInterest);
        self.info.SeetsUsed = ko.observable(data.SeetsUsed);
        self.mode = ko.observable(mode);
        self.statys = ko.observable(data.Statys);
        self.seetsFree = ko.pureComputed(function() {
            return self.info.MaxGuests() - self.info.SeetsUsed();
        },self);
    }
    var patyData = function() {
        var self = this;
        self.Id = 0;
        self.ExpDate = "";
        self.Title = "";
        self.Description = "";
        self.Place = "";
        self.MaxGuests = "";
        self.Price = "";
        self.Dres = "";
        self.AvatarId = 0;
        self.AvatarPath = "";
        self.Category = {};
        self.Category.Id = 0;
        self.Category.Title = "";
        self.Rate = "";
        self.PatyInterest = "";
        self.Orders = [];
        self.Statys = "NEW";
        self.SeetsUsed = 0;
    }
    var order = function(data) {
        var self = this;
        self.Id = ko.observable(data.Id);
        self.Fio = ko.observable(data.Fio);
        self.Email = ko.observable(data.Email);
        self.Phone = ko.observable(data.Phone);
        self.Seets = ko.observable(data.Place);
        self.SeetsNumbers = ko.observable(data.SeetsNumbers);
    }
    var orderData = function() {
        var self = this;
        self.Id = 0;
        self.Fio = "";
        self.Email = "";
        self.Phone = "";
        self.Place = "";
        self.SeetsNumbers = "";
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
    var getPatysCallback = function (data) {
        var tempArr = [];
        data.forEach(function(item) {
            var dt = new patyData();
            dt.Id = item.Id;
            dt.Rate = item.AddRate;
            dt.Description = item.Descr;
            dt.Dres = item.Dres;
            dt.MaxGuests = item.MaxGuests;
            dt.ExpDate = new Date(parseInt(item.PatyDate.replace("/Date(", "").replace(")/", ""), 10));
            dt.PatyInterest = item.PatyInterest;
            dt.Price = item.Price;
            dt.Title = item.Title;
            dt.SeetsUsed = item.UsedPlaceCount;
            dt.Statys = "UPDATE";
            if (item.Avatar!==null) {
                dt.AvatarId = item.Avatar.Id;
                dt.AvatarPath = item.Avatar.Path;
            };
            if (item.ord.length>0) {
                item.ord.forEach(function(g) {
                    var ord = new orderData();
                    ord.Id = g.Id;/* id заказа*/
                    ord.Fio = g.Fio;/* фио заказчика*/
                    ord.Email = g.Email; /* мыло заказчика*/
                    ord.Phone = g.Phone;/* телефон*/
                    ord.Place = g.Place;/*кол-во мест*/
                    ord.SeetsNumbers = g.PlaceNumbers;
                    dt.Orders.push(new order(ord));
                });
            };
            if (item.Category!==null) {
                dt.Category.Id = item.Category.Id;
                dt.Category.Title = item.Category.Title;
            }
            tempArr.push(new paty(dt, displayMode.view));
        });
        model.events.all(tempArr);
    }
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
        }       
        model.categorys.all.push(new category(dt, displayMode.view));
        if (dt.ParentCategory !== "") {
            model.categorys.sub.push(new category(dt, displayMode.view));
        } else {
            model.categorys.root.push(new category(dt, displayMode.view));
        }
        viewmodel.editCategory().mode(displayMode.view);
        alert(ko.toJSON(item));
    };
    var savePatyCallback = function(item) {
        var dt = new patyData();
        dt.Id = item.Id;
        dt.Rate = item.AddRate;
        dt.Description = item.Descr;
        dt.Dres = item.Dres;
        dt.MaxGuests = item.MaxGuests;
        dt.ExpDate = new Date(parseInt(item.PatyDate.replace("/Date(", "").replace(")/", ""), 10));
        dt.PatyInterest = item.PatyInterest;
        dt.Price = item.Price;
        dt.Title = item.Title;
        if (item.Avatar!==null) {
            dt.AvatarId = item.Avatar.Id;
            dt.AvatarPath = item.Avatar.Path;
        };
        if (item.Category!==null) {
            dt.Category.Id = item.Category.Id;
            dt.Category.Title = item.Category.Title;
        };
        model.events.all.push(new paty(dt, displayMode.view));
        viewmodel.editPaty(new paty(new patyData(), displayMode.view));
    };
    var updPaty = function(data) {
        alert("Сохранено");
        viewmodel.editPaty().mode(displayMode.view);
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

    function previewPatyImg(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                viewmodel.haveAvatar(true);
                $("#patyAvatar").attr("src", e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    };
    $("#patyInput").change(function () {
        previewPatyImg(this);
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

    var submitPaty = function() {
        var data = new FormData($("#patyForm")[0]);
        var img = $("#patyInput").get(0).files;
        if (img.length>0) {
            data.append("Avatar", img[0]);
        }
        if (viewmodel.editPaty().statys() === "NEW") {
            pClient.savePaty(data, savePatyCallback);
        } else {
            pClient.savePaty(data, updPaty);
        }
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
    var serSubDir = function(data) {
        model.workDirectory(data);
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
    var addPaty = function(data) {
        var dt = new patyData();
        dt.Category = data.id();
        viewmodel.editPaty(new paty(dt, displayMode.edit));
        viewmodel.newPhoto(null);
        viewmodel.haveAvatar(false);
        report(dt);
    }
    var editPaty = function (data) {
        data.mode(displayMode.edit);
        var newPaty = data;
        viewmodel.editPaty(newPaty);
        viewmodel.newPhoto(false);
        viewmodel.haveAvatar(false);
        if (data.info.Avatar.Id() !== 0) {
            viewmodel.haveAvatar(true);
            viewmodel.newPhoto(null);
        }
    }
    var init = function () {
        var dt = new catData();
        dt.Id = 0;
        model.categorys.current(new category(dt));
        model.workDirectory(new category(dt));
        viewmodel.editPaty(new paty(new patyData(), displayMode.view));
        viewmodel.editCategory(new category(dt, displayMode.view));
        pClient.getCategorys(getCatCallback);
        pClient.getPatys(getPatysCallback);
        $("#dt").datetimepicker();
        ko.applyBindings(model, document.getElementById("patySection"));
    };

    $(init);
    return {
        viewmodel: viewmodel, submitNewRoot: submitNewRoot, setCurrCat: setCurrCat,
        setWorkDirectory: setWorkDirectory, addRoot: addRoot, addSubRoot: addSubRoot,
        deleteCategory: deleteCategory, serSubDir: serSubDir, addPaty: addPaty, submitPaty: submitPaty,
        editPaty: editPaty
    };
}();

