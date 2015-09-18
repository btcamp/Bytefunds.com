/* Theme Name: The Project - Responsive Website */
jQuery(function ($) {
    var ByteFunds = window.ByteFunds || {};
    $(document).ready(function () {
        $(".qqService").click(function () { ByteFunds.qqService(); });
        $(".qq").click(function () { ByteFunds.qqService(); });
        ByteFunds.ObtainNews1();
        ByteFunds.ObtainNews2();
        ByteFunds.hiddenModal();
        ByteFunds.showModal();
        ByteFunds.showCircle();
        //ByteFunds.showCharts();
    });
    ByteFunds.qqService = function (qq) {
        var qq_list = new Array("578485754","3226588475");
        //随机
        var qq_i = Math.floor(Math.random() * qq_list.length);
        if (!qq) {
            qq = qq_list[qq_i];
        }
        var element = "<iframe style='display:none;' class='qq_iframe' src='tencent://message/?uin=" + qq + "&Site=&menu=yes'></iframe>";
        $("body").append(element);
    };
    ByteFunds.ObtainNews1 = function () {
        var trs = $(".news-content-1").find("tr");
        if (trs.length) {
            var trIndex = 0;
            var date = trs.eq(trIndex).children("td").eq(0).html();
            var url = trs.eq(trIndex).children("td").eq(1).html();
            var content = trs.eq(trIndex).children("td").eq(2).html();
            var temp = '<li class="animated fadeInUp"><span>' + date + '</span><a href="' + url + '">' + content + '</a></li>';
            $("#news-1").html(temp);
            setInterval(function () {
                trIndex++;
                if (trIndex >= trs.length) {
                    trIndex = 0;
                }
                var date = trs.eq(trIndex).children("td").eq(0).html();
                var url = trs.eq(trIndex).children("td").eq(1).html();
                var content = trs.eq(trIndex).children("td").eq(2).html();
                var temp = '<li class="animated fadeInUp"><span>' + date + '</span><a href="' + url + '">' + content + '</a></li>';
                $("#news-1").html(temp);
            }, 6000);
        }

    };
    ByteFunds.ObtainNews2 = function () {
        var trs = $(".news-content-2").find("tr");
        if (trs.length) {
            var trIndex = 0;
            var time = trs.eq(trIndex).children("td").eq(0).html();
            var account = trs.eq(trIndex).children("td").eq(1).html();
            if (account) {
                account = account.replace(/(.{3}).*(.{3})/, "$1*****$2");
            }
            var content = trs.eq(trIndex).children("td").eq(2).html();
            var temp = '<li class="animated fadeInUp"><span>' + time + '</span>用户: <span>' + account + '</span><span>' + content + '</span></li>';
            $("#news-2").html(temp);
            setInterval(function () {
                trIndex++;
                if (trIndex >= trs.length) {
                    trIndex = 0;
                }
                var time = trs.eq(trIndex).children("td").eq(0).html();
                var account = trs.eq(trIndex).children("td").eq(1).html();
                if (account) {
                    account = account.replace(/(.{3}).*(.{3})/, "$1*****$2");
                }
                var content = trs.eq(trIndex).children("td").eq(2).html();
                var temp = '<li class="animated fadeInUp"><span>' + time + '</span>用户: <span>' + account + '</span><span>' + content + '</span></li>';
                $("#news-2").html(temp);
            }, 5000);
        }
    };
    ByteFunds.hiddenModal = function () {
        var btn = $(".closed");
        var modal = $(".modal");
        btn.click(function () {
            modal.attr("class", "modal animated fadeOut hidden");
        })
    };
    ByteFunds.showModal = function () {
        var btn = $(".reservation-btn");
        var modal = $(".modal");
        btn.click(function () {
            modal.attr("class", "modal animated fadeIn");
        })
    };
    ByteFunds.showCircle = function () {
        var indicatorContainer = $('#indicatorContainer');
        if (indicatorContainer.length) {
            var value = $("#indicatorValue").val();
            $('#indicatorContainer').radialIndicator({
                barColor: '#F5B024',
                barWidth: 5,
                initValue: value,
                roundCorner: true,
                percentage: true,
                radius: 30,
                fontFamily: 'Calibri'
            });
        }
    };
    ByteFunds.showCharts = function () {
        var chart = $('#charts');
        if (chart.length) {
            $('#charts').highcharts({
                chart: {
                },
                title: {
                    text: '零钱计划第一季 一周收益',
                    style: { color: "#F5B024", fontSize: "1.8em", fontFamily: '黑体', fontWeight: 600 }
                },
                xAxis: {
                    categories: ['09-06', '09-07', '09-08', '09-09', '09-10', '09-11', '09-12']
                },
                series: [{
                    type: 'spline',
                    name: '当日收益',
                    data: [0.121, 0.125, 0.123, 0.124, 0.131, 0.134, 0.135],
                    marker: {
                        lineWidth: 3,
                        lineColor: Highcharts.getOptions().colors[3],
                        fillColor: 'white'
                    },
                    itemStyle: { color: 'red' }
                }]
            });
        }
    }
});


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
        msg = "正在提交数据，请稍候...";
    }
    bootbox.dialog({
        title: $(document).attr('title'),
        message: '<img src="/assets/images/ajax-loader2.gif" style="float:left;margin:4px 5px 0 0" />' + msg,
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
                        if (data.RedirectUrl != null && data.RedirectUrl != "" && data.IsRedirect) {
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
                if (data.RedirectUrl != null && data.IsRedirect) {
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

