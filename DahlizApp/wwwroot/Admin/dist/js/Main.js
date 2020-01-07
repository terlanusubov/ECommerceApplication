$(document).ready(function () {


    var counter = 0;
    //SizeAdd -delete
    $(".addSize").click(function (e) {
        e.preventDefault()
        $.ajax({
            url: "/admin/product/fillsize",
            dataType: "json",
            type: "post",
            data: "",
            success: function (response) {
                if (response.status == 200) {
                    var select = $("<select data-element='true' data-error='size_error' id='Sizes' name='Sizes' class='form-control' ></select>`")
                    for (var size of response.data) {
                        $(select).append("<option value=" + size.id + ">" + size.name + "</option>")
                    }
                    var productSizeCount = $(`   <div class="ProductSizeCount">
                            <div class="size">
                                <label>Sizes</label>
                            </div>
                            <div class="quantity">
                               <div class="row">
                                        <div class="col-4">
                                             <label>Quantity</label>
                                             <input data-element="true" data-type="number" data-error="product_quantity${counter}" name="Count" type="text" class="form-control">
                                          </div>
                                       <div class="col-8">
                                          <div class="error_wrapper product_quantity${counter}"></div>
                                       </div>
                               </div>
                            </div>
                            <a class="delete"><i class="fas fa-times"></i></a>
                        </div>
                     
                    `)

                    $(productSizeCount).find(".size").append(select)
                    $(".SizeCountForm").append(productSizeCount)
                }
                if ($(".SizeCountForm .ProductSizeCount").length != response.data.length) {
                    $(".addSize").css("display", "block")
                } else {
                    $(".addSize").css("display", "none")
                }

                counter++;
            }
        })
    })
})

$(".SizeCountForm").on("click", ".ProductSizeCount .delete", function (e) {
    e.preventDefault();
    $(this).parents(".ProductSizeCount").remove()
    $(".addSize").css("display", "block")

})

//-------------------------------------------PRODUCT PHOTO UPLOAD--------------------------------------------------//
var current_array = [];
//Photo Fill to Product Edit
var photos = [];
var options = $("select[name='old_photos'] option");
var root_folder_name = $("select[name='old_photos']").data("root");
for (var option of options) {
    photos.push(option.innerText);
}
for (var photo of photos) {
    FillOldPhotos(photo, $(".photos"), root_folder_name);
}
console.log(current_array);
//FillPhotoNamesSelect();
//Photo Fill to Product Edit End


var delete_counter = 0;

$("#Upload").change(async function () {
    //Server path Name
    var path = $(this).data("path");

    var controller = $(this).data("controller");


    //single photo upload
    if ($(this).data("type") == "single" && $(".photos li:first-child img").data("name") != undefined) {
        //take file name
        var file_name = $(".photos li:first-child img").data("name");
        //remove from view
        $(".photos img[data-name='" + file_name + "'").parent().remove();
        //remove from server
        PhotoDelete(file_name, path);
    } else {
        console.log("if-e girmedi");
    }
    //Empty Current Array
    current_array = [];
    //Make photo array
    var photos = $(this).get(0).files;
    for (var photo of photos) {
        current_array.push(photo)
    }

    //Photo Upload
    await PhotoUpload(path, controller);

    //Photo names for send to action
    FillPhotoNamesSelect();

    delete_counter++;


    console.log(current_array);
});


//Photo Remove
$(".photos").on("click", "li .fa-times", function () {
    //Delete from html
    $(this).parent().remove();

    //Take file name
    let file_name = $(this).next().data("name");


    //Delete from Input
    DeleteFileFromInput(current_array, file_name);

    //Update Photo Names Select
    FillPhotoNamesSelect(current_array);

    //Make delete photos for edit
    if ($(this).next().data("status") == "old") {
        FillProductDeletePhotos(file_name);
    }

    var path = $("#Upload").data("path");
    //Delete From Server
    PhotoDelete(file_name, path);



    console.log(current_array);
});

//Photo Upload With Ajax
async function PhotoUpload(path, controller) {
    $("#photo_info").text("Please Wait ... Do not Close the window!");
    var data = new FormData();
    for (var i = 0; i < current_array.length; i++) {
        data.append("Photos[]", current_array[i]);
    }
    data.append("path", path);

    await $.ajax({
        url: `/Admin/${controller}/AddPhoto`,
        data: data,
        dataType: "json",
        type: "post",
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.status == 200) {
                $("#photo_info").text("");
                for (var i = 0; i < response.data.length; i++) {
                    var element = `<li>
                                <i class='fas fa-times'></i>
                                <img data-name="`+ response.data[i].filename + `"  src=` + response.data[i].url + ` />
                                  </li>`
                    $(".photos").append(element);
                }
            } else {
                $("#photo_info").text("Error occured please try again");
            }
        }
    })
}

function PhotoDelete(file_name, path) {
    var data = new FormData();
    data.append("filename", file_name);
    data.append("path", path);
    $.ajax({
        url: "/Admin/Product/RemoveFile",
        data: data,
        dataType: "json",
        type: "post",
        cache: false,
        contentType: false,
        processData: false
    })
}

