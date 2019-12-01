// "use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/likeHub").build();


//Disable send button until connection is established
// document.getElementById("sendButton").disabled = true;

connection.on("PostLiked", function (username, message) {
    console.log("username: " + username + " message: " + message);
    $.notify("Hello World");
    // alert("username: " + username + " message: " + message);
    // $.notify({
    //     // options
    //     message: username + " liked post: " + message
    // },{
    //     // settings
    //     type: 'danger'
    // });
});

connection.start().then(function(){
    console.log("connection started");
}).catch(function (err) {
    return console.error(err.toString());
});




window.addEventListener("load", function(event) {
    var liketags_ = document.getElementsByClassName("liketag");
    console.log(Object(liketags_).length + " ((((");
    for (var i = 0; i < liketags_.length; i++) {
        liketags_[i].addEventListener("click", function (event) {
            var liked = "like";
            if (this.classList.contains("fa-heart-o")) {
                liked = "dislike";
            };
            
            var message = this.id;
            connection.invoke("LikePost", liked, message).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });
    }
});