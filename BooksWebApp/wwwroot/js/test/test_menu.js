
var menuController = {
    init: function () {
        menuController.registerEvent();
    },
    registerEvent: function () {
        $("input[type=text].sort").blur(function () {
            $(this).parent().submit();
        });
    }
};

menuController.init();