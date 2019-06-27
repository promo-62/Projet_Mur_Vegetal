#include "client.h"


void ClientSocket::connectTo(char host[],int nbTryConnection){

    int nbConnection = 0;int connected = 0;

    if ((this->s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1){
        this->die("socket");
    }

    memset((char *) &si_other, 0, sizeof(this->si_other));
    this->si_other.sin_family = AF_INET;
    this->si_other.sin_port = htons(PORT);

    if (inet_aton(host, &si_other.sin_addr) == 0) {
        fprintf(stderr, "inet_aton() failed\n");
        exit(1);
    }
    while (!connected){
        if (nbConnection < nbTryConnection){
            if(connect(this->s,(struct sockaddr *) &this->si_other,sizeof(this->si_other)) == -1){
                fprintf(stdout,"[+] Impossible to connect to %s:%d. Try again %d/%d ... \n",host,PORT,nbConnection,nbTryConnection);
                nbConnection++; //Error while trying to connect, increment nb of try 
                usleep(500 * 1000); //wait 0.5 sec
            }else{
                connected = 1;
            }
        }else{
            die("connect()");
        }
    }

    fprintf(stdout,"[+] Connected to %s:%d  ... \n\n",host,PORT);
    
}
void ClientSocket::connectTo(char host[],float timeTry){

    int connected = 0;

    if ((this->s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1){
        this->die("socket");
    }

    memset((char *) &si_other, 0, sizeof(this->si_other));
    this->si_other.sin_family = AF_INET;
    this->si_other.sin_port = htons(PORT);

    if (inet_aton(host, &si_other.sin_addr) == 0) {
        fprintf(stderr, "inet_aton() failed\n");
        exit(1);
    }
    while (!connected){
        if(connect(this->s,(struct sockaddr *) &this->si_other,sizeof(this->si_other)) == -1){
            fprintf(stdout,"[+] Impossible to connect to %s:%d. Try again in %.2f sec ... \n",host,PORT,timeTry /1000 );
            usleep(timeTry * 1000); // takes microseconds and Error while trying to connect, wait timeTry sec
        }else{
            connected = 1;
        }
    }

    fprintf(stdout,"[+] Connected to %s:%d  ... \n\n",host,PORT);
}

int ClientSocket::sendToServer(uint8_t messageBin[BUFLEN],uint8_t * msgTx){

    //send the message
    if (send(this->s, messageBin, BUFLEN , 0)==-1)
    {
        die("send()");
    }

    //receive a reply and print it
    //clear the buffer by filling null, it might have previously received data
    memset(this->buf,'\0', BUFLEN);
    //try to receive some data, this is a blocking call
    
    if (recv(this->s, this->buf, BUFLEN, 0) == -1)
    {
        die("recvfrom()");
    }

    fprintf(stdout,"|------[+] Message received by the server: ");
    //puts(buf);
    for (size_t m = 0; m < BUFLEN; m++){
        fprintf(stdout,"%x ",this->buf[m]);
    }
    
    fprintf(stdout,"\n");

    for (size_t k = 0; k < BUFLEN; k++) {
        msgTx[k] = (uint8_t) this->buf[k];
    }
    
    return 1;
}