using System;
using System.Collections.Generic;
using CapteursApi.Models;
using CapteursApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapteursApi.Controllers
{
    // Controlleur api/Capteurs
    [Route("api/[controller]")]
    [ApiController]
    public class CapteursController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public CapteursController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Capteur>> Get()
        {
            return _capteurService.Get<Capteur>("Capteurs");
        }

        [HttpGet("{id:length(24)}", Name = "GetCapteur")]
        public ActionResult<Capteur> Get(string id)
        {
            var capteur = _capteurService.Get<Capteur>("Capteurs", id); // cast fonctionnel ?

            if (capteur == null)
            {
                return NotFound();
            }

            return capteur;
        }

        [HttpPost]
        public ActionResult<Capteur> Create(Capteur capteur)
        {
            _capteurService.Create("Capteurs", capteur);

            return CreatedAtRoute("GetCapteur", new { id = capteur.Id.ToString() }, capteur);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Capteur capteurIn)
        {
            var capteur = _capteurService.Get<Capteur>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            _capteurService.Update("Capteurs", id, capteurIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var capteur = _capteurService.Get<Capteur>("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Capteurs", capteur.Id);

            return NoContent();
        }
        
    }

    // Controlleur api/event
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public EventController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Event>> Get()
        {
            return _capteurService.Get<Event>("Event");
        }

        [HttpGet("{id:length(24)}", Name = "GetEvent")]
        public ActionResult<Event> Get(string id)
        {
            var evenement = _capteurService.Get<Event>("Event", id); // cast fonctionnel ?

            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        [HttpPost]
        public ActionResult<Event> Create(Event evenement)
        {
            _capteurService.Create("Event", evenement);

            return CreatedAtRoute("GetEvent", new { id = evenement.Id.ToString() }, evenement);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Event evenementIn)
        {
            var evenement = _capteurService.Get<Event>("Event", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _capteurService.Update("Event", id, evenementIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var evenement = _capteurService.Get<Event>("Event", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Event", evenement.Id);

            return NoContent();
        }

    }

    // Controlleur api/media
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public MediaController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Media>> Get()
        {
            return _capteurService.Get<Media>("Media");
        }

        [HttpGet("{id:length(24)}", Name = "GetMedia")]
        public ActionResult<Media> Get(string id)
        {
            var media = (Media)_capteurService.Get<Media>("Media", id); // cast fonctionnel ?

            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        [HttpPost]
        public ActionResult<Media> Create(Media media)
        {
            _capteurService.Create("Media", media);

            return CreatedAtRoute("GetMedia", new { id = media.Id.ToString() }, media);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Media mediaIn)
        {
            var media = _capteurService.Get<Media>("Media", id);

            if (media == null)
            {
                return NotFound();
            }

            _capteurService.Update("Media", id, mediaIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var media = _capteurService.Get<Media>("Media", id);

            if (media == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Media", media.Id);

            return NoContent();
        }

    }

    // Controlleur api/CompteARebours
    [Route("api/[controller]")]
    [ApiController]
    public class CompteAReboursController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public CompteAReboursController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<CompteARebours>> Get()
        {
            return _capteurService.Get<CompteARebours>("CompteARebours");
        }

        [HttpGet("{id:length(24)}", Name = "GetCompteARebours")]
        public ActionResult<CompteARebours> Get(string id)
        {
            var compteARebour = _capteurService.Get<CompteARebours>("CompteARebours", id); // cast fonctionnel ?

            if (compteARebour == null)
            {
                return NotFound();
            }

            return compteARebour;
        }

        [HttpPost]
        public ActionResult<CompteARebours> Create(CompteARebours compteARebour)
        {
            _capteurService.Create("CompteARebours", compteARebour);

            return CreatedAtRoute("GetCompteARebours", new { id = compteARebour.Id.ToString() }, compteARebour);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, CompteARebours compteARebourIn)
        {
            var compteARebour = _capteurService.Get<CompteARebours>("CompteARebours", id);

            if (compteARebour == null)
            {
                return NotFound();
            }

            _capteurService.Update("CompteARebours", id, compteARebourIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var compteARebour = _capteurService.Get<CompteARebours>("CompteARebours", id);

            if (compteARebour == null)
            {
                return NotFound();
            }

            _capteurService.Remove("CompteARebours", compteARebour.Id);

            return NoContent();
        }

    }

    // Controlleur api/Social
    [Route("api/[controller]")]
    [ApiController]
    public class SocialController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public SocialController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Social>> Get()
        {
            return _capteurService.Get<Social>("Social");
        }

        [HttpGet("{id:length(24)}", Name = "GetSocial")]
        public ActionResult<Social> Get(string id)
        {
            var social = _capteurService.Get<Social>("Social", id); // cast fonctionnel ?

            if (social == null)
            {
                return NotFound();
            }

            return social;
        }

        [HttpPost]
        public ActionResult<Social> Create(Social social)
        {
            _capteurService.Create("Social", social);

            return CreatedAtRoute("GetSocial", new { id = social.Id.ToString() }, social);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Social socialIn)
        {
            var social = _capteurService.Get<Social>("Social", id);

            if (social == null)
            {
                return NotFound();
            }

            _capteurService.Update("Social", id, socialIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var social = _capteurService.Get<Social>("Social", id);

            if (social == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Social", social.Id);

            return NoContent();
        }

    }

    // Controlleur api/tableau
    [Route("api/[controller]")]
    [ApiController]
    public class TableauController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public TableauController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Tableau>> Get()
        {
            return _capteurService.Get<Tableau>("Tableau");
        }

        [HttpGet("{id:length(24)}", Name = "GetTableau")]
        public ActionResult<Tableau> Get(string id)
        {
            var tableau = _capteurService.Get<Tableau>("Tableau", id); // cast fonctionnel ?

            if (tableau == null)
            {
                return NotFound();
            }

            return tableau;
        }

        [HttpPost]
        public ActionResult<Tableau> Create(Tableau tableau)
        {
            _capteurService.Create("Tableau", tableau);

            return CreatedAtRoute("GetTableau", new { id = tableau.Id.ToString() }, tableau);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Tableau tableauIn)
        {
            var tableau = _capteurService.Get<Tableau>("Tableau", id);

            if (tableau == null)
            {
                return NotFound();
            }

            _capteurService.Update("Tableau", id, tableauIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var tableau = _capteurService.Get<Tableau>("Tableau", id);

            if (tableau == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Tableau", tableau.Id);

            return NoContent();
        }

    }

}