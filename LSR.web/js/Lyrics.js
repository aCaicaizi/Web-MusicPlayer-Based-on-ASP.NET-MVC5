////var lineNo = 0;//当前行
////var C_pos = 6; //C位
////var offset = -20; //滚动距离（应等于行高）
//////var audio = document.getElementById("audio");//播放器
////var ul = document.getElementById("lyric"); //歌词容器列表

//////高亮显示歌词当前行及文字滚动控制，行号为lineNo
////function lineHigh() {
////	var lis = ul.getElementsByTagName("li");//歌词数组
////	if(lineNo>0){
//// 		lis[lineNo-1].removeAttribute("class");//去掉上一行的高亮样式
////	}
////	lis[lineNo].className = "lineHigh";//高亮显示当前行

////	//文字滚动
////	if(lineNo>C_pos){
////		ul.style.transform = "translateY("+(lineNo-C_pos)*offset+"px)"; //整体向上滚动一行高度
////	}
////}

//////滚回到开头，用于播放结束时
////function goback() {
////	document.querySelector("#lyric .lineHigh").removeAttribute("class");
////	ul.style.transform = "translateY(0)";
////	lineNo = 0;
////}

//////监听播放器的timeupdate事件，实现文字与音频播放同步
////ap.audio.on('timeupdate',function () {
////    if(lineNo==oLRC.ms.length)
////		return;
////	var curTime = ap.audio.currentTime; //播放器时间
////	if(parseFloat(oLRC.ms[lineNo].t)<=curTime){
////		lineHigh();//高亮当前行
////		lineNo++;
////	}
////});

//////监听播放器的ended事件，播放结束时回滚歌词
////ap.audio.on('ended',function () {
////    goback(); //回滚歌词
////});
var medisArray = new Array();// 定义一个新的数组

