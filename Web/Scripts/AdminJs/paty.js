var Paty = function () {
    var patyClient = function () {
        var getCategorys = function (callback) {
            $.ajax({
                type: "GET",
                url: "/paty/GetCategorys",
                success: function (data) {
                    console.log(data);
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
                    alert(ko.toJSON(data));
                }

            });
        };
        return {
            getCategorys: getCategorys, saveRootCat: saveRootCat
        };
    };

    var pClient = patyClient();

    var viewmodel = {
        currView: ko.observable("INDEX"),
        addRoot: ko.observable(false),
        newPhoto:ko.observable(null)
    };

    var model = {
        categorys: {
            all: ko.observableArray([]),
            current: ko.observable(),
            root: ko.observableArray([]),
            sub:ko.observableArray()
        },
        currentCategory: ko.observableArray([]),
        
    };

    /*Models*/
    var category = function(data) {
        var self = this;
        self.id = ko.observable(data.Id);
        self.title = ko.observable(data.Title);
        self.description = ko.observable(data.Description);
        self.avatar = ko.observable(data.Avatar.Path);
        self.parentCat = ko.observable(data.ParentCategory.Id);
    }
    var catData = {
        Id: "",
        Title: "",
        Description: "",
        Avatar: { Path: "" },
        ParentCategory:{Id:""}
    }
    /*Callbacks*/
    var getCatCallback = function (data) {
        model.categorys.all.removeAll();
        data.forEach(function (item) {
            var dt = catData;
            dt.Id = item.Id;
            dt.Title = item.Title;
            dt.Description = item.Description;
            if (item.Avatar!==null) {
                dt.Avatar.Path = item.Avatar.Path;
            }
            if (item.ParentCategory!==null) {
                dt.ParentCategory.Id = item.ParentCategory.Id;
            }
            console.log(dt);
            model.categorys.all.push(new category((dt)));
        });
    }

    /*Methods*/
    function previewImg(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#prv").attr("src", e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    };

    $("#imgInput").change(function () {
        console.log(ko.toJSON(model));
        previewImg(this);
        viewmodel.newPhoto(true);
    });

    var submit = function () {
        var data = new FormData($("#formRootCat")[0]);
        var img = $("#imgInput").get(0).files;
        if (img.length > 0) {
            data.append("UploadedImage", img[0]);
        }
        pClient.saveRootCat(data);
    };

    var init = function () {
        pClient.getCategorys(getCatCallback);
        ko.applyBindings(model, document.getElementById("patySection"));
    };
    $(init);
    return {
        viewmodel: viewmodel, submit: submit
    };
}();

