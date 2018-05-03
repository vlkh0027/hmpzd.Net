"use strict";

(function (g) {
    var o = {

        /**
         * /Admin/Team用のJavaScript
         */
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

            // 部署変更
            $('select[name=DepartmentID]').on('change', function (e) {
                var $this=$(this), val = $this.val() || '';
                $.ajax({
                    type:'POST',
                    dataType: 'json',
                    url: my.apiUrl + '/GetTeams',
                    data:{
                        id:val
                    },
                    cache:false,
                })
                .then(function (data) {
                    console.log(data);
                    var list = data || [], $team = $('select[name=TeamID]');
                    $team.empty().append($('<option />').text('選択してください'));
                    _.each(list, function (row) {
                        $team.append($('<option />').text(row.Text||'').val(row.Value||''));
                    });
                })
                return false;
            });

            // ユーザ選択
            $('.jsShowUserSelect').on('click',function(e){
                var $td = $(this).closest('td');
                // ユーザ選択モーダルを表示(POPUP)
                my.modal.showUserSelect.call(my.modal).then(

                    // resolveで呼び出される
                    function (list) {
                    console.log(list);
                    $('span', $td).remove();
                    list = list || [];
                    _.each(list, function (v) {
                        $('<span />')
                        .text(v.name || '')
                        .append($('<input type="hidden" name="UserIDList" />').val(v.id || ''))
                        .appendTo($td)
                    });
                    },
                    // reject
                    function () {

                    }
                );
                return false;
            });

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
        // initDisplay:function(){
        //     var self=this,
        //         uri=(new URI()),   // URL処理用
        //         param=uri.search(true) // パラメータ一式
        //         ;

        //     $('input[name=S_COLUMN]').val([param.S_COLUMN||'']);
        //     $('input[name=S_ROW1]').prop('checked',(param.S_ROW1||'')==1);
        //     $('input[name=S_ROW2]').prop('checked',(param.S_ROW2||'')==1);
        //     $('input[name=S_ROW3]').prop('checked',(param.S_ROW3||'')==1);

        // },
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