function FillPhotoNamesSelect() {
    $("#add_photo_names").empty();
    var array = document.querySelectorAll(".photos li img")
    if (array != null) {
        var photo_names = [];

        for (var photo of array) {
            photo_names.push(photo.getAttribute("data-name"));
        }

        for (var i = 0; i < photo_names.length; i++) {
            var element = `<option selected>` + photo_names[i] + `</option>`
            $("#add_photo_names").append(element);
        }
    }
}



function DeleteFileFromInput(current_array, fileName) {
    var date = new Date();
    var photo_date = `${date.getMonth() + 1}${date.getDate()}${date.getFullYear()}`
    var base_file_name = fileName.substring(photo_date.length, fileName.length);
    for (var i = 0; i < current_array.length; i++) {
        if (current_array[i].name == base_file_name) {
            current_array.splice(i, 1);
        }
    }
}


//Fill photo to html
function FillOldPhotos(photo, appendElement, rootfolder) {
    var element = `<li>
                                <i class='fas fa-times'></i>
                                <img data-status="old"  data-name="`+ photo + `" src=/Admin/Uploads/` + rootfolder + `/` + photo + ` />
                            </li>`
    appendElement.append(element);
}


function FillProductDeletePhotos(filename) {
    var element = `<option selected>` + filename + `</option>`;
    $("#delete_photos").append(element);
}

//-------------------------------------------PRODUCT PHOTO UPLOAD END--------------------------------------------------//



//-------------------------------------------Validation------------------------------------------//
$("#btnSubmit").click(function (e) {
    var photo = document.querySelector("#Upload");
    var validator = new Validation($(this).data("parent"), current_array, photo != null ? photo.getAttribute("data-element") == "true" ? photo : null : null);

    $(".error_wrapper").text("");
    $(".error_wrapper").removeClass("alert")
    $(".error_wrapper").removeClass("alert-danger")


    var errors = validator.TryValidateForm();
    if (errors.length != 0) {
        e.preventDefault();
        for (var error of errors) {
            var error_wrapper = document.querySelector(".error_wrapper" + "." + error.errorKey);
            error_wrapper.classList.add("alert")
            error_wrapper.classList.add("alert-danger")
            error_wrapper.innerText = error.errorText;
        }

        if ($(".error_wrapper.alert").offset() != undefined) {
            $("html, body").animate({ scrollTop: $(".error_wrapper.alert").offset().top }, "slow");
        }
    }
})
//-------------------------------------------End Validation------------------------------------------//






//Multiple Select Plugin




//Subcategory Fill
$("select[name = 'CategoryId']").change(function () {
    $("select[name = 'SubCategoryId']").empty();
    var langId = $("select[name='culture']").val();
    var data = {
        langId: langId,
        CategoryId: parseInt($(this).val())
    };

    console.log(data);
    $.ajax({
        url: "/admin/product/subcategory",
        data: data,
        dataType: "json",
        type: "get",
        success: function (response) {
            $("select[name = 'SubCategoryId']").removeAttr("disabled")
            for (var i = 0; i < response.data.length; i++) {
                var option = $("<option value=" + response.data[i].subcategoryId + ">" + response.data[i].name + "</option>");
                $("select[name = 'SubCategoryId']").append(option);
            }

        }
    })

});


//Ck editor get data


$("#terms_add").click(function () {

    var form_data = new FormData();
    for (var editor of editor_variables) {
        form_data.append("Data[]", editor.getData());
    }
    $("#editor_info").text("Please wait data is sending...");
    $.ajax({
        url: "/Admin/Terms/Add",
        data: form_data,
        dataType: "json",
        type: "post",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status == 200) {
                $("#editor_info").text("Data sended successfully!");
                $("#editor_info").css("color", "green");
            }
        }
    })
});


$("#terms-edit").click(function () {
    var form_data = new FormData();
    form_data.append("Id", $("#TermsId").val());
    for (var editor of editor_variables) {
        form_data.append("Data[]", editor.getData());
    }
    $("#editor_info").text("Please wait data is sending...");
    $.ajax({
        url: "/Admin/Terms/Edit",
        data: form_data,
        dataType: "json",
        type: "post",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status == 200) {
                $("#editor_info").text("Data sended successfully!");
                $("#editor_info").css("color", "green");
            }
        }
    })
})




$("#about_add").click(function () {

    var form_data = new FormData();
    for (var editor of editor_variables) {
        form_data.append("Data[]", editor.getData());
    }
    $("#editor_info").text("Please wait data is sending...");
    $.ajax({
        url: "/Admin/About/Add",
        data: form_data,
        dataType: "json",
        type: "post",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status == 200) {
                $("#editor_info").text("Data sended successfully!");
                $("#editor_info").css("color", "green");
            }
        }
    })
});


$("#about-edit").click(function () {
    var form_data = new FormData();
    form_data.append("Id", $("#AboutId").val());
    for (var editor of editor_variables) {
        form_data.append("Data[]", editor.getData());
    }
    $("#editor_info").text("Please wait data is sending...");
    $.ajax({
        url: "/Admin/About/Edit",
        data: form_data,
        dataType: "json",
        type: "post",
        cache: false,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status == 200) {
                $("#editor_info").text("Data sended successfully!");
                $("#editor_info").css("color", "green");
            }
        }
    })
})


function GetEditorData() {
    var inputs = document.querySelectorAll("input[data-type='ck-editor-text']");
    for (var i = 0; i < editor_variables.length; i++) {
        editor_variables[i].data.set(inputs[i].value);
    }
}






