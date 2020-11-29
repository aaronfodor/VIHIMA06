$(function () {
    openTab("profile");
    getProfile();
});


function openTab(tabName) {

    $(".tabcontent").hide();
    $("#" + tabName).show();

    $(".tablinks").removeClass("active");
    $("#tablinks-" + tabName).addClass("active");

}



function getProfile() {
    $.ajax({
        type: "GET",
        url: url + "/user",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            user = data;
            $("#profile_email").val(user["email"]);
            $("#profile_name").val(user["name"]);
            $("#profile_username").val(user["userName"]);

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function editProfile() {
    var data = JSON.stringify({ userName: $("#profile_username").val(), email: $("#profile_email").val(), name: $("#profile_name").val() });



    $.ajax({
        type: "PUT",
        url: url + "/user",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            alert("Profile saved!");
        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function editPassword() {
    if (validatePassword($("#password1").val())) {
        if ($("#password1").val() == $("#password2").val()) {
            var data = JSON.stringify({ oldpassword: $("#password_old").val(), newpassword: $("#password1").val() });



            $.ajax({
                type: "PUT",
                url: url + "/user/editpassword",
                contentType: "application/json",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + token);
                },
                data: data,
                timeout: 600000,
                processData: false,
                success: function (data) {
                    alert("Password changed.");
                },
                error: function (e) {
                    console.log(e);
                    var result = JSON.parse(e.responseText);
                    alert("An error occured.");
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

function validatePassword(password) {
    var regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    return password.match(regex);
}