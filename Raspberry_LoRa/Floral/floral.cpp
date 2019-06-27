/*
f-LoRa-l using LoRa module

28/06/2019

use with rx8803 RTC

Clément Sanchez
*/

/****************************************
UNIX includes
****************************************/
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <linux/i2c-dev.h>
#include <sys/ioctl.h>
#include <sys/select.h>
#include <sys/mman.h>
#include <string.h>
#include <math.h>
#include <linux/types.h>
#include <linux/spi/spidev.h>
#include <signal.h>
#include <sys/time.h>
#include <sys/param.h>


/****************************************
Project specific includes
****************************************/
#include "libs/spi/SPI.h"

#include "libs/floral.h"

#include "libs/utilities/utilities.h"

#include "libs/SX1272_registers.h"

#include "libs/socket/client.h"



/****************************************
Objects
****************************************/

ClientSocket client;

SPI spi;




/****************************************
global variables declaration
****************************************/

FILE * fd_log;

FILE *fd_index;


//int i,j; // general purpose index

__u32 chanHex[8];


volatile unsigned *gpio;

__u32 channel;

unsigned int lastMessageIndex[8];


__u8 reg_val_table[255];
int rssi;

int rxIndex = RX_CHANNEL - 10;
int rxCount;



/****************************************
main
****************************************/

int main(int argc, char **argv){

	/*
	starting platform configuration
	*/

	fprintf(stdout,"[+] Starting configuration...\n");

	gpio = map_peripherals();	// for direct access (i.e. non sysfile) to pins SEL0..2 and RX_RESETPIN



	setChanHex();		// 32 bit value containing REG_FRF_MSB, REG_FRF_MID & REG_FRF_LSB 
	spi.begin(SPI0);	// init SPI0 interface

	//Connect to the server socket for the communication
	client.connectTo(HOST,(float)5000); // try indefinitely and try each 5 sec
	
	/*
	set up all the modules
	*/
		
	initModule(RX_MODE);	// Init the LoRa module with the Rx mode
	selectLoRaMode(RX_MODE); // Select the Rx mode and charge some configuration

	sleep(1);

		
	//  IRQ reset for all receivers anyway to avoid lock at startup if int pin is already low
	// side effect: messages sent before are lost 
	writeRegister(REG_IRQ_FLAGS, 0xff);  


	while(1){ //infnite loop	

		/*
		Verified if a message was received
		*/
		checkMessageLoRa(rxIndex); // new strategy to avoid blocking
		
	} // end while(1) infinite loop




	return 0;

} // end main



#include "libs/utilities/utilities.cpp"
