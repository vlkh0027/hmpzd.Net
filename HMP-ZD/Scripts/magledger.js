
var cb = null;
var Sender = null;

$(document).ready(function () {

    $(".other-text").hide();

    $(".other").click(function () {
        var oth = $(this).next('.other-text');

        if ($(oth).is(':visible')) {
            $(oth).slideUp('falst');
        } else {
            $(oth).slideDown('fast');
        }

        return false;
    });

    $('.other-text').on('change', 'input:text', function () {
        $(this).parents('dd').find('select').prop('disabled', $(this).val() !== '');
    });

    //
    //  ヘッダ情報
    //
    $('.setUser').on('click', function () {
        $(this).parent().find('.user').val('test');

        var now = new Date();
        var mmdd = now.getFullYear() + '/' +
            ('0' + (now.getMonth() + 1)).slice(-2) + '/' +
            ('0' + now.getDate()).slice(-2);
        $(this).parent().find('.datepicker').val(mmdd);
    });

    //  ユーザ選択の重複チェック
    function isUserDuplicated(ul, userID) {
        var ret = false;

        $(ul).children('li').each(function (i, li) {
            if ($(li).attr('id') === userID) {
                ret = true;
                return false;
            }
        });

        return ret;
    }

    //  ユーザ選択
    function seluser() {
        var ul = null;

        if ($(Sender).hasClass('selectusers')) {
            ul = $(Sender).find('.selusers');
        } else {
            ul = $(Sender).parents('.selectusers').find('.selusers');
        }

        //  0行目はヘッダ行なのでスキップ
        $('#inserts').find('tr:gt(0)').each(function (i, v) {
            var id = $(v).attr('id');
            var company = $(v.cells[0]).text();
            var group = $(v.cells[1]).text();
            var division = $(v.cells[2]).text();
            var department = $(v.cells[3]).text();
            var name = $(v.cells[4]).text();

            if (!isUserDuplicated(ul, id)) {
                $(ul).append($('<li></li>').attr('id', id).text(company + ' ' + group + ' ' + division + ' ' + department + ' ' + name + ' ').append('<a class="deluser" href="javascript:void(0);"><i class="fa fa-lg fa-fw fa-times"></i></a>'));
            }
        });
    }

    //  ユーザを削除する×ボタン
    $(document).on('click', '.deluser', function () {
        $(this).parents('li').remove();
    });

    //  部署が変更された
    function OnDepartmentChanged(teamID) {
        $.getJSON(
            '/Ledger/Api/GetTeams',
            {
                'id': $('#Department').val()
            },
            function (data) {
                var chTeam = $('#Team');

                chTeam.html('');

                chTeam.append('<option value="__nosel__">選択してください</option>');

                $.each(data, function (i, v) {
                    chTeam.append($('<option></option>').val(v.Value).text(v.Text));
                });
            }).done(function () {
                if (teamID !== undefined) {
                    $('#Team').val(teamID).trigger('change');
                }
            });
    }

    //  チーム情報取得
    $('#Department').on('change', function () {
        OnDepartmentChanged();
    });

    //  媒体社情報取得
    $('#Team').on('change', function () {
        $.getJSON(
            '/Ledger/Api/GetMagazineCompanies',
            {
                'id': $(this).val()
            },
            function (data) {
                var cust = $('#CustomerID');

                cust.html('');
                cust.append('<option value="__nosel__">選択してください</option>');

                $.each(data, function (i, v) {
                    cust.append($('<option></option>').val(v.Value).text(v.Text));
                });
            });
    });

    //  媒体誌情報取得
    $('#CustomerID').on('change', function () {
        //  自由入力のEnable/Disable判定
        $(this).parent().find('.other-text input').prop('disabled', $(this).val() !== '__nosel__');

        $.getJSON(
            '/Ledger/Api/GetVehiclesByID',
            {
                'id': $(this).val()
            },
            function (data) {
                var veh = $('#VehicleID');

                veh.html('');
                veh.append('<option value="__nosel__">選択してください</option>');

                $.each(data, function (i, v) {
                    veh.append($('<option></option>').val(v.Value).text(v.Text));
                });
            });
    });

    //  媒体社自由入力
    $('#CustomerFree').on('change', function () {
        if ($('#Department').val() === '__nosel__' ||
            $('#Team').val() === '__nosel__') {
            return;
        }

        $.getJSON(
            '/Ledger/Api/GetVehiclesByName',
            {
                'name': $(this).val()
            },
            function (data) {
                var veh = $('#VehicleID');

                veh.html('');
                veh.append('<option value="__nosel__">選択してください</option>');

                $.each(data, function (i, v) {
                    veh.append($('<option></option>').val(v.Value).text(v.Text));
                });
            });
    });

    //  媒体誌が選択された
    $('#VehicleID').on('change', function () {
        //  自由入力のEnable/Disable判定
        $(this).parent().find('.other-text input').prop('disabled', $(this).val() !== '__nosel__');

        $.getJSON(
            '/Ledger/Api/GetVehicleSchedule',
            {
                'id': $(this).val(),
            },
            function (data) {
                $('#ReleaseDate').val(data.ReleaseDate);
            });
    });

    //  自分を担当者に設定
    $('.setown').on('click', function () {
        $.getJSON(
            '/Ledger/Api/GetMyDepartmentTeamInfo',
            {
                'id': $(this).attr('id'),
            },
            function (data) {
                //  部署・チーム
                /*
                 *  この部分は暫定で。
                 */
                $('#Department').val(data.departmentCode);
                OnDepartmentChanged(data.teamID);

                //  雑誌部担当者に自分を追加
                //  重複していたら追加しない
                if (!isUserDuplicated($('#magazineRepresentatives'), data.userID)) {
                    $('#magazineRepresentatives').append($('<li></li>').attr('id', data.userID).text(data.companyName + ' ' + data.groupName + ' ' + data.divisionName + ' ' + data.departmentName + ' ' + data.name + ' ').append('<a class="deluser" href="javascript:void(0);"><i class="fa fa-lg fa-fw fa-times"></i></a>'));
                }
            });

        return false;
    });

    //
    //  支払情報
    //
    $('#recordingDate').on('change', function () {
        //  コミッション率は3.7(%)固定
        $('#commissionRate').val('3.7');
    });

    //  指定行の金額合計計算
    function calcRowPrice(tr) {
        var gross = tr.find('.gross > input').val();
        var rate = tr.find('.rate > input').val();
        var net = tr.find('.netprice');

        //  グロスが空欄?
        if (gross === '') {
            //  計算しない
            $(net).text('0');
            return;
        }

        //  支払金額ネット
        var price = Math.round(gross * ((100 - rate) / 100), 1);

        //  3ケタ区切りで表示
        $(net).text(digitSeparate(price));
    }

    //  その他の合計計算
    function calcOtherPrice() {
        var trTotal = $('#payee').find('.otherTotal');
        var grossTotal = 0;
        var netTotal = 0;

        $('#payee').find('.others').each(function (i, v) {
            //  グロス
            var cellPrice = Number($(v).find('.gross > input').val());

            if (cellPrice === '') {
                cellPrice = 0;
            }

            grossTotal += cellPrice;

            //  ネット金額はケタ区切り記号が入っている場合があるので、
            //  取り除いてから数値に変換する
            cellPrice = Number($(v).find('.netprice').text().replace(/,/g, ""));

            if (cellPrice === '') {
                cellPrice = 0;
            }

            netTotal += cellPrice;
        });

        $(trTotal).find('.gross').text(digitSeparate(grossTotal));
        $(trTotal).find('.netprice').text(digitSeparate(netTotal));
    }

    //  払い金額合計計算
    function calcTotalPrice() {
        var trTotal = $('#payee').find('.payeeTotal');
        var grossTotal = 0;
        var netTotal = 0;

        $('#payee').find('.publication, .others').each(function (i, v) {
            //  グロス
            var cellPrice = Number($(v).find('.gross > input').val());

            if (cellPrice === '') {
                cellPrice = 0;
            }

            grossTotal += cellPrice;

            //  ネット金額はケタ区切り記号が入っている場合があるので、
            //  取り除いてから数値に変換する
            cellPrice = Number($(v).find('.netprice').text().replace(/,/g, ""));

            if (cellPrice === '') {
                cellPrice = 0;
            }

            netTotal += cellPrice;
        });

        $(trTotal).find('.gross').text(digitSeparate(grossTotal));
        $(trTotal).find('.netprice').text(digitSeparate(netTotal));
    }

    //  引き渡し建値計算
    function calcDeliveryQuotationPrice() {
        var elm = null;
        var deliveryGross = 0;
        var deliveryNet = 0;

        //  払い金額合計
        elm = $('#payee').find('.payeeTotal');
        var gross = $(elm).find('.gross').text().replace(/,/g, "");
        var net = $(elm).find('.netprice').text().replace(/,/g, "");

        gross = (gross === 0) ? 0 : gross;
        net = (net === 0) ? 0 : net;

        deliveryGross = Number(gross);
        deliveryNet = Number(net);

        //  媒体収益
        elm = $('#payee').find('.revenue');
        gross = $(elm).find('.gross > input').val();
        net = $(elm).find('.netprice').text().replace(/,/g, "");

        gross = (gross === 0) ? 0 : gross;
        net = (net === 0) ? 0 : net;

        deliveryGross -= Number(gross);
        deliveryNet -= Number(net);

        //  媒体戦略原価
        gross = 0;
        net = 0;

        $('#payee').find('.strategic').each(function (i, v) {
            gross = $(v).find('.gross > input').val();
            net = $(v).find('.netprice').text().replace(/,/g, "");

            gross = (gross === 0) ? 0 : gross;
            net = (net === 0) ? 0 : net;

            deliveryGross -= Number(gross);
            deliveryNet -= Number(net);
        });

        elm = $('#payee').find('.deliveryquotation');
        $(elm).find('.gross').text(digitSeparate(deliveryGross));
        $(elm).find('.netprice').text(digitSeparate(deliveryNet));
    }

    //  営収・売上計算
    function calcIncome() {
        var pm = 0;
        var income = 0;

        //  売上
        var commission = $('#commission').find('.comm input:text').val();
        var deliveryGross = $('#payee').find('.deliveryquotation .gross').text().replace(/,/g, "");
        var deliveryNet = $('#payee').find('.deliveryquotation .netprice').text().replace(/,/g, "");

        pm = Math.round((Number(deliveryGross) * Number(commission)) + deliveryNet, 1);

        //  営収
        income = Math.round(pm - deliveryNet, 1);

        $('#total').find('.pm .total').text(digitSeparate(pm));
        $('#total').find('.businessincome .total').text(digitSeparate(income));
    }

    $('#payee').on('change', '.publication input:text, .others input:text, .strategic input:text, .revenue input:text', function () {
        //  入力がコメントなら何もしない
        if (!($(this).parent().hasClass('gross') || $(this).parent().hasClass('rate'))) {
            return;
        }

        //  数値チェック
        if (isNaN($(this).val())) {
            alert('数値を入力してください');
            return;
        }

        if ($(this).parents('tr').find('.rate input:text').val() === '') {
            //  デフォルトは20%
            $(this).parents('tr').find('.rate input:text').val('20');
        }

        //  各計算処理へ
        calcRowPrice($(this).parents('tr'));
        calcTotalPrice();
        calcOtherPrice();
        calcDeliveryQuotationPrice();
        calcIncome();
    });

    //
    //  メール送信先一覧
    //
    $('.show_seluser').on('click', function () {
        //  コールバック設定
        if ($(this).parents('div.selectusers')[0]) {
            seluser_callback = seluser;
        } else if ($(this).parents('div#sendmailuser')[0]) {
            seluser_callback = seluserMail;
        }
    });

    //  メール送信先の表示
    function seluserMail() {
        var table = $('#sendmailuser table');

        $('#inserts').find('tr:gt(0)').each(function (i, v) {
            var id = $(v).attr('id');
            var company = $(v.cells[0]).text();
            var group = $(v.cells[1]).text();
            var division = $(v.cells[2]).text();
            var department = $(v.cells[3]).text();
            var name = $(v.cells[4]).text();
            var mail = $(v.cells[5]).text();

            $(table).append($('<tr></tr>').attr('id', id)
                .append($('<td></td>').text(name))
                .append($('<td></td>').text(department))
                .append($('<td></td>').append($('<a></a>').attr('href', 'mailto:' + mail).text(mail)))
                .append('<td class="ta-c"><a class="btn-hk btn-small" id="delmail" href="#">削除</a></td>'));
        });
    }

    $('#sendmailuser').on('click', '#delmail', function () {
        $(this).parents('tr').remove();
        return false;
    });

    //  登録ボタンが押された
    $('#regist').on('click', function () {
        //        $(this).prop('disabled', true);
        /*
         *   リスト系
         */
        //  営業担当者

        var saleRepresentatives = [];

        $('#saleRepresentatives').children('li').each(function (i, li) {
            saleRepresentatives.push($(li).attr('id'));
        });
        alert("abc");
        //  雑誌部担当者
        var magazineRepresentatives = [];

        $('#magazineRepresentatives').children('li').each(function (i, li) {
            magazineRepresentatives.push($(li).attr('id'));
        });

        //  制作担当者
        var productionRepresentatives = [];

        $('#productionRepresentatives').children('li').each(function (i, li) {
            productionRepresentatives.push($(li).attr('id'));
        });

        //  進行部担当者
        var progressRepresentatives = [];

        $('#progressRepresentatives').children('li').each(function (i, li) {
            progressRepresentatives.push($(li).attr('id'));
        });

        //  BA担当者
        var baRepresentatives = [];

        $('#baRepresentatives').children('li').each(function (i, li) {
            baRepresentatives.push($(li).attr('id'));
        });

        //  更新情報メール送信先
        var mailContents = [];

        $('#mailContents').children('li').each(function (i, li) {
            mailContents.push($(li).attr('id'));
        });

        //  メール送信先一覧
        var mailDestinations = [];

        $('#sendmailuser table').find('tr:gt(0)').each(function (i, tr) {
            mailDestinations.push($(tr).attr('id'));
        });

        //  広告種類
        var advertisementTypes = [];

        $('div #Advertisements').children('#adv').each(function (i, adv) {
            var advertisement = {};

            advertisement['Advertisement'] = $(this).find('#name').parent().text();
            advertisement['Checked'] = $(this).find('#name').prop('checked');
            advertisement['Picture'] = $(this).find('#Pic option:selected').text();
            advertisement['Color'] = $(this).find('#Col option:selected').text();
            advertisement['Page'] = $(this).find('#Pag option:selected').text();

            advertisementTypes.push(advertisement);
        });

        //  formのデータを変換
        var ledger = parseFormData($('form').serializeArray());

        if ($('#company').text() === '') {
            ledger['Company'] = '';
        } else {
            ledger['Company'] = $('#company').text();
        }

        ledger['ElectronicMagazine'] = $('#emag').prop('checked');
        ledger['IssuanceDate'] = $('#IssuanceDate').text();
        ledger['IssuanceUserName'] = $('#IssuanceUserName').text();
        ledger['AdvertisementTypes'] = advertisementTypes;
        ledger['SaleRepresentatives'] = saleRepresentatives;
        ledger['MagazineRepresentatives'] = magazineRepresentatives;
        ledger['ProductionRepresentatives'] = productionRepresentatives;
        ledger['ProgressRepresentatives'] = progressRepresentatives;
        ledger['BARepresentatives'] = baRepresentatives;
        ledger['MailContents'] = mailContents;
        ledger['MailDestinations'] = mailDestinations;

        //  ajax通信
        $.ajax({
            type: 'POST',
            url: '/Ledger/Magazine/RegistMagazineLedger',
            data: JSON.stringify(ledger),
            contentType: 'application/json',
            datatype: 'json',
            success: function (data) {
                $('#error-desc').html('');

                if (data.Result === true) {
                    alert('success');
                } else if (data.Result === false) {
                    //  エラー表示
                    $.each(data.Reason, function (i, v) {
                        $('#error-desc').append($('<li></li>').text(v));
                    });

                    $('#error').show();

                    //  画面上部へ
                    window.location.href = '#';
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                var i = 0;
            },
            complete: function () {
                var i = 0;
            },
        })

        return false;
    });

    $('#delete').on('click', function () {
        var msg = confirm('削除します。よろしいですか?');

        if (msg == false) {
            return;
        }

        //  削除処理
        $.getJSON(
            '/Ledger/Magazine/DeleteLedger',
            {
                'id': $('#ID').val()
            },
            function (data) {
                //  結果表示
                alert(data.Reason);

                if (data.Result === true) {
                    //  成功
                    //  indexへ移動

                } else if (data.Result === false) {
                    //  失敗
                }
            });

        return false;
    });

    function parseFormData(data) {
        var ret = {};

        $.each(data, function (i, v) {
            ret[v.name] = v.value;
        });

        return ret;
    }

});
