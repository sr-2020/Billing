$(function () {
    $("#uploadpt").click(function () {
        var $input = $("#dataFile");
        var fd = new FormData();
        fd.append('formFile', $input.prop('files')[0]);
        $.ajax({
            type: "POST",
            url: 'UploadProductsList',
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
        if (!confirm("Точно удалить все?!?!")) {
            return false;
        }
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
                location.reload();
            }
        });
        return false;
    });
});