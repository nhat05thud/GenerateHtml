﻿'use strict';

; (function (document, window, index) {
    var elements = document.querySelectorAll('.customfile-input');
    Array.prototype.forEach.call(elements, function (element) {
        var label = element,
            labelVal = label.innerHTML;
        var input = element.querySelector('input[type="file"]');
        input.addEventListener('change', function (e) {
            var fileName = '';
            if (this.files && this.files.length > 1)
                fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
            else
                fileName = e.target.value.split('\\').pop();

            if (fileName) {
                label.querySelector('span').innerHTML = fileName;
                $(label).find('input[type="hidden"]').val(fileName);
            } else {
                label.innerHTML = labelVal;
            }
        });

        // Firefox bug fix
        input.addEventListener('focus', function () { input.classList.add('has-focus'); });
        input.addEventListener('blur', function () { input.classList.remove('has-focus'); });
    });

    $('i[rel="pre"]').replaceWith(function () {
        return $('<pre><code>' + $(this).html() + '</code></pre>');
    });
    var pres = document.querySelectorAll('pre,kbd,blockquote');
    for (var i = 0; i < pres.length; i++) {
        pres[i].addEventListener("dblclick", function () {
            var selection = getSelection();
            var range = document.createRange();
            range.selectNodeContents(this);
            selection.removeAllRanges();
            selection.addRange(range);
        }, false);
    }
}(document, window, 0));