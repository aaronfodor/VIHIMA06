
function register() {
    //window.location.href = "home.html";

    if (notEmpty()) {


        if (validatePassword($("#register_password").val())) {
            if ($("#register_password").val() == $("#register_password2").val()) {
                var data = JSON.stringify({ userName: $("#register_username").val(), password: $("#register_password").val(), name: $("#register_name").val(), email: $("#register_email").val() });
                console.log(data);
                $.ajax({
                    type: "POST",
                    url: url + "/user/register",
                    contentType: "application/json",
                    //beforeSend: function (xhr) {   //Include the bearer token in header
                    //    xhr.setRequestHeader("Authorization", 'Bearer '+ token);
                    //},
                    data: data,
                    timeout: 600000,
                    processData: false,
                    success: function (data) {
                        console.log(data);
                        window.location = "index.html";
                    },
                    error: function (e) {
                        console.log(e);
                        var result = JSON.parse(e.responseText);

                    }
                });
            }
            else {
                console.log("passwords must match")
                alert("Passwords must match.")
            }
        }
        else {
            console.log("6 char, capital, lower, number...")
            alert("Password must contain at least 6 characters, an upper case character, a lower case character and a number.");

        }
    }
    else {
        alert("All fields must be filled out.");
    }

}
function notEmpty() {
    var empty = false;
    $(".login").each(function () {
        if ($(this).val() == "") {
            empty = true;
        }
    });
    return !empty;
}


function validatePassword(password) {
    var regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    return password.match(regex);
}

var url = "https://7f69288b929b.ngrok.io";
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}