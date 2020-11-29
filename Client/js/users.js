$(function () {
    getUsers();
});

function getUsers() {
    $.ajax({
        type: "GET",
        url: url + "/admin/all",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);

            data = sorting(data, 'name');
            console.log(data);

            $.each(data, function (key, user) {


                $.ajax({
                    type: "GET",
                    url: "content/user-line.html",
                    success: function (line) {
                        console.log(line);

                        $("#users").append(line);
                        var userElement = $(".linelist").last();
                        console.log(user);
                        userElement.prop("id", user["id"]);
                        userElement.find(".user_name").html(user["name"]);
                        userElement.find(".user_username").html(user["userName"]);
                        userElement.find(".user_email").html(user["email"]);
                        userElement.find(".user_role").html(user["role"]);

                        userElement.find(".del-icon").attr("onclick", 'deleteUser("' + user["id"] + '")');
                        userElement.find(".edit-icon").attr("onclick", 'editUser("' + user["id"] + '","' + user["role"] + '")');

                        /*
                        userElement.find(".del-icon").click(function() {
                            deleteUser(user["userid"]);
                        });*/

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

function searchUsers() {
    console.log($("#search-input").val());
    var data = JSON.stringify($("#search-input").val());

    $("#users").html("");

    $.ajax({
        type: "POST",
        url: url + "/admin/search",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);

            data = sorting(data, 'name');
            console.log(data);

            $.each(data, function (key, user) {

                $.ajax({
                    type: "GET",
                    url: "content/user-line.html",
                    success: function (line) {
                        console.log(line);

                        $("#users").append(line);
                        var userElement = $(".linelist").last();
                        console.log(user);
                        userElement.prop("id", user["id"]);
                        userElement.find(".user_name").html(user["name"]);
                        userElement.find(".user_username").html(user["userName"]);
                        userElement.find(".user_email").html(user["email"]);
                        userElement.find(".user_role").html(user["role"]);

                        userElement.find(".del-icon").attr("onclick", 'deleteUser("' + user["id"] + '")');
                        userElement.find(".edit-icon").attr("onclick", 'editUser("' + user["id"] + '","' + user["role"] + '")');

                        /*
                        userElement.find(".del-icon").click(function() {
                            deleteUser(user["userid"]);
                        });*/

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

var userid;

function deleteUser(userId) {
    userid = userId;
    console.log(userid);
    $("#delModal").show();
}

function del() {
    $.ajax({
        type: "DELETE",
        url: url + "/admin/" + userid,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);

            $("#delModal").hide();
            $("#users").html("");
            getUsers();

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

var role;
var editId;
function editUser(userId, userRole) {
    editId = userId;
    role = userRole;

    console.log(editId);
    $("#editModal").find("#radio" + userRole).prop("checked", true);
    $("#editModal").show();
}

function edit() {
    var data = JSON.stringify({ userid: editId, admin: $("#radioAdmin").is(":checked") });
    console.log(editId);
    console.log($("#adminRole").is(":checked"));
    $.ajax({
        type: "PUT",
        url: url + "/admin",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            console.log(data);

            $("#editModal").hide();
            $("#users").html("");
            getUsers();

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function sorting(json_object, key_to_sort_by) {
    function sortByKey(a, b) {
        var x = a[key_to_sort_by];
        var y = b[key_to_sort_by];
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    }

    console.log(json_object.sort(sortByKey));
    var obj = json_object.sort(sortByKey);
    return obj;
}
