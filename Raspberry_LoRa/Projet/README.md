# Install

## Requirement

You need to install gettext and libunwind8 :

```bash
$ sudo apt−get −y update
$ sudo apt−get −y install curl libunwind8 gettext
```



## Choose a path for the project

install the project in a path like /home/projectRaspberry (you can change the path as you like)



## .NET Installation

Download the compressed files of .NET core runtime

```bash
$ curl -sSL -o dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.2/dotnet-runtime-latest-linux-arm.tar.gz
```





Then install .NET core runtime

```bash
$ sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
$ sudo ln -s /opt/dotnet/dotnet /usr/local/bin
$ sudo rm dotnet.tar.gz
```





Now download the compressed files of .NET core SDK 

```bash
$ curl -sSL -o dotnet.tar.gz https://download.visualstudio.microsoft.com/download/pr/d79ab9a0-937f-4b93-beb4-8b5a24b96085/16141146887856795ba21c0315c09c2b/dotnet-sdk-2.2.202-linux-arm.tar.gz
```





Finally we install .NET core SDK

```bash
$ sudo tar zxf dotnet.tar.gz -C /opt/dotnet
$ sudo rm dotnet.tar.gz
```



## Check



```bash
$ dotnet −−info
```



And you should have an equivalent of these lines :

```bash
pi@raspberrypi:~ $ dotnet --info

Host (useful for support):
  Version: 2.2.3
  Commit:  6b8ad509b6

.NET Core SDKs installed:
  No SDKs were found.

.NET Core runtimes installed:
  Microsoft.NETCore.App 2.2.3 [/opt/dotnet/shared/Microsoft.NETCore.App]

To install additional .NET Core runtimes or SDKs:
  https://aka.ms/dotnet-download
```




## Add libraries

We use the path /home/projectRaspberry where our project is, and open a terminal

```bash
$ cd /home/projectRaspberry/Projet
```



## Json .NET

Add Json .NET to your project

```bash
$ dotnet add package Newtonsoft.Json
```



## MQTTnet

Then install MQTTnet

```bash
$ dotnet add package MQTTnet
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