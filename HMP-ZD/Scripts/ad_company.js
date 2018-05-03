
$(document).ready(function () {
    Sender = null;

    //  閉じる Đóng
    function closeDialog() {
        $("#overlay").hide();
        $('#ad_company').hide();
    }

    //  表示 Hiển thị
    $(".show_ad_company").click(function () {
        $('#overlay').show();
        $('#ad_company').show();
        Sender = $(this).parent();
        return false;
    });

    //  閉じる //Đóng
    $('#ad_company').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel,.btn-ok", function () {
        closeDialog();
    });

    $('#overlay').on('click', function () {
        closeDialog();
    });

    //  追加ボタンが押された Nút nhấn thêm
    $(document).on('click', '.select', function () {
        var id = $(this).attr('id');

        //  会社名取得
        $.getJSON(
            '/Ledger/Api/GetCompany',
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

    //  その他の追加ボタンが押された Các nút bổ sung khác được nhấn
    $('#other').on('click', function () {
        var text = $(this).parents('tr').find('input').val();

        $(Sender).find('#company').text(text);

        //  閉じる đóng
        closeDialog();
    });

    //  検索ボタン Nút tìm kiếm
    $('#search').on('click', function () {
        $.getJSON(
            '/Ledger/Api/SearchCompany',
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
                $('#result tbody > tr:last').after('<tr><th>広告主</th><th></th></tr>');

                $.each(data, function (i, v) {
                    $('#result tbody > tr:last').after($('<tr></tr>')
                        .append($('<td></td>').text(v.Name))
                        .append($('<td id="company" class="ta-c"></td>').append($('<a href="javascrpipt:void(0);" class="btn-ok btn-small select"></a>').attr('id', v.ID).text('追加'))));
                });
            });
    });
});
