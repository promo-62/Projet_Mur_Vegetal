# MQTT Server installation and execution



## Dependencies install

#### Mosquitto Server

You can refer to the Mosquitto Server ReadMe to install it if you haven't done it before. This server will let our messages transmit over MQTT protocol.

#### .NET and its environnement

In order to allow program compilation you must have a working environnement which can compile .NET projects.



## Program install

Program files, which are located in folder **Recuperation**, shouldn't need to be changed and can be used directly. In case sources have been moved in another project, you have to install project's dependencies.

Source code of the main project is located in files **Recuperation.cs** and **Protocol.cs**.

You must first go in the **Recuperation** folder to ensure you will correctly install:

* `cd Recuperation`

Then, we can install dependencies to make JSON, MQTT's client and MongoDB's api working:

* `dotnet add package Newtonsoft.Json`
* `dotnet add package MongoDB`
* `dotnet add package MQTTnet`

In order to prevent emerging conflict, we run command line:

* `dotnet restore`



## Program execution

To run the program, we must go where source code is located, by default in **Recuperation** folder. We can then run the command to launch it:

* `dotnet run`

A console waiting for message from **Rpi/#** queue will appear.

Only message from **Rpi/DemandeID/Server**, **Rpi/EnvoiInfos/Server** and **Rpi/DemandeAction/Server** queues will be taken care about.