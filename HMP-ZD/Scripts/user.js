 //  コールバック用
var seluser_callback = null;
var Sender = null;

$(document).ready(function () {
    //  閉じる
    function closeDialog() {
        $("#overlay").hide();
        $('#seluser').hide();
    }

    function openDialog() {
        $('#overlay').show();
        $('#seluser').show();
    }

    //  表示
    $(".show_seluser").click(function () {
        Sender = $(this).parent();
        openDialog();

        return false;
    });

    //  閉じる
    $('#seluser').on("click", "#overlay,.mdl-btn-close,.mdl-btn-cancel", function () {
        closeDialog();
    });

    $('#overlay').on('click', function () {
        closeDialog();
    });

    

    //  会社名が選択された
    $('#companies').on('change', function () {
        //  DropDownのリセット
        $.each([$('#divisiongroups'), $('#divisions'), $('#departments')], function (i, elm) {
            elm.html('');
            elm.append('<option value="__nosel__">選択してください</option>');
        });

        $.getJSON(
            '/Ledger/Api/GetOrganizations',
            {
                'id': $(this).val()
            },
            function (data) {
                $.each(data, function (i, v) {
                    $('#divisiongroups').append($('<option></option>').val(v.id).text(v.divisionGroupName));
                });
            });
    });

    $('#divisiongroups').on('change', function () {
        //  DropDownのリセット
        $.each([$('#divisions'), $('#departments')], function (i, elm) {
            elm.html('');
            elm.append('<option value="__nosel__">選択してください</option>');
        });

        $.getJSON(
            '/Ledger/Api/GetOrganizations',
            {
                'id': $(this).val()
            },
            function (data) {
                $.each(data, function (i, v) {
                    $('#divisions').append($('<option></option>').val(v.id).text(v.divisionName));
                });
            });
    });

    $('#divisions').on('change', function () {
        //  DropDownのリセット
        $.each([$('#departments')], function (i, elm) {
            elm.html('');
            elm.append('<option value="__nosel__">選択してください</option>');
        });

        $.getJSON(
            '/Ledger/Api/GetOrganizations',
            {
                'id': $(this).val()
            },
            function (data) {
                $.each(data, function (i, v) {
                    $('#departments').append($('<option></option>').val(v.id).text(v.departmentName));
                });
            });
    });

    $('#seluser').on('click', '#search', function () {
        var company = $('#companies').val();
        var divisiongroup = $('#divisiongroups').val();
        var division = $('#divisions').val();
        var department = $('#departments').val();

        if (company === 0) {
            company = '';
        }

        if (divisiongroup === '__nosel__') {
            divisiongroup = '';
        }

        if (division === '__nosel__') {
            division = '';
        }

        if (department === '__nosel__') {
            department = '';
        }

        $.getJSON(
            '/Ledger/Api/SearchUser',
            {
                'q': $('#seluser').find('#query').val(),
                'company': company,
                'group': divisiongroup,
                'division': division,
                'department': department,
            },
            function (data) {
                $('#candidates').find('tr:gt(0)').remove();
                $('#inserts').find('tr:gt(0)').remove();

                $('#seluser').find('#searchresult').text('検索結果 ' + data.length + ' 件');
                $('#seluser').find('#selnum').text('選択ユーザー (0)');


                $.each(data, function (i, v) {
                    $('#candidates tbody > tr:last').after($('<tr></tr>').attr('id', v.id)
                        .append($('<td></td>').text(v.company))
                        .append($('<td></td>').text(v.divisiongroup))
                        .append($('<td></td>').text(v.division))
                        .append($('<td></td>').text(v.department))
                        .append($('<td></td>').text(v.name))
                        .append($('<td style="display: none;"></td>').text(v.mail))
                        .append('<td class="ta-c"><button id="selectuser">選択</button></td>'));
                });
            });
    });

    $(document).on('click', '#selectuser', function () {
        var dst = '';

        if ($(this).parents('table').attr('id') === 'candidates') {
            dst = '#inserts';
        } else {
            dst = '#candidates';
        }

        var curTr = $(this).parents('tr');

        //  追加
        $(dst).find('tbody > tr:last').after($('<tr></tr>').attr('id', $(curTr).attr('id')).append(curTr.html()));
        //  削除
        curTr.remove();

        //  選択ユーザ数更新
        $('#seluser').find('#selnum').text('選択ユーザー (' + ($('#inserts').prop('rows').length - 1) + ')');
    });

    //  登録ボタン
    $('#seluser').on('click', '.btn-ok', function () {
        if (seluser_callback !== null) {
            seluser_callback();
        }

        //  閉じる
        closeDialog();

        return false;
    });
});
