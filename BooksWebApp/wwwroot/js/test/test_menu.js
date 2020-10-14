
var myController = {
    init: function () {
        myController.registerEvent();
    },
    registerEvent: function () {
        $("input[type=text].sort").blur(function () {
            $(this).parent().submit();
        });
    }
};

myController.init();