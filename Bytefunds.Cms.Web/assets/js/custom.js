/* Theme Name: The Project - Responsive Website */
jQuery(function($){
    var ByteFunds = window.ByteFunds || {};
    $(document).ready(function(){
        $(".qqService").click(function(){ByteFunds.qqService();});
        $(".qq").click(function(){ByteFunds.qqService();});
        ByteFunds.ObtainNews1();
        ByteFunds.ObtainNews2();
        ByteFunds.hiddenModal();
        ByteFunds.showModal();
        ByteFunds.showCircle();
        ByteFunds.showCharts();
    });
    ByteFunds.qqService=function(qq){
        var qq_list = new Array("263662237","578485754");
        //随机
        var qq_i = Math.floor(Math.random()*qq_list.length);
        if(!qq){
            qq=qq_list[qq_i];
        }
        var element="<iframe style='display:none;' class='qq_iframe' src='tencent://message/?uin="+qq+"&Site=&menu=yes'></iframe>";
        $("body").append(element);
    };
    ByteFunds.ObtainNews1=function(){
        var trs=$(".news-content-1").find("tr");
        if(trs.length){
            var trIndex=0;
            var date=trs.eq(trIndex).children("td").eq(0).html();
            var url=trs.eq(trIndex).children("td").eq(1).html();
            var content=trs.eq(trIndex).children("td").eq(2).html();
            var temp='<li class="animated fadeInUp"><span>'+date+'</span><a href="'+url+'">'+content+'</a></li>';
            $("#news-1").html(temp);
            setInterval(function(){
                trIndex++;
                if(trIndex>=trs.length){
                    trIndex=0;
                }
                var date=trs.eq(trIndex).children("td").eq(0).html();
                var url=trs.eq(trIndex).children("td").eq(1).html();
                var content=trs.eq(trIndex).children("td").eq(2).html();
                var temp='<li class="animated fadeInUp"><span>'+date+'</span><a href="'+url+'">'+content+'</a></li>';
                $("#news-1").html(temp);
            },6000);
        }

    };
    ByteFunds.ObtainNews2=function(){
        var trs=$(".news-content-2").find("tr");
        if(trs.length){
            var trIndex=0;
            var time=trs.eq(trIndex).children("td").eq(0).html();
            var account=trs.eq(trIndex).children("td").eq(1).html();
            if(account){
                account=account.replace(/(.{3}).*(.{3})/,"$1*****$2");
            }
            var content=trs.eq(trIndex).children("td").eq(2).html();
            var temp='<li class="animated fadeInUp"><span>'+time+'</span>用户: <span>'+account+'</span><span>'+content+'</span></li>';
            $("#news-2").html(temp);
            setInterval(function(){
                trIndex++;
                if(trIndex>=trs.length){
                    trIndex=0;
                }
                var time=trs.eq(trIndex).children("td").eq(0).html();
                var account=trs.eq(trIndex).children("td").eq(1).html();
                if(account){
                    account=account.replace(/(.{3}).*(.{3})/,"$1*****$2");
                }
                var content=trs.eq(trIndex).children("td").eq(2).html();
                var temp='<li class="animated fadeInUp"><span>'+time+'</span>用户: <span>'+account+'</span><span>'+content+'</span></li>';
                $("#news-2").html(temp);
            },5000);
        }
    };
    ByteFunds.hiddenModal=function(){
        var btn=$(".closed");
        var modal=$(".modal");
        btn.click(function(){
            modal.attr("class","modal animated fadeOut hidden");
        })
    };
    ByteFunds.showModal=function(){
        var btn=$(".reservation-btn");
        var modal=$(".modal");
        btn.click(function(){
            modal.attr("class","modal animated fadeIn");
        })
    };
    ByteFunds.showCircle=function() {
        var indicatorContainer=$('#indicatorContainer');
        if(indicatorContainer.length){
            $('#indicatorContainer').radialIndicator({
                barColor: '#F5B024',
                barWidth: 5,
                initValue: 25.2,
                roundCorner : true,
                percentage: true,
                radius: 30,
                fontFamily: 'Calibri'
            });
        }
    };
    ByteFunds.showCharts=function() {
        var chart=$('#charts');
        if(chart.length){
            $('#charts').highcharts({
                chart: {
                },
                title: {
                    text: '零钱计划第一季 一周收益',
                    style:{color:"#F5B024",fontSize:"1.8em",fontFamily: '黑体',fontWeight:600}
                },
                xAxis: {
                    categories: ['09-06', '09-07', '09-08', '09-09', '09-10','09-11','09-12']
                },
                series: [  {
                    type: 'spline',
                    name: '当日收益',
                    data: [0.121, 0.125, 0.123, 0.124, 0.131, 0.134, 0.135],
                    marker: {
                        lineWidth:3,
                        lineColor: Highcharts.getOptions().colors[3],
                        fillColor: 'white'
                    },
                    itemStyle:{color:'red'}
                }]
            });
        }
    }
});

