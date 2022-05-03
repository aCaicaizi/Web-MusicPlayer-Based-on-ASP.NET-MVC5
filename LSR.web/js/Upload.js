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
                getAudioDuration(_URL.createObjectURL(file));
            },
            onError: function (error) {
                console.log(error);
            }
        })
        var _URL = window.URL || window.webkitURL;
        
       
                $('#Upload_btn').on('click', function () {
                    var formdata = new FormData();
                    var MusicName = $('#MusicName').val();
                    var Author = $('#ArtistName').val();
                    var AlbumName = $('#AlbumName').val();
                    var Duration = $('#duration').val();
                    formdata.append('Music', $('#MusicFile')[0].files[0]);
                    formdata.append('Lyrics', $('#LyricsFile')[0].files[0]);
                    formdata.append('Image', $('#ImageFile')[0].files[0]);
                    $.ajax({
                        url: '../Music/Upload?MusicName='+MusicName+'&Author='+Author+'&AlbumName='+AlbumName+'&Duration='+Duration,
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
})