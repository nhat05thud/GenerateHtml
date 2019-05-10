$(window).resize(function () {
    $('body').css('min-height', $(window).height() - 90);
    $('.demo').css('min-height', $(window).height() - 160);
});
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
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
                                if (result.css != null && !$("link[href='/Uploads/css/" + result.css + "']").length) {
                                    $('<link rel="stylesheet" href="/Uploads/css/' + result.css + '" type="text/css" />')
                                        .appendTo('head');
                                }
                                if (result.script != null &&
                                    !$("script[src='/Uploads/scripts/" + result.script + "']").length) {
                                    $('<script src="/Uploads/scripts/' + result.script + '" />').appendTo('body');
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
});
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
            $this.parent().children(".view").find(".box.box-element").each(function() {
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

    var downloadContent = $('#download-layout').children();

    downloadContent.find('.preview, .configuration, .drag, .remove').remove();
    downloadContent.find('.lyrow').addClass('removeClean');
    //downloadContent.find('.box-element').addClass('removeClean');

    downloadContent.find('.lyrow .lyrow .lyrow .lyrow .lyrow .removeClean').each(function(){ cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .lyrow .lyrow .removeClean').each(function(){ cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .lyrow .removeClean').each(function(){ cleanHtml(this) });
    downloadContent.find('.lyrow .lyrow .removeClean').each(function(){ cleanHtml(this) });
    downloadContent.find('.lyrow .removeClean').each(function(){ cleanHtml(this) });
    downloadContent.find('.removeClean').each(function(){ cleanHtml(this) });

    downloadContent.find('.removeClean').remove();
	
    $('#download-layout .column').removeClass('ui-sortable');	
    downloadContent.find('.column').removeClass('column');

    if($('#download-layout .container').length > 0) {
        changeStructure('row-fluid','row');
    }

    var formatSrc = $.htmlClean($('#download-layout').html(), {
        format:true, 
        allowedAttributes:[
            ['id'], ["class"], ['data-toggle'], ['data-target'], ['data-parent'], ['role'], ['data-dismiss'], ['aria-labelledby'],
            ['aria-hidden'], ['data-slide-to'], ['data-slide']
        ] 
    });

    $('#download-layout').html(formatSrc);
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
                var data = {
                    websiteName
                }
                $.ajax({
                    type: "POST",
                    url: "/home/handledownloadhtml",
                    data: data,
                    success: function(result) {
                        if (result.success) {
                            swal("Success", result.message, "success");
                        } else {
                            swal("Error", result.message, "error");
                        }
                        $this.attr("disabled", false);
                        $(".loading_div").css("display", "none");
                    }
                });
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
    var data = {
        html,
        elementIds,
        websiteName,
        fileName
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