var r;
var move_offset = 40;
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(jQuery);
    }
}(function ($) {
    var cropbox = function (options, el) {
        var el = el || $(options.imageBox),
            obj =
            {
                state: {},
                ratio: 1,
                options: options,
                imageBox: el,
                thumbBox: el.find(options.thumbBox),
                spinner: el.find(options.spinner),
                image: new Image(),
                getDataURL: function () {
                    //1.设偏移量
                    let coor_offset = 200;//坐标轴偏移量
                    //              	let p_offset_x=15;
                    //              	let p_offset_y=18;
                    //2.获取属性变量
                    let le = $(".thumbBox").css("left").replace("px", ".");
                    let le2 = le.substr(0, le.indexOf("."));
                    let top = $(".thumbBox").css("top").replace("px", ".")
                    let top2 = top.substr(0, top.indexOf("."));
                    //3.获取截图框的x,y坐标
                    let x, y, x2, y2, p_offset_x, p_offset_y;

                    var width = this.thumbBox.width(),
                        height = this.thumbBox.height(),
                        canvas = document.createElement("canvas"),
                        dim = el.css('background-position').split(' '),
                        size = el.css('background-size').split(' '),
                        //dx = parseInt(dim[0]) - el.width()/2 + width/2,
                        //dy = parseInt(dim[1]) - el.height()/2 + height/2,
                        dw = parseInt(size[0]),
                        dh = parseInt(size[1]),
                        sh = parseInt(this.image.height),
                        sw = parseInt(this.image.width);

                    if (dw > width || dh > height) {
                        x = le2 - coor_offset;
                        y = top2 - coor_offset;
                        x = x / r;
                        y = y / r;
                        x2 = parseInt(dim[0]) - 100;
                        y2 = parseInt(dim[1]) - 100;
                    } else {
                        p_offset_x = 15;
                        p_offset_y = 18;
                        x = le2 - coor_offset;
                        y = top2 - coor_offset
                        x = x * r - p_offset_x;
                        y = y * r - p_offset_y;
                        x2 = (parseInt(dim[0]) - 100) * r - p_offset_x;//100为margin
                        y2 = (parseInt(dim[1]) - 100) * r - p_offset_y;
                    }

                    //console.log(x2);
                    //console.log(y2);
                    canvas.width = width;
                    canvas.height = height;
                    var context = canvas.getContext("2d");
                    //context.drawImage(this.image, -170, -150, sw, sh, dx, dy, dw, dh);
                    context.drawImage(this.image, x, y, sw, sh, x2, y2, dw, dh);
                    var imageData = canvas.toDataURL('image/png');
                    return imageData;
                },
                getBlob: function () {
                    var imageData = this.getDataURL();
                    var b64 = imageData.replace('data:image/png;base64,', '');
                    var binary = atob(b64);
                    var array = [];
                    for (var i = 0; i < binary.length; i++) {
                        array.push(binary.charCodeAt(i));
                    }
                    return new Blob([new Uint8Array(array)], { type: 'image/png' });
                },
                zoomIn: function () {
                    this.ratio *= 1.1;
                    setBackground();
                },
                zoomOut: function () {
                    this.ratio *= 0.9;
                    setBackground();
                }
            },
            setBackground = function () {
                var w = parseInt(obj.image.width) * obj.ratio;
                var h = parseInt(obj.image.height) * obj.ratio;
                r = obj.ratio;
                var pw = (el.width() - w) / 2;
                var ph = (el.height() - h) / 2;

                el.css({
                    'background-image': 'url(' + obj.image.src + ')',
                    'background-size': w + 'px ' + h + 'px',
                    'background-position': pw + 'px ' + ph + 'px',
                    'background-repeat': 'no-repeat'
                });
            },
            imgMouseDown = function (e) {
                e.stopImmediatePropagation();

                obj.state.dragable = true;
                obj.state.mouseX = e.clientX;
                obj.state.mouseY = e.clientY;
            },
            imgMouseMove = function (e) {
                e.stopImmediatePropagation();

                if (obj.state.dragable) {
                    var x = e.clientX - obj.state.mouseX;
                    var y = e.clientY - obj.state.mouseY;

                    var bg = el.css('background-position').split(' ');

                    var bgX = x + parseInt(bg[0]);
                    var bgY = y + parseInt(bg[1]);

                    el.css('background-position', bgX + 'px ' + bgY + 'px');

                    obj.state.mouseX = e.clientX;
                    obj.state.mouseY = e.clientY;
                }
            },
            imgMouseUp = function (e) {
                e.stopImmediatePropagation();
                obj.state.dragable = false;
            },
            zoomImage = function (e) {
                e.originalEvent.wheelDelta > 0 || e.originalEvent.detail < 0 ? obj.ratio *= 1.1 : obj.ratio *= 0.9;
                setBackground();
            }

        obj.spinner.show();
        obj.image.onload = function () {
            obj.spinner.hide();
            setBackground();
            //el.bind('mousedown', imgMouseDown);
            //el.bind('mousemove', imgMouseMove);
            //$(window).bind('mouseup', imgMouseUp);
            el.bind('mousewheel DOMMouseScroll', zoomImage);
        };
        obj.image.src = options.imgSrc;
        
        //el.on('remove', function(){$(window).unbind('mouseup', imgMouseUp)});
        return obj;
    };

    jQuery.fn.cropbox = function (options) {
        return new cropbox(options, this);
    };
}));

$(function () {
    $(document).mousemove(function (e) {
        if (!!this.move) {
            var posix = !document.move_target ? { 'x': 0, 'y': 0 } : document.move_target.posix,
                callback = document.call_down || function () {
                    $(this.move_target).css({
                        'top': e.pageY - posix.y + move_offset,
                        'left': e.pageX - posix.x - move_offset
                    });
                };
            callback.call(this, e, posix);
        }
    }).mouseup(function (e) {
        if (!!this.move) {
            var callback = document.call_up || function () { };
            callback.call(this, e);
            $.extend(this, {
                'move': false,
                'move_target': null,
                'call_down': false,
                'call_up': false
            });
        }
    });
    var $box = $('.thumbBox').mousedown(function (e) {
        var offset = $(this).offset();
        this.posix = { 'x': e.pageX - offset.left, 'y': e.pageY - offset.top };
        $.extend(document, { 'move': true, 'move_target': this });
    }).on('mousedown', '#coor', function (e) {
        var posix = {
            'w': $box.width(),
            'h': $box.height(),
            'x': e.pageX,
            'y': e.pageY
        };
        $.extend(document, {
            'move': true, 'call_down': function (e) {
                wa = Math.max(30, e.pageX - posix.x + posix.w);
                ha = Math.max(30, e.pageY - posix.y + posix.h);
                $box.css({
                    'width': wa,
                    'height': ha
                });
            }
        });
        return false;
    });
});

