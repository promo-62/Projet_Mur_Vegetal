var frame = {

    displayer (walltime, newstime, countdowntime, mediastime, socialnetworkstime){
        let wall = document.getElementsByClassName("wallpage-frame")[0];
        let news = document.getElementsByClassName("newspage-frame")[0];
        let countdown = document.getElementsByClassName("countdown-frame")[0];
        let medias = document.getElementsByClassName("mediaspage-frame")[0];
        let socialnetworks = document.getElementsByClassName("socialnetworks-frame")[0];

        wall.style.display = "none";
        news.style.display = "none";
        countdown.style.display = "none";
        medias.style.display = "none";
        socialnetworks.style.display = "none";

        framesTimers = new Array();
        framesTimers.wall = walltime;
        framesTimers.news = newstime;
        framesTimers.countdown = countdowntime;
        framesTimers.medias = mediastime;
        framesTimers.socialnetworks = socialnetworkstime;
    }

}