showModalDialog = (url, title, file = false) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            if (!file) {
                $('#form-modal .modal-body').html(res);
                $('#form-modal .modal-title').html(title);
                $('#form-modal').modal('show'); 
            }
            else {
                $('#form-modal-file .modal-body').html(res);
                $('#form-modal-file .modal-title').html(title);
                $('#form-modal-file').modal('show');
            }

            if (!res.isValid && res.mes !== undefined) {
                toastr.error(res.mes);
            }
        },
        error: function () {
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
                    $('.modal-backdrop').css('display', 'none');
                    //$('#form-modal .modal-body').html('');
                    //$('#form-modal .modal-title').html('');
                }
                else {
                    if (res.html !== undefined) {
                        $('#form-modal .modal-body').html(res.html);
                    }
                    if (res.mes !== undefined) {
                        toastr.error(res.mes);
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
                    if (res.isValid) toastr.success('Đã xóa dữ liệu thành công !');

                    if (!res.isValid && res.mes !== undefined) {
                        toastr.error(res.mes);
                    }
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


