$(() => {
    $(".shorten").on('click', function () {
        const url = $(".url").val();
        $.post('/home/shortenUrl', { url }, function (h) {
            $(".well").append(`<a href="${h}">${h}</a>`);
        });
        $(".shorten").hide();
        $(".url").val('');
    });
    $(".url").on('keyup', function () {
        $(".shorten").show();
    });

});

