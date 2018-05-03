$(document).ready(function () {
    Sender = null;

    //  閉じる
    function closeDialog() {
        $("#overlay").hide();
        $('#file_popup').hide();
    }

    function openDialog() {
        $('#overlay').show();
        $('#file_popup').show();
    }

    function uploadFile() {
        var fileUpload = $("#FileUpload").get(0);
        var files = fileUpload.files;  
        // Create FormData object  
        var fileData = new FormData();
        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }  

        $.ajax({
            url: '/Magazine/UploadFiles',
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                alert(result);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });  
    }


    //  表示
    $(".show_url_popup").click(function () {
        Sender = $(this).parent();
        openDialog();
        return false;
    });

    //toggle
    //$(".tgl-btn").on("click", function () {
    //    $(this).toggleClass("on").next().slideToggle();
    //    return false;
    //});

    //$(".order_company").on("click", function () {
    //    openDialog();
    //});

    //  閉じる
    $('#file_popup').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel,.btn-ok", function () {
        closeDialog();
    });

    $('#file_popup').on("click", ".btn-ok", function () {
        uploadFile();
    });

    $('#overlay').on('click', function () {
        closeDialog();
    });

    // Đóng popup khi khởi động
    closeDialog();
});
