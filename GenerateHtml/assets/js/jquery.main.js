$(window).resize(function () {
    $('body').css('min-height', $(window).height() - 90);
    $('.demo').css('min-height', $(window).height() - 160);
});
$(document).ready(function () {
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
    /* drag and drop boxes */
    $(".sidebar-nav .box").draggable({
        connectToSortable: ".column",
        helper: "clone",
        handle: ".drag",
        drag: function (e, ui) {
            ui.helper.width(400);
        },
        stop: function (e, ui) {
            var element = e.target;
            var id = $(element).data("id");
            var viewHtml = $(element).find(".view").text();
            if (viewHtml.trim() === "") {
                $(".loading_div").css("display", "block");
                var data = {
                    elementId: id
                }
                $.ajax({
                    type: "POST",
                    url: "/home/testloadhtmlpage",
                    data: data,
                    success: function (result) {
                        $(element).find(".view").html(result.html);
                        $(".demo .box.box-element[data-id=" + id + "]").find(".view").html(result.html);
                        if (result.css != null && !$("link[href='/Uploads/css/" + result.css + "']").length) {
                            $('<link rel="stylesheet" href="/Uploads/css/' + result.css + '" type="text/css" />').appendTo('head');
                        }
                        if (result.script != null && !$("script[src='/Uploads/scripts/" + result.script + "']").length) {
                            $('<script src="/Uploads/scripts/' + result.script + '" />').appendTo('body');
                        }
                        arrIds.push(id);
                        $("#hdnElementIds").val(arrIds);
                        $(".loading_div").css("display", "none");
                    }
                });
            }
        }
    });

    $('#clear').click(function (e) {
        e.preventDefault();
        clearDemo();
    });
    removeElm();
    gridSystemGenerator();
});

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


function removeElm() {
    $('.demo').on('click', '.remove', function (e) {
        e.preventDefault();
        $(this).parent().remove();
        if (!$('.demo .lyrow').length > 0) {
            clearDemo();
        }
        var dataId = $(this).parent().data("id");
        var id = dataId.toString();
        var arrIds = $("#hdnElementIds").val().split(",");
        if ($(".demo .box.box-element[data-id=" + dataId + "]").length <= 0) {
            var index = arrIds.indexOf(id);
            if (index > -1) {
                arrIds.splice(index, 1);
            }
            $("#hdnElementIds").val(arrIds);
        }
    });
}

function clearDemo() {
    $('.demo').empty();
}
function removeMenuClasses() {
    $('#menu-layoutit li button').removeClass('active');
}