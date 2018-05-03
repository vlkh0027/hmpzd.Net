$(document).ready(function () {
    Sender = null;

    //  閉じる Đóng
    //function closeDialog() {
    //    $("#overlay").hide();
    //    $('#url_popup').hide();
    //}

    ////  表示 Hiển thị
    //$(".show_url_popup").click(function () {
    //    $('#overlay').show();
    //    $('#url_popup').show();
    //    Sender = $(this).parent();
    //    return false;
    //});

    ////  閉じる //Đóng nút
    $('#url_popup').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel,.btn-ok", function () {
        closeDialog();
    });

    $(document).ready(function () {
        $('#btnUpload').click(function () {

            // Checking whether FormData is available in browser  
            if (window.FormData !== undefined) {

                var fileUpload = $("#FileUpload1").get(0);
                var files = fileUpload.files;

                // Create FormData object  
                var fileData = new FormData();

                // Looping over all files and add it to FormData object  
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }

                // Adding one more key to FormData object  
                fileData.append('username', ‘Manas’);

                $.ajax({
                    url: '/Home/UploadFiles',
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
            } else {
                alert("FormData is not supported.");
            }
        });
    });  

    //$('#overlay').on('click', function () {
    //    closeDialog();
    //});
});
