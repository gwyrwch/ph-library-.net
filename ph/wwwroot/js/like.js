// "use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/likeHub").build();


connection.on("PostLiked", function (username, message) {
    console.log("username: " + username + " message: " + message);
    if (message === "like") {
        notifyMe(username + " " + message + "d your post", "like!");
    } else if (message === "dislike") {
        notifyMe(username + " " + message + "d your post  :(", "dislike :c");
    }
    
});

connection.start().then(function(){
    console.log("connection started");
}).catch(function (err) {
    return console.error(err.toString());
});

function notifyMe(message, like) {
    if (!window.Notification) {
        console.log('Browser does not support notifications.');
    } else {
        // check if permission is already granted
        if (Notification.permission === 'granted') {
            // show notification here
            var notify = new Notification('New ' + like, {
                body: message,
            });
        } else {
            // request permission from user
            Notification.requestPermission().then(function (p) {
                if (p === 'granted') {
                    // show notification here
                    var notify = new Notification('New ' + like, {
                        body: message,
                    });
                } else {
                    console.log('User blocked notifications.');
                }
            }).catch(function (err) {
                console.error(err);
            });
        }
    }
}




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
                return console.error(":(  " + err.toString());
            });
            event.preventDefault();
        });
    }
});