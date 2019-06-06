#!/bin/bash
 
#Required
domain=$1
 
#Change to your company details
country=FR
state=Nord
locality=Lille
organization=ISEN
organizationalunit=Promo\ 62

if [ -z "$domain" ]
then
    echo "---> You must provide a common name."
    echo "---> Useage $0 [common name]--"
 
    exit 99
fi
 
 
#Generate a key
echo "---> Generating key request for $domain"
openssl genrsa -des3 -out $domain.key 2048
 
if [ ! -s $domain.key ]
then
	echo "---> Passwords do not match"
	echo "---> Try again with correct passwords"

	rm $domain.key
	exit 99
fi

#Create the certificate
echo "Creating certificate from the previous key"
openssl req -new -x509 -days 7200 -extensions v3_ca -key $domain.key -out $domain.crt \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$domain"

if [ ! -s $domain.crt ]
then
	echo "---> Wrong password"
	echo "---> Try again with correct password"

	rm $domain.key
	exit 99
fi

echo
echo "---------------------------"
echo "--Files have been created--"
echo "---------------------------"
echo