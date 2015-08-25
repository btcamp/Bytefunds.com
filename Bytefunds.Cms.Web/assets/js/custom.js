$(function () {
    if (window.location.href.indexOf("Admin") >= 0) {

        $("body").css("min-height", $("html").height() + "px");
    }

    initAajxform();
    $(".logout").click(function () {
        ajaxSubmit(this.href, null, "正在退出....");
        return false;
    });
});

function modalLoading(msg) {
    if ('undefined' == typeof (document.body.style.maxHeight)) {
        return;
    }
    if (msg == null) {
        msg = "正在提交数据，请稍候";
    }
    bootbox.dialog({
        title: $(document).attr('title'),
        message: '<img src="/assets/images/ajax-loader2.gif" style="display:inline-block" />' + msg,
        animate: false,
        buttons: {
        }
    });
}
function modalConfirm(title, message, callback) {
    bootbox.dialog({
        message: message,
        title: title,
        buttons: {
            success: {
                label: "取消",
                className: "btn-success",
                callback: function () {
                    callback(false);
                }
            },
            main: {
                label: "确定",
                className: "btn-primary",
                callback: function () {
                    callback(true);
                }
            }
        }
    });
}

function finAlert(message, issuccess, config) {
    if ('undefined' == typeof (document.body.style.maxHeight)) {
        alert(message);
        return;
    }
    Messenger.options = {
        extraClasses: 'messenger-fixed messenger-theme-flat messenger-on-top',
        theme: 'future'
    }
    var msgConfig = $.extend({
        message: message,
        type: 'error',
        hideAfter: 10,
        hideOnNavigate: true,
        showCloseButton: true
    }, config);
    if (issuccess == false) {
        Messenger().post(msgConfig);
    }
    else {
        msgConfig.type = "success";
        Messenger().post(msgConfig);
    }
}

function initAajxform() {
    $("form").each(function (index, ele) {
        if ($(ele).attr("id") != "search") {
            $(ele).ajaxForm({
                beforeSubmit: function () {
                    if ($(ele).valid()) {
                        modalLoading();
                    } else {
                        return false;
                    }
                },
                error: function () {
                    bootbox.hideAll();
                    finAlert("提交数据过程中出现错误，请检查数据后重试提交", false);
                },
                dataType: "JSON",
                success: function (data) {
                    bootbox.hideAll();
                    if (data.Success) {
                        finAlert(data.Msg, true);
                        if (data.RedirectUrl != null && data.RedirectUrl != "") {
                            setTimeout(function () {
                                location = data.RedirectUrl;
                            }, 1000);
                        }
                        if ($("#myModal").length > 0) {
                            $("#myModal").modal("hide");
                        }
                    }
                    else {
                        finAlert(data.Msg, false);
                    }
                }
            });
        }
    });
}

function ajaxSubmit(url, data, beforAjaxMsg) {
    $.ajax({
        url: url,
        data: data,
        beforeSend: function myfunction() {
            beforAjaxMsg = beforAjaxMsg == null ? "正在提交数据...." : beforAjaxMsg;
            modalLoading(beforAjaxMsg);
        },
        error: function () {
            bootbox.hideAll();
            finAlert("提交数据过程中出现错误，请检查数据后重试提交", false);
        },
        dataType: "json",
        success: function (data) {
            bootbox.hideAll();
            if (data.Success) {
                finAlert(data.Msg, true);
                if (data.RedirectUrl != null) {
                    setTimeout(function () {
                        location = data.RedirectUrl;
                    }, 1000);
                }
            }
            else {
                finAlert(data.Msg, false);
            }
        }
    });
}