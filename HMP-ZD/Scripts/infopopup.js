
"use strict";

(function (g) {
    var o = {

        init: function () {

            //  お知らせを表示するか判定
            if($('#ShowInformation').val() === "False"){
                return;
            }

            //  メンテナンスのおしらせが存在していない場合は
            //  メンテナンス部分は出さない
            if ($('#ShowMaintenance').val() === "False") {
                $('#maintenance').hide();
            }

            //  表示
            $('#overlay').show();
            $('#info_popup').show();

            //  閉じる
            $('.mdl-btn-cancel, .fa-close, #overlay').on('click', function () {
                $('#overlay').hide();
                $('#info_popup').hide();
            });
        },
    };

    if (g.my === void 0) {
        g.my = {};
    }

    g.my.infopopup = o;

})((this || 0).self || global);

// 実行
(function ($) {
    $(function () {
        my.infopopup.init.call(my.infopopup);
    });
})(jQuery);
