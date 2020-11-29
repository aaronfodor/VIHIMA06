$(function () {
    getCaffs();
});

function getCaffs() {
    $.ajax({
        type: "GET",
        url: url + "/caff/all",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {

            //data = sorting(data, 'name');
            //console.log(data);
            $("#caffs").html("");

            $.each(data, function (key, caffFile) {


                $.ajax({
                    type: "GET",
                    url: "content/caff-element.html",
                    success: function (line) {

                        $("#caffs").append(line);
                        var caff = $(".contents_row_c").last();
                        caff.find(".card_img").attr("src", "data:image/bmp;base64," + caffFile["preview"]);
                        caff.find(".card_title").html(caffFile["originalFileName"]);

                        caff.prop("id", caffFile["id"]);
                        caff.attr("onclick", 'caffDetails("' + caffFile["id"] + '")');

                        /*
                        console.log(user);
                        userElement.prop("id", user["id"]);
                        userElement.find(".user_name").html(user["name"]);
                        userElement.find(".user_username").html(user["userName"]);
                        userElement.find(".user_email").html(user["email"]);
                        userElement.find(".user_role").html(user["role"]);
                        
                        userElement.find(".del-icon").attr("onclick", 'deleteUser("'+user["id"]+ '")');
                        userElement.find(".edit-icon").attr("onclick", 'editUser("'+user["id"]+ '","'+user["role"]+'")');
*/

                    },
                    error: function (e) {
                        console.log(e);
                        var result = JSON.parse(e.responseText);

                    }
                });

            });

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}


function searchCaffs() {
    var data = JSON.stringify($("#search-input").val());

    $.ajax({
        type: "POST",
        url: url + "/caff/search",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);

            //data = sorting(data, 'name');
            //console.log(data);

            $("#caffs").html("");
            $.each(data, function (key, caffFile) {


                $.ajax({
                    type: "GET",
                    url: "content/caff-element.html",
                    success: function (line) {
                        console.log(line);

                        $("#caffs").append(line);
                        var caff = $(".contents_row_c").last();
                        caff.find(".card_img").attr("src", "data:image/bmp;base64," + caffFile["preview"]);
                        caff.find(".card_title").html(caffFile["originalFileName"]);

                        caff.prop("id", caffFile["id"]);
                        caff.attr("onclick", 'caffDetails("' + caffFile["id"] + '")');

                    },
                    error: function (e) {
                        console.log(e);
                        var result = JSON.parse(e.responseText);

                    }
                });

            });

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function caffDetails(caffId) {
    window.location.href = "details.html?caff=" + caffId;
}
