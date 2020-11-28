$(function() {
    openTab("profile");
    getProfile();
});


function openTab(tabName) {

    $(".tabcontent").hide();
    $("#" + tabName).show();

    $(".tablinks").removeClass("active");
    $("#tablinks-"+tabName).addClass("active");

  }



  function getProfile() {
    $.ajax({
        type: "GET",
        url: "https://845644db5a0e.ngrok.io/user",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer '+ token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function(data) {
            console.log(data);
            user = data;
            console.log(user);
            $("#profile_email").val(user["email"]);
            $("#profile_name").val(user["name"]);
            $("#profile_username").val(user["userName"]);

        },
        error: function(e) {
            console.log(e);
            var result = JSON.parse(e.responseText);
            
        }
    });     
  }

  function editProfile() {
    var data = JSON.stringify({ userName: $("#profile_username").val(), email: $("#profile_email").val(), name: $("#profile_name").val()});

    console.log(data);

    
    $.ajax({
        type: "PUT",
        url: "https://845644db5a0e.ngrok.io/user",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer '+ token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function(data) {
            console.log(data);

        },
        error: function(e) {
            console.log(e);
            var result = JSON.parse(e.responseText);
            
        }
    });     
  }

function editPassword() {
    if(validatePassword($("#password1").val())) {
        if($("#password1").val() == $("#password2").val()) {
            var data = JSON.stringify({ oldpassword: $("#password_old").val(), newpassword: $("#password1").val()});

            console.log(data);

            
            $.ajax({
                type: "PUT",
                url: url + "/user/editpassword",
                contentType: "application/json",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer '+ token);
                },
                data: data,
                timeout: 600000,
                processData: false,
                success: function(data) {
                    console.log(data);

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
    console.log(password)
    var regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    return password.match(regex);
}