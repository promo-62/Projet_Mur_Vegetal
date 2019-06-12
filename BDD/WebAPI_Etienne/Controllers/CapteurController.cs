using System;
using System.Collections.Generic;
using System.Linq;
using CapteursApi.Models;
using CapteursApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace CapteursApi.Controllers
{

    // ----- API Partie Web ----- //
    [Route("api/[controller]")]
    [ApiController]
    public class WebController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public WebController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        // obtenir les [idCapteur, typeCapteur, nom, projet]  des capteurs fonctionnels
        [HttpGet("capteurs")]
        public ActionResult<List<CapteurWeb>> GetCapteurs()
        {
            var filter = "{Fonctionne:true}";
            var projection = "{IdCapteur:1, TypeCapteur:1, Nom:1, Projet:1}";
            return _capteurService.Get<CapteurWeb>("Capteurs", filter, projection);
        }

        // obtenir les [idCapteur, valeur] du relevé le plus récent pour chaque capteur
        [HttpGet("releves/derniers")]
        public ActionResult<List<ReleveWeb>> GetDerniersReleves()
        {
            var group = "{_id:'$IdCapteur', Id:{$last:'$_id'}, IdCapteur:{$last:'$IdCapteur'}, Note:{$last:'$Note'}, DateReleve:{$max:'$DateReleve'}, Valeur:{$last:'$Valeur'}, TypeCapteur:{$last:'$TypeCapteur'}, Fiabilite:{$last:'$Fiabilite'}}";
            var projection = "{_id:'$Id', IdCapteur: '$IdCapteur', Valeur: '$Valeur'}";
            var sort = "{'IdCapteur': -1, 'DateReleve': 1}";
            return _capteurService.GetDerniersReleves<ReleveWeb>("Releve", group, projection, sort);
        }

        // obtenir [tous les relevés] pour un capteur d'idCapteur {id}
        [HttpGet("releves/{id}")]
        public ActionResult<List<Releve>> GetReleve(string id)
        {
            var filter = "{IdCapteur:" + id + "}";
            var releve = _capteurService.Get<Releve>("Releve", filter);
            if (releve == null)
            {
                return NotFound();
            }

            return releve;
        }

        // obtenir [tous les champs] de tous les éléments de la collection CompteARebours
        [HttpGet("comptearebours")]
        public ActionResult<List<CompteARebours>> GetCompteARebours()
        {
            var compteARebours = _capteurService.Get<CompteARebours>("CompteARebours");
            if (compteARebours == null)
            {
                return NotFound();
            }

            return compteARebours;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Event
        [HttpGet("events")]
        public ActionResult<List<Event>> GetEvent()
        {
            var evenement = _capteurService.Get<Event>("Event");
            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        // obtenir [tous les champs] du dernier élément ajouté à Event
        [HttpGet("events/dernier")]
        public ActionResult<Event> GetDernierEvent()
        {
            var evenement = _capteurService.Get<Event>("Event").LastOrDefault();
            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Media
        [HttpGet("medias")]
        public ActionResult<List<Media>> GetMedia()
        {
            var media = _capteurService.Get<Media>("Media");
            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Tableau
        [HttpGet("tableaux")]
        public ActionResult<List<Tableau>> GetTableau()
        {
            var tableau = _capteurService.Get<Tableau>("Tableau");
            if (tableau == null)
            {
                return NotFound();
            }

            return tableau;
        }

    }

}