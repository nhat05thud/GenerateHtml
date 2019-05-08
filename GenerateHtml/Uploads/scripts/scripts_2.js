$(".input-type-1").each(function(){
	var title__size = $(this).children("span").width();
	$(this).children("textarea").css("text-indent", title__size + 5);
});
$(".input-type-2").each(function(){
	var title__size = $(this).children("span").width();
	$(this).children("input[type='text']").css("text-indent", title__size + 5);
});
$('textarea[data-limit-rows=true]')
.on('keypress', function (event) {
	var textarea = $(this),
		text = textarea.val(),
		numberOfLines = (text.match(/\n/g) || []).length + 1,
		maxRows = parseInt(textarea.attr('rows'));

	if (event.which === 13 && numberOfLines === maxRows ) {
	  return false;
	}
});