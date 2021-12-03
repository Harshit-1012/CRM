function copyText(textToCopy, toolTip) {
    var copyText = document.getElementById(textToCopy);
    copyText.select();
    copyText.setSelectionRange(0, 99999);
    document.execCommand("copy");

    var tooltip = document.getElementById(toolTip);
    tooltip.innerHTML = "Link Copied";
}

function exitTooltip(toolTip) {
    var tooltip = document.getElementById(toolTip);
    tooltip.innerHTML = "Click to copy referral link";
}

$(".selectText").on('click', function () {
	var sel, range;
	var el = $(this)[0];
	if (window.getSelection && document.createRange) { //Browser compatibility
		sel = window.getSelection();
		if (sel.toString() == '') { //no text selection
			window.setTimeout(function () {
				range = document.createRange(); //range object
				range.selectNodeContents(el); //sets Range
				sel.removeAllRanges(); //remove all ranges from selection
				sel.addRange(range);//add Range to a Selection.
			}, 1);
		}
	} else if (document.selection) { //older ie
		sel = document.selection.createRange();
		if (sel.text == '') { //no text selection
			range = document.body.createTextRange();//Creates TextRange object
			range.moveToElementText(el);//sets Range
			range.select(); //make selection.
		}
	}
});