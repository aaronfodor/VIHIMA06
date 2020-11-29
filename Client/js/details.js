$(function () {
    var query = window.location.search.substring(1);
    var qs = parse_query_string(query);

    caffId = qs["caff"];

    getCaff();

    getComments();
    //downloadCaff(caffId);
});

var caffId;

function parse_query_string(query) {
    var vars = query.split("&");
    var query_string = {};
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        var key = decodeURIComponent(pair[0]);
        var value = decodeURIComponent(pair[1]);
        // If first entry with this name
        if (typeof query_string[key] === "undefined") {
            query_string[key] = decodeURIComponent(value);
            // If second entry with this name
        } else if (typeof query_string[key] === "string") {
            var arr = [query_string[key], decodeURIComponent(value)];
            query_string[key] = arr;
            // If third or later entry with this name
        } else {
            query_string[key].push(decodeURIComponent(value));
        }
    }
    return query_string;
}




function getCaff() {
    $.ajax({
        type: "GET",
        url: url + "/caff/" + caffId,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            $("#imgDetails").attr("src", "data:image/bmp;base64," + data["preview"]);
            $("h1").html(data["originalFileName"]);


        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}


function deleteCaff() {
    $.ajax({
        type: "DELETE",
        url: url + "/caff/" + caffId,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            window.location = "home.html";
        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function getComments() {
    $.ajax({
        type: "GET",
        url: url + "/comment/all/" + caffId,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            //$("#imgDetails").attr("src", "data:image/bmp;base64,"+ data["preview"]);
            $("#comments-list").html("");

            $.each(data, function (key, commentRow) {
                //console.log(commentRow)

                $.ajax({
                    type: "GET",
                    url: "content/details-comment.html",
                    success: function (line) {

                        $("#comments-list").append(line);
                        var comment = $(".comment").last();
                        comment.find(".name-date").html("<b>" + commentRow["name"] + " - " + commentRow["timestamp"].slice(0, 10) + "</b>");
                        comment.find(".text").html(commentRow["text"]);
                        comment.find(".delComment").attr("onclick", "deleteComment(" + commentRow["id"] + ")");

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

function addComment() {
    var data = JSON.stringify($("#search-input").val());

    $.ajax({
        type: "POST",
        url: url + "/comment/" + caffId,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            //$("#imgDetails").attr("src", "data:image/bmp;base64,"+ data["preview"]);
            $("#search-input").val("");

            getComments();

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}


function deleteComment(commentId) {
    $.ajax({
        type: "DELETE",
        url: url + "/comment/" + commentId,
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            //$("#imgDetails").attr("src", "data:image/bmp;base64,"+ data["preview"]);
            $("#search-input").val("");

            getComments();

        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function downloadCaff(caffId) {
    $.ajax({
        type: "GET",
        url: url + "/caff/" + caffId + "/download",
        contentType: "application/json",
        beforeSend: function (xhr) {   //Include the bearer token in header
            xhr.setRequestHeader("Authorization", 'Bearer ' + token);
        },
        //data: data,
        timeout: 600000,
        processData: false,
        success: function (data) {
            download(data, "ize.caff", 'application/octet-stream');
        },
        error: function (e) {
            console.log(e);
            var result = JSON.parse(e.responseText);

        }
    });
}

function download(content, filename, contentType) {
    if (!contentType) contentType = 'application/octet-stream';
    var a = document.createElement('a');
    var blob = new Blob([content], { encoding: "ANSI", 'type': contentType });
    a.href = window.URL.createObjectURL(blob);
    a.download = filename;
    a.click();
}