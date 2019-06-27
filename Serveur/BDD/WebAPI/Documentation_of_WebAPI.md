# Documentation of WebAPI

### ➢ WebAPI v6

**Summary of functionalities :**

- Read only for all collections except UsersHololens, UsersAdmin, UsersAPI (GET request).
- Custom read only for web interface (GET requests).
- Insert new elements for all collections (POST request).
- Modify an element for all collections (PUT request).
- Delete an element for all collections except UsersHololens, UsersAdmin, UsersAPI (DELETE request).
- Send an 401 Unauthorized header for not handled GET, PUT, POST, PUT, DELETE, HEAD, OPTIONS, PATCH requests.

**Command:**  `api/web/[Route]`



| Method |         Route          |                        Functionality                         |
| :----: | :--------------------: | :----------------------------------------------------------: |
|  GET   |       /sensors/        |             Get [all fields] for working sensors             |
|        |     /sensors/{id}      |        Get [all fields] for sensor with ObjectId {id}        |
|        |      /sensors/ids      | Get [IdSensor, IdSensorType, Name, Project] fields for working sensors |
|        |       /samples/        |     Get [all fields] for elements in Samples collection      |
|        |     /samples/{id}      |        Get [all fields] for sample with ObjectId {id}        |
|        |     samples/last/      |     Get fields [IdSensor, Value] for most recent sample      |
|        |  /samples/sensor/{id}  |       Get [all fields] for samples with IdSensor {id}        |
|        |   /protocolversions/   | Get [all fields] for elements in ProtocolVersions collection |
|        | /protocolversions/{id} |   Get [all fields] for protocol version with ObjectId {id}   |
|        |        /plants/        |      Get [all fields] for elements in Plants collection      |
|        |      /plants/{id}      |        Get [all fields] for plant with ObjectId {id}         |
|        |        /events/        |      Get [all fields] for elements in Events collection      |
|        |      /events/{id}      |        Get [all fields] for event with ObjectId {id}         |
|        |     /events/last/      | Get [all fields] for last element added in Events collection |
|        |       /socials/        |     Get [all fields] for elements in Socials collection      |
|        |     /socials/{id}      |    Get [all fields] for social account with ObjectId {id}    |
|        |        /tables/        |      Get [all fields] for elements in Tables collection      |
|        |      /tables/{id}      |        Get [all fields] for table with ObjectId {id}         |
|        |        /medias/        |      Get [all fields] for elements in Medias collection      |
|        |      /medias/{id}      |        Get [all fields] for media with ObjectId {id}         |
|        |      /countdowns/      |    Get [all fields] for elements in Countdowns collection    |
|        |    /countdowns/{id}    |      Get [all fields] for countdown with ObjectId {id}       |
|        |     /sensortypes/      |   Get [all fields] for elements in SensorTypes collection    |
|        |   /sensortypes/{id}    |     Get [all fields] for sensor type with ObjectId {id}      |
|        |        /alerts/        |      Get [all fields] for elements in Alerts collection      |
|        |      /alerts/{id}      |        Get [all fields] for alert with ObjectId {id}         |
|        |       /screens/        |     Get [all fields] for elements in Screens collection      |
|        |     /screens/{id}      |        Get [all fields] for screen with ObjectId {id}        |



| Method |       Route        |                 Functionality                 |
| :----: | :----------------: | :-------------------------------------------: |
|  POST  |     /sensors/      |     Insert element in Sensors collection      |
|        |     /samples/      |     Insert element in Samples collection      |
|        | /protocolversions/ | Insert element in ProtocolVersions collection |
|        |      /plants/      |      Insert element in Plants collection      |
|        |      /events/      |      Insert element in Events collection      |
|        |  /usershololens/   |  Insert element in UsersHololens collection   |
|        |    /usersadmin/    |    Insert element in UsersAdmin collection    |
|        |     /usersapi/     |     Insert element in UsersAPI collection     |
|        |     /socials/      |     Insert element in Socials collection      |
|        |      /tables/      |      Insert element in Tables collection      |
|        |      /medias/      |      Insert element in Medias collection      |
|        |    /countdowns/    |    Insert element in Countdowns collection    |
|        |   /sensortypes/    |   Insert element in SensorTypes collection    |
|        |      /alerts/      |      Insert element in Alerts collection      |
|        |     /screens/      |     Insert element in Screens collection      |



| Method |         Route          |                        Functionality                        |
| :----: | :--------------------: | :---------------------------------------------------------: |
|  PUT   |     /sensors/{id}      |      Modify [all fields] for sensor with ObjectId {id}      |
|        |     /samples/{id}      |      Modify [all fields] for sample with ObjectId {id}      |
|        | /protocolversions/{id} | Modify [all fields] for protocol version with ObjectId {id} |
|        |      /plants/{id}      |      Modify [all fields] for plant with ObjectId {id}       |
|        |      /events/{id}      |      Modify [all fields] for event with ObjectId {id}       |
|        |  /usershololens/{id}   |  Modify [all fields] for hololens user with ObjectId {id}   |
|        |    /usersadmin/{id}    |    Modify [all fields] for admin user with ObjectId {id}    |
|        |     /usersapi/{id}     |     Modify [all fields] for API user with ObjectId {id}     |
|        |     /socials/{id}      |  Modify [all fields] for social account with ObjectId {id}  |
|        |      /tables/{id}      |      Modify [all fields] for table with ObjectId {id}       |
|        |      /medias/{id}      |      Modify [all fields] for media with ObjectId {id}       |
|        |    /countdowns/{id}    |    Modify [all fields] for countdown with ObjectId {id}     |
|        |   /sensortypes/{id}    |   Modify [all fields] for sensor type with ObjectId {id}    |
|        |      /alerts/{id}      |      Modify [all fields] for alert with ObjectId {id}       |
|        |     /screens/{id}      |      Modify [all fields] for screen with ObjectId {id}      |



| Method |         Route          |                        Fonctionality                         |
| :----: | :--------------------: | :----------------------------------------------------------: |
| DELETE |     /sensors/{id}      |  Delete element with ObjectId {id} from collection Sensors   |
|        |     /samples/{id}      |  Delete element with ObjectId {id} from collection Samples   |
|        | /protocolversions/{id} | Delete element with ObjectId {id} from collection ProtocolVersions |
|        |      /plants/{id}      |   Delete element with ObjectId {id} from collection Plants   |
|        |      /events/{id}      |   Delete element with ObjectId {id} from collection Events   |
|        |     /socials/{id}      |  Delete element with ObjectId {id} from collection Socials   |
|        |      /tables/{id}      |   Delete element with ObjectId {id} from collection Tables   |
|        |      /medias/{id}      |   Delete element with ObjectId {id} from collection Medias   |
|        |    /countdowns/{id}    | Delete element with ObjectId {id} from collection Countdowns |
|        |   /sensortypes/{id}    | Delete element with ObjectId {id} from collection SensorTypes |
|        |      /alerts/{id}      |   Delete element with ObjectId {id} from collection Alerts   |
|        |     /screens/{id}      |  Delete element with ObjectId {id} from collection Screens   |

[^Dev]: Etienne Schelfhout

