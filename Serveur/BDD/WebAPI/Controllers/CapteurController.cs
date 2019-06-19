using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CapteursApi.Models;
using CapteursApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace CapteursApi.Controllers
{
    /* ----- API Partie api/web ----- */
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class WebController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public WebController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }


        /* ----- REQUETES GET ----- */

        // obtenir [tous les champs] des capteurs fonctionnels
        [Microsoft.AspNetCore.Mvc.HttpGet("capteurs")]
        public ActionResult<List<Capteur>> GetCapteurs()
        {
            var filter = "{Fonctionne:true}";
            var capteur = _capteurService.Get<Capteur>("Capteurs", filter);

            if (!capteur.Any())
            {
                return NoContent();
            }

            return capteur;
        }

        // obtenir [tous les champs] du capteur avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("capteurs/{id:length(24)}", Name = "GetCapteur")]
        public ActionResult<Capteur> GetCapteur(string id)
        {
            var capteur = _capteurService.GetById<Capteur>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            return capteur;
        }

        // obtenir les [idCapteur, typeCapteur, nom, projet] des capteurs fonctionnels
        [Microsoft.AspNetCore.Mvc.HttpGet("capteurs/ids")]
        public ActionResult<List<CapteurWeb>> GetCapteursIds()
        {
            var filter = "{Fonctionne:true}";
            var projection = "{IdCapteur:1, TypeCapteur:1, Nom:1, Projet:1}";
            var capteur = _capteurService.Get<CapteurWeb>("Capteurs", filter, projection);

            if (!capteur.Any())
            {
                return NoContent();
            }

            return capteur;
        }

        // obtenir [tous les champs]  de tous les éléments de la collection Releve
        [Microsoft.AspNetCore.Mvc.HttpGet("releves")]
        public ActionResult<List<Releve>> GetReleves()
        {
            var releve = _capteurService.Get<Releve>("Releve");

            if (!releve.Any())
            {
                return NoContent();
            }

            return releve;
        }

        // obtenir [tous les champs] du relevé avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/{id:length(24)}", Name = "GetReleve")]
        public ActionResult<Releve> GetReleve(string id)
        {
            var releve = _capteurService.GetById<Releve>("Releve", id);

            if (releve == null)
            {
                return NotFound();
            }

            return releve;
        }

        // obtenir les [idCapteur, valeur] du relevé le plus récent pour chaque capteur
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/derniers")]
        public ActionResult<List<ReleveWeb>> GetDerniersReleves()
        {
            var group = "{_id:'$IdCapteur', Id:{$last:'$_id'}, IdCapteur:{$last:'$IdCapteur'}, Note:{$last:'$Note'}, DateReleve:{$max:'$DateReleve'}, Valeur:{$last:'$Valeur'}, TypeCapteur:{$last:'$TypeCapteur'}, Fiabilite:{$last:'$Fiabilite'}}";
            var projection = "{_id:'$Id', IdCapteur: '$IdCapteur', Valeur: '$Valeur'}";
            var sort = "{'IdCapteur': -1, 'DateReleve': 1}";
            var releve = _capteurService.GetDerniersReleves<ReleveWeb>("Releve", group, projection, sort);

            if (!releve.Any())
            {
                return NoContent();
            }

            return releve;
        }

        // obtenir [tous les champs] des relevés dont l'idCapteur est {id}
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/capteur/{id}")]
        public ActionResult<List<Releve>> GetRelevesCapteur(string id)
        {
            var filter = "{IdCapteur:" + id + "}";
            var releve = _capteurService.Get<Releve>("Releve", filter);

            if (!releve.Any())
            {
                return NotFound();
            }

            return releve;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Version_protocole
        [Microsoft.AspNetCore.Mvc.HttpGet("versionsprotocole")]
        public ActionResult<List<VersionProtocole>> GetVersionProtocole()
        {
            var versionProtocole = _capteurService.Get<VersionProtocole>("Version_protocole");

            if (!versionProtocole.Any())
            {
                return NoContent();
            }

            return versionProtocole;
        }

        // obtenir [tous les champs] de la version_protocole avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("versionsprotocole/{id:length(24)}", Name = "GetVersionProtocole")]
        public ActionResult<VersionProtocole> GetVersionProtocole(string id)
        {
            var versionProtocole = _capteurService.GetById<VersionProtocole>("Version_protocole", id);

            if (versionProtocole == null)
            {
                return NotFound();
            }

            return versionProtocole;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpGet("plantes")]
        public ActionResult<List<Plante>> GetPlantes()
        {
            var plante = _capteurService.Get<Plante>("Plantes");

            if (!plante.Any())
            {
                return NoContent();
            }

            return plante;
        }

        // obtenir [tous les champs] de la plante avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("plantes/{id:length(24)}", Name = "GetPlante")]
        public ActionResult<Plante> GetPlante(string id)
        {
            var plante = _capteurService.GetById<Plante>("Plante", id);

            if (plante == null)
            {
                return NotFound();
            }

            return plante;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Event
        [Microsoft.AspNetCore.Mvc.HttpGet("events")]
        public ActionResult<List<Event>> GetEvent()
        {
            var evenement = _capteurService.Get<Event>("Event");

            if (!evenement.Any())
            {
                return NoContent();
            }

            return evenement;
        }

        // obtenir [tous les champs] de l'évènement avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("events/{id:length(24)}", Name = "GetEvent")]
        public ActionResult<Event> GetEvent(string id)
        {
            var evenement = _capteurService.GetById<Event>("Event", id);

            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        // obtenir [tous les champs] du dernier élément ajouté à Event
        [Microsoft.AspNetCore.Mvc.HttpGet("events/dernier")]
        public ActionResult<Event> GetDernierEvent()
        {
            var evenement = _capteurService.Get<Event>("Event").LastOrDefault();

            if (evenement == null)
            {
                return NoContent();
            }

            return evenement;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Tableau
        [Microsoft.AspNetCore.Mvc.HttpGet("users")]
        public ActionResult<List<User>> GetUser()
        {
            var user = _capteurService.Get<User>("User");

            if (!user.Any())
            {
                return NoContent();
            }

            return user;
        }

        // obtenir [tous les champs] du user avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("users/{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _capteurService.GetById<User>("User", id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Tableau
        [Microsoft.AspNetCore.Mvc.HttpGet("socials")]
        public ActionResult<List<Social>> GetSocial()
        {
            var social = _capteurService.Get<Social>("Social");

            if (!social.Any())
            {
                return NoContent();
            }

            return social;
        }

        // obtenir [tous les champs] du compte social avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("socials/{id:length(24)}", Name = "GetSocial")]
        public ActionResult<Social> GetSocial(string id)
        {
            var social = _capteurService.GetById<Social>("Social", id);

            if (social == null)
            {
                return NotFound();
            }

            return social;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Tableau
        [Microsoft.AspNetCore.Mvc.HttpGet("tableaux")]
        public ActionResult<List<Tableau>> GetTableau()
        {
            var tableau = _capteurService.Get<Tableau>("Tableau");

            if (!tableau.Any())
            {
                return NoContent();
            }

            return tableau;
        }

        // obtenir [tous les champs] du tableau avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("tableaux/{id:length(24)}", Name = "GetTableau")]
        public ActionResult<Tableau> GetTableau(string id)
        {
            var tableau = _capteurService.GetById<Tableau>("Tableau", id);

            if (tableau == null)
            {
                return NotFound();
            }

            return tableau;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Media
        [Microsoft.AspNetCore.Mvc.HttpGet("medias")]
        public ActionResult<List<Media>> GetMedia()
        {
            var media = _capteurService.Get<Media>("Media");

            if (!media.Any())
            {
                return NoContent();
            }

            return media;
        }

        // obtenir [tous les champs] du media avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("medias/{id:length(24)}", Name = "GetMedia")]
        public ActionResult<Media> GetMedia(string id)
        {
            var media = _capteurService.GetById<Media>("Media", id);

            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        // obtenir [tous les champs] de tous les éléments de la collection CompteARebours
        [Microsoft.AspNetCore.Mvc.HttpGet("comptearebours")]
        public ActionResult<List<CompteARebours>> GetCompteARebours()
        {
            var compteARebours = _capteurService.Get<CompteARebours>("CompteARebours");

            if (!compteARebours.Any())
            {
                return NoContent();
            }

            return compteARebours;
        }

        // obtenir [tous les champs] du compteARebours avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("comptearebours/{id:length(24)}", Name = "GetCompteARebours")]
        public ActionResult<CompteARebours> GetCompteARebours(string id)
        {
            var compteARebours = _capteurService.GetById<CompteARebours>("CompteARebours", id);

            if (compteARebours == null)
            {
                return NotFound();
            }

            return compteARebours;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Alerte
        [Microsoft.AspNetCore.Mvc.HttpGet("alertes")]
        public ActionResult<List<Alerte>> GetAlertes()
        {
            var alerte = _capteurService.Get<Alerte>("Alerte");

            if (!alerte.Any())
            {
                return NoContent();
            }

            return alerte;
        }

        // obtenir [tous les champs] de l'alerte avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("comptearebours/{id:length(24)}", Name = "GetAlerte")]
        public ActionResult<Alerte> GetAlerte(string id)
        {
            var alerte = _capteurService.GetById<Alerte>("Alerte", id);

            if (alerte == null)
            {
                return NotFound();
            }

            return alerte;
        }



        /* ----- REQUETES POST ----- */

        // insérer un élément dans la collection Capteurs
        [Microsoft.AspNetCore.Mvc.HttpPost("capteurs")]
        public ActionResult<Capteur> CreateCapteur(Capteur capteur)
        {
            _capteurService.Create("Capteurs", capteur);

            return CreatedAtRoute("GetCapteur", new { id = capteur.Id.ToString() }, capteur);
        }

        // insérer un élément dans la collection Releve
        [Microsoft.AspNetCore.Mvc.HttpPost("releves")]
        public ActionResult<Releve> CreateReleve(Releve releve)
        {
            _capteurService.Create("Releve", releve);

            return CreatedAtRoute("GetReleve", new { id = releve.Id.ToString() }, releve);
        }

        // insérer un élément dans la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpPost("plantes")]
        public ActionResult<Plante> CreatePlante(Plante plante)
        {
            _capteurService.Create("Plantes", plante);

            return CreatedAtRoute("GetPlante", new { id = plante.Id.ToString() }, plante);
        }

        // insérer un élément dans la collection Event
        [Microsoft.AspNetCore.Mvc.HttpPost("events")]
        public ActionResult<Event> CreateEvent(Event evenement)
        {
            _capteurService.Create("Event", evenement);

            return CreatedAtRoute("GetEvent", new { id = evenement.Id.ToString() }, evenement);
        }

        // insérer un élément dans la collection Social
        [Microsoft.AspNetCore.Mvc.HttpPost("socials")]
        public ActionResult<Social> CreateSocial(Social social)
        {
            _capteurService.Create("Social", social);

            return CreatedAtRoute("GetSocial", new { id = social.Id.ToString() }, social);
        }

        // insérer un élément dans la collection Tableau
        [Microsoft.AspNetCore.Mvc.HttpPost("tableaux")]
        public ActionResult<Tableau> CreateTableau(Tableau tableau)
        {
            _capteurService.Create("Tableau", tableau);

            return CreatedAtRoute("GetTableau", new { id = tableau.Id.ToString() }, tableau);
        }

        // insérer un élément dans la collection Media
        [Microsoft.AspNetCore.Mvc.HttpPost("medias")]
        public ActionResult<Media> CreateMedia(Media media)
        {
            _capteurService.Create("Media", media);

            return CreatedAtRoute("GetMedia", new { id = media.Id.ToString() }, media);
        }

        // insérer un élément dans la collection CompteARebours
        [Microsoft.AspNetCore.Mvc.HttpPost("comptearebours")]
        public ActionResult<CompteARebours> CreateCompteARebours(CompteARebours compteARebours)
        {
            _capteurService.Create("CompteARebours", compteARebours);

            return CreatedAtRoute("GetCompteARebours", new { id = compteARebours.Id.ToString() }, compteARebours);
        }

        // insérer un élément dans la collection Alerte
        [Microsoft.AspNetCore.Mvc.HttpPost("alertes")]
        public ActionResult<Alerte> CreateAlerte(Alerte alerte)
        {
            _capteurService.Create("Alerte", alerte);

            return CreatedAtRoute("GetAlerte", new { id = alerte.Id.ToString() }, alerte);
        }



        /* ----- REQUETES PUT ----- */

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs
        [Microsoft.AspNetCore.Mvc.HttpPut("capteurs/{id:length(24)}")]
        public IActionResult UpdateCapteur(string id, Capteur capteurIn)
        {
            var capteur = _capteurService.GetById<Capteur>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            _capteurService.Update("Capteurs", id, capteurIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Releve
        [Microsoft.AspNetCore.Mvc.HttpPut("releves/{id:length(24)}")]
        public IActionResult UpdateReleve(string id, Releve releveIn)
        {
            var releve = _capteurService.GetById<Releve>("Releve", id);

            if (releve == null)
            {
                return NotFound();
            }

            _capteurService.Update("Releve", id, releveIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpPut("plantes/{id:length(24)}")]
        public IActionResult UpdatePlante(string id, Plante planteIn)
        {
            var plante = _capteurService.GetById<Plante>("Plantes", id);

            if (plante == null)
            {
                return NotFound();
            }

            _capteurService.Update("Plantes", id, planteIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Event
        [Microsoft.AspNetCore.Mvc.HttpPut("events/{id:length(24)}")]
        public IActionResult UpdateEvent(string id, Event evenementIn)
        {
            var evenement = _capteurService.GetById<Event>("Event", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _capteurService.Update("Event", id, evenementIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Social
        [Microsoft.AspNetCore.Mvc.HttpPut("socials/{id:length(24)}")]
        public IActionResult UpdateSocial(string id, Social socialIn)
        {
            var social = _capteurService.GetById<Social>("Social", id);

            if (social == null)
            {
                return NotFound();
            }

            _capteurService.Update("Social", id, socialIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Tableau
        [Microsoft.AspNetCore.Mvc.HttpPut("tableaux/{id:length(24)}")]
        public IActionResult UpdateTableau(string id, Tableau tableauIn)
        {
            var tableau = _capteurService.GetById<Social>("Tableau", id);

            if (tableau == null)
            {
                return NotFound();
            }

            _capteurService.Update("Tableau", id, tableauIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Media
        public IActionResult UpdateMedia(string id, Media mediaIn)
        {
            var media = _capteurService.GetById<Media>("Media", id);

            if (media == null)
            {
                return NotFound();
            }

            _capteurService.Update("Social", id, mediaIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection CompteARebours
        [Microsoft.AspNetCore.Mvc.HttpPut("comptearebours/{id:length(24)}")]
        public ActionResult UpdateCompteARebours(string id, CompteARebours compteAReboursIn)
        {
            var compteARebours = _capteurService.GetById<CompteARebours>("CompteARebours", id);

            if (compteARebours == null)
            {
                return NotFound();
            }

            _capteurService.Update("CompteARebours", id, compteAReboursIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Alerte
        [Microsoft.AspNetCore.Mvc.HttpPut("alertes/{id:length(24)}")]
        public ActionResult UpdateAlerte(string id, Alerte alerteIn)
        {
            var alerte = _capteurService.GetById<Alerte>("Alerte", id);

            if (alerte == null)
            {
                return NotFound();
            }

            _capteurService.Update("CompteARebours", id, alerteIn);

            return NoContent();
        }

    }
}