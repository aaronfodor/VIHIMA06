$(".circle-header").click(function () {
    window.location.href = "home.html";
})

var user;
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
        console.log(data);
        user = data;
        $("#user-name").html(data["name"]);

        if (user["role"] == "Admin") {
            $("#users-link").show();
        }
    },
    error: function (e) {
        console.log(e);
        //var result = JSON.parse(e.responseText);
        //window.location = "index.html";
    }
});


function logout() {
    $.ajax({
        type: "POST",
        url: url + "/user/logout",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);
            document.cookie = "token=" + "-";
            window.location = "index.html"
        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function goToProfile() {
    window.location = "edit-profile.html"
}