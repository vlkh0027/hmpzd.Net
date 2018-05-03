// 共通処理
(function (g) {
    var o = {

        /**
         * 共通初期化
         */
		init:function(){
		    var self = this;

		    // API URL
		    self.apiUrl = '/Ledger/Api';

			if (my.modal) {				
				my.modal.init.call(my.modal);
			}
		},
		
		 /**
         * コントローラごとの基本URL
         */
        getControllerUrl: function () {
            var self = this, pathname=location.pathname;
            pathname=pathname.replace(/\/$/,'').replace(/\/index$/i,'').replace(/\/edit$/i,'');
			return pathname;
        },
    };
    g.my = o;

})((this || 0).self || global);


$(function(){
	//datepicker
	$.datepicker.setDefaults({
		showOn: "both",
		firstDay: 1,
		buttonImage: "/img/ico-cal.png",
		buttonText: "カレンダーから選択",
		buttonImageOnly: true,
		beforeShowDay:function(date){
			if (date.getDay() == 0) {                     // 日曜日
				return [true, 'class-sunday', '日曜日'];
			} else if (date.getDay() == 6) {              // 土曜日
				return [true, 'class-saturday', '土曜日'];
			} else {                                      // 平日
				return [true, 'class-weekday', '平日'];
			}
		}
	});
	$(".datepicker").datepicker();

	
	//timepicker
	function setTimepicker() {
		var options = {step:15, timeFormat:'H:i'};
		$('.timepicker').timepicker(options);
	}
	setTimepicker();
	
	//slide_menu
	var menu = $('.slide_menu'), // スライドインするメニューを指定
	menuBtn = $('#menu-sort-btn'), // トリガーとなるボタンを指定
	body = $(".contents"),
	menuWidth = menu.outerWidth();
	menuBtn.on('click', function(){
	body.toggleClass('open');
		if(body.hasClass('open')){
			menuBtn.addClass('on');
			//body.animate({'left' : 0 }, 300);
			menu.animate({'left' : 0 }, 300);
			menu.css("position","relative");
		} else {
			menuBtn.removeClass('on');
			menu.animate({'left' : -(menuWidth + 10) }, 300);
			//body.animate({'left' : 0 }, 300);
			menu.css("position","");
		}
	});
	
	//toggle
	$(".tgl-btn").on("click", function() {
		$(this).toggleClass("on").next().slideToggle();
		return false;
	});

	//powertip
    $('.tooltips').powerTip({ placement: 'n' })
});



$(document).ready(function() {
	//自由入力
	$('.select-input-text').select2({
		placeholder: "自由入力可",
		allowClear: true,
		multiple: false,
		tags: true,
		createTag: function(tag){return {id: tag.term, text: tag.term, tag: true}}
	});
});

function digitSeparate(num) {
    var buf = num;

    if (!isNaN(num)) {
        //  数値を文字列に変換
        buf = String(num);
    }

    return buf.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
}
