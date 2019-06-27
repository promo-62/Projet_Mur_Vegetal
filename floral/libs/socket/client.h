/*
    Simple tcp client
*/
#include <stdio.h>   
#include <string.h> 
#include <stdlib.h> 
#include <arpa/inet.h>
#include <sys/socket.h>
#include <linux/types.h>

#include <unistd.h> //sleep

#define BUFLEN 100    //Max length of buffer
#define PORT 15200    //The port on which to send data
#define HOST "127.0.0.1" // HOST we want to connect


class ClientSocket{
    private:
        struct sockaddr_in si_other;
        int s, i, slen=sizeof(si_other);

        char buf[BUFLEN];

    public:
        void connectTo(char[],int); //try to connect indefinitely and wait x sec
        void connectTo(char[],float); // try to connect x times in millisecondes
        void die(char *s){
            perror(s);
            exit(1);
        }

        int sendToServer(__u8 [BUFLEN],__u8 *);
};
