(function () {
    'use strict';

    if ($('.loadInput').length > 0) {

        $('.loadInput').change(function (e) {
            console.log(e.target)
            const dt = new DataTransfer();
            for (var i = 0; i < this.files.length; i++) {
                let fileBloc = $('<div/>', { class: 'file-item' }),
                    fileName = $('<span/>', { class: 'file-label', text: this.files.item(i).name });
                fileBloc.append('<span class="file-delete">&#10006;</span>').append(fileName);
                $(this).siblings(".files-names").append(fileBloc);
            };
            for (let file of this.files) {
                dt.items.add(file);
            }
            this.files = dt.files;
            $('span.file-delete').click(function () {
                let name = $(this).next('.file-label').text();
                $(this).parent().remove();
                for (let i = 0; i < dt.items.length; i++) {
                    if (name === dt.items[i].getAsFile().name) {
                        dt.items.remove(i);
                        continue;
                    }
                }
                e.target.files = dt.files;
                console.log(e.target.files)
            });
        });
    }

})();