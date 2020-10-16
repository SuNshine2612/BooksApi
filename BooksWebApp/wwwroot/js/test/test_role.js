var roleController = {
    init: function () {
        roleController.registerEvent();
    },
    registerEvent: function () {
        // Change System Funtion & Menu when checkbox change event
        $("input[data-bootstrap-switch]").on('switchChange.bootstrapSwitch', function (e) {
           e.preventDefault();
            var _id = $(this).parent().parent().parent().attr("id");
            var _url = $(this).parent().parent().parent().data("url");
            // get status checked !
            var beforeChange = !$(this).is(':checked');
            $.ajax({
                url: _url,
                data: { id: _id.replace('active_', ''), group: $("#myGroup").val() },
                dataType: "json",
                type: "POST",
                success: function (res) {
                    if (!res.isValid) {
                        if (res.type == "warning") { toastr.warning(res.mes); }
                        else if (res.type == "error") { toastr.error(res.mes); }
                        // after toast message, must set checked status again !!! Rollback !
                        // Remember to use the third parameter (true) to make sure the event is not executed again.
                        $("#" + _id).children().children().find("input[data-bootstrap-switch]").bootstrapSwitch('state', beforeChange, true);
                    }
                    else {
                        // Success
                    }
                }
            });
        });
    }
};
roleController.init();

jQueryAjaxPostChangeUser = form => {
    try {
        // send groupId
        var dataToSend = new FormData(form);
        dataToSend.append('groupId', $("#myGroup").val());

        $.ajax({
            type: 'POST',
            url: form.action,
            data: dataToSend,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    toastr.success(res.mes);
                }
                else {
                    if (res.type == "error") { toastr.error(res.mes); }
                    else if (res.type == "warning") { toastr.warning(res.mes); }
                }
            },
            error: function (err) {
                console.log(err);
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex);
    }
}