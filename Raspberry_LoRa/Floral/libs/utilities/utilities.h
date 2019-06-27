#ifndef _UTILITIES_
#define _UTILITIES_

#include "../spi/SPI.h"

//SPI spi;

#define RX_MODE 0
#define TX_MODE 1

#define WAITING_CHANGE_MODE 1 //define the time in sec we wait during the changement from RX to TX


volatile unsigned * map_peripherals(void);
void pinMode(int, int);
void digitalWrite(int, int);
int digitalRead(int);
int createInterruptFile(int, int);
void hardwareSetup(int);
__u8 readRegister(__u8);
void writeRegister(__u8, __u8);
void initModule(int);
void setChanHex(void);
void setLoraChannel(int);
__u32 getLoraChannel(void);
void printLoraChannelToStdout(__u32);
void printLoraChannelToFile(__u32, FILE *);

void checkMessageLoRa(int);

void selectLoRaMode(int mode);

void sendTxMessage(__u8 message[]);

void hardwareReset();

void rxConfig(void);

#endif
