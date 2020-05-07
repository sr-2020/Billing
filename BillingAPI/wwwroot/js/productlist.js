$(function () {
    $("#uploadpt").click(function () {
        var $input = $("#dataFile");
        var fd = new FormData();
        fd.append('formFile', $input.prop('files')[0]);
        $.ajax({
            type: "POST",
            url: 'UploadPProductTypeList',
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (data) {
                alert(data);
                location.reload();
            },
            error: function (data) {
                console.log(data);
                alert(data.responseText);
            }
        });
        return false;
    });

    $("#deleteallpt").click(function () {
        $.ajax({
            type: "GET",
            url: 'deleteallpt',
            processData: false,
            contentType: false,
            success: function (data) {
                alert(data);
                location.reload();
            },
            error: function (data) {
                console.log(data);
                alert(data.responseText);
            }
        });
        return false;
    });
});