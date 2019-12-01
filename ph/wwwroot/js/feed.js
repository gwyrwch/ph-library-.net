/*
   Creates a like tag
   param liked: was post liked or not
*/
function createLike(postId, liked) {
    var cl = "";
    if (liked) {
        cl = 'fa fa-heart liketag';
    } else {
        cl = 'fa fa-heart-o liketag';
    }
    return `<i class="${cl}" id="${postId}"></i>`;
}
var feed_ = null;
function fetchAndDisplayFeed(typeId) {
    return function () {
        fetch("/api/feed?type=" + typeId)
            .then(function (response) {
                if (response.status === 200) {
                    return response.json();
                } else {
                    return null;
                }
            })
            .then(function (feed) {
                if (feed == null) {
                    return;
                }
                feed_ = feed;
                var newFeedHtml = "<div class=\"container\" id=\"posts\">\n" +
                    "<div class=\"col my-grid\" id=\"posts-feed\">";
                for (var i = 0; i < feed.length; i++) {
                    var item = feed[i];
                    newFeedHtml += `
                        <div class="row my-grid-row">
                          <div class="card my-card" style="width: 550px; height: auto; margin: 3px">
                            <div class="card-body" style="padding: 10px !important;">
                              <div class="row cust-grid-left">
                               
                                <div class="col-md-2 cust-grid-left-col">
                                  <img src="${item.userProfileImage}" alt="" 
                                    class="img-circle-feed-profile img-no-padding load-profile" id="${item.userName}"
                                  >
                                </div>
                                <div class="col-md-4 cust-grid-left-col my-auto" style="vertical-align: center; ">
                                  <p>${item.userName}</p>
                                </div>
                              </div>
                            </div>
                            <img class="img-responsive img-feed mx-auto" src="${item.post.imagePath}" alt="">
                            <div class="card-body">
                              ${createLike(item.post.id, item.liked)}
                              <h4 class="card-title">${item.userName}</h4>
                              <p class="card-text">${item.post.description}</p>
                              <p class="card-text"><small class="text-muted">${item.post.publicationTime}</small></p>
                            </div>
                          </div>
                        </div>
                      `;
                }
                newFeedHtml += "</div>\n" + "</div>";

                var feedParent = document.getElementById('feedParent');
                feedParent.innerHTML = newFeedHtml;

                var liketags = document.getElementsByClassName("liketag");

                for (var i = 0; i < liketags.length; i++) {
                    liketags[i].onclick = function () {
                        var cl = this.classList;
                        if (cl.contains("fa-heart-o")) {
                            // new like
                            cl.replace("fa-heart-o", "fa-heart");
                        } else {
                            cl.replace("fa-heart", "fa-heart-o");
                        }
                        $.get("/api/likes?PostId=" + this.id);
                    };
                }
            }
            ).then(function () {
                setOnClicks();
            });
    }
}

var  filterButtons = document.getElementsByClassName("btn-post-type");
for (var i = 0; i < filterButtons.length; i++) {
    filterButtons[i].onclick = fetchAndDisplayFeed(filterButtons[i].id);
}


function fetchAndDisplayProfile(petId, username) {
    return function () {
        console.log("petId: " + petId, "username: " + username);
        location.assign("/home/profile/" + username);
    }
}

fetchAndDisplayFeed(-1)();

function setOnClicks() {
    var profileButtons = document.getElementsByClassName("img-circle-feed-profile");
    for (var i = 0; i < profileButtons.length; i++) {
        profileButtons[i].onclick = fetchAndDisplayProfile(null, profileButtons[i].id);
    }
}



