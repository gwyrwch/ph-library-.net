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
        $.get("/Home/LikeEvent?PostId=" + this.id);
    };
}