function parse(lrc_s) {
    if (lrc_s) {
        lrc_s = lrc_s.replace(/([^\]^\n])\[/g, (match, p1) => p1 + '\n[');
        const lyric = lrc_s.split('\n');
        const lyricLen = lyric.length;
        for (let i = 0; i < lyricLen; i++) {
            // match lrc time
            const lrcTimes = lyric[i].match(/\[(\d{2}):(\d{2})(\.(\d{2,3}))?]/g);
            // match lrc text
            const lrcText = lyric[i]
                .replace(/.*\[(\d{2}):(\d{2})(\.(\d{2,3}))?]/g, '')
                .replace(/<(\d{2}):(\d{2})(\.(\d{2,3}))?>/g, '')
                .replace(/^\s+|\s+$/g, '');

            if (lrcTimes) {
                // handle multiple time tag
                const timeLen = lrcTimes.length;
                for (let j = 0; j < timeLen; j++) {
                    const oneTime = /\[(\d{2}):(\d{2})(\.(\d{2,3}))?]/.exec(lrcTimes[j]);
                    const min2sec = oneTime[1] * 60;
                    const sec2sec = parseInt(oneTime[2]);
                    const msec2sec = oneTime[4] ? parseInt(oneTime[4]) / ((oneTime[4] + '').length === 2 ? 100 : 1000) : 0;
                    const lrcTime = min2sec + sec2sec + msec2sec;
                    medisArray.push({ t: lrcTime, c: lrcText });
                    
                }
            }
        }
        // sort by time
        //lrc = lrc.filter((item) => item[1]);
        //lrc.sort((a, b) => a[0] - b[0]);
    }
}
function createLrc() {
    medisArray = new Array();
    var medis = document.getElementById('lrc_content').innerText;
    parse(medis);
    var ul = $("#lyric");
    ul.empty();
    // 遍历medisArray，并且生成li标签，将数组内的文字放入li标签
    $.each(medisArray, function (i, item) {
        var li = $("<li style='list-style: none;'>");
        li.html(item.c);
        ul.append(li);
    });
    $(ul.find("li").get(2)).addClass('linenotnearheight');
    $(ul.find("li").get(1)).addClass('linenearheight');
    $(ul.find("li").get(0)).addClass("lineheight");
}
createLrc();
var fraction = 0.5;
var topNum = 0;
function lineHeight(lineno) {
    var ul = $("#lyric");
    var ulEl = document.getElementById('lyric');
    // 令正在唱的那一行高亮显示
    $('#lyric>li').removeAttr('class');

    $(ul.find("li").get(topNum + lineno - 2)).addClass('linenotnearheight');
    $(ul.find("li").get(topNum + lineno + 2)).addClass('linenotnearheight');
    $(ul.find("li").get(topNum + lineno - 1)).addClass('linenearheight');
    $(ul.find("li").get(topNum + lineno + 1)).addClass('linenearheight');
    var nowline = ul.find("li").get(topNum + lineno);
    $(nowline).addClass("lineheight");

    // 实现文字滚动
    var _scrollTop;
    //if ($ul.clientHeight * fraction > nowline.offsetParent.offsetTop) {
    //    _scrollTop = 0;
    //} else if (nowline.offsetTop > (ulEl.scrollHeight - ulEl.clientHeight * (1 - fraction))) {
    //    _scrollTop = $ul.scrollHeight - $ul.clientHeight;
    //} else {
    //    _scrollTop = nowline.offsetTop - $ul.clientHeight * fraction;
    //}
    _scrollTop = (nowline.offsetTop - ulEl.offsetTop) - ulEl.clientHeight * fraction;

    //以下声明歌词高亮行固定的基准线位置成为 “A”
    //if ((nowline.offsetTop - $ul.scrollTop) >= $ul.clientHeight * fraction) {
    //    //如果高亮显示的歌词在A下面，那就将滚动条向下滚动，滚动距离为 当前高亮行距离顶部的距离-滚动条已经卷起的高度-A到可视窗口的距离
    //    $ul.scrollTop += Math.ceil(nowline.offsetTop - $ul.scrollTop - $ul.clientHeight * fraction);

    //} else if ((nowline.offsetTop - $ul.scrollTop) < $ul.clientHeight * fraction && _scrollTop != 0) {
    //    //如果高亮显示的歌词在A上面，那就将滚动条向上滚动，滚动距离为 A到可视窗口的距离-当前高亮行距离顶部的距离-滚动条已经卷起的高度
    //    $ul.scrollTop -= Math.ceil($ul.clientHeight * fraction - (nowline.offsetTop - $ul.scrollTop));

    //} else if (_scrollTop == 0) {
    //    $ul.scrollTop = 0;
    //} else {
    //    $ul.scrollTop += $(ul.find('li').get(0)).height();
    //}
    ulEl.scrollTop = Math.ceil(_scrollTop)+18;
}
var lineNo = 0;
ap.on('timeupdate',function () {
    if (lineNo == medisArray.length - 1 && ap.audio.currentTime.toFixed(3) >= parseFloat(medisArray[lineNo].t)) {
        lineHeight(lineNo);
    }
    if (parseFloat(medisArray[lineNo].t) <= ap.audio.currentTime.toFixed(3) &&
        ap.audio.currentTime.toFixed(3) <= parseFloat(medisArray[lineNo + 1].t)) {
        lineHeight(lineNo);
        lineNo++;
    }
});
ap.on('seeked', function () {
    if (ap.audio.currentTime.toFixed(3) < parseFloat(medisArray[0].t)) {
        lineNo = 0;
        lineHeight(lineNo);
        lineNo++;
    }
    else if (ap.audio.currentTime.toFixed(3) > parseFloat(medisArray[medisArray.length - 1].t)) {
        lineNo = medisArray.length - 1;
        lineHeight(lineNo);
    }
    for (var i = 0; i <= medisArray.length - 1; i++) {
        if (parseFloat(medisArray[i].t) <= ap.audio.currentTime.toFixed(3) &&
            ap.audio.currentTime.toFixed(3) <= parseFloat(medisArray[i + 1].t)) {
            lineNo = i;
            lineHeight(lineNo);
            lineNo++;
            break;
        }
    }

});
