const jtags = window.jsmediatags;
$(function () {
    
    
    $('#MusicFile').on('change', function (e) {
        $('#MusicInput').fadeIn();
        
        var file = e.target.files[0];
        jtags.read(file, {
            onSuccess: function (tag) {
                console.log(tag);
                $('#MusicName').val(tag.tags.title);
                $('#ArtistName').val(tag.tags.artist);
                $('#AlbumName').val(tag.tags.album);
                if (tag.tags.picture != null) {
                    $('#ImageFile').attr("style","display:none;");
                    var imgStr = BytesArrToBase64(tag.tags.picture);
                    $('#ImageInside').val(imgStr/*tag.tags.picture.data*/);
                    //console.log('Image: ' + imgStr);
                }
                else {
                    $('#ImageFile').removeAttr("style", "display:none;");
                }
                if (tag.tags.lyrics != null) {
                    $('#LyricsFile').attr("style", "display:none;");
                    $('#LyricsInside').val(tag.tags.lyrics);
                    console.log('Lyrics: ' + tag.tags.lyrics);
                }
                else { $('#LyricsFile').removeAttr("style", "display:none;");}
                var _URL = window.URL || window.webkitURL;
                getAudioDuration(_URL.createObjectURL(file));
            },
            onError: function (error) {
                console.log(error);
            }
        })
        var _URL = window.URL || window.webkitURL;
        
       
        $('#Upload_btn').on('click', function () {
            $('#Upload_btn').toggleClass('disabled');
            var ImageBase64 = $('#ImageInside').val();
            var LyricsInside = $('#LyricsInside').val();
            var formdata = new FormData();

            //var MusicName = $('#MusicName').val();
            formdata.append('MusicName',$('#MusicName').val());
            //var Author = $('#ArtistName').val();
            formdata.append('Author',$('#ArtistName').val());
            //var AlbumName = $('#AlbumName').val();
            formdata.append('AlbumName', $('#AlbumName').val());
            /*var Duration = $('#duration').val();*/
            formdata.append('Duration',$('#duration').val());
            formdata.append('Music', $('#MusicFile')[0].files[0]);
            
            if (LyricsInside != null && LyricsInside!="") formdata.append('LyricsString',LyricsInside);
            else formdata.append('Lyrics', $('#LyricsFile')[0].files[0]);
            
            if (ImageBase64 != null) formdata.append('ImageBase64String', ImageBase64);
            else formdata.append('Image', $('#ImageFile')[0].files[0]);
            
                    $.ajax({
                        url: '../Music/Upload',
                        type: 'post',
                        data: formdata, 
                        contentType: false,
                        processData: false,
                        success: function (data) {
                            $('#MusicSuccess').fadeIn();
                            console.log(data);
                },
                        error: function () {
                            alert('上传失败');
                            $('#Upload_btn').toggleClass('disabled');
                }
            })
        })
    })
    function getAudioDuration(src) {
        let audio = document.createElement('audio'); //生成一个audio元素
        audio.src = src; //音乐的路径
        audio.addEventListener("canplay", function () {
            var duration=parseInt(audio.duration);
            console.log(duration);
            $('#duration').val(duration);
        });
    }
    //function Base64ToFile(bytes) {

    //            //去掉url的头，并转换为byte

    //    //处理异常,将ascii码小于0的转换为大于0
    //    var ab = new ArrayBuffer(bytes.length);
    //    var ia = new Uint8Array(ab);
    //    for (var i = 0; i < bytes.length; i++) {
    //        ia[i] = bytes.charCodeAt(i);
    //    }

    //    return new Blob([ab], { type: 'image/png' });
    //}
    function BytesArrToBase64(image) {
        var base64String = "";
        for (var i = 0; i < image.data.length; i++) {
            base64String += String.fromCharCode(image.data[i]);
        }
        
        return "data:" + image.format + ";base64," +window.btoa(base64String);
    }
})