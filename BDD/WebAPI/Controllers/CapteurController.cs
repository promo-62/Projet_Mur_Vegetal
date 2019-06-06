using System.Collections.Generic;
using CapteursApi.Models;
using CapteursApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapteursApi.Controllers
{
    // Controlleur api/controller
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
            var capteur = (Capteur)_capteurService.Get("Capteurs", id); // cast fonctionnel ?

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
            var capteur = _capteurService.Get("Capteurs", id);

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
            var capteur = _capteurService.Get("Capteurs", id);

            if (capteur == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Capteurs", capteur.Id);

            return NoContent();
        }
        
    }

    // Controlleur api/events
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public EventsController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Event>> Get()
        {
            return _capteurService.Get<Event>("Event");
        }

        [HttpGet("{id:length(24)}", Name = "Get Event")]
        public ActionResult<Event> Get(string id)
        {
            var evenement = (Event)_capteurService.Get("Event", id); // cast fonctionnel ?

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

            return CreatedAtRoute("GetCapteur", new { id = evenement.Id.ToString() }, evenement);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Event evenementIn)
        {
            var evenement = _capteurService.Get("Event", id);

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
            var evenement = _capteurService.Get("Event", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Event", evenement.Id);

            return NoContent();
        }

    }

    // Controlleur api/releves
    [Route("api/[controller]")]
    [ApiController]
    public class RelevesController : ControllerBase
    {
        private readonly CapteurService _capteurService;

        public RelevesController(CapteurService capteurService)
        {
            _capteurService = capteurService;
        }

        [HttpGet]
        public ActionResult<List<Releve>> Get()
        {
            return _capteurService.Get<Releve>("Releve");
        }

        [HttpGet("{id:length(24)}", Name = "Get Releve")]
        public ActionResult<Releve> Get(string id)
        {
            var releve = (Releve)_capteurService.Get("Releve", id); // cast fonctionnel ?

            if (releve == null)
            {
                return NotFound();
            }

            return releve;
        }

        [HttpPost]
        public ActionResult<Releve> Create(Releve releve)
        {
            _capteurService.Create("Releve", releve);

            return CreatedAtRoute("GetCapteur", new { id = releve.Id.ToString() }, releve);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Releve releveIn)
        {
            var releve = _capteurService.Get("Releve", id);

            if (releve == null)
            {
                return NotFound();
            }

            _capteurService.Update("Releve", id, releveIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var releve = _capteurService.Get("Releve", id);

            if (releve == null)
            {
                return NotFound();
            }

            _capteurService.Remove("Releve", releve.Id);

            return NoContent();
        }

    }

}