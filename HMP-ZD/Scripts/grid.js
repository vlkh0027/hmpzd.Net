"use strict";

(function (g) {
    var o = {

        /**
         * config設定
         * @param {object} config 
         */
        setConfig: function(config) {
            var self = this;
            self.config = config;
        },

        /**
         * 初期化
         */
        init: function () {
            var self = this,
                dfd = $.Deferred(), // 非同期処理用
                uri = (new URI()),   // URL処理用
                param = uri.search(true) // パラメータ一式
            ;

            self.$grid = $('.jsGrid');
            // 何もしない
            if (self.$grid.length != 1) {
                dfd.resolve();                                
                return dfd.promise();
            }

            // 初期表示時、ページャ、検索結果は隠す
            $('.jsGridPager,.jsGridSearchResult,.jsGridSearchResult2').hide();

            // 検索
            $('.jsGridSearchForm').on('submit', function (e) {
                $('input', this).each(function (e) {
                    var $this = $(this), name = $this.attr('name'), val = $this.val();
                    uri.setSearch(name, val);
                });
                location.href = uri.toString();
                return false;
            });
            $('.jsGridSearch').on('click', function (e) {
                $('.jsGridSearchForm').trigger('submit');
                return false;
            });


            // slickgrid作成
            self.createSlickGrid.call(self).then(function(){
                dfd.resolve();
                return;
            });

            return dfd.promise();
            
		},

        /**
         * slickgrid作成
         */
        createSlickGrid:function(){

            var self=this, 
                dfd=$.Deferred(), // 非同期処理用
                //header = self.header||[], // ヘッダー
                cols=[],
                uri=(new URI()),   // URL処理用
                param=uri.search(true), // パラメータ一式
                rows=[] // 表示するデータ
                ;

            // 現在のURLパラメータにpage,typeを付与する
            param.page=1;
            param.type="GRIDDATA";

            console.log("Controller url = " +my.getControllerUrl.call(my) + '/GridData');

            $.ajax({
                url:my.getControllerUrl.call(my)+'/GridData',
                data:param,
                type:'GET',
                cache:false,
                dataType:'json'
            }).then(function(data){

                console.log("Data = " + data);


                var showHeader =(function(){
                    try {
                        return JSON.parse(localStorage['SHOW_HEADER']);
                    } catch(e){};
                    return [];
                })();


                self.loadData = data;

                // ヘッダーデータをセット
                self.header=self.loadData.header||[];

                // underscore.js ライブラリ(配列特化)            
                _.each(self.header,function(row,i){
                    var name=row.name||'',title=row.title||'';

                    if (showHeader.length) {
                        if (!(showHeader.indexOf(name)>=0)) {
                            return;
                        }
                    }

                    // 並べ替えアイコン
                    // title += '<div class="sort jsSubFilterLink" style="display:none;">' +
                    // '<i class="fa fa-lg fa-fw" style="font-size:14px;margin-top:px;"></i></div>';
                    title += '<div class="sort jsSubFilterLink" style="">' +
                    '<i class="fa fa-lg fa-fw" style="font-size:14px;margin-top:px;"></i></div>';

                    // 列ごとの設定
                    cols.push({
                        id: name,
                        field: name,
                        name: title,
                        resizable: true,
                        rerenderOnResize: true,
                        formatter: function(row, cell, value, columnDef, dataContext) {
                            return self.formatter.call(self, row, cell, value, columnDef, dataContext);
                        },
                            
                        // options:options,
                        //    focusable:true,
                        // resizable: true,
                        // selectable: true,
                        // sortable: true,
                        // rerenderOnResize: true,
                        // behavior: 'selectAndMove',
                        // cssClass: cssClass,
                        // headerCssClass: 'cell-header',
                        // width: width,
                        // minWidth: 35,
                        // editor:editor,
                        // asyncPostRender: function(cellNode, row, dataContext, colDef) {
                        //     return self.asyncPostRender.call(self, type, cellNode, row, dataContext, colDef)
                        // },
                    });
                });

                // 全体の設定
                var options = {
                    autoHeight:true,
                    fullWidthRows:true,
                    forceFitColumns: true,
                    // showHeaderRow:true,
                    // headerRowHeight:100,
                    // //showHeaderRow: true,
                    // enableAsyncPostRender: true,
                    // // rowHeight: 25,
                    // headerRowHeight:50,
                    // // editable: true,
                    // enableAddRow: false,
                    // enableCellNavigation: false,
                    // enableColumnReorder: true,
                    // enableTextSelectionOnCells: true,
                    // asyncEditorLoading: false,
                    // forceFitColumns: true,
                    // autoEdit: false,
                    // //topPanelHeight: 25,
                    // // frozenColumn: frozenColumn,
                    // //frozenRow: 5,
                    // //showHeaderRow: true,
                    // syncColumnCellResize: false,
                    // fullWidthRows:true,
                };

                // 対象セレクタ、表データ、列設定、全体設定を渡す
                // ひとまず空配列で作成する
                self.slickgrid = new Slick.Grid(".jsGrid", [], cols, options);
                self.slickgrid.registerPlugin( new Slick.AutoTooltips({ enableForHeaderCells: true }) );   //Plugin tooltip
                
                // クリック
                self.slickgrid.onClick.subscribe(function(e, args) {
                    var $target = $(e.target),
                        targetName=$target.attr('name'),
                        loadData = self.loadData,
                        rows = loadData.rows || [],
                        row = (args || {}).row,
                        cell = (args || {}).cell,
                        column = self.slickgrid.getColumns()[cell] || {},
                        item = rows[row]||{},
                        ID=item.ID||'',
                        res
                        ;
                   
                    if (self.config && self.config.onClick) {
                        res=self.config.onClick.call(self,$target,rows[row]||{});
                        // falseならそのまま終了
                        if (res===true) {
                            return false;
                        }
                    }

                    // 削除
                    if (targetName=='btnGridDel') {
                        if (!confirm('データを削除しますか?')){
                            return false;
                        }
                        $.ajax({
                            type:'post',
                            url:my.getControllerUrl.call(my)+'/Del',
                            data:{ID:ID},
                            cache:false,
                            dataType:'json'
                        })
                        .then(function(data){
                            if ((data.res||'')=='ok') {
                                // 結果表示は変える必要がある
                                alert('データを削除しました');
                                location.reload();
                                return;
                            }
                        });
                        return false;
                    }
                    alert("asdasdasdasd");
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
                    // 編集画面へ遷移
                    location.href = my.getControllerUrl.call(my) + '/Edit?ID=' + ID;
                   
                    return false;
                });

                // ヘッダークリック
                self.slickgrid.onHeaderClick.subscribe(function(e, args) {
                    var columnID = args.column.id;
                    console.log(columnID);
                });
                
                // ヘッダー描画時
                self.slickgrid.onHeaderCellRendered.subscribe(function (e, args) {
                    try {
                        self.onHeaderCellRendered.call(self, $(args.node), args.column);                
                    } catch(e){};                
                });
                
                self.refreshRowsData.call(self);

                // 成功時、非同期オブジェクト成功を呼び出す
                dfd.resolve(data);
            },
            function(err){
                // 失敗時
                console.log(err);
                // 失敗時、非同期オブジェクト失敗を呼び出す
                dfd.reject(err);
            });
    
            // 非同期処理に対応させる
            return dfd.promise();

        },   
        
        /**
         * slickgrid表データ更新
         */
        refreshRowsData:function() {

            var self=this, loadData=self.loadData||{}, rows=loadData.rows||[];

            // 表データセット
            self.slickgrid.setData(rows);
            // 多分必要
            self.slickgrid.updateRowCount();
            // 再描画
            self.slickgrid.render();
        
            // ページャー作成
            self.refreshPager.call(self);
            
        },

        /**
         * 値変換(Formatter)
         * 
         * @param {int} row
         * @param {int} cell
         * @param {string} value
         * @param {object} columnDef
         * @param {object} dataContext
         * @return {string} 変換後文字列
         */
        formatter: function(row, cell, value, columnDef, dataContext) {
            
            var self = this,
                id = columnDef.id
                ;

            if (id === 'Del_') {
                return '<button name="btnGridDel">削除</button>';
            }

            return value;
        },

        /**
         * pagerはgridにしか存在しない。ここで作成する
         */
        refreshPager:function(){
            
            var self=this, 
                loadData = self.loadData || {},
                rows = loadData.rows||[],
                total=loadData.total||0,
                totalPage=loadData.totalPage||0,
                page=loadData.page||1,
                limit=loadData.limit||0,
                $pager = $('.jsGridPager'),
                $searchResult = $('.jsGridSearchResult'),
                $searchResult2 = $('.jsGridSearchResult2'),
                $ul = $('<ul />');
            

            // 検索結果は一旦隠す
            $pager.hide();
            $searchResult.hide();
            $searchResult2.hide();

            // 部品がなければ何もしない
            if (!$pager.length) {
                return;
            }
            
            $pager.addClass('pager').empty();

            // データがない場合、ページャー表示せず
            if (totalPage==0) {
                return;
            }

            //前へ移動
            if (page > 1){
                $('<li class="prev" />')
                .append(
                    $('<a href="#"><i class="fa fa-chevron-left"></i></a>').data('my-page', page-1)
                )
                .appendTo($ul);    
            }
            
            // 1ページ目へ移動
            $('<li class="num" />')
            .append(
                $('<a href="#">1</a>').data('my-page',1)
            )
            .appendTo($ul);    
        
            // ...
            if (page > 3) {
                $('<li><span>…</span></li>').appendTo($ul);
            }

            for (var i=page-2;i<=page+2;i++) {
                if (i > 1 && i < totalPage) {
                    $('<li class="num" />')
                    .append(
                        $('<a href="#"></a>').text(i).data('my-page',i)
                    )
                    .appendTo($ul);                            
                }
            }

            // ...
            if ((page+3) < totalPage){
                $('<li><span>…</span></li>').appendTo($ul);
            }

            // 最終ページへ移動
            $('<li class="num" />')
            .append(
                $('<a href="#"></a>').text(totalPage).data('my-page',totalPage)
            )
            .appendTo($ul);    

            // 次へ移動
            if (page < totalPage) {
                $('<li class="next" />')
                .append(
                    $('<a href="#"><i class="fa fa-chevron-right"></i></a>').data('my-page',page+1)
                )
                .appendTo($ul);    
            }

            $('a', $ul).each(function(){
                console.log($(this));
                var $li=$(this).closest('li'),
                    newPage=$(this).data('my-page'),
                    uri=(new URI()),   // URL処理用
                    param=uri.search(true) // パラメータ一式
                ;
                
                // 現在のURLパラメータにpage,typeを付与する                
                param.page=newPage;
                param.type='GRIDDATA';

                if (newPage==page){
                    $li.addClass('current');
                }
                // 実際のページング処理
                $(this).on('click',function(){
                    $.ajax({
                        url:my.getControllerUrl.call(my)+'/GridData',
                        data:param,
                        type:'GET',
                        cache:false,
                        dataType:'json'
                    }).then(function(data){
                        self.loadData = data;
                        self.refreshRowsData.call(self);
                    });
                    return false;
                });
            });
            
            $pager.append($ul).show();

            // 検索結果
            $searchResult.text(total + '件見つかりました。').show();
            $searchResult2.text(total + '件中 '+((page-1)*limit+1)+'件-'+((page-1)*limit+rows.length)+'件').show();
        },

        /**
         * 表示列制御
         */
        showHeaderSelectModal:function(){
            var self=this, 
                header=self.header||[],
                showHeader=(function(){
                    try {
                        return JSON.parse(localStorage['SHOW_HEADER']);
                    } catch(e){};
                    return [];
                })();
            
            if (!header.length) {
                return;
            }

            my.modal.showHeaderSelect.call(my.modal,header,showHeader).then(function(showHeader){

                localStorage.setItem('SHOW_HEADER', JSON.stringify(showHeader));
                location.reload();
            });
        },

    };
    if (g.my === void 0) {
        g.my = {};
    }
    g.my.grid = o;

})((this || 0).self || global);

