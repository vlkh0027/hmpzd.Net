"use strict";

(function (g) {
    var o = {

        /**
         * 初期化
         */
        init: function () {
            var self = this, 
                $frm=$('.jsRegForm'),
                $err=$('#error'),
                uri=(new URI()),   // URL処理用
                param=uri.search(true) // パラメータ一式

            ;

            // 新規作成ページ
            $('.jsNew').on('click',function(e){
                location.href=my.getControllerUrl.call(my)+'/Edit';
                return false;
            });
            
            // データ登録
            $('.jsReg').on('click', function (e) {
                var data = {};
                $err.hide();
                $('input,select', $frm).each(function () {
                    var $this=$(this), 
                        name=$this.attr('name')||'',
                        m,
                        val=$this.val();

                    // XXXXIDLIST は , 区切りで渡す
                    if(name.match(/^(.+id)list$/i)){
                        if (data[name]===void 0){
                            data[name] = val;
                        } else {
                            data[name] += (',' + val);
                        }
                        return;
                    }
                    data[name]=val;
                    return;
                });
//console.log(data);return;
                // データ登録
                $.ajax({
                    type: 'post',
                    url: my.getControllerUrl.call(my) + '/Update',
                    data: data,
                    cache: false,
                    dataType:'json',
                }).then(function (data) {
                    if ((data.res || '') == 'ok') {

                        var id = data.ID || '';
                        $('[name=ID]', $frm).val(id);
                        alert('データが登録されました');
                        if (id.length) {
                            //alert('データが登録されました');
                            location.href = my.getControllerUrl.call(my) + '/Edit?ID=' + (data.ID || '');
                        }
                        return;
                    }

                    // エラー表示
                    $('ul',$err).empty();
                    _.each(data.err||[], function(v){
                        $('ul',$err).append($('<li />').text(v));
                    });
                    $err.show();
                })

                return false;
            });
            
            // 戻る
            $('.jsBack').on('click',function(){
                location.href=my.getControllerUrl.call(my);
                return false;
            });


            // 初期データ
            //$.ajax({
            //    type: 'post',
            //    url: my.getControllerUrl.call(my) + '/DetailData',
            //    data:{ID:param.ID||''},
            //    cache: false,
            //    dataType: 'json',
            //}).then(function (data) {
            //    _.each(data||{}, function (v, k) {
            //        $('[name=' + k + ']', $frm).val(v);
            //    });
            //})

		},


    };
    if (g.my === void 0) {
        g.my = {};
    }
    g.my.edit = o;

})((this || 0).self || global);

