"use strict";

(function (g) {
    var o = {

        /**
         * 初期化
         */
        init: function () {
            var self = this;

            // 共通初期化
            if (my.init) {
                my.init.call(my);
            }

            // 個票初期化
            if (my.edit) {
                my.edit.init.call(my.edit);
            }
            //// 表示初期化
            //self.initDisplay.call(self);

            //// 画面遷移
            //$('input[name=S_COLUMN],input[name=S_ROW1],input[name=S_ROW2],input[name=S_ROW3]').on('change', function(e){
            //    var param={};
            //    param.S_COLUMN=$('input[name=S_COLUMN]:checked').val()||'';
            //    param.S_ROW1=$('input[name=S_ROW1]').is(':checked') ? 1 : 0;
            //    param.S_ROW2=$('input[name=S_ROW2]').is(':checked') ? 1 : 0;
            //    param.S_ROW3=$('input[name=S_ROW3]').is(':checked') ? 1 : 0;                
            //    location.href=location.pathname+'?'+URI.buildQuery(param);
            //    return false;
            //});

            // グリッド初期化
            if (my.grid) {
                my.grid.init.call(my.grid);
            }

        },

        /**
         * 表示初期化
         */
        //initDisplay:function(){
        //    var self=this,
        //        uri=(new URI()),   // URL処理用
        //        param=uri.search(true) // パラメータ一式
        //        ;

        //    $('input[name=S_COLUMN]').val([param.S_COLUMN||'']);
        //    $('input[name=S_ROW1]').prop('checked',(param.S_ROW1||'')==1);
        //    $('input[name=S_ROW2]').prop('checked',(param.S_ROW2||'')==1);
        //    $('input[name=S_ROW3]').prop('checked',(param.S_ROW3||'')==1);

        //},
    };
    if (g.my === void 0) {
        g.my = {};
    }
    g.my.page = o;

})((this || 0).self || global);

// 実行
(function ($) {
    $(function () {
        my.page.init.call(my.page);
    });
})(jQuery);

