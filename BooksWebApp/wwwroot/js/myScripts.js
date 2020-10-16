showModalDialog = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        },
        error: function(){
            toastr.error('Không tìm thấy trang !');
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    if (res.mes === undefined) {
                        toastr.success('Đã lưu dữ liệu thành công !');
                    }
                    else {
                        if (res.type == "success") { toastr.success(res.mes); }
                        else if (res.type == "warning") { toastr.warning(res.mes); }
                    }
                    $('#form-modal').modal('hide');

                    if (res.html !== undefined) {
                        $('#view-all').html(res.html);
                    }
                    
                    // reset
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                }
                else {
                    if (res.html !== undefined) {
                        $('#form-modal .modal-body').html(res.html);
                    }
                    
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

jQueryAjaxDelete = form => {
    if (confirm('Chắc chắn xóa chứ ?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#view-all').html(res.html);
                    toastr.success('Đã xóa dữ liệu thành công !');
                },
                error: function (err) {
                    console.log(err);
                    toastr.error('Không thể xóa dữ liệu');
                }
            })
        } catch (ex) {
            console.log(ex);
        }
    }

    //prevent default form submit event
    return false;
}

