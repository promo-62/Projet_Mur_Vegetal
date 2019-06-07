#!/bin/bash
 
#Required
domain=$1
 
#Change to your company details
country=FR
state=Nord
locality=Lille
organization=ISEN
organizationalunit=Promo\ 62

server=server
client=client

if [ -z "$domain" ]
then
    echo "---> You must provide the common name used before."
    echo "---> Useage $0 [common name] --"
 
    exit 99
fi
 
 
#Generate server key
echo "---> Generating key request for $server"
openssl genrsa -out $server.key 2048
 
#Create the server certificate request
echo "---> Creating the certificate request with the previous key"
openssl req -new -out $server.csr -key $server.key \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$server"

if [ ! -s $server.csr ]
then
	echo "---> Error"
	echo "---> Try again"

	rm $server.key
	rm $server.csr
	exit 99
fi

#Validate the server certificate
echo "---> Validating the certificate request"
openssl x509 -req -in $server.csr -CA $domain.crt -CAkey $domain.key -CAcreateserial -out $server.crt -days 7200

if [ ! -s $server.crt ]
then
	echo "---> Wrong password"
	echo "---> Try again with correct password"

	rm $server.key
	rm $server.csr
	rm $server.crt
	exit 99
fi

#Generate client key
echo "---> Generating key request for $client"
openssl genrsa -out $client.key 2048
 
#Create the client certificate request
echo "---> Creating the certificate request with the previous key"
openssl req -new -out $client.csr -key $client.key \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$client"

if [ ! -s $client.csr ]
then
	echo "---> Error"
	echo "---> Try again"

	rm $server.key
	rm $server.csr
	rm $server.crt
	rm $client.key
	rm $client.csr
	exit 99
fi

#Validate the client certificate
echo "---> Validating the certificate request"
openssl x509 -req -in $client.csr -CA $domain.crt -CAkey $domain.key -CAcreateserial -out $client.crt -days 7200

if [ ! -s $client.crt ]
then
	echo "---> Wrong password"
	echo "---> Try again with correct password"

	rm $server.key
	rm $server.csr
	rm $server.crt
	rm $client.key
	rm $client.csr
	rm $client.crt
	exit 99
fi


echo
echo "---------------------------"
echo "--Files have been created--"
echo "---------------------------"
echo