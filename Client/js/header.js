$(".circle-header").click( function() {
    window.location.href = "home.html";
})

var user;
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
            $("#user-name").html(data["name"]);
        },
        error: function(e) {
            console.log(e);
            var result = JSON.parse(e.responseText);
            
        }
    });


function logout() {
    $.ajax({
        type: "POST",
        url: "https://845644db5a0e.ngrok.io/user/logout",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer '+ token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function(data) {
            console.log(data);
            window.location = "index.html"
        },
        error: function(e) {
            console.log(e);
            var result = JSON.parse(e.responseText);
            
        }
    });
}

function goToProfile() {
    window.location= "edit-profile.html"
}