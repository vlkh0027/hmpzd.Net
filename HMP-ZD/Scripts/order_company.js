$(document).ready(function () {
    Sender = null;

    //  閉じる
    function closeDialog() {
        $("#overlay").hide();
        $('#order_company').hide();
    }

    function openDialog() {
        $('#overlay').show();
        $('#order_company').show();
    }

    //  表示
    $(".show_order_company").click(function () {
        Sender = $(this).parent();
        openDialog();
        return false;
    });

    $(".order_company").on("click", function () {
        openDialog();
    });

    //  閉じる
    $('#order_company').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel,.btn-ok", function () {
        closeDialog();
    });
    //  追加ボタンが押された Nút nhấn thêm
    $(document).on('click', '.select', function () {
        var id = $(this).attr('id');

        //  会社名取得
        $.getJSON(
            '/Ledger/Api/GetOrderCompany',
            {
                'id': $(this).attr('id')
            },
            function (data) {
                $(Sender).find('#company').text(data.Name);
                $(Sender).parents('dl').find('#industry').text(data.IndustryName);
            }
        );

        //  閉じる Đóng
        closeDialog();
    });
        //  検索ボタン Nút tìm kiếm
    $('#search').on('click', function () {
        $.getJSON(
            '/Ledger/Api/SearchOrderCompany',
            {
                'q': $('#query').val()
            },
            function (data) {
                if (data.length === 0) {
                    $('#searchResult').text('該当するデータがありませんでした。');
                } else {
                    $('#searchResult').text('検索結果 ' + data.length + ' 件該当しました。');
                }

                $('#result tbody > tr').html('');
                $('#result tbody > tr:last').after('<tr><th>発注先</th><th>期限</th><th></th></tr>');

                $.each(data, function (i, v) {
                    $('#result tbody > tr:last').after($('<tr></tr>')
                        .append($('<td></td>').text(v.Name))

                        .append($('<td id="company" class="ta-c"></td>').append($('<a href="javascrpipt:void(0);" class="btn-ok btn-small select"></a>').attr('id', v.ID).text('追加'))));
                });
            });
    });
    

});
