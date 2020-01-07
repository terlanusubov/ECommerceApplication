$(document).ready(function () {



    //Shipping

    $("#City").change(function () {
        var key = this.value;
        var obj = {
            key: key
        };  

        var shipLang = $(".right-navigation .left-navigation-list span").text();
        $.ajax({
            url: "/card/changeshippingprice",
            data: obj,
            dataType: "json",
            type: "post",
            success: function (response) {
                if (shipLang == "Azerbaijan") {
                    $(".shipping").html("+Çatdırılma: " + response.shipping + " &#x20bc;");
                }
                else {
                    $(".shipping").html("+Shipping: " + response.shipping + " &#x20bc;");
                }
               
                
                $(".proceed-to-checkout p.total span").html("&nbsp;" + response.total);
            }
        })
    })
    //Checkout
    $("#checkout").click(function (e) {
        e.preventDefault();
        $("#checkout-form").submit();
    })

    //Navigation dropdown
    $(".left-navigation-list").click(function (e) {
        //remove all active class from dropdown elements
        $(".left-navigation-list").each(function () {
            $(this).removeClass("active");
        });

        //add active class to specific element
        $(this).addClass("active");

        //if($(this).hasClass("mydropdown-wrapper")){
        //    e.preventDefault();
        //}

        $(this).find(".mydropdown").toggleClass("active");
    });


    $(".mobile-nav-list .dropdownable_href").click(function (e) {
        e.preventDefault();
    })



    //Side bar opening
    $(".left-navigation i[data-name='mobile']").click(function () {
        $(this).removeClass("active");
        $(this).next().addClass("active");

        if ($(this).next().length == 0) {
            $(this).prev().addClass("active");
        }


        //side bar opening
        $(".mobile-nav").toggleClass("active")
    })



    //mobile dropdown
    $(".mobile-nav-list").click(function () {
        $(".mobile-nav-list").each(function () {
            $(this).removeClass("active");
        })
        $(this).addClass("active");
        $(this).find(".mobile-dropdown").toggleClass("active");

    })



    //shopping cart opening
    $(".pe-7s-shopbag").parent().click(function () {
        $(".shopping-cart").toggleClass("active");

    })
    //$(".right-navigation-list .pe-7s-shopbag").click(function () {
    //})



    //Add to bag
    $(".add-to-bag").click(function (e) {
        e.preventDefault();

        var productId = $("input[name= 'productId' ]").val()
        var sizeId = $("select[name= 'size']").val();
        var quantity = $("select[name = 'quantity']").val();
        var obj = {
            id: productId,
            sizeId: sizeId,
            quantity: quantity
        };
        $.ajax({
            url: "/card/add",
            data: obj,
            dataType: "json",
            type: "post",
            success: function (response) {
                swal("", "", "success");
                updateCard(response.data);
            }
        })
    })

    //bag element remove

    $(".bag-inner").on("click", ".bag-delete .fa-trash-alt", function (e) {
        console.log("test");
        e.preventDefault();
        var id = $(this).parents(".ci-item").data("id");
        var sizeId = $(this).parents(".ci-item").data("sizeid");
        var obj = {
            id: id,
            sizeId: sizeId
        }
        console.log("clicked")
        swal({
            title: "Are you sure?",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: "/card/remove",
                        data: obj,
                        dataType: "json",
                        success: function (response) {
                            if (response.status == 200) {
                                updateCard(response.data)
                            }
                        }
                    })
                }
            });
    });

    $(".bag-delete .fa-trash-alt").click(function (e) {
        console.log("clicked");
    })

    //size change get quantity
    $(".sizing select[name='size']").change(function () {
        var sizeId = $(this).val();
        var id = $(".product_info input[name='productId']").val()
        var obj = {
            sizeId: sizeId,
            id: id
        }
        $.ajax({
            url: "/product/fillquantity",
            data: obj,
            dataType: "json",
            type: "post",
            success: function (response) {
                if (response.status == 200) {
                    $(".quantity select[name='quantity']").empty()
                    for (var i = 0; i < response.data; i++) {
                        var option = $(" <option value=" + (i + 1) + ">" + (i + 1) + "</option>")
                        $(".quantity select[name='quantity']").append(option)
                    }
                }
            }
        })
    });

    //Wishlist
    $(".likeitem").click(function (e) {
        console.log("asd")
        e.preventDefault();
        var id = 0;
        if ($(this).parents(".product-item").data("id") != null) {
            id = $(this).parents(".product-item").data("id");
        } else {
            id = $(this).parents(".single-product-item").data("id");
        }
        console.log(id);
        if ($(this).hasClass("active")) {
            $.ajax({
                url: "/wishlist/remove/" + id,
                type: "post",
                dataType: "json",
                success: function (response) {
                }
            });
        }
        else {
            $.ajax({
                url: "/wishlist/add/" + id,
                type: "post",
                dataType: "json",
                success: function (response) {
                }
            })
        }

        $(this).toggleClass("active");

    })

    $(".wishlist-delete").click(function (e) {
        e.preventDefault()
        var el = $(this).parents(".wishlist-item")
        var id = $(this).parents(".product-item").data("id");
        swal({
            title: "Are you sure?",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: "wishlist/remove/" + id,
                        type: "post",
                        dataType: "json",
                        success: function (response) {
                            el.remove()
                        }
                    })
                }
            });
    })

    //LoadMore
    $(".load-button").click(function (e) {
        e.preventDefault();
        var subcategoryId = $(".product-list-wrapper").data("subcategoryid")
        var categoryId = $(".product-list-wrapper").data("categoryid")
        var obj = {
            subcategoryId: subcategoryId,
            categoryId: categoryId
        }
        $.ajax({
            url: "/category/loadmore",
            dataType: "json",
            data: obj,
            success: function (response) {

                for (var product of response.data.list) {

                    //var photoPath = "";
                    //for (var photo of response.data.productPhotos) {
                    //    if (product.product.id == photo.product.id) {
                    //        photoPath += photo.photoPath;
                    //        break;
                    //    }
                    //}
                    //var discoutn_price = "";
                    //if (product.discountPercent != 0) {
                    //    discoutn_price = `<span class="product-price-discount">${Math.floor((product.price - (product.price * product.discountPercent / 100)))} &#x20bc;</span>`
                    //}
                    var wishlistactive = ""
                    if (response.data.WishlistCards === undefined) {
                        wishlistactive = ""
                    }
                    else if (response.data.WishlistCards.find(product.Id) != null) {
                        wishlistactive = "active"
                    }

                    var prd = `<div class="col-lg-3 col-md-4 col-sm-6 product-item" data-id="${product.id}">
                    <div class="shadow-wrapper mx-auto">
                        <div class="product-img d-flex justify-content-center">
                            <a href="product/index/${product.id}">
                                <img src="/Admin/Uploads/Products/${product.productPhotos[0].photoPath}" alt="product">
                                    <span class="discount" ${product.discountPercent == 0 ? `style="display:none;"` : `""`}>${product.discountPercent} %</span>
                            </a>
                            <span class="wishlist likeitem ${wishlistactive}"><i class="far fa-heart"></i></span>
                        </div>
                        <div class="product-info my-2">
                            <a href="product/index/${product.id}">${product.name}</a>
                            <p>${product.price} &#x20bc;</p>
                        </div>
                    </div>
                </div>`
                    $(".product-list-wrapper").append(prd);

                }
                var prdCount = $(".product-item").length
                $(".viewed p").html("You've viewed " + prdCount + " of " + response.data.total + " products")
            }
        })
    })

    $(".cart-table .fa-trash-alt").click(function (e) {
        e.preventDefault();
        var tr = $(this).parents(".ck-item")
        var id = $(this).parents(".ck-item").data("id")
        var sizeId = $(this).parents(".ck-item").data("sizeid")
        var obj = {
            id: id,
            sizeId: sizeId
        }
        swal({
            title: "Are you sure?",
            text: "",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        url: "/card/remove",
                        data: obj,
                        dataType: "json",
                        success: function (response) {
                            if (response.status == 200) {
                                updateCard(response.data);
                                tr.remove();
                                $(this).parents(".ck-item").find(".item-price.total").html("" + response.data.subtotal + " &#x20bc;")

                                console.log(response.data.withoutShipping);
                                if (reponse.data.withoutShipping != undefined) {
                                    $(".proceed-to-checkout p:first-child").html("Subtotal:" + response.data.withoutShiping + "  &#x20bc;")
                                } else {
                                    console.log("girdi");
                                    $(".proceed-to-checkout p:first-child").html("Subtotal:" + response.data.total + "  &#x20bc;")
                                }
                                $(".proceed-to-checkout p.total").html("Subtotal:" + response.data.total + "  &#x20bc;")

                            }
                        }
                    })
                }
            });
    })

    $("select[name='quantity']").change(function (e) {
        var id = $(this).parents(".ck-item").data("id");
        var sizeId = $(this).parents(".ck-item").data("sizeid")
        var tableSubtotal = $(this).parents(".ck-item").find(".item-price.subtotal");
        var subtotal = $(".proceed-to-checkout p.subtotal");
        var key = $("#City").val()
        var total = $(".proceed-to-checkout p.total");
        var qty = parseInt($(this).val());
        var obj = {
            key: key,
            id: id,
            sizeid: sizeId,
            qty: qty
        }
        $.ajax({
            url: "/card/changeqty",
            dataType: "json",
            data: obj,
            type: "post",
            success: function (response) {
                if (response.status == 200) {
                    updateCard(response.data);
                    tableSubtotal.html("" + response.data.subtotal + " &#x20bc;");
                    $(".ck-item .item-price.total").html(response.data.subtotal + " &#x20bc; ")
                        subtotal.html("Subtotal: &nbsp;" + response.data.subtotal + " &#x20bc;");
                   
                    total.html("Total: &nbsp;" + response.data.total + "  &#x20bc;");
                }
            }

        });
    });

    //Functions
    function updateCard(data) {
        $(".bag-count p span").text(data.count)
        $(".bag-total").html("<span>Subtotal: " + data.total + "  &#x20bc;</span>")
        $(".ci-item").remove();

        $.each(data.list, function (key, value) {
            var div = `<div class="my-2 ci-item" data-id="${value.id}" data-sizeId="${value.sizeId}">
                                  <div class="row">
  <div class="col-3 p-0 bag-inner-photo">
                                                            <img src="${value.photo}" alt="foto">
                                                        </div>
                                                        <div class="col-9 bag-inner-info">
                                                            <h4>${value.name}</h4>
                                                            <p>${value.quantity} X ${value.price}  &#x20bc;</p>
                                                            <div class="bag-controls">
                                                                <a href="#">
                                                                    <span class="bag-edit">
                                                                        <i class="fas fa-edit"></i>
                                                                    </span>
                                                                </a>
                                                                <a href="#">
                                                                    <span class="bag-delete">
                                                                        <i class="far fa-trash-alt"></i>
                                                                    </span>
                                                                </a>
                                                            </div>
                                                        </div></div>
                                                    </div>`
            $(".bag-inner .container-fluid").append(div);
        })

        var count = $(".bag-inner .container-fluid").children().length;
        $(".right-navigation-list .mybadge").text(count);

    }




})