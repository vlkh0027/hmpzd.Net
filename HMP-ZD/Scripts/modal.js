"use strict";
(function (g) {

    var o = {
        /**
         * 共通modal部品初期化
         */
        init: function () {
            var self = this;
            // 重複実行なし
            if (self.initAlready !== void 0) {
                return;
            }

            // html一式
            self.htmlData = {};

            // api
            self.apiUrl = '/Ledger/Api';

        },

        /**
         * modalのclone取得。initで行うとワンテンポ処理が遅れる
         * @param {string} id( or class)
         * @return {jQuery object} 
         */
        getModal: function (url) {
            var self = this, html=self.htmlData[url]||'';

            if (!html.length){
                // 処理簡略化の為、非同期にしてみる
                self.htmlData[url] = (function () {
                    return $.ajax({
                        type: 'GET',
                        url: url,
                        async: false
                    }).responseText
                })();
            }
            return $(self.htmlData[url]);
        },

        /**
         * overlayを表示  
         */
        overlay: function () {
            if ($('#overlay').length) {
                return;
            }
            $('<div id="overlay" />')
            .on('click',function(e){
                self.close.call(self);
            })
            .appendTo('body').show();
        },

        /**
         * 閉じる
         * 
         * @param {string} className 閉じるmodal
         */
        close: function (className) {
            var self = this;
            if (!className) {
                className = '.jsModal';
            }
            $(className).remove();
            if (!$('.jsModal:visible').length) {
                $('#overlay').remove();
            }
            return;
        },

        /**
         * 引数のhtmlをモーダル表示
         */
        showHtml:function(html){
            var self=this,
                dfd=$.Deferred(),
                $modal = $(html);

            // 閉じる
            $('.jsClose', $modal).on('click',function(e){
                self.close.call(self);
                return false;
            })

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();
            
            // TODO:

            return dfd.promise();
        },

        /**
         * 削除履歴一覧				一覧表示	/Modal/DeleteHistory
         */
        showDeleteHistory:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/DeleteHistory',
                $modal = self.getModal.call(self,url);


            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();
            
            // TODO:

            return dfd.promise();
        },

        /**
         * お知らせ				表示	/Modal/Notice
         */
        showNotify:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/Notify',
                $modal = self.getModal.call(self,url);


            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },

        /**
         * ユーザー追加 選択	/Modal/UserSelect
         */
        showUserSelect:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/UserSelect',
                $modal = self.getModal.call(self, url),
                refreshEvent=function(){

                    //  会社名が選択された
                    $('[name=CompanyID]', $modal).off('change').on('change', function () {
                        //  DropDownのリセット
                        $('[name=DivisionGroupID],[name=DivisionID],[name=DepartmentID]',$modal).each(function(){
                            $(this).html('').append('<option value="">選択してください</option>');
                        });
                        // データ呼出
                        $.ajax({
                            type:'POST',
                            url:self.apiUrl+'/GetOrganizations',
                            dataType:'json',
                            data:{id:$(this).val()}
                        }).then(function(data){
                            var list=data||[];
                            _.each(list, function(v){
                                $('<option />').val(v.id).text(v.divisionGroupName)
                                .appendTo($('[name=DivisionGroupID]', $modal));
                            });
                            refreshEvent();
                        });
                        return false;                    
                    });

                    // 部門グループ
                    $('[name=DivisionGroupID]', $modal).off('change').on('change', function () {
                        // TODO
                        return false;
                    });

                    // 部門
                    $('[name=DivisionID]', $modal).off('change').on('change', function () {
                        // TODO
                        return false;
                    });

                    // 部署
                    $('[name=DepartmentID]', $modal).off('change').on('change', function () {
                        // TODO
                        return false;
                    });

                    // 検索
                    $('.jsSearch', $modal).off('click').on('click', function (e) {
                        $.ajax({
                            type:'POST',
                            url:self.apiUrl+'/SearchUser',
                            dataType:'json',
                            data:{
                                'q': $('[name=Words]', $modal).val() || '',
                                'company': $('[name=CompanyID]', $modal).val()||'',
                                'group': $('[name=DivisionGroupID]', $modal).val()||'',
                                'division': $('[name=DivisionID]', $modal).val()||'',
                                'department': $('[name=DeprtmentID]', $modal).val()||'',
                            }
                        }).then(function(data){
                            var list=data||[];

                            $('.jsCandidates tr:gt(0),.jsInserts tr:gt(0)', $modal).remove();
            
//                          $('.jsSelnum', $modal).text('選択ユーザー (0)');
            
                            _.each(list,function(v){
                                $('.jsCandidates tbody', $modal).append(
                                    $('<tr></tr>').attr('id', v.id)
                                    .append($('<td></td>').text(v.company))
                                    .append($('<td></td>').text(v.divisiongroup))
                                    .append($('<td></td>').text(v.division))
                                    .append($('<td></td>').text(v.department))
                                    .append($('<td></td>').text(v.name))
                                    .append($('<td style="display: none;"></td>').text(v.mail))
                                    .append('<td class="ta-c"><button id="selectuser">選択</button></td>')
                                    .data('my-item', _.clone(v))
                                );
                            });
                            
                            refreshEvent();
                            refreshSelectedCount();
                        });
                        return false;
                    });                
                  
                    // テーブル
                    $('.jsCandidates tr td button,.jsInserts tr td button').off('click').on('click',function(){
                        var $this=$(this), 
                            $tr=$this.closest('tr'),
                            item=_.clone($tr.data('my-item')||{}),
                            $clone=$tr.clone(false),
                            $table=$this.closest('table'),
                            isCandidates = $table.is('.jsCandidates');
                        
                        // clone(false) だと引き継がれないので
                        $clone.data('my-item', item);

                        $tr.remove();
                        if ($table.is('.jsCandidates')) {
                            $('.jsInserts tbody', $modal).append($clone);
                        } else {
                            $('.jsCandidates tbody', $modal).append($clone);
                        }
                        refreshEvent();
                        refreshSelectedCount();                                                                            
                    });
                },
                refreshSelectedCount = function(){
                    var len = ($('.jsInserts tbody tr', $modal).length || 1) - 1;
                    $('.jsSelnum', $modal).text('選択ユーザー ('+len+')');                    
                };

            // イベント初期化
            refreshEvent();

            // 全追加
            $('.jsAllInsert', $modal).on('click', function () {


                console.log($('.jsCandidates tbody tr', $modal).length);

                var ids = [];
                $('.jsInserts tbody tr', $modal).each(function () {
                    var id= ($(this).data('my-item') || {}).id ||'';
                    if (id.length) {
                        ids.push(id);
                    }
                });

                $('.jsCandidates tbody tr', $modal).each(function (i) {

                    console.log(i);
                    var $clone = $(this).clone(),
                        item = _.clone($(this).data('my-item') || {}),
                        id = item.id||'';
                    if (!id.length) {
                        return;
                    }
                    if (ids.indexOf(id)>=0){
                        return;
                    }
                    $('.jsInserts tbody tr', $modal).append($clone);
                    ids.push(id);
                });
                refreshEvent();
                refreshSelectedCount();
            });

            // 全削除
            $('.jsAllDelete', $modal).on('click', function () {
                $('.jsInserts tbody tr', $modal).each(function (i) {
                    if (i == 0) {
                        return;
                    }
                    //var $clone = $(this).clone();
                    //$clone.data('my-item', _.clone($(this).data('my-item') || {}));
                    //$('.jsCandidates tbody', $modal).append($clone);
                    $(this).remove();
                });
                refreshEvent();
                refreshSelectedCount();
            });


            // 閉じる(OK)
            $('.jsOk',$modal).on('click',function(e){
                var list=[];
                $('.jsInserts tr', $modal).each(function(){
                    var item=_.clone($(this).data('my-item')||{});
                    if (!_.isEmpty(item)) {
                        list.push(item);
                    }
                });
                dfd.resolve(list);
                self.close.call(self);
                return false;
            });

            // 閉じる
            $('.jsClose',$modal).on('click',function(e){
                self.close.call(self);
                return false;
            });

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },

        /**
         * 媒体社選択 選択	/Modal/VehicleSelect
         */
        showVehicleSelect:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/Vehicle',
                $modal = self.getModal.call(self,url);

            
            // TODO:

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();
            
            return dfd.promise();
        },
        
        /**
         * 広告主追加 選択	/Modal/CustomerSelect
         */
        showCustomer:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/CustomerSelect',
                $modal = self.getModal.call(self,url);


            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();
            
            // TODO:

            return dfd.promise();
        },

        /**
         * 部署追加				選択	/Modal/TeamSelect
         */
        showTeamSelect:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/TeamSelect',
                $modal = self.getModal.call(self,url);

                
            // TODO:

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();
            
            return dfd.promise();
        },
        
        /**
         * 発注先追加 選択	/Modal/CooperativeCompanySelect
         */
        showCooperativeCompanySelect:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/CooperativeCompanySelect',
                $modal = self.getModal.call(self,url);


                
            // TODO:

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },
        
        /**
         * URL共有				ツール	/Modal/UrlShare
         */
        showUrlShare:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/UrlShare',
                $modal = self.getModal.call(self,url);

            
            // TODO:
            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },

        /**
         * 得意先検索詳細				詳細表示	/Modal/CustomerDetail
         */
        showCustomerDetail:function(){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/CustomerDetail',
                $modal = self.getModal.call(self,url);

            // TODO:

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },
        
        /**
         * 表示項目追加
         */
        showHeaderSelect:function(header,showHeader){
            var self=this,
                dfd=$.Deferred(),
                url='/Modal/HeaderSelect',
                $modal = self.getModal.call(self,url),
                $tsptc=null,i;

            // for (i=0;i<header.length;i++){
                
            // }
            $('.dsp-t', $modal).empty();
            _.each(header,function(v,i){
                if ((i%14)==0){
                    $tsptc=$('<div class="dsp-tc" />').appendTo($('.dsp-t', $modal));
                }
                $tsptc.append(
                    $('<label />')
                    .append($('<input type="checkbox" />').attr('name', v.name))
                    .append(v.title)
                )
                .append('<br>');
                //console.log(i%14);console.log(v);
            });

            if (!showHeader.length) {
                $('input[type=checkbox]', $modal).prop('checked', true);
            } else {
                $('input[type=checkbox]', $modal).each(function(){
                    var name=$(this).attr('name');
                    if (showHeader.indexOf(name)>=0){
                        $(this).prop('checked', true);
                    }
                });
            }
    
            // 閉じる(OK)
            $('.jsOk',$modal).on('click',function(e){
                var list=[];
                $('input[type=checkbox]:checked', $modal).each(function(){
                    list.push($(this).attr('name'));
                });
                dfd.resolve(list);
                self.close.call(self);
                return false;
            });

            // 閉じる
            $('.jsClose', $modal).on('click',function(e){
                self.close.call(self);
                return false;
            })

            // 表示
            self.overlay.call(self);
            $modal.addClass('jsModal').appendTo('body').show();

            return dfd.promise();
        },
        
    };

    if (g.my === void 0) {
        g.my = {};
    }

    my.modal = o;

})((this || 0).self || global);
