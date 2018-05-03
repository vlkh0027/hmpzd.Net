"use strict";

// サンプル

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
            
            // 表示初期化
            self.initDisplay.call(self);

            // 画面遷移
            $('input[name=S_COLUMN],input[name=S_WEB],input[name=S_MAGAZINE],input[name=S_PUBLICATION]').on('change', function(e){
                var param={};
                param.S_COLUMN=$('input[name=S_COLUMN]:checked').val()||'';
                param.S_WEB = $('input[name=S_WEB]').is(':checked') ? 1 : 0;
                param.S_MAGAZINE = $('input[name=S_MAGAZINE]').is(':checked') ? 1 : 0;
                param.S_PUBLICATION = $('input[name=S_PUBLICATION]').is(':checked') ? 1 : 0;
                location.href=location.pathname+'?'+URI.buildQuery(param);
                return false;
            });

            // グリッド初期化
            my.grid.setConfig.call(my.grid, {
                onClick: function ($target, item) {
                    console.log($target);
                    console.log(item);
                }
            });
            my.grid.init.call(my.grid);

            $('.jsTopHeaderSelect').on('click',function(){
                my.grid.showHeaderSelectModal.call(my.grid);
                return false;
            });
		},

        /**
         * 表示初期化
         */
        initDisplay:function(){
            var self=this,
                uri=(new URI()),   // URL処理用
                param=uri.search(true) // パラメータ一式
                ;

            $('input[name=S_COLUMN]').val([param.S_COLUMN||'']);
            $('input[name=S_WEB]').prop('checked', (param.S_WEB || '') == 1);
            $('input[name=S_MAGAZINE]').prop('checked', (param.S_MAGAZINE || '') == 1);
            $('input[name=S_PUBLICATION]').prop('checked', (param.S_PUBLICATION || '') == 1);

        },
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

