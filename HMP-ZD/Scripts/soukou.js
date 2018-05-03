
var seluser_callback = null;
var Sender = null;

$(document).ready(function () {
    //  閉じる
    function closeDialog() {
        $("#overlay").hide();
        $('#Show_Soukou').hide();
    }

    function openDialog() {
        $('#overlay').show();
        $('#Show_Soukou').show();
    }

    //  表示
    $(".show_soukou").click(function () {
        Sender = $(this).parent();
        openDialog();

        return false;
    });

    //  閉じる
    $('#Show_Soukou').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel", function () {
        closeDialog();
    });

    $('#overlay').on('click', function () {
        closeDialog();
    });

    closeDialog();
    //  登録Show_Soukou
    $('#Show_Soukou').on('click', '.btn-ok', function () {
        if (seluser_callback !== null) {
            seluser_callback();
        }

        //  閉じる
        closeDialog();

        return false;
    });
});
