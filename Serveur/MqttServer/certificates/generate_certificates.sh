#!/bin/bash
 
# Only those values can be changed
files_name_ca=ca
files_name_server=server
files_name_client=client

country=FR
state=Nord
locality=Lille
organization=ISEN
organizationalunit=Promo\ 62

common_name_ca=ca

# Temporary while we are working locally
ip_address="$(hostname -I)"
substring=$(echo $ip_address| cut -d' ' -f 1) 
common_name_apps="$substring"

# Do not change anything under this line
read -s -p "--> Enter a password for the CA key: " password
echo
read -s -p "--> Confirm your Password: " confirm
echo

until [[ "$password" == "$confirm" && ${#password} > 3 ]]
do
	echo "--> Passwords don't match or password's length is inferior to 4"
	read -s -p "--> Enter a password for the CA key: " password
	echo
	read -s -p "--> Confirm your Password: " confirm
	echo
done
 
# Generate a key
echo "--> Generating key request for $files_name_ca..."
openssl genrsa -des3 -passout pass:$password -out $files_name_ca.key 2048

# Create the certificate
echo "--> Creating certificate by using the key..."
openssl req -new -x509 -days 7200 -extensions v3_ca -key $files_name_ca.key -passin pass:$password -out $files_name_ca.crt \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$common_name_ca"


# Generate server key
echo "--> Generating key request for $files_name_server..."
openssl genrsa -out $files_name_server.key 2048
 
# Create the server certificate request
echo "--> Creating the certificate request with the previous key..."
openssl req -new -out $files_name_server.csr -key $files_name_server.key \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$common_name_apps"

# Validate the server certificate
echo "--> Validating the certificate request..."
openssl x509 -req -in $files_name_server.csr -passin pass:$password -CA $files_name_ca.crt -CAkey $files_name_ca.key -CAcreateserial -out $files_name_server.crt -days 7200

# Generate client key
echo "--> Generating key request for $files_name_client"
openssl genrsa -out $files_name_client.key 2048
 
# Create the client certificate request
echo "--> Creating the certificate request with the previous key..."
openssl req -new -out $files_name_client.csr -key $files_name_client.key \
    -subj "/C=$country/ST=$state/L=$locality/O=$organization/OU=$organizationalunit/CN=$common_name_apps"

# Validate the client certificate
echo "--> Validating the certificate request..."
openssl x509 -req -in $files_name_client.csr -passin pass:$password -CA $files_name_ca.crt -CAkey $files_name_ca.key -CAcreateserial -out $files_name_client.crt -days 7200

echo
echo "------------------"
echo "--Task completed--"
echo "------------------"
echo
