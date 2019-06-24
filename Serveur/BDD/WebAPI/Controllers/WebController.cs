using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebAPI.Models;
using WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /* ----- API WebController api/web ----- */
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class WebController : ControllerBase
    {
        private readonly WebService _webService;

        public WebController(WebService webService)
        {
            _webService = webService;
        }


        /* ----- GET REQUESTS ----- */

        // Startup Ok test
        [HttpGet("ok")]
        public HttpResponseMessage GetOk()
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");

            var response = new HttpResponseMessage();
            response.Headers.Add("X-WebAPI-Infos", new string[] { "Datetime: "+ timeStamp, "Version: WebAPI v5.1", "Dev: Etienne" });

            return response;
        }

        // get [all fields] for working sensors
        // obtenir [tous les champs] des capteurs fonctionnels
        [Microsoft.AspNetCore.Mvc.HttpGet("sensors")]
        public ActionResult<List<Sensors>> GetSensors()
        {
            var filter = "{IsWorking:true}";
            var sensor = _webService.Get<Sensors>("Sensors", filter);

            if (!sensor.Any())
            {
                return NoContent();
            }

            return sensor;
        }

        // get [all fields] for sensor with ObjectId {id}
        // obtenir [tous les champs] du capteur d'Object {id}
        [Microsoft.AspNetCore.Mvc.HttpGet("sensors/{id:length(24)}", Name = "GetSensor")]
        public ActionResult<Sensors> GetSensorById(string id)
        {
            var sensor = _webService.GetById<Sensors>("Sensors", id);

            if (sensor == null)
            {
                return NotFound();
            }

            return sensor;
        }

        // get [IdSensor, IdSensorType, Name, Project] fields for working sensors 
        // obtenir les champs [IdSensor, IdSensorType, Name, Project] des capteurs fonctionnels
        [Microsoft.AspNetCore.Mvc.HttpGet("sensors/ids")]
        public ActionResult<List<SensorsWeb>> GetSensorsIds()
        {
            var filter = "{IsWorking:true}";
            var projection = "{IdSensor:1, IdSensorType:1, Name:1, Project:1}";
            var sensor = _webService.Get<SensorsWeb>("Sensors", filter, projection);

            if (!sensor.Any())
            {
                return NoContent();
            }

            return sensor;
        }

        // get [all fields] for elements in Samples collection
        // obtenir [tous les champs] de tous les éléments de la collection Samples
        [Microsoft.AspNetCore.Mvc.HttpGet("samples")]
        public ActionResult<List<Samples>> GetSamples()
        {
            var sample = _webService.Get<Samples>("Samples");

            if (!sample.Any())
            {
                return NoContent();
            }

            return sample;
        }

        // get [all fields] for sample with ObjectId {id}
        // obtenir [tous les champs] du relevé d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("samples/{id:length(24)}", Name = "GetSample")]
        public ActionResult<Samples> GetSampleById(string id)
        {
            var sample = _webService.GetById<Samples>("Samples", id);

            if (sample == null)
            {
                return NotFound();
            }

            return sample;
        }

        // get fields [IdSensor, Value] for most recent sample
        // obtenir les champs [IdSensor, Value] du relevé le plus récent pour chaque capteur
        [Microsoft.AspNetCore.Mvc.HttpGet("samples/last")]
        public ActionResult<List<SamplesWeb>> GetLastSamples()
        {
            var group = "{_id:'$IdSensor', Id:{$last:'$_id'}, IdSensor:{$last:'$IdSensor'}, IdSampleType:{$last:'$IdSampleType'}, Note:{$last:'$Note'}, SampleDate:{$max:'$SampleDate'}, Value:{$last:'$Value'}}";
            var projection = "{_id:'$Id', IdSensor: '$IdSensor', Value: '$Value'}";
            var sort = "{'IdSensor': -1, 'SampleDate': 1}";
            var sample = _webService.GetDerniersReleves<SamplesWeb>("Samples", group, projection, sort);

            if (!sample.Any())
            {
                return NoContent();
            }

            return sample;
        }

        // get [all fields] for samples with IdSensor {id}
        // obtenir [tous les champs] des relevés dont l'IdSensor est {id}
        [Microsoft.AspNetCore.Mvc.HttpGet("samples/sensor/{id}")]
        public ActionResult<List<Samples>> GetSamplesSensor(string id)
        {
            var filter = "{IdSensor:" + id + "}";
            var sample = _webService.Get<Samples>("Samples", filter);

            if (!sample.Any())
            {
                return NotFound();
            }

            return sample;
        }

        // get [all fields] for elements in ProtocolVersions collection
        // obtenir [tous les champs] de tous les éléments de la collection ProtocolVersions
        [Microsoft.AspNetCore.Mvc.HttpGet("protocolversions")]
        public ActionResult<List<ProtocolVersions>> GetProtocolVersions()
        {
            var protocolVersion = _webService.Get<ProtocolVersions>("ProtocolVersions");

            if (!protocolVersion.Any())
            {
                return NoContent();
            }

            return protocolVersion;
        }

        // get [all fields] for protocol version with ObjectId {id}
        // obtenir [tous les champs] de la version du protocole d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("protocolversions/{id:length(24)}", Name = "GetProtocolVersion")]
        public ActionResult<ProtocolVersions> GetProtocolVersionById(string id)
        {
            var protocolVersion = _webService.GetById<ProtocolVersions>("ProtocolVersions", id);

            if (protocolVersion == null)
            {
                return NotFound();
            }

            return protocolVersion;
        }

        // get [all fields] for elements in Plants collection
        // obtenir [tous les champs] de tous les éléments de la collection Plants
        [Microsoft.AspNetCore.Mvc.HttpGet("plants")]
        public ActionResult<List<Plants>> GetPlants()
        {
            var plant = _webService.Get<Plants>("Plants");

            if (!plant.Any())
            {
                return NoContent();
            }

            return plant;
        }

        // get [all fields] for plant with ObjectId {id}
        // obtenir [tous les champs] de la plante d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("plants/{id:length(24)}", Name = "GetPlant")]
        public ActionResult<Plants> GetPlantById(string id)
        {
            var plant = _webService.GetById<Plants>("Plants", id);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        // get [all fields] for elements in Events collection
        // obtenir [tous les champs] de tous les éléments de la collection Events
        [Microsoft.AspNetCore.Mvc.HttpGet("events")]
        public ActionResult<List<Events>> GetEvents()
        {
            var evenement = _webService.Get<Events>("Events");

            if (!evenement.Any())
            {
                return NoContent();
            }

            return evenement;
        }

        // get [all fields] for event with ObjectId {id}
        // obtenir [tous les champs] de l'évènement d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("events/{id:length(24)}", Name = "GetEvent")]
        public ActionResult<Events> GetEventById(string id)
        {
            var evenement = _webService.GetById<Events>("Events", id);

            if (evenement == null)
            {
                return NotFound();
            }

            return evenement;
        }

        // get [all fields] for last element added in Events collection
        // obtenir [tous les champs] du dernier élément ajouté à Events
        [Microsoft.AspNetCore.Mvc.HttpGet("events/last")]
        public ActionResult<Events> GetLastEvents()
        {
            var evenement = _webService.Get<Events>("Events").LastOrDefault();

            if (evenement == null)
            {
                return NoContent();
            }

            return evenement;
        }

        // get [all fields] for elements in UsersHololens collection
        // obtenir [tous les champs] de tous les éléments de la collection UsersHololens
        [Microsoft.AspNetCore.Mvc.HttpGet("usershololens")]
        public ActionResult<List<UsersHololens>> GetUsersHololens()
        {
            var userHololens = _webService.Get<UsersHololens>("UsersHololens");

            if (!userHololens.Any())
            {
                return NoContent();
            }

            return userHololens;
        }

        // get [all fields] for hololens user with ObjectId {id}
        // obtenir [tous les champs] du user d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usershololens/{id:length(24)}", Name = "GetUserHololens")]
        public ActionResult<UsersHololens> GetUserHololensById(string id)
        {
            var userHololens = _webService.GetById<UsersHololens>("UsersHololens", id);

            if (userHololens == null)
            {
                return NotFound();
            }

            return userHololens;
        }

        // get [all fields] for elements in UsersAdmin collection
        // obtenir [tous les champs] de tous les éléments de la collection UsersAdmin
        [Microsoft.AspNetCore.Mvc.HttpGet("usersadmin")]
        public ActionResult<List<UsersAdmin>> GetUsersAdmin()
        {
            var userAdmin = _webService.Get<UsersAdmin>("UsersAdmin");

            if (!userAdmin.Any())
            {
                return NoContent();
            }

            return userAdmin;
        }

        // get [all fields] for admin user with ObjectId {id}
        // obtenir [tous les champs] de l'utilisateur admin d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usersadmin/{id:length(24)}", Name = "GetUserAdmin")]
        public ActionResult<UsersAdmin> GetUserAdminById(string id)
        {
            var userAdmin = _webService.GetById<UsersAdmin>("UsersAdmin", id);

            if (userAdmin == null)
            {
                return NotFound();
            }

            return userAdmin;
        }

        // get [all fields] for elements in UsersAPI collection
        // obtenir [tous les champs] de tous les éléments de la collection UsersAPI
        [Microsoft.AspNetCore.Mvc.HttpGet("usersapi")]
        public ActionResult<List<UsersAPI>> GetUsersAPI()
        {
            var userAPI = _webService.Get<UsersAPI>("UsersAPI");

            if (!userAPI.Any())
            {
                return NoContent();
            }

            return userAPI;
        }

        // get [all fields] for API user with ObjectId {id}
        // obtenir [tous les champs] du user d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("usersapi/{id:length(24)}", Name = "GetUserAPI")]
        public ActionResult<UsersAPI> GetUserAPIById(string id)
        {
            var userAPI = _webService.GetById<UsersAPI>("UsersAPI", id);

            if (userAPI == null)
            {
                return NotFound();
            }

            return userAPI;
        }

        // get [all fields] for elements in Socials collection
        // obtenir [tous les champs] de tous les éléments de la collection Socials
        [Microsoft.AspNetCore.Mvc.HttpGet("socials")]
        public ActionResult<List<Socials>> GetSocials()
        {
            var social = _webService.Get<Socials>("Socials");

            if (!social.Any())
            {
                return NoContent();
            }

            return social;
        }

        // get [all fields] for social account with ObjectId {id}
        // obtenir [tous les champs] du compte social d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("socials/{id:length(24)}", Name = "GetSocial")]
        public ActionResult<Socials> GetSocialById(string id)
        {
            var social = _webService.GetById<Socials>("Socials", id);

            if (social == null)
            {
                return NotFound();
            }

            return social;
        }

        // get [all fields] for elements in Tables collection
        // obtenir [tous les champs] de tous les éléments de la collection Tables
        [Microsoft.AspNetCore.Mvc.HttpGet("tables")]
        public ActionResult<List<Tables>> GetTables()
        {
            var table = _webService.Get<Tables>("Tables");

            if (!table.Any())
            {
                return NoContent();
            }

            return table;
        }

        // get [all fields] for table with ObjectId {id}
        // obtenir [tous les champs] du tableau d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("tables/{id:length(24)}", Name = "GetTable")]
        public ActionResult<Tables> GetTableById(string id)
        {
            var table = _webService.GetById<Tables>("Tables", id);

            if (table == null)
            {
                return NotFound();
            }

            return table;
        }

        // get [all fields] for elements in Medias collection
        // obtenir [tous les champs] de tous les éléments de la collection Medias
        [Microsoft.AspNetCore.Mvc.HttpGet("medias")]
        public ActionResult<List<Medias>> GetMedias()
        {
            var media = _webService.Get<Medias>("Medias");

            if (!media.Any())
            {
                return NoContent();
            }

            return media;
        }

        // get [all fields] for media with ObjectId {id}
        // obtenir [tous les champs] du media d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("medias/{id:length(24)}", Name = "GetMedia")]
        public ActionResult<Medias> GetMediaById(string id)
        {
            var media = _webService.GetById<Medias>("Medias", id);

            if (media == null)
            {
                return NotFound();
            }

            return media;
        }

        // get [all fields] for elements in Countdowns collection
        // obtenir [tous les champs] de tous les éléments de la collection Countdowns
        [Microsoft.AspNetCore.Mvc.HttpGet("countdowns")]
        public ActionResult<List<Countdowns>> GetCountdowns()
        {
            var countdown = _webService.Get<Countdowns>("Countdowns");

            if (!countdown.Any())
            {
                return NoContent();
            }

            return countdown;
        }

        // get [all fields] for countdown with ObjectId {id}
        // obtenir [tous les champs] du compte à rebours d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("countdowns/{id:length(24)}", Name = "GetCountdown")]
        public ActionResult<Countdowns> GetCountdownById(string id)
        {
            var countdown = _webService.GetById<Countdowns>("Countdowns", id);

            if (countdown == null)
            {
                return NotFound();
            }

            return countdown;
        }

        // get [all fields] for elements in SensorTypes collection
        // obtenir [tous les champs] de tous les éléments de la collection SensorTypes
        [Microsoft.AspNetCore.Mvc.HttpGet("sensortypes")]
        public ActionResult<List<SensorTypes>> GetSensorTypes()
        {
            var sensorType = _webService.Get<SensorTypes>("SensorTypes");

            if (!sensorType.Any())
            {
                return NoContent();
            }

            return sensorType;
        }

        // get [all fields] for sensor type with ObjectId {id}
        // obtenir [tous les champs] du type de capteur d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("sensortypes/{id:length(24)}", Name = "GetSensorType")]
        public ActionResult<SensorTypes> GetSensorTypeById(string id)
        {
            var sensorType = _webService.GetById<SensorTypes>("SensorTypes", id);

            if (sensorType == null)
            {
                return NotFound();
            }

            return sensorType;
        }

        // get [all fields] for elements in Alerts collection
        // obtenir [tous les champs] de tous les éléments de la collection Alerts
        [Microsoft.AspNetCore.Mvc.HttpGet("alerts")]
        public ActionResult<List<Alerts>> GetAlerts()
        {
            var alert = _webService.Get<Alerts>("Alerts");

            if (!alert.Any())
            {
                return NoContent();
            }

            return alert;
        }

        // get [all fields] for alert with ObjectId {id}
        // obtenir [tous les champs] de l'alerte d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("alerts/{id:length(24)}", Name = "GetAlert")]
        public ActionResult<Alerts> GetAlertById(string id)
        {
            var alert = _webService.GetById<Alerts>("Alerts", id);

            if (alert == null)
            {
                return NotFound();
            }

            return alert;
        }

        // get [all fields] for elements in Screens collection
        // obtenir [tous les champs] de tous les éléments de la collection Screens
        [Microsoft.AspNetCore.Mvc.HttpGet("screens")]
        public ActionResult<List<Screens>> GetScreens()
        {
            var screen = _webService.Get<Screens>("Screens");

            if (!screen.Any())
            {
                return NoContent();
            }

            return screen;
        }

        // get [all fields] for screen with ObjectId {id}
        // obtenir [tous les champs] de l'écran d'{id}
        [Microsoft.AspNetCore.Mvc.HttpGet("screens/{id:length(24)}", Name = "GetScreen")]
        public ActionResult<Screens> GetScreenById(string id)
        {
            var screen = _webService.GetById<Screens>("Screens", id);

            if (screen == null)
            {
                return NotFound();
            }

            return screen;
        }



        /* ----- REQUETES POST ----- */

        // insert element in Sensors collection
        // insérer un élément dans la collection Sensors
        [Microsoft.AspNetCore.Mvc.HttpPost("sensors")]
        public ActionResult<Sensors> CreateSensor(Sensors sensor)
        {
            _webService.Create("Sensors", sensor);

            return CreatedAtRoute("GetSensor", new { id = sensor.Id.ToString() }, sensor);
        }

        // insert element in Samples collection
        // insérer un élément dans la collection Samples
        [Microsoft.AspNetCore.Mvc.HttpPost("samples")]
        public ActionResult<Samples> CreateSample(Samples sample)
        {
            _webService.Create("Samples", sample);

            return CreatedAtRoute("GetSample", new { id = sample.Id.ToString() }, sample);
        }

        // insert element in ProtocolVersions collection
        // insérer un élément dans la collection ProtocolVersions
        [Microsoft.AspNetCore.Mvc.HttpPost("protocolversions")]
        public ActionResult<ProtocolVersions> CreateProtocolVersion(ProtocolVersions protocolVersion)
        {
            _webService.Create("ProtocolVersions", protocolVersion);

            return CreatedAtRoute("GetProtocolVersion", new { id = protocolVersion.Id.ToString() }, protocolVersion);
        }

        // insert element in Plants collection
        // insérer un élément dans la collection Plants
        [Microsoft.AspNetCore.Mvc.HttpPost("plants")]
        public ActionResult<Plants> CreatePlant(Plants plant)
        {
            _webService.Create("Plants", plant);

            return CreatedAtRoute("GetPlant", new { id = plant.Id.ToString() }, plant);
        }

        // insert element in Events collection
        // insérer un élément dans la collection Events
        [Microsoft.AspNetCore.Mvc.HttpPost("events")]
        public ActionResult<Events> CreateEvents(Events evenement)
        {
            _webService.Create("Events", evenement);

            return CreatedAtRoute("GetEvent", new { id = evenement.Id.ToString() }, evenement);
        }

        // insert element in UsersHololens collection
        // insérer un élément dans la collection UsersHololens
        [Microsoft.AspNetCore.Mvc.HttpPost("usershololens")]
        public ActionResult<UsersHololens> CreateUserHololens(UsersHololens userHololens)
        {
            _webService.Create("UsersHololens", userHololens);

            return CreatedAtRoute("GetUserHololens", new { id = userHololens.Id.ToString() }, userHololens);
        }

        // insert element in UsersAdmin collection
        // insérer un élément dans la collection UsersAdmin
        [Microsoft.AspNetCore.Mvc.HttpPost("usersadmin")]
        public ActionResult<UsersAdmin> CreateUsersAdmin(UsersAdmin userAdmin)
        {
            _webService.Create("UsersAdmin", userAdmin);

            return CreatedAtRoute("GetUserAdmin", new { id = userAdmin.Id.ToString() }, userAdmin);
        }

        // insert element in UsersAPI collection
        // insérer un élément dans la collection UsersAPI
        [Microsoft.AspNetCore.Mvc.HttpPost("usersapi")]
        public ActionResult<UsersAPI> CreateUsersAPI(UsersAPI userAPI)
        {
            _webService.Create("UsersAPI", userAPI);

            return CreatedAtRoute("GetUserAPI", new { id = userAPI.Id.ToString() }, userAPI);
        }

        // insert element in Socials collection
        // insérer un élément dans la collection Socials
        [Microsoft.AspNetCore.Mvc.HttpPost("socials")]
        public ActionResult<Socials> CreateSocials(Socials social)
        {
            _webService.Create("Socials", social);

            return CreatedAtRoute("GetSocial", new { id = social.Id.ToString() }, social);
        }

        // insert element in Tables collection
        // insérer un élément dans la collection Tableaux
        [Microsoft.AspNetCore.Mvc.HttpPost("tables")]
        public ActionResult<Tables> CreateTable(Tables table)
        {
            _webService.Create("Tables", table);

            return CreatedAtRoute("GetTable", new { id = table.Id.ToString() }, table);
        }

        // insert element in Medias collection
        // insérer un élément dans la collection Medias
        [Microsoft.AspNetCore.Mvc.HttpPost("medias")]
        public ActionResult<Medias> CreateMedia(Medias media)
        {
            _webService.Create("Medias", media);

            return CreatedAtRoute("GetMedia", new { id = media.Id.ToString() }, media);
        }

        // insert element in Countdowns collection
        // insérer un élément dans la collection Countdowns
        [Microsoft.AspNetCore.Mvc.HttpPost("countdowns")]
        public ActionResult<Countdowns> CreateCountdown(Countdowns countdown)
        {
            _webService.Create("Countdowns", countdown);

            return CreatedAtRoute("GetCountdown", new { id = countdown.Id.ToString() }, countdown);
        }

        // insert element in SensorTypes collection
        // insérer un élément dans la collection SensorTypes
        [Microsoft.AspNetCore.Mvc.HttpPost("sensortypes")]
        public ActionResult<SensorTypes> CreateSensorType(SensorTypes sensorType)
        {
            _webService.Create("SensorTypes", sensorType);

            return CreatedAtRoute("GetSensorType", new { id = sensorType.Id.ToString() }, sensorType);
        }

        // insert element in Alerts collection
        // insérer un élément dans la collection Alerts
        [Microsoft.AspNetCore.Mvc.HttpPost("alerts")]
        public ActionResult<Alerts> CreateAlert(Alerts alert)
        {
            _webService.Create("Alerts", alert);

            return CreatedAtRoute("GetAlert", new { id = alert.Id.ToString() }, alert);
        }

        // insert element in Screens collection
        // insérer un élément dans la collection Screens
        [Microsoft.AspNetCore.Mvc.HttpPost("screens")]
        public ActionResult<Alerts> CreateScreen(Screens screen)
        {
            _webService.Create("Screens", screen);

            return CreatedAtRoute("GetScreen", new { id = screen.Id.ToString() }, screen);
        }



        /* ----- REQUETES PUT ----- */

        // modify [all fields] for sensor with ObjectId {id}
        // modifier [tous les champs] du capteur d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("sensors/{id:length(24)}")]
        public IActionResult UpdateSensor(string id, Sensors sensorIn)
        {
            if (id != sensorIn.Id)
            {
                return BadRequest();
            }

            var sensor = _webService.GetById<Sensors>("Sensors", id);

            if (sensor == null)
            {
                return NotFound();
            }

            _webService.Update("Sensors", id, sensorIn);

            return NoContent();
        }

        // modify [all fields] for sample with ObjectId {id}
        // modifier [tous les champs] du relevé d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("samples/{id:length(24)}")]
        public IActionResult UpdateSample(string id, Samples sampleIn)
        {
            if (id != sampleIn.Id)
            {
                return BadRequest();
            }

            var sample = _webService.GetById<Samples>("Samples", id);

            if (sample == null)
            {
                return NotFound();
            }

            _webService.Update("Samples", id, sampleIn);

            return NoContent();
        }

        // modify [all fields] for protocol version with ObjectId {id}
        // modifier [tous les champs] de la version de protocole d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("protocolversions/{id:length(24)}")]
        public IActionResult UpdateProtocolVersion(string id, ProtocolVersions protocolVersionIn)
        {
            if (id != protocolVersionIn.Id)
            {
                return BadRequest();
            }

            var protocolVersion = _webService.GetById<ProtocolVersions>("ProtocolVersions", id);

            if (protocolVersion == null)
            {
                return NotFound();
            }

            _webService.Update("ProtocolVersions", id, protocolVersionIn);

            return NoContent();
        }

        // modify [all fields] for plant with ObjectId {id}
        // modifier [tous les champs] de la plante d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("plants/{id:length(24)}")]
        public IActionResult UpdatePlant(string id, Plants plantIn)
        {
            if (id != plantIn.Id)
            {
                return BadRequest();
            }

            var plant = _webService.GetById<Plants>("Plants", id);

            if (plant == null)
            {
                return NotFound();
            }

            _webService.Update("Plants", id, plantIn);

            return NoContent();
        }

        // modify [all fields] for event with ObjectId {id}
        // modifier [tous les champs] de l’évènement d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("events/{id:length(24)}")]
        public IActionResult UpdateEvent(string id, Events evenementIn)
        {
            if (id != evenementIn.Id)
            {
                return BadRequest();
            }

            var evenement = _webService.GetById<Events>("Events", id);

            if (evenement == null)
            {
                return NotFound();
            }

            _webService.Update("Events", id, evenementIn);

            return NoContent();
        }

        // modify [all fields] for hololens user with ObjectId {id}
        // modifier [tous les champs] de l'utilisateur hololens d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("usershololens/{id:length(24)}")]
        public IActionResult UpdateUserHololens(string id, UsersHololens userHololensIn)
        {
            if (id != userHololensIn.Id)
            {
                return BadRequest();
            }

            var userHololens = _webService.GetById<UsersHololens>("UsersHololens", id);

            if (userHololens == null)
            {
                return NotFound();
            }

            _webService.Update("UsersHololens", id, userHololensIn);

            return NoContent();
        }

        // modify [all fields] for admin user with ObjectId {id}
        // modifier [tous les champs] de l’utilisateur admin d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("usersadmin/{id:length(24)}")]
        public IActionResult UpdateUserAdmin(string id, UsersAdmin userAdminIn)
        {
            if (id != userAdminIn.Id)
            {
                return BadRequest();
            }

            var userAdmin = _webService.GetById<UsersAdmin>("UsersAdmin", id);

            if (userAdmin == null)
            {
                return NotFound();
            }

            _webService.Update("UsersAdmin", id, userAdminIn);

            return NoContent();
        }

        // modify [all fields] for API user with ObjectId {id}
        // modifier [tous les champs] de l’utilisateur API d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("usersapi/{id:length(24)}")]
        public IActionResult UpdateUserAPI(string id, UsersAPI userAPIIn)
        {
            if (id != userAPIIn.Id)
            {
                return BadRequest();
            }

            var userAPI = _webService.GetById<UsersAPI>("UsersAPI", id);

            if (userAPI == null)
            {
                return NotFound();
            }

            _webService.Update("UsersAPI", id, userAPIIn);

            return NoContent();
        }

        // modify [all fields] for social account with ObjectId {id}
        // modifier [tous les champs] du compte social d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("socials/{id:length(24)}")]
        public IActionResult UpdateSocial(string id, Socials socialIn)
        {
            if (id != socialIn.Id)
            {
                return BadRequest();
            }

            var social = _webService.GetById<Socials>("Socials", id);

            if (social == null)
            {
                return NotFound();
            }

            _webService.Update("Socials", id, socialIn);

            return NoContent();
        }

        // modify [all fields] for table with ObjectId {id}
        // modifier [tous les champs] du tableau d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("tables/{id:length(24)}")]
        public IActionResult UpdateTable(string id, Tables tableIn)
        {
            if (id != tableIn.Id)
            {
                return BadRequest();
            }

            var table = _webService.GetById<Tables>("Tables", id);

            if (table == null)
            {
                return NotFound();
            }

            _webService.Update("Tables", id, tableIn);

            return NoContent();
        }

        // modify [all fields] for media with ObjectId {id}
        // modifier [tous les champs] du media d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("medias/{id:length(24)}")]
        public IActionResult UpdateMedia(string id, Medias mediaIn)
        {
            if (id != mediaIn.Id)
            {
                return BadRequest();
            }

            var media = _webService.GetById<Medias>("Medias", id);

            if (media == null)
            {
                return NotFound();
            }

            _webService.Update("Medias", id, mediaIn);

            return NoContent();
        }

        // modify [all fields] for countdown with ObjectId {id}
        // modifier [tous les champs] du compte à rebours d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("countdowns/{id:length(24)}")]
        public ActionResult UpdateCountdown(string id, Countdowns countdownIn)
        {
            if (id != countdownIn.Id)
            {
                return BadRequest();
            }

            var countdown = _webService.GetById<Countdowns>("Countdowns", id);

            if (countdown == null)
            {
                return NotFound();
            }

            _webService.Update("Countdowns", id, countdownIn);

            return NoContent();
        }

        // modify [all fields] for sensor type with ObjectId {id}
        // modifier [tous les champs] du type de capteur d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("sensortypes/{id:length(24)}")]
        public ActionResult UpdateSensorType(string id, SensorTypes sensorTypeIn)
        {
            if (id != sensorTypeIn.Id)
            {
                return BadRequest();
            }

            var sensorType = _webService.GetById<SensorTypes>("SensorTypes", id);

            if (sensorType == null)
            {
                return NotFound();
            }

            _webService.Update("SensorTypes", id, sensorTypeIn);

            return NoContent();
        }

        // modify [all fields] for alert with ObjectId {id}
        // modifier [tous les champs] de l’alerte d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("alerts/{id:length(24)}")]
        public ActionResult UpdateAlert(string id, Alerts alertIn)
        {
            if (id != alertIn.Id)
            {
                return BadRequest();
            }

            var alert = _webService.GetById<Alerts>("Alerts", id);

            if (alert == null)
            {
                return NotFound();
            }

            _webService.Update("Alerts", id, alertIn);

            return NoContent();
        }

        // modify [all fields] for screen with ObjectId {id}
        // modifier [tous les champs] de l'écran d’ObjectId {id}
        [Microsoft.AspNetCore.Mvc.HttpPut("screens/{id:length(24)}")]
        public ActionResult UpdateScreen(string id, Screens screenIn)
        {
            if (id != screenIn.Id)
            {
                return BadRequest();
            }

            var screen = _webService.GetById<Alerts>("Screens", id);

            if (screen == null)
            {
                return NotFound();
            }

            _webService.Update("Screens", id, screenIn);

            return NoContent();
        }


        // default NotFound
        [HttpGet("{*url}"), HttpPost("{*url}"), HttpPut("{*url}"), HttpDelete("{*url}"), HttpHead("{*url}"), HttpOptions("{*url}"), HttpPatch("{*url}")]
        public ActionResult Error()
        {
            return Unauthorized();
        }

    }

    /* ----- API default NotFound ----- */
    [Microsoft.AspNetCore.Mvc.Route("")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("{*url}"), HttpPost("{*url}"), HttpPut("{*url}"), HttpDelete("{*url}"), HttpHead("{*url}"), HttpOptions("{*url}"), HttpPatch("{*url}")]
        public ActionResult Error()
        {
            return Unauthorized();
        }
    }

}