# Install

## Requirement

You need to install gettext and libunwind8 :

```bash
$ sudo apt−get −y update
$ sudo apt−get −y install libunwind8 gettext
```



## Choose a path for the project

install the project in a path like /home/projectRaspberry (you can change the path as you like)



## Dotnet Installation

Download compressed files with sdk and runtime in them:

```bash
$ wget "https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.105-linux-arm64-binaries"
$ wget "https://dotnet.microsoft.com/download/thank-you/dotnet-runtime-2.2.3-linux-arm64-binaries"
```





Finally we use command to install software:

```bash
$ sudo mkdir /opt /dotnet
$ sudo tar −xvf dotnet−sdk−2.2.105−linux−arm.tar.gz −C /opt /dotnet/
$ sudo tar −xvf dotnet−runtime−2.2.3−linux−arm.tar.gz −C /opt /dotnet/
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
  Version: 2.2.105

Runtime Environment :
  OS Name:     raspbian
  OS Version:  9
  OS Platform: Linux
  RID:         lnux−arm
  Base Path:   /home/pi/dotnet/sdk/2.2.105/

Host (useful for support):
  Version: 2.2.3

.NET Core SDKs installed:
  2.2.105 [/home/pi/dotnet/sdk]

.NET Core run times installed:
  Microsoft.AspNetCore.All2.2.1 [/home/pi/dotnet/shared/Microsoft.AspNetCore.All]
  Microsoft.AspNetCore.App2.2.1 [/home/pi/dotnet/shared/Microsoft.AspNetCore.App]
  Microsoft.NETCore.App2.2.1 [/home/pi/dotnet/shared/Microsoft.NETCore.App]
```




## Move the project in the path you want and open a terminal window

We use the path /home/projectRaspberry where our project is, and open a terminal

```bash
$ cd /home/projectRaspberry/Projet
```



## Json .NET

Add Json .NET to your project

```bash
$ dotnet add package Newtonsoft.Json −−version12.0.2
```



## M2MQTT

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

- An array ”Header_Response”, it will contain all header formats possible that a sensor can receive.

- An array ”Sizes”, it will contain all maximal sizes of the payload depending of the protocol version. 

- An array ”Payload”, it will contain every payload format possible for a sensor. 

Both array "Header" and "Header_Response" have in each format a field "TO_ADD" and "TO_REMOVE", these field are used to remove some property of the header before sending it (in the case of "Header" the added elements will be added at the end of the header and in the case of "Header_Response" the added elements will need to have a position which is the position in the byte array starting from 0).