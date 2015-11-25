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
        ByteFunds.showUserList();
        ByteFunds.changeVersetList();
        ByteFunds.loadHeight();
        ByteFunds.loadBackgroundImg();
        ByteFunds.clickZan();

    });
    ByteFunds.qqService = function (qq) {
        var qq_list = new Array("578485754", "3226588475");
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
        var modal = $("#reservation-content");
        btn.click(function () {
            modal.attr("class", "modal animated fadeOut hidden");
        });
    };
    ByteFunds.showModal = function () {
        var btn = $(".reservation-btn");
        var modal = $("#reservation-content");
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
                chart: {},
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
    };
    ByteFunds.showUserList = function () {
        var userCenter = $(".user-center");
        var userBtn = $(".header-top-dropdown .uesr-btn");
        var flag = true, timeout;
        userBtn.click(
            function () {
                if (flag) {
                    userCenter.attr("class", "user-center list animated fadeInRight");
                    userBtn.css("outline", "");
                    flag = false;
                    timeout = setTimeout(function () {
                        userCenter.attr("class", "user-center list  hidden"); userBtn.css("outline", "#F5B024");
                        flag = true; clearTimeout(timeout);
                    }, 6000);
                }
                else {
                    userCenter.attr("class", "user-center list  hidden");
                    userBtn.css("outline", "#F5B024");
                    clearTimeout(timeout); flag = true;
                }
            }
        );
    };
    ByteFunds.changeVersetList = function () {
        var name_list = ["蒋", "沈", "韩", "李", "张", "王", "赵", "钱", "孙", "何", "王", "许", "秦", "朱", "郑", "冯", "易", "邓", "周", "杨", "曹", "孔", "汪", "杜", "范", "彭", "杜", "严", "袁", "唐"];
        var top = 0;
        var invest_tablle = $(".invest-div table");
        var dom = ""
        for (var i = 0; i < 7; i++) {
            var name_i = Math.floor(Math.random() * name_list.length);
            var name = name_list[name_i];
            var invest_tablle = $(".invest-div table");
            var money = Math.round(Math.random() * 200 + 1) * 100;
            var date = new Date().Format("yyyy-MM-dd");
            var h = new Date().getHours() - 1;
            if (h < 8) {
                h = 23;
            }
            var m = Math.round(Math.random() * 59);
            if (m < 10) {
                m = "0" + m;
            }
            var s = Math.round(Math.random() * 59);
            if (s < 10) {
                s = "0" + s;
            }
            var time = date + " " + h + ":" + m + ":" + s;
            dom += '<tr><td class="td1">' + name + '*</td><td class="td2">' + money + '.00元</td><td class="td3">' + time + '</td></tr>'
        }
        $(".invest-body").html(dom);
        setInterval(function () {
            var name_i = Math.floor(Math.random() * name_list.length);
            var name = name_list[name_i];
            var invest_tablle = $(".invest-div table");
            var money = Math.round(Math.random() * 200 + 1) * 100;
            var date = new Date().Format("yyyy-MM-dd");
            var h = new Date().getHours() - 1;
            if (h < 8) {
                h = 23;
            }
            var m = Math.round(Math.random() * 59);
            if (m < 10) {
                m = "0" + m;
            }
            var s = Math.round(Math.random() * 59);
            if (s < 10) {
                s = "0" + s;
            }
            var time = date + " " + h + ":" + m + ":" + s;
            invest_tablle.animate({
                top: "-30px"
            }, 1500, function () {
                invest_tablle.css("top", "0");
                var temp = '<tr><td class="td1">' + name + '*</td><td class="td2">' + money + '.00元</td><td class="td3">' + time + '</td></tr>'
                $(".invest-body").append(temp);
                $(".invest-body").children().eq(0).remove();
            });
        }, 3000);
    };
    ByteFunds.loadHeight = function () {
        var header = $("#header").innerHeight();
        var footer = $("#footer").innerHeight();
        var win = $(window).height();
        var height = win - footer - header;
        $(".loadPage").css({
            minHeight: height + "px",
        })
    }
    ByteFunds.loadBackgroundImg = function () {
        var bg = $(".loadBg");
        var url = ["/assets/images/bg01.jpg", "/assets/images/bg02.jpg", "/assets/images/bg03.jpg"];
        var i = Math.floor(Math.random() * url.length);
        if (bg.length > 0 && $(window).width() > 960) {
            bg.css({
                background: "url(" + url[i] + ")",
                backgroundSize: "100% 100%"
            })
        }
    }
    ByteFunds.clickZan = function () {
        var target = $(".money-box").find("a")
        for (var i = 0; i < target.length; i++) {
            target.eq(i).one("click", function () {
                var $this = $(this);
                var url = $this.attr("href");
                $.get(url, function (data) {
                    if (data.Success) {
                        //$this.find(".fonttext").text("已" + $this.find(".fonttext").text());
                        var left = parseInt($this.offset().left) + 92;
                        var top = parseInt($this.offset().top) - 5;
                        var element = '<div id="zan"><b>+1<\/b></div>';
                        var obj = $this;
                        obj.css("color", "#fff");
                        $("body").append(element);
                        $('#zan').css({
                            'position': 'absolute', 'z-index': '500', 'color': '#C30',
                            'left': left + 'px', 'top': top + 'px', fontSize: "16px"
                        });
                        $('#zan').animate({ top: top - 10, opacity: 0 }, 500,
                            function () {
                                $(element).fadeOut(600).remove();
                                var Num = parseInt(obj.find('span').text());
                                Num++;
                                obj.find('span').text(Num);
                            });
                    }
                    else {
                        finAlert(data.Msg, false);
                    }
                    $this.attr("href", "#");
                });

                return false;
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
        $(this).attr("disabled", true);
        return false;
    });
    $(".btnregister").click(function () {
        $("#myregisterModal").modal("show");
    });
    $(".newsbtn").click(function () {
        $.get("/umbraco/Api/News/Get?key=" + $(this).data("id"), function (result) {
            $("#myLargeModalLabel").text(result.title);
            $("#content").html(result.content);
            $("#myModal").modal("show");
        }, "json");
    });
    $(".productBtn").click(function () {
        $.get("/umbraco/Api/News/Get?key=" + $(this).data("id"), function (result) {
            $("#myProductModalLabel").text(result.title);
            $("#productcontent").html(result.content);
            if (result.canbuy) {
                $("#buyaction").show()
                $("#buyaction").attr("href", "/Pay?id=" + result.id);
            } else {
                $("#buyaction").hide();
            }
            $("#myModalproduct").modal("show");
        }, "json");
    });
    $('#myModal').on('hidden.bs.modal', function (e) {
        $("#myLargeModalLabel").text("比特金服");
        $("#content").html('<div><img style="margin:0 auto" src="/assets/images/loading.jpg" alt="loading" /></div>');
    });
    $("#myModalproduct").on('hidden.bs.modal', function (e) {
        $("#myProductModalLabel").text("产品信息");
        $("#productcontent").html('<div><img style="margin:0 auto" src="/assets/images/loading.jpg" alt="loading" /></div>');
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
                        var modal = $(".modal");
                        modal.attr("class", "modal animated fadeOut hidden");
                        $('#reservation-content input').val('');
                        $('#reservation-content textarea').val('');
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
Date.prototype.Format = function (formatStr) {
    var str = formatStr;
    var Week = ['日', '一', '二', '三', '四', '五', '六'];

    str = str.replace(/yyyy|YYYY/, this.getFullYear());
    str = str.replace(/yy|YY/, (this.getYear() % 100) > 9 ? (this.getYear() % 100).toString() : '0' + (this.getYear() % 100));
    var month = this.getMonth() + 1;
    str = str.replace(/MM/, month > 9 ? month.toString() : '0' + month);
    str = str.replace(/M/g, month);

    str = str.replace(/w|W/g, Week[this.getDay()]);

    str = str.replace(/dd|DD/, this.getDate() > 9 ? this.getDate().toString() : '0' + this.getDate());
    str = str.replace(/d|D/g, this.getDate());

    str = str.replace(/hh|HH/, this.getHours() > 9 ? this.getHours().toString() : '0' + this.getHours());
    str = str.replace(/h|H/g, this.getHours());
    str = str.replace(/mm/, this.getMinutes() > 9 ? this.getMinutes().toString() : '0' + this.getMinutes());
    str = str.replace(/m/g, this.getMinutes());

    str = str.replace(/ss|SS/, this.getSeconds() > 9 ? this.getSeconds().toString() : '0' + this.getSeconds());
    str = str.replace(/s|S/g, this.getSeconds());
    return str;
}
