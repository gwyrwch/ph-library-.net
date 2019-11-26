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
                setHtmlToModal(posts_);
                var newProfileHtml = "<div class=\"container\" id=\"posts\">\n" + "<div class=\"row\">" ;
                for (var i = 0; i < posts_.length; i++) {
                    var item = posts_[i];
                    

                    newProfileHtml += `
                        <div class="col-md-3 col-sm-6 col-xs-12">
                            <img src="${item.imagePath}" class="img-responsive img-kek" 
                                 onclick="makeModal(this.src)"
                            >
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
}


function makeModal(smth) {
    console.log("this: " + smth);
    var id = smth.toString();
    id = id.substring(id.indexOf('posts/') + 6, id.indexOf('.'));
    // id = '/images/posts/' + id; 
    console.log('id: ' + id);
    console.log('clicked');
    id = '#' + id;
    var mdl = document.getElementById('gwyrwch4');
    console.log(mdl);
    $(id).modal('show');
}

function setHtmlToModal(posts) {
    var modalHtml='';
    for (var i = 0; i < posts.length; i++) {
        var item = posts[i];
        console.log('img: ' + item.imagePath.substring(item.imagePath.indexOf('posts/') + 6, item.imagePath.indexOf('.')));
        modalHtml += `
                  <div class="modal fade" id="${item.imagePath.substring(item.imagePath.indexOf('posts/') + 6, item.imagePath.indexOf('.'))}" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                    <div class="modal-dialog">
                      <div class="modal-content" style="background-color: #FFFFFF; width: 550px; height: auto; margin: 3px">
                        <div class="modal-header">
                          <div class="row cust-grid-left">
    
                            <div class="col-md-2 cust-grid-left-col">
                              <img src="${item.user.profileImagePath}" alt="" class="img-circle-feed-profile img-no-padding">
                            </div>
                            <div class="col-md-4 cust-grid-left-col my-auto" style="vertical-align: center; ">
                              <p>${item.user.userName}</p>
                            </div>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close" 
                              style="padding-right: 30px; color: #000000" >
                              <span aria-hidden="true">&times;</span>
                            </button>
                          </div>
                        </div>
                        <div class="modal-body " style="padding: 0 !important;" >
                          <div class="row my-grid-row" style="width: 550px; height: auto; margin: 3px">
                            <img class="img-responsive img-feed mx-auto" style="width: 450px; height: 450px;  padding-top: 20px" 
                                src="${item.imagePath}" alt="">
                            <div class="card-body">
                              <h4 class="card-title">${item.user.userName}</h4>
                              <p class="card-text">"hi my name is Rita"</p>
                              <p class="card-text"><small class="text-muted">22/01/2019</small></p>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>`;
    }
    
    var modalParent = document.getElementById('modalParent');
    modalParent.innerHTML = modalHtml;
}



fetchAndDisplayPosts("null")();