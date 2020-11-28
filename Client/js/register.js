function register() {
    //window.location.href = "home.html";

    if(validatePassword($("#register_password").val())) {
        if($("#register_password").val() == $("#register_password2").val()) {
            var data = JSON.stringify({ userName: $("#register_username").val(), password: $("#register_password").val(), name: $("#register_name").val(), email: $("#register_email").val()});
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
                    success: function(data) {
                        console.log(data);
                        window.location = "index.html";
                    },
                    error: function(e) {
                        console.log(e);
                        var result = JSON.parse(e.responseText);
                        
                    }
                });
        }
        else {
            console.log("passwords must match")
        }
    }
    else {
        console.log("6 char, capital, lower, number...")
    }
    

}

function validatePassword(password) {
    var regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    return password.match(regex);
}