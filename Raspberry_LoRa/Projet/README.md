# Install

## Requirement

You need to install gettext and libunwind8 :

```bash
$ sudo apt−get −y update
$ sudo apt−get −y install libunwind8 gettext
```



## Choose a path for the project

install the project in a path like C:\\home\RaspberryPart (you can change the path as you like)



## Dotnet Installation

Download compressed files with sdk and runtime in them:

```bash
$ wget https://download.visualstudio.microsoft.com/download/pr/9650e3a6−0399−4330−a363−1add761127f9/14d80726c16d0e3d36db2ee5c11928e4/dotnet−sdk−2.2.102−linux−arm.tar.gz
$ wget https://download.visualstudio.microsoft.com/download/pr/9d049226−1f28−4d3d−a4ff−314e56b223c5/f67ab05a3d70b2bff46ff25e2b3acd2a/aspnetcore−runtime−2.2.1−linux−arm.tar.gz
```





Finally we use command to install software:

```bash
$ sudo mkdir /opt /dotnet
$ sudo tar −xvf dotnet−sdk−2.2.102−linux−arm.tar.gz −C /opt /dotnet/
$ sudo tar −xvf aspnetcore−runtime−2.2.1−linux−arm.tar.gz −C /opt /dotnet/
$ sudo ln −s /opt /dotnet/dotnet /usr/local/bin
```





## Check



```bash
$ dotnet −−info
```



And you should have an equivalent of these lines :

```bash
pi@crowpi:˜ $ dotnet −−info
.NET Core SDK (reflecting any global.json) :
  Version: 2.2.102

Runtime Environment :
  OS Name:     raspbian
  OS Version:  9
  OS Platform: Linux
  RID:         lnux−arm
  Base Path:   /home/pi/dotnet/sdk/2.2.102/

Host (useful for support):
  Version: 2.2.1

.NET Core SDKs installed:
  2.2.102 [/home/pi/dotnet/sdk]

.NET Core run times installed:
  Microsoft.AspNetCore.All2.2.1 [/home/pi/dotnet/shared/Microsoft.AspNetCore.All]
  Microsoft.AspNetCore.App2.2.1 [/home/pi/dotnet/shared/Microsoft.AspNetCore.App]
  Microsoft.NETCore.App2.2.1 [/home/pi/dotnet/shared/Microsoft.NETCore.App]
```





## Json.NET



```bash
$ dotnet add package Newtonsoft.Json −−version12.0.2
```



## Move the project in the path you want

Choose a path for the next step we use the path /home/projectRaspberry



## M2MQTT

You need to install M2MQTT in your project so move to your project folder

```bash
$ cd /home/projectRaspberry
```



Then install M2MQTT

```bash
$ Install-Package M2Mqtt -Version 4.3.0
```





# Execute

Move in the project folder and type the following command :

```bash
$ dotnet run
```



# Maintenance

In order for the protocol code to adapt to new versions of protocol tram and new parameters added or deleted, the administrator need to check that the configuration Config.json have adequate adaptation. The file is divided in 3 arrays :
- An array ”Header”, it will contain all header formats possible that a sensor can send.

- An array ”Sizes”, it will contain all maximal sizes of the payload depending of the protocol version. 

- An array ”Payload”, it will contain all every payload format possible of the sensors. 



Warning: If the header format is modified, you will have to adapt the Config.json file but also the global variables in the Protocol.cs files situated in Raspberry pi.