function fetchAndDisplayPosts(petId) {
    return function () {
        console.log("petId: " + petId);
        fetch("/api/profile?petId=" + petId)
            .then(function (response) {
                if (response.status === 200) {
                    console.log("status 200");
                    return response.json();
                } else {
                    return null;
                }
            })
            .then(function (posts_) {
                if (posts_ == null) {
                    return;
                }
                console.log("posts " + posts_);
                var newProfileHtml = "<div class=\"container\" id=\"posts\">\n" + "<div class=\"row\">" ;
                for (var i = 0; i < posts_.length; i++) {
                    var item = posts_[i];

                    newProfileHtml += `
                        <div class="col-md-3 col-sm-6 col-xs-12">
                         <img src="${item.imagePath}" class="img-responsive img-kek">
                        </div>
                      `;
                }
                newProfileHtml += "</div>\n" + "</div>";

                var postsParent = document.getElementById('postsParent');
                postsParent.innerHTML = newProfileHtml;
            });
    }
}


var  petButtons = document.getElementsByClassName("img-circle-pet");
for (var i = 0; i < petButtons.length; i++) {
    petButtons[i].onclick = fetchAndDisplayPosts(petButtons[i].id);
    console.log(petButtons[i].id);
    console.log("kek" + petButtons[i].id);
}


fetchAndDisplayPosts("null")();