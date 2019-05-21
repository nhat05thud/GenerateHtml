$(window).resize(function () {
    $('body').css('min-height', $(window).height() - 90);
    $('.demo').css('min-height', $(window).height() - 160);
});
$(document).ready(function () {
    $(".builder .sidebar-nav").niceScroll();
    $(".builder .sidebar-nav .nav-parent > ul").niceScroll();
    $(".builder .sidebar-nav .nav-parent p").click(function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
        } else {
            $(".builder .sidebar-nav .nav-parent p").removeClass("active");
            $(this).addClass("active");
        }
    });
    $(".fancybox__item").fancybox({
        transitionEffect: "slide",
        loop: true
    });
    var arrIds = [];
    $('body').css('min-height', $(window).height() - 90);
    $('.demo').css('min-height', $(window).height() - 160);
    /* sortables */
    $(".demo, .demo .column").sortable({
        connectWith: '.column',
        opacity: 0.35,
        handle: ".drag"
    });
    /* drag and drop rows */
    $(".sidebar-nav .lyrow").draggable({
        connectToSortable: ".demo",
        helper: "clone",
        handle: ".drag",
        drag: function (e, ui) {
            ui.helper.width(400);
        },
        stop: function (e, ui) {
            $('.demo .column').sortable({
                opacity: 0.35,
                connectWith: '.column'
            });
        }
    });
    /* drag and drop rows */
    $(".sidebar-nav .contentrow").draggable({
        connectToSortable: ".demo",
        helper: "clone",
        handle: ".drag",
        drag: function (e, ui) {
            ui.helper.width(400);
        },
        stop: function (e, ui) {
            $('.demo .column').sortable({
                opacity: 0.35,
                connectWith: '.column'
            });
        }
    });
    /* drag and drop boxes */
    $(".sidebar-nav .box").draggable({
        connectToSortable: ".column",
        helper: "clone",
        handle: ".drag",
        drag: function (e, ui) {
            ui.helper.width(400);
            $(".builder .sidebar-nav .nav-parent p").removeClass("active");
        },
        stop: function (e, ui) {
            if (ui.helper[0].isConnected === false) {
                var element = e.target;
                var id = $(element).data("id");
                if (id != null) {
                    var viewHtml = $(element).find(".view").text();
                    if (viewHtml.trim() === "") {
                        $(".loading_div").css("display", "block");
                        var data = {
                            elementId: id
                        }
                        $.ajax({
                            type: "POST",
                            url: "/home/loadhtmlcomponent",
                            data: data,
                            success: function (result) {
                                $(element).find(".view").html(result.html);
                                $(".demo .box.box-element[data-id=" + id + "]").find(".view").html(result.html);
                                if (result.css != null && $("link[href='/Uploads/css/" + result.css + "']").length <= 0) {
                                    $("head").append('<style type="text/css">' + result.css + '</style>');
                                }
                                if (result.script != null && $("script[src='/Uploads/scripts/" + result.script + "']").length <= 0) {
                                    $("body").append('<script src="/Uploads/scripts/' + result.script + '" type="text/javascript"></script>');
                                }
                                $(".loading_div").css("display", "none");
                            }
                        });
                    }
                    arrIds.push(id);
                    var arrIdsDistinct = unique(arrIds);
                    $("#hdnElementIds").val(arrIdsDistinct);
                }
            }
        }
    });
    $('#clear').click(function (e) {
        e.preventDefault();
        clearDemo();
    });
    removeElm();
    gridSystemGenerator();
    imageGenerator();
    contentWidthGenerator();

    var arr;

    $(".wrap-plugins label").on("change",
        function () {
            if ($("#hdnPluginIds").val() === "") {
                arr = [];
            } else {
                arr = $("#hdnPluginIds").val().split(",");
            }
            var value = $(this).children("input").val();
            var index = arr.indexOf(value);
            if (index > -1) {
                arr.splice(index, 1);
            } else {
                arr.push(value);
            }
            $("#hdnPluginIds").val(arr);
        });
});
function activeWrapPlugins(e) {
    $(e).next().toggleClass("active");
}
function unique(list) {
    var result = [];
    $.each(list, function (i, e) {
        if ($.inArray(e, result) === -1) result.push(e);
    });
    return result;
}
function gridSystemGenerator() {
    $('.lyrow .preview input').bind('keyup', function () {
        var sum = 0;
        var src = '';
        var invalidValues = false;
        var cols = $(this).val().split(" ", 12);
        $.each(cols, function (index, value) {
            if (!invalidValues) {
                if (parseInt(value) <= 0) invalidValues = true;
                sum = sum + parseInt(value);
                src += '<div class="col-md-' + value + ' column"></div>';
            }
        });
        if (sum === 12 && !invalidValues) {
            $(this).parent().next().children().html(src);
            $(this).parent().prev().show();
        } else {
            $(this).parent().prev().hide();
        }
    });
}
function contentWidthGenerator() {
    var width = 0;
    var src = "";
    $(".contentrow .preview input").bind("keyup", function () {
        width = parseInt($(this).val());
        src = '<div class="custom__width column" style="width:' + width + 'px"></div>';
        if (width > 0) {
            $(this).parent().next().html(src);
            $(this).parent().prev().show();
        } else {
            $(this).parent().prev().hide();
        }
    });
}
function imageGenerator() {
    var width = 0;
    var height = 0;
    $(".image-row .preview input#input-width").bind("keyup", function () {
        width = parseInt($(this).val());
        checkImageSize($(this), width, height);
    });
    $(".image-row .preview input#input-height").bind("keyup", function () {
        height = parseInt($(this).val());
        checkImageSize($(this), width, height);
    });
}
function checkImageSize(element, width, height) {
    if (width > 0 && height > 0) {
        var src = "<img class='img-fluid' src='https://via.placeholder.com/" + width + "x" + height + "' />";
        element.parent().next().html(src);
        element.parent().prev().show();
    } else {
        element.parent().prev().hide();
    }
}
function removeElm() {
    $('.demo').on('click', '.remove', function (e) {
        e.preventDefault();
        var $this = $(this);
        $this.parent().remove();
        if (!$('.demo .lyrow').length > 0) {
            clearDemo();
            $("#hdnElementIds").val("");
        }
        if ($this.parent().children(".view").find(".box.box-element").length > 0) {
            $this.parent().children(".view").find(".box.box-element").each(function () {
                removeHdnElementIds($(this).data("id"));
            });
        }
        else {
            removeHdnElementIds($this.parent().data("id"));
        }
    });
}
function removeHdnElementIds(dataId) {
    if (dataId != null) {
        var id = dataId.toString();
        var arrIds = $("#hdnElementIds").val().split(",");
        if ($(".demo .box.box-element[data-id=" + dataId + "]").length <= 0) {
            var index = arrIds.indexOf(id);
            if (index > -1) {
                arrIds.splice(index, 1);
            }
            $("#hdnElementIds").val(arrIds);
        }
    }
}
function clearDemo() {
    $('.demo').empty();
}
function removeMenuClasses() {
    $('#menu-layoutit li button').removeClass('active');
}
function changeStructure(oldClass, newClass) {
    $('#download-layout .' + oldClass).removeClass(oldClass).addClass(newClass);
}
function cleanHtml(elm) {
    $(elm).parent().append($(elm).children().html());
}
function downloadLayoutSrc() {
    var src = '';
    $('#download-layout').html($('.demo').html());

    var downloadContent = $('#download-layout');

    downloadContent.find('.preview, .configuration, .drag, .remove').remove();
    downloadContent.find('.lyrow').addClass('removeClean');
    downloadContent.find('.box-element').addClass('removeClean');

    downloadContent.find('.lyrow .lyrow .lyrow .lyrow .lyrow .removeClean').each(function () { cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .lyrow .lyrow .removeClean').each(function () { cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .lyrow .removeClean').each(function () { cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .removeClean').each(function () { cleanHtml(this) });
    downloadContent.find('.lyrow .removeClean').each(function () { cleanHtml(this) });
    downloadContent.find('.removeClean').each(function () { cleanHtml(this) });

    downloadContent.find('.removeClean').remove();

    $('#download-layout .column').removeClass('ui-sortable');
    downloadContent.find('.column').removeClass('column');

    if ($('#download-layout .container').length > 0) {
        changeStructure('row-fluid', 'row');
    }

    //var formatSrc = $.htmlClean($('#download-layout').html(), {
    //    format:true, 
    //    allowedAttributes:[
    //        ['id'], ["class"], ['data-toggle'], ['data-target'], ['data-parent'], ['role'], ['data-dismiss'], ['aria-labelledby'],
    //        ['aria-hidden'], ['data-slide-to'], ['data-slide'], ['style']
    //    ] 
    //});

    //$('#download-layout').html(formatSrc);
}
function download(element) {
    var $this = $(element);
    $this.attr("disabled", true);
    swal({
        title: "Are you sure?",
        text: "Hãy download khi đã tạo đủ file html !!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    })
        .then((willDownload) => {
            if (willDownload) {
                $(".loading_div").css("display", "block");
                var websiteName = $("#downloadModal #websiteName").val();
                if (websiteName.trim() === "") {
                    swal("Error", "Nhập tên website", "error");
                }
                else {
                    var data = {
                        websiteName
                    }
                    $.ajax({
                        type: "POST",
                        url: "/home/handledownloadhtml",
                        data: data,
                        success: function (result) {
                            if (result != null && result.success === false) {
                                swal("Error", result.message, "error");
                            } else {
                                swal("Success", "Download success", "success");
                            }
                            $this.attr("disabled", false);
                            $("#downloadModal").modal("hide");
                            $(".loading_div").css("display", "none");
                        }
                    });
                }
            } else {
                $this.attr("disabled", false);
            }
        });
}
function savehtmlfile(element) {
    $(".loading_div").css("display", "block");
    var $this = $(element);
    $this.attr("disabled", true);
    downloadLayoutSrc();
    var html = $('#download-layout').html();
    var elementIds = $("#hdnElementIds").val();
    var websiteName = $("#downloadModal #websiteName").val();
    var fileName = $("#downloadModal #fileName").val();
    var pluginIds = $("#hdnPluginIds").val();
    if (websiteName.trim() === "") {
        swal("Error", "Nhập tên website", "error");
    }
    else {
        if (fileName.trim() === "") {
            swal("Error", "Nhập tên file", "error");
        }
        else {
            var data = {
                html,
                elementIds,
                websiteName,
                fileName,
                pluginIds
            }
            $.ajax({
                type: "POST",
                url: "/home/handlesavehtmlfile",
                data: data,
                success: function (result) {
                    if (result.success) {
                        swal("Success", result.message, "success");
                    } else {
                        swal("Error", result.message, "error");
                    }
                    $this.attr("disabled", false);
                    $(".loading_div").css("display", "none");
                }
            });
        }
    }
}
function handleAddPlugins(element) {
    var $this = $(element);
    $this.attr("disabled", true);
    $(".loading_div").css("display", "block");
    var pluginIds = $("#hdnPluginIds").val();
    var data = {
        pluginIds
    }
    $.ajax({
        type: "POST",
        url: "/home/HandleAddPlugins",
        data: data,
        success: function (result) {
            if (result !== null) {
                if (result.success) {
                    if (result.css != null) {
                        var cssLength = result.css.length;
                        for (var i = 0; i < cssLength; i++) {
                            if ($("link[href='" + result.css[i] + "']").length <= 0) {
                                $("head").append("<link rel='stylesheet' href='" + result.css[i] + "' />");
                            }
                        }
                    }
                    if (result.script != null) {
                        var scriptLength = result.script.length;
                        for (var i = 0; i < scriptLength; i++) {
                            if ($("script[src='" + result.script[i] + "']").length <= 0) {
                                $("body").append('<script src="' + result.script[i] + '" type="text/javascript"></script>');
                            }
                        }
                    }
                    $(".loading_div").css("display", "none");
                    swal("Success", "Thêm plugin thành công", "success");
                } else {
                    swal("Error", "Thêm plugin lỗi", "error");
                }
            }
            $(".wrap-plugins").removeClass("active");
            $this.attr("disabled", false);
            $(".loading_div").css("display", "none");
        }
    });
}