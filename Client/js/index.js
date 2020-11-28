$(document).ready(function() {

    /*
    $("#login_button").click(function(event) {

        event.preventDefault();

        $.ajax({
            type: "POST",
            url: "api.php",
            data: { method: "user_login", email: $('#login_email').val(), password: $('#login_password').val() },
            timeout: 600000,
            success: function(data) {
                console.log(data);
                window.location.href = 'pages/users.php';
            },
            error: function(e) {
                console.log(e);
                var result = JSON.parse(e.responseText);
                console.log(result['login']);

                if (result['login'] == '2factorauth') {
                    modal.style.display = "block";
                    $('[tabindex=1]').focus();

                    $.ajax({
                        type: "POST",
                        url: "https://bssolution.hu/smssendingapi.php",
                        data: { telephone: '+36209521724', code: six_digit },
                        timeout: 600000,
                        success: function(data) {
                            console.log(data);
                            window.location.href = 'pages/users.php';
                        },
                        error: function(e) {
                            console.log(e);
                            var result = JSON.parse(e.responseText);
                            console.log(result['login']);
                            if (result['login'] == '2factorauth') {
                                modal.style.display = "block";
                                $('[tabindex=1]').focus();
                            }
                        }
                    });
                }
            }
        });

    });*/

    tabindex = 1;
    $(".passcode").on('input', function() {

        var $input = $(this);
        tabindex = $input.prop("tabindex");
        tabindex++;
        console.log('tabidx: ' + tabindex)
        $('[tabindex=' + tabindex + ']').focus();

        var complete = true;
        var userCode = "";
        $(".passcode").each(function(index) {
            if ($(this).val().length < 1) {
                complete = false;
            } else {
                userCode += $(this).val();
            }
        });

        if (complete) {
            $.ajax({
                type: "POST",
                url: "api.php",
                data: { method: "two_factor_auth", email: $('#login_email').val(), password: $('#login_password').val(), code: code, user_code: userCode },
                timeout: 600000,
                success: function(data) {
                    console.log(data);
                    //var result = JSON.parse(data);
                    //console.log(result['2factorauth']);
                    success();
                    window.location.href = 'pages/menu.php';

                },
                error: function(e) {
                    console.log(e);
                    var result = JSON.parse(e.responseText);
                    shake();

                }
            });
        }

    });


    $(".passcode").keyup(function(event) {
        var key = event.keyCode || event.charCode;
        if (key == 8 || key == 46) {
            console.log(key);
            if (tabindex > 0) {
                tabindex--;
                console.log('tabidx: ' + tabindex);

                $('[tabindex=' + tabindex + ']').focus();
                $('[tabindex=' + tabindex + ']').val('');
            }
        }


    });

});

function shake() {
    $(".passcode").each(function(index) {
        $(this).addClass("input-err");
    });

    $('.modal-content').effect("shake", function() {
        $(".passcode").each(function(index) {
            $(this).removeClass("input-err");
        });
    });

}

function success() {
    $(".passcode").each(function(index) {
        $(this).addClass("input-succ");
    });
}