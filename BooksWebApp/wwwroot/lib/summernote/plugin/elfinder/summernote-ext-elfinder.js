(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    }
    else if (typeof module === 'object' && module.exports) {
        module.exports = factory(require('jquery'));
    } else {
        factory(window.jQuery);
    }
}(function ($) {
    $.extend($.summernote.plugins, {
        // Tạo plugin tên elfinder  
        'elfinder': function (context) {
            var self = this;
            // ui has renders to build ui elements.
            var ui = $.summernote.ui;
            // Tạo nút bấm
            context.memo('button.elfinder', function () {
                var button = ui.button({
                    contents: '<i class="note-icon-picture"/> elFinder',
                    tooltip: 'Quản lý file',
                    click: function () {
                        // Bấm vào nút bấm gọi hàm elfinderDialog   
                        elfinderDialog(context);
                    }
                });
                // create jQuery object from button instance.
                var $elfinder = button.render();
                return $elfinder;
            });
            // This methods will be called when editor is destroyed by $('..').summernote('destroy');
            // You should remove elements on `initialize`.
            this.destroy = function () {
                this.$panel.remove();
                this.$panel = null;
            };
        }

    });
}));

function elfinderDialog(editor) {
    var fm = $('<div/>').dialogelfinder({
        url: '/el-finder-file-system/connector', // change with the url of your connector
        baseUrl: "/lib/elfinder/",
        lang: 'vi',
        width: 840,
        height: 450,
        destroyOnClose: true,
        getFileCallback: function (files, fm) {
            console.log(files);
            editor.invoke('editor.insertImage', files.url);
        },
        commandsOptions: {
            getfile: {
                oncomplete: 'close',
                folders: false
            }
        }
    }).dialogelfinder('instance');
}