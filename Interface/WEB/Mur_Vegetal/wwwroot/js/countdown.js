/* --------------------------------------------------------------------------------------
COUNT DOWN 
--------------------------------------------------------------------------------------*/
function countDown(userDefinedEventDate,id){ //Paramètre : date de fin et id de l'element HTML où afficher le countdown

    let actualDate = Date.now(); //Date actuelle
    let eventDate = new Date(userDefinedEventDate*1000); //date de l'événement au format UNIX
    let duration = (eventDate - actualDate)/1000; //Calcul de la durée totale en secondes

    let days = Math.floor(duration / (60 * 60 * 24)); //Calcul du nombre de jours
    let hours = Math.floor((duration - (days * 60 * 60 * 24)) / (60 * 60)); //Calcul du nombre d'heures
    let minutes = Math.floor((duration - ((days * 60 * 60 * 24 + hours * 60 * 60))) / 60); //Calcul du nombre de minutes
    let secondes = Math.floor(duration - ((days * 60 * 60 * 24 + hours * 60 * 60 + minutes * 60))); //Calcul du nombre de secondes

    days = (days).toLocaleString('fr-FR', {minimumIntegerDigits: 2, useGrouping:false});
    hours = (hours).toLocaleString('fr-FR', {minimumIntegerDigits: 2, useGrouping:false});
    minutes = (minutes).toLocaleString('fr-FR', {minimumIntegerDigits: 2, useGrouping:false});
    secondes = (secondes).toLocaleString('fr-FR', {minimumIntegerDigits: 2, useGrouping:false});

    let actualisation = setTimeout('countDown("'+userDefinedEventDate+'","'+id+'");', 1000);
    if (days <= 0 && hours <= 0 && minutes <= 0 && secondes <= 0){
        document.getElementById(id).innerHTML = 'C\'est maintenant !';
        clearTimeout(actualisation);
    }
    else if (days <= 0 ){
        document.getElementById(id).innerHTML = hours  + 'H' + minutes + 'm' + secondes +'s';
    }
    else {
        document.getElementById(id).innerHTML = days + 'J ' + hours  + 'H' + minutes + 'm' + secondes +'s';
    }
}  