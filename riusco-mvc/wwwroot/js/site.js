$('textarea').on('input', function () {
    this.style.height = 'auto';
    this.style.height = (this.scrollHeight + 10) + 'px';
});

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();

        reader.onload = function(e) {
            $('.image-upload-wrap').hide();

            $('.file-upload-image').attr('src', e.target.result);
            $('.file-upload-content').show();

            $('.image-title').html(input.files[0].name);
        };

        reader.readAsDataURL(input.files[0]);

    } else {
        removeUpload();
    }
}

function removeUpload() {
    $('.file-upload-input').replaceWith($('.file-upload-input').clone());
    $('.file-upload-content').hide();
    $('.image-upload-wrap').show();
}

$('.image-upload-wrap').bind('dragover', function () {
    $('.image-upload-wrap').addClass('image-dropping');
});

$('.image-upload-wrap').bind('dragleave', function () {
    $('.image-upload-wrap').removeClass('image-dropping');
});

!(function (a) {
    a(function () {
        a('[data-toggle="password"]').each(function () {
            var b = a(this);
            var c = a(this).parent().find(".password-toggle");
            c.css("cursor", "pointer").addClass("input-password-hide");
            c.on("click", function () {
                if (c.hasClass("input-password-hide")) {
                    c.removeClass("input-password-hide").addClass("input-password-show");
                    c.find("path").attr("d", "M 1.09,3.03\n" +
                        "           C 1.09,3.03 3.58,5.52 3.58,5.52\n" +
                        "             3.58,5.52 4.07,6.01 4.07,6.01\n" +
                        "             2.27,7.42 0.85,9.29 0.00,11.45\n" +
                        "             1.89,16.24 6.54,19.63 12.00,19.63\n" +
                        "             13.69,19.63 15.30,19.31 16.78,18.71\n" +
                        "             16.78,18.71 17.24,19.18 17.24,19.18\n" +
                        "             17.24,19.18 20.42,22.36 20.42,22.36\n" +
                        "             20.42,22.36 21.81,20.98 21.81,20.98\n" +
                        "             21.81,20.98 2.48,1.64 2.48,1.64\n" +
                        "             2.48,1.64 1.09,3.03 1.09,3.03 Z\n" +
                        "           M 7.12,9.06\n" +
                        "           C 7.12,9.06 8.81,10.74 8.81,10.74\n" +
                        "             8.76,10.97 8.73,11.21 8.73,11.45\n" +
                        "             8.73,13.26 10.19,14.73 12.00,14.73\n" +
                        "             12.24,14.73 12.48,14.69 12.71,14.64\n" +
                        "             12.71,14.64 14.39,16.33 14.39,16.33\n" +
                        "             13.67,16.69 12.86,16.91 12.00,16.91\n" +
                        "             8.99,16.91 6.54,14.46 6.54,11.45\n" +
                        "             6.54,10.59 6.76,9.79 7.12,9.06 Z\n" +
                        "           M 12.00,6.00\n" +
                        "           C 15.01,6.00 17.45,8.44 17.45,11.45\n" +
                        "             17.45,12.16 17.31,12.83 17.06,13.45\n" +
                        "             17.06,13.45 20.25,16.64 20.25,16.64\n" +
                        "             21.90,15.26 23.20,13.48 24.00,11.45\n" +
                        "             22.11,6.67 17.46,3.27 12.00,3.27\n" +
                        "             10.47,3.27 9.01,3.55 7.65,4.04\n" +
                        "             7.65,4.04 10.01,6.39 10.01,6.39\n" +
                        "             10.62,6.15 11.29,6.00 12.00,6.00 Z\n" +
                        "           M 11.82,8.20\n" +
                        "           C 11.82,8.20 15.25,11.63 15.25,11.63\n" +
                        "             15.25,11.63 15.27,11.45 15.27,11.45\n" +
                        "             15.27,9.65 13.80,8.18 12.00,8.18\n" +
                        "             12.00,8.18 11.82,8.20 11.82,8.20 Z");
                    b.attr("type", "text");
                } else {
                    c.removeClass("input-password-show").addClass("input-password-hide");
                    c.find("path").attr("d", "M 12.00,3.82\n" +
                        "           C 6.55,3.82 1.89,7.21 0.00,12.00\n" +
                        "             1.89,16.79 6.55,20.18 12.00,20.18\n" +
                        "             17.46,20.18 22.11,16.79 24.00,12.00\n" +
                        "             22.11,7.21 17.46,3.82 12.00,3.82 Z\n" +
                        "           M 12.00,17.45\n" +
                        "           C 8.99,17.45 6.55,15.01 6.55,12.00\n" +
                        "             6.55,8.99 8.99,6.55 12.00,6.55\n" +
                        "             15.01,6.55 17.45,8.99 17.45,12.00\n" +
                        "             17.45,15.01 15.01,17.45 12.00,17.45 Z\n" +
                        "           M 12.00,8.73\n" +
                        "           C 10.19,8.73 8.73,10.19 8.73,12.00\n" +
                        "             8.73,13.81 10.19,15.27 12.00,15.27\n" +
                        "             13.81,15.27 15.27,13.81 15.27,12.00\n" +
                        "             15.27,10.19 13.81,8.73 12.00,8.73 Z");
                    b.attr("type", "password");
                }
            });
        });
    });
})(window.jQuery);