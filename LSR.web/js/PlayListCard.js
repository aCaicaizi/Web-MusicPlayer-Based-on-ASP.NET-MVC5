class Action {
    constructor(parameter) {
        this._text_input = parameter.text_input;
        this._form = parameter.form;
        this._submit_btn = parameter.submit_btn;
        this._del_btn = parameter.del_btn;
        this._items_group = parameter.items_group;

        this._local_key = 'items';
        this._local_data = this._loctionStorageGet(this._local_key) || parameter.default_item;

        this._init()
        this._add_event();
    }
    _init() {
        let data = [];
        //存在储存数据
        data = this._local_data;

        if (data && data.length > 0) {
            for (let i = 0; i < data.length; i++) {
                this._add_item_html(data[i]);
            }

        }
    }
    //_add_item_html(text) {
    //    if (text) {
    //        let that = this;
    //        (async function () {
    //            let html = '';
    //            html += '<li class="item">';
    //            html += '   <span class="text">' + text + '</span>';
    //            html += '   <span class="icon">X</span>';
    //            html += '</li>';
    //            that._items_group.append(html);
    //            await that._sleep(0.2);
    //            $('.item').css({ left: 0, opacity: 1 })
    //        })();
    //    }
    //}
    _add_event() {
        let that = this;
        this._form.submit(function () {
            let text = that._text_input.val();
            if (text) {
                that._add_item_html(text);
            }
            that._text_input.val('');
            that._local_data.push(text);
            that._loctionStorageSet(that._local_key, that._local_data);
            return false;
        });

        this._items_group.on('click', '.icon', function () {
            let this_item = $(this).parent();
            let index = this_item.index();
            (async function () {
                this_item.css({ left: '-19px', opacity: 0 });
                await that._sleep(0.4);
                this_item.remove();
                that._local_data.splice(index, 1);
                that._loctionStorageSet(that._local_key, that._local_data);
            })();
        });
        this._del_btn.click(function () {
            that._items_group.find('.icon').click();
        });
    }

    _sleep(time) {
        return new Promise((resolve) => setTimeout(resolve, time * 1000));
    }

    _loctionStorageSet(key, value, expired) {
        /*
        * set 存储方法
        * @ param {String} 	key 键
        * @ param {String} 	value 值，
        * @ param {String} 	expired 过期时间，以分钟为单位，非必须
        */
        var source = window.localStorage;
        source[key] = JSON.stringify(value);
        if (expired) {
            source[key + '__expires__'] = Date.now() + 1000 * 60 * expired
        };
        return value;
    }
    _loctionStorageGet(key) {
        /*
        * get 获取方法
        * @ param {String} 	key 键
        * @ param {String} 	expired 存储时为非必须字段，所以有可能取不到，默认为 Date.now+1
        */
        var source = window.localStorage;
        let expired = source[key + '__expires__'] || Date.now + 1;
        const now = Date.now();

        if (now >= expired) {
            this.remove(key);
            return;
        }
        var value = source[key] ? JSON.parse(source[key]) : source[key];
        return value;
    }
}