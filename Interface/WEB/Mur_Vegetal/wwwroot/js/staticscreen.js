var elements = [];
var timers = [];
var displayTimer = 5000;
var timerCountdown = 5000;
var timerWall = 5000;
var timerInsta = 5000;
var wall = document.getElementById("a"); //WALL
var news = document.getElementById("b");
var tabsNews = news.getElementsByClassName("news-block"); //NEWS
var countdown = document.getElementById("c"); //COMPTEUR
var medias = document.getElementById("d");
var tabMedias = medias.getElementsByClassName("medias-block"); //MEDIAS
var insta = document.getElementById("e"); //INSTA
wall.style.display = "none";
news.style.display = "block";
countdown.style.display = "none";
medias.style.display = "block";
insta.style.display = "none";


modifyTime(1, 12000, 200000);
fillElments();
defile();
launchIntervail();

async function defile() {
    if (elements.length === timers.length) {
        for (let i = 0; i < elements.length; i++) {
            if (i - 1 < 0) {
                elements[elements.length - 1].style.display = 'none';
            } else {
                elements[i - 1].style.display = 'none';
            }
            elements[i].style.display = 'grid';
            animate(elements[i]);
            await wait1Second(timers[i]);

        }

    } else {
        document.write("<H1>c'est pas la même taille</H1>");
    }
}

function wait1Second(displayTimer) {
    return new Promise(resolve => {
        setTimeout(() => {
            resolve();
        }, displayTimer);
    });
}

function launchIntervail() {
    setInterval(() => {
        defile();
    }, timers.reduce((a, b) => a + b, 0));

}

function fillElments() {
    elements.push(wall, countdown, insta);
    timers.push(timerWall, timerCountdown, timerInsta);
    for (let index = 0; index < tabsNews.length; index++) {
        tabsNews[index].style.display = "none";
        elements.push(tabsNews[index]);
        timers.push(displayTimer);
    }

    for (let index = 0; index < tabMedias.length; index++) {
        tabMedias[index].style.display = "none";
        elements.push(tabMedias[index]);
        timers.push(displayTimer);
    }
}

function animate(element) {
    transition.begin(element, ["opacity 0 1 1s", "transform translateX(-200px) translateX(0px) 1s ease-in-out"]);
}

function modifyTime(timerCountdown, timerInsta, timerWall) {
    /* si 1 c'est le temps / défaut   donc si vous voulez modifier que wall / exp 
    on met modifyTime(1,1, x )   */
    this.timerCountdown = (timerCountdown === 1) ? displayTimer : timerCountdown;
    this.timerInsta = (timerInsta === 1) ? displayTimer : timerInsta;
    this.timerWall = (timerWall === 1) ? displayTimer : timerWall;
}