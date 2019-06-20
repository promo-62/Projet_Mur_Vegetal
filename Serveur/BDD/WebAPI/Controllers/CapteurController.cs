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
        public ActionResult<List<Capteurs>> GetCapteurs()
        {
            var filter = "{Fonctionne:true}";
            var capteur = _capteurService.Get<Capteurs>("Capteurs", filter);

            if (!capteur.Any())
            {
                return NoContent();
            }

            return capteur;
        }

        // obtenir [tous les champs] du capteur avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("capteurs/{id:length(24)}", Name = "GetCapteurs")]
        public ActionResult<Capteurs> GetCapteursById(string id)
        {
            var capteur = _capteurService.GetById<Capteurs>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            return capteur;
        }

        // obtenir les [idCapteur, typeCapteur, nom, projet] des capteurs fonctionnels
        [Microsoft.AspNetCore.Mvc.HttpGet("capteurs/ids")]
        public ActionResult<List<CapteursWeb>> GetCapteursIds()
        {
            var filter = "{Fonctionne:true}";
            var projection = "{IdCapteur:1, TypeCapteur:1, Nom:1, Projet:1}";
            var capteur = _capteurService.Get<CapteursWeb>("Capteurs", filter, projection);

            if (!capteur.Any())
            {
                return NoContent();
            }

            return capteur;
        }

        // obtenir [tous les champs]  de tous les éléments de la collection Releve
        [Microsoft.AspNetCore.Mvc.HttpGet("releves")]
        public ActionResult<List<Releves>> GetReleves()
        {
            var releve = _capteurService.Get<Releves>("Releves");

            if (!releve.Any())
            {
                return NoContent();
            }

            return releve;
        }

        // obtenir [tous les champs] du relevé avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/{id:length(24)}", Name = "GetReleves")]
        public ActionResult<Releves> GetRelevesById(string id)
        {
            var releve = _capteurService.GetById<Releves>("Releves", id);

            if (releve == null)
            {
                return NotFound();
            }

            return releve;
        }

        // obtenir les [idCapteur, valeur] du relevé le plus récent pour chaque capteur
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/derniers")]
        public ActionResult<List<RelevesWeb>> GetDerniersReleves()
        {
            var group = "{_id:'$IdCapteur', Id:{$last:'$_id'}, IdCapteur:{$last:'$IdCapteur'}, Note:{$last:'$Note'}, DateReleve:{$max:'$DateReleve'}, Valeur:{$last:'$Valeur'}, TypeCapteur:{$last:'$TypeCapteur'}, Fiabilite:{$last:'$Fiabilite'}}";
            var projection = "{_id:'$Id', IdCapteur: '$IdCapteur', Valeur: '$Valeur'}";
            var sort = "{'IdCapteur': -1, 'DateReleve': 1}";
            var releve = _capteurService.GetDerniersReleves<RelevesWeb>("Releves", group, projection, sort);

            if (!releve.Any())
            {
                return NoContent();
            }

            return releve;
        }

        // obtenir [tous les champs] des relevés dont l'idCapteur est {id}
        [Microsoft.AspNetCore.Mvc.HttpGet("releves/capteur/{id}")]
        public ActionResult<List<Releves>> GetRelevesCapteur(string id)
        {
            var filter = "{IdCapteur:" + id + "}";
            var releve = _capteurService.Get<Releves>("Releves", filter);

            if (!releve.Any())
            {
                return NotFound();
            }

            return releve;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Version_protocole
        [Microsoft.AspNetCore.Mvc.HttpGet("versionsprotocoles")]
        public ActionResult<List<VersionsProtocoles>> GetVersionsProtocoles()
        {
            var versionProtocole = _capteurService.Get<VersionsProtocoles>("VersionsProtocoles");

            if (!versionProtocole.Any())
            {
                return NoContent();
            }

            return versionProtocole;
        }

        // obtenir [tous les champs] de la version_protocole avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("versionsprotocoles/{id:length(24)}", Name = "GetVersionsProtocoles")]
        public ActionResult<VersionsProtocoles> GetVersionsProtocolesById(string id)
        {
            var versionProtocole = _capteurService.GetById<VersionsProtocoles>("VersionsProtocoles", id);

            if (versionProtocole == null)
            {
                return NotFound();
            }

            return versionProtocole;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpGet("plantes")]
        public ActionResult<List<Plantes>> GetPlantes()
        {
            var plante = _capteurService.Get<Plantes>("Plantes");

            if (!plante.Any())
            {
                return NoContent();
            }

            return plante;
        }

        // obtenir [tous les champs] de la plante avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("plantes/{id:length(24)}", Name = "GetPlantes")]
        public ActionResult<Plantes> GetPlantesById(string id)
        {
            var plante = _capteurService.GetById<Plantes>("Plantes", id);

            if (plante == null)
            {
                return NotFound();
            }

            return plante;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Events
        [Microsoft.AspNetCore.Mvc.HttpGet("events")]
        public ActionResult<List<Events>> GetEvents()
        {
            var evenement = _capteurService.Get<Events>("Events");

            if (!evenement.Any())
            {
                return NoContent();
            }

            return evenement;
        }

        // obtenir [tous les champs] de l'évènement avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("events/{id:length(24)}", Name = "GetEvents")]
        public ActionResult<Events> GetEventsById(string id)
        {
            var evenement = _capteurService.GetById<Events>("Events", id);

            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        // obtenir [tous les champs] du dernier élément ajouté à Events
        [Microsoft.AspNetCore.Mvc.HttpGet("events/dernier")]
        public ActionResult<Events> GetDernierEvents()
        {
            var evenement = _capteurService.Get<Events>("Events").LastOrDefault();

            if (evenement == null)
            {
                return NoContent();
            }

            return evenement;
        }

        // obtenir [tous les champs] de tous les éléments de la collection UsersHololens
        [Microsoft.AspNetCore.Mvc.HttpGet("usershololens")]
        public ActionResult<List<UsersHololens>> GetUsersHololens()
        {
            var user = _capteurService.Get<UsersHololens>("UsersHololens");

            if (!user.Any())
            {
                return NoContent();
            }

            return user;
        }

        // obtenir [tous les champs] du user avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usershololens/{id:length(24)}", Name = "GetUsersHololens")]
        public ActionResult<UsersHololens> GetUsersHololensById(string id)
        {
            var userHololens = _capteurService.GetById<UsersHololens>("UsersHololens", id);

            if (userHololens == null)
            {
                return NotFound();
            }

            return userHololens;
        }

        // obtenir [tous les champs] de tous les éléments de la collection UsersAdmin
        [Microsoft.AspNetCore.Mvc.HttpGet("usersadmin")]
        public ActionResult<List<UsersAdmin>> GetUsersAdmin()
        {
            var userAdmin = _capteurService.Get<UsersAdmin>("UsersAdmin");

            if (!userAdmin.Any())
            {
                return NoContent();
            }

            return userAdmin;
        }

        // obtenir [tous les champs] du user avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usersadmin/{id:length(24)}", Name = "GetUsersAdmin")]
        public ActionResult<UsersAdmin> GetUsersAdminById(string id)
        {
            var userAdmin = _capteurService.GetById<UsersAdmin>("UsersAdmin", id);

            if (userAdmin == null)
            {
                return NotFound();
            }

            return userAdmin;
        }

        // obtenir [tous les champs] de tous les éléments de la collection UsersAPI
        [Microsoft.AspNetCore.Mvc.HttpGet("usersapi")]
        public ActionResult<List<UsersAPI>> GetUsersAPI()
        {
            var usersAPI = _capteurService.Get<UsersAPI>("UsersAPI");

            if (!usersAPI.Any())
            {
                return NoContent();
            }

            return usersAPI;
        }

        // obtenir [tous les champs] du user avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usersapi/{id:length(24)}", Name = "GetUsersAPI")]
        public ActionResult<UsersAPI> GetUsersAPIById(string id)
        {
            var usersAPI = _capteurService.GetById<UsersAPI>("UsersAPI", id);

            if (usersAPI == null)
            {
                return NotFound();
            }

            return usersAPI;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Socials
        [Microsoft.AspNetCore.Mvc.HttpGet("socials")]
        public ActionResult<List<Socials>> GetSocials()
        {
            var social = _capteurService.Get<Socials>("Socials");

            if (!social.Any())
            {
                return NoContent();
            }

            return social;
        }

        // obtenir [tous les champs] du compte social avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("socials/{id:length(24)}", Name = "GetSocials")]
        public ActionResult<Socials> GetSocialsById(string id)
        {
            var social = _capteurService.GetById<Socials>("Socials", id);

            if (social == null)
            {
                return NotFound();
            }

            return social;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Tableaux
        [Microsoft.AspNetCore.Mvc.HttpGet("tableaux")]
        public ActionResult<List<Tableaux>> GetTableaux()
        {
            var tableau = _capteurService.Get<Tableaux>("Tableaux");

            if (!tableau.Any())
            {
                return NoContent();
            }

            return tableau;
        }

        // obtenir [tous les champs] du tableau avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("tableaux/{id:length(24)}", Name = "GetTableaux")]
        public ActionResult<Tableaux> GetTableauxById(string id)
        {
            var tableau = _capteurService.GetById<Tableaux>("Tableaux", id);

            if (tableau == null)
            {
                return NotFound();
            }

            return tableau;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Medias
        [Microsoft.AspNetCore.Mvc.HttpGet("medias")]
        public ActionResult<List<Medias>> GetMedias()
        {
            var media = _capteurService.Get<Medias>("Medias");

            if (!media.Any())
            {
                return NoContent();
            }

            return media;
        }

        // obtenir [tous les champs] du media avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("medias/{id:length(24)}", Name = "GetMedias")]
        public ActionResult<Medias> GetMediasById(string id)
        {
            var media = _capteurService.GetById<Medias>("Medias", id);

            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        // obtenir [tous les champs] de tous les éléments de la collection ComptesARebours
        [Microsoft.AspNetCore.Mvc.HttpGet("comptesarebours")]
        public ActionResult<List<ComptesARebours>> GetComptesARebours()
        {
            var compteARebours = _capteurService.Get<ComptesARebours>("ComptesARebours");

            if (!compteARebours.Any())
            {
                return NoContent();
            }

            return compteARebours;
        }

        // obtenir [tous les champs] du compte à rebours avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("comptesarebours/{id:length(24)}", Name = "GetComptesARebours")]
        public ActionResult<ComptesARebours> GetComptesAReboursById(string id)
        {
            var compteARebours = _capteurService.GetById<ComptesARebours>("ComptesARebours", id);

            if (compteARebours == null)
            {
                return NotFound();
            }

            return compteARebours;
        }

        // obtenir [tous les champs] de tous les éléments de la collection TypesCapteurs
        [Microsoft.AspNetCore.Mvc.HttpGet("typescapteurs")]
        public ActionResult<List<TypesCapteurs>> GetTypesCapteurs()
        {
            var typesCapteurs = _capteurService.Get<TypesCapteurs>("TypesCapteurs");

            if (!typesCapteurs.Any())
            {
                return NoContent();
            }

            return typesCapteurs;
        }

        // obtenir [tous les champs] de l'alerte avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("typescapteurs/{id:length(24)}", Name = "GetTypesCapteurs")]
        public ActionResult<TypesCapteurs> GetTypesCapteursById(string id)
        {
            var typesCapteurs = _capteurService.GetById<TypesCapteurs>("TypesCapteurs", id);

            if (typesCapteurs == null)
            {
                return NotFound();
            }

            return typesCapteurs;
        }

        // obtenir [tous les champs] de tous les éléments de la collection Alertes
        [Microsoft.AspNetCore.Mvc.HttpGet("alertes")]
        public ActionResult<List<Alertes>> GetAlertes()
        {
            var alerte = _capteurService.Get<Alertes>("Alerte");

            if (!alerte.Any())
            {
                return NoContent();
            }

            return alerte;
        }

        // obtenir [tous les champs] de l'alerte avec l'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("alertes/{id:length(24)}", Name = "GetAlertes")]
        public ActionResult<Alertes> GetAlertesById(string id)
        {
            var alerte = _capteurService.GetById<Alertes>("Alertes", id);

            if (alerte == null)
            {
                return NotFound();
            }

            return alerte;
        }



        /* ----- REQUETES POST ----- */

        // insérer un élément dans la collection Capteurs
        [Microsoft.AspNetCore.Mvc.HttpPost("capteurs")]
        public ActionResult<Capteurs> CreateCapteurs(Capteurs capteur)
        {
            _capteurService.Create("Capteurs", capteur);

            return CreatedAtRoute("GetCapteurs", new { id = capteur.Id.ToString() }, capteur);
        }

        // insérer un élément dans la collection Releves
        [Microsoft.AspNetCore.Mvc.HttpPost("releves")]
        public ActionResult<Releves> CreateReleves(Releves releve)
        {
            _capteurService.Create("Releves", releve);

            return CreatedAtRoute("GetReleves", new { id = releve.Id.ToString() }, releve);
        }

        // insérer un élément dans la collection VersionsProtocoles
        [Microsoft.AspNetCore.Mvc.HttpPost("versionsprotocoles")]
        public ActionResult<VersionsProtocoles> CreateVersionsProtocoles(VersionsProtocoles versionsProtocoles)
        {
            _capteurService.Create("VersionsProtocoles", versionsProtocoles);

            return CreatedAtRoute("GetVersionsProtocoles", new { id = versionsProtocoles.Id.ToString() }, versionsProtocoles);
        }

        // insérer un élément dans la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpPost("plantes")]
        public ActionResult<Plantes> CreatePlantes(Plantes plante)
        {
            _capteurService.Create("Plantes", plante);

            return CreatedAtRoute("GetPlantes", new { id = plante.Id.ToString() }, plante);
        }

        // insérer un élément dans la collection Events
        [Microsoft.AspNetCore.Mvc.HttpPost("events")]
        public ActionResult<Events> CreateEvents(Events evenement)
        {
            _capteurService.Create("Events", evenement);

            return CreatedAtRoute("GetEvents", new { id = evenement.Id.ToString() }, evenement);
        }

        // insérer un élément dans la collection UsersHololens
        [Microsoft.AspNetCore.Mvc.HttpPost("usershololens")]
        public ActionResult<UsersHololens> CreateUsersHololens(UsersHololens usersHololens)
        {
            _capteurService.Create("UsersHololens", usersHololens);

            return CreatedAtRoute("GetUsersHololens", new { id = usersHololens.Id.ToString() }, usersHololens);
        }

        // insérer un élément dans la collection UsersAdmin
        [Microsoft.AspNetCore.Mvc.HttpPost("usershololens")]
        public ActionResult<UsersAdmin> CreateUsersAdmin(UsersAdmin usersAdmin)
        {
            _capteurService.Create("UsersAdmin", usersAdmin);

            return CreatedAtRoute("GetUsersAdmin", new { id = usersAdmin.Id.ToString() }, usersAdmin);
        }

        // insérer un élément dans la collection UsersAPI
        [Microsoft.AspNetCore.Mvc.HttpPost("usersapi")]
        public ActionResult<UsersAPI> CreateUsersAPI(UsersAPI usersAPI)
        {
            _capteurService.Create("UsersAPI", usersAPI);

            return CreatedAtRoute("GetUsersAPI", new { id = usersAPI.Id.ToString() }, usersAPI);
        }
        
        // insérer un élément dans la collection Socials
        [Microsoft.AspNetCore.Mvc.HttpPost("socials")]
        public ActionResult<Socials> CreateSocials(Socials social)
        {
            _capteurService.Create("Socials", social);

            return CreatedAtRoute("GetSocials", new { id = social.Id.ToString() }, social);
        }

        // insérer un élément dans la collection Tableaux
        [Microsoft.AspNetCore.Mvc.HttpPost("tableaux")]
        public ActionResult<Tableaux> CreateTableaux(Tableaux tableau)
        {
            _capteurService.Create("Tableaux", tableau);

            return CreatedAtRoute("GetTableaux", new { id = tableau.Id.ToString() }, tableau);
        }

        // insérer un élément dans la collection Medias
        [Microsoft.AspNetCore.Mvc.HttpPost("medias")]
        public ActionResult<Medias> CreateMedias(Medias media)
        {
            _capteurService.Create("Medias", media);

            return CreatedAtRoute("GetMedias", new { id = media.Id.ToString() }, media);
        }

        // insérer un élément dans la collection ComptesARebours
        [Microsoft.AspNetCore.Mvc.HttpPost("comptesarebours")]
        public ActionResult<ComptesARebours> CreateComptesARebours(ComptesARebours compteARebours)
        {
            _capteurService.Create("ComptesARebours", compteARebours);

            return CreatedAtRoute("GetComptesARebours", new { id = compteARebours.Id.ToString() }, compteARebours);
        }

        // insérer un élément dans la collection Alertes
        [Microsoft.AspNetCore.Mvc.HttpPost("alertes")]
        public ActionResult<Alertes> CreateAlertes(Alertes alerte)
        {
            _capteurService.Create("Alertes", alerte);

            return CreatedAtRoute("GetAlertes", new { id = alerte.Id.ToString() }, alerte);
        }



        /* ----- REQUETES PUT ----- */

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs
        [Microsoft.AspNetCore.Mvc.HttpPut("capteurs/{id:length(24)}")]
        public IActionResult UpdateCapteurs(string id, Capteurs capteurIn)
        {
            var capteur = _capteurService.GetById<Capteurs>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            _capteurService.Update("Capteurs", id, capteurIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Releves
        [Microsoft.AspNetCore.Mvc.HttpPut("releves/{id:length(24)}")]
        public IActionResult UpdateReleves(string id, Releves releveIn)
        {
            var releve = _capteurService.GetById<Releves>("Releves", id);

            if (releve == null)
            {
                return NotFound();
            }

            _capteurService.Update("Releves", id, releveIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection VersionsProtocoles
        [Microsoft.AspNetCore.Mvc.HttpPut("versionsprotocoles/{id:length(24)}")]
        public IActionResult UpdateVersionsProtocoles(string id, VersionsProtocoles versionProtocoleIn)
        {
            var versionProtocole = _capteurService.GetById<VersionsProtocoles>("VersionsProtocoles", id);

            if (versionProtocole == null)
            {
                return NotFound();
            }

            _capteurService.Update("VersionsProtocoles", id, versionProtocoleIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Plantes
        [Microsoft.AspNetCore.Mvc.HttpPut("plantes/{id:length(24)}")]
        public IActionResult UpdatePlantes(string id, Plantes planteIn)
        {
            var plante = _capteurService.GetById<Plantes>("Plantes", id);

            if (plante == null)
            {
                return NotFound();
            }

            _capteurService.Update("Plantes", id, planteIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Events
        [Microsoft.AspNetCore.Mvc.HttpPut("events/{id:length(24)}")]
        public IActionResult UpdateEvents(string id, Events evenementIn)
        {
            var evenement = _capteurService.GetById<Events>("Events", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _capteurService.Update("Events", id, evenementIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersHololens
        [Microsoft.AspNetCore.Mvc.HttpPut("usershololens/{id:length(24)}")]
        public IActionResult UpdateUsersHololens(string id, UsersHololens userHololensIn)
        {
            var userHololens = _capteurService.GetById<VersionsProtocoles>("UsersHololens", id);

            if (userHololens == null)
            {
                return NotFound();
            }

            _capteurService.Update("UsersHololens", id, userHololensIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersAdmin
        [Microsoft.AspNetCore.Mvc.HttpPut("usersadmin/{id:length(24)}")]
        public IActionResult UpdateUsersAdmin(string id, UsersAdmin userAdminIn)
        {
            var userAdmin = _capteurService.GetById<UsersAdmin>("UsersAdmin", id);

            if (userAdmin == null)
            {
                return NotFound();
            }

            _capteurService.Update("UsersAdmin", id, userAdminIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersAPI
        [Microsoft.AspNetCore.Mvc.HttpPut("usersapi/{id:length(24)}")]
        public IActionResult UpdateUsersAPI(string id, UsersAPI userAPIIn)
        {
            var userAPI = _capteurService.GetById<UsersAPI>("UsersAPI", id);

            if (userAPI == null)
            {
                return NotFound();
            }

            _capteurService.Update("UsersAPI", id, userAPIIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Socials
        [Microsoft.AspNetCore.Mvc.HttpPut("socials/{id:length(24)}")]
        public IActionResult UpdateSocials(string id, Socials socialIn)
        {
            var social = _capteurService.GetById<Socials>("Socials", id);

            if (social == null)
            {
                return NotFound();
            }

            _capteurService.Update("Socials", id, socialIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Tableaux
        [Microsoft.AspNetCore.Mvc.HttpPut("tableaux/{id:length(24)}")]
        public IActionResult UpdateTableaux(string id, Tableaux tableauIn)
        {
            var tableau = _capteurService.GetById<Socials>("Tableaux", id);

            if (tableau == null)
            {
                return NotFound();
            }

            _capteurService.Update("Tableaux", id, tableauIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Medias
        public IActionResult UpdateMedias(string id, Medias mediaIn)
        {
            var media = _capteurService.GetById<Medias>("Medias", id);

            if (media == null)
            {
                return NotFound();
            }

            _capteurService.Update("Medias", id, mediaIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection ComptesARebours
        [Microsoft.AspNetCore.Mvc.HttpPut("comptesarebours/{id:length(24)}")]
        public ActionResult UpdateComptesARebours(string id, ComptesARebours compteAReboursIn)
        {
            var compteARebours = _capteurService.GetById<ComptesARebours>("ComptesARebours", id);

            if (compteARebours == null)
            {
                return NotFound();
            }

            _capteurService.Update("ComptesARebours", id, compteAReboursIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection TypesCapteurs
        [Microsoft.AspNetCore.Mvc.HttpPut("typescapteurs/{id:length(24)}")]
        public ActionResult UpdateTypesCapteurs(string id, TypesCapteurs typesCapteursIn)
        {
            var typesCapteurs = _capteurService.GetById<TypesCapteurs>("TypesCapteurs", id);

            if (typesCapteurs == null)
            {
                return NotFound();
            }

            _capteurService.Update("TypesCapteurs", id, typesCapteursIn);

            return NoContent();
        }

        // modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Alertes
        [Microsoft.AspNetCore.Mvc.HttpPut("alertes/{id:length(24)}")]
        public ActionResult UpdateAlertes(string id, Alertes alerteIn)
        {
            var alerte = _capteurService.GetById<Alertes>("Alertes", id);

            if (alerte == null)
            {
                return NotFound();
            }

            _capteurService.Update("Alertes", id, alerteIn);

            return NoContent();
        }

    }
}