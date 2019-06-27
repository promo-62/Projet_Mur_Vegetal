#include "SPI.h"
#include <fcntl.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

#include <linux/types.h>
#include <linux/spi/spidev.h>

#include <sys/ioctl.h>
#include <string.h>

struct spi_ioc_transfer xfer[1];
struct spi_ioc_transfer xfer0[1];
struct spi_ioc_transfer xfer1[1];


SPI::SPI(){}

// initialises SPI with default parameters speed=4MHz, msb first, mode = 0
// standard arduino function, uses _CS1
void SPI::begin(void) {

int fd_spi;
__u8    mode, lsb, bits;
__u32 	speed;

        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)

	mode = SPI_MODE_0;
	ioctl(fd_spi, SPI_IOC_WR_MODE, &mode);

	lsb = 0; // MSB first
	ioctl(fd_spi, SPI_IOC_WR_LSB_FIRST, &lsb);

	bits = 8; // 8 bits
//	ioctl(fd_spi, SPI_IOC_WR_BITS_PER_WORD, &bits);

	speed = 6500000;
//	ioctl(fd_spi, SPI_IOC_WR_MAX_SPEED_HZ, &speed);

	close(fd_spi);

	
	xfer[0].cs_change = 0, /* Keep CS activated if = 1 */
	xfer[0].delay_usecs = 0, //delay in us
	xfer[0].speed_hz = speed, //speed
        xfer[0].bits_per_word = bits; // bits per word 8

} // end begin(void)





// Set bit order
// standard arduino function, uses _CS1
void SPI::setBitOrder(int order){

__u8 lsb;

int fd_spi;
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)

if (order == 0){
lsb = 0;
} else {
lsb = 1;
} // end if (order == 0)

ioctl(fd_spi, SPI_IOC_WR_LSB_FIRST, &lsb);

	close(fd_spi);

} // end setBitOrder(int order)

// set clock divider
// standard arduino function, uses _CS1
void SPI::setClockDivider(int div){

__u32 speed;

int fd_spi;
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        } // end if (fd_spi < 0)


if ((div <1) || (div > 255)) {

	if ((div <300) || (div > 306)) {
	div = 21;
	} else {
			
			switch(div){
			case 300:
			div = 11;
			break;
			case 301:
			div = 21;
			break;
			case 302:
			div = 42;
			break;
			case 303:
			div = 84;
			break;
			case 304:
			div= 168;
			break;
			case 305:
			div = 336;
			break;
			case 306:
			speed = 672;
			break;
			}//end switch(div)

			
	}// end if ((div <300) || (div > 306))

} // end if ((div <1) || (div > 255))

div = div*13/21;
speed = (__u32)(84000000 / div);
//ioctl(fd_spi, SPI_IOC_WR_MAX_SPEED_HZ, &speed);
	xfer[0].speed_hz = speed, //speed

close(fd_spi);


//fprintf(stdout,"div= %d\n", div);
//fprintf(stdout,"speed= %d\n", speed);

} // end setClockDivider(int div)

// set data mode
// standard arduino function, uses _CS1

void SPI::setDataMode(int arduino_mode){

__u8 mode;

int fd_spi;

fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        } // end if (fd_spi < 0)

switch(arduino_mode){
case 0:
mode = SPI_MODE_0;
break;
case 1:
mode = SPI_MODE_1;
break;
case 2:
mode = SPI_MODE_2;
break;
case 3:
mode = SPI_MODE_3;
break;
default:
mode = SPI_MODE_0;
break;
} // end switch(arduino_mode)

	ioctl(fd_spi, SPI_IOC_WR_MODE, &mode);
	close(fd_spi);

} // end setDataMode(int arduino_mode)


// spi transfer
// standard arduino function, uses _CS1
char SPI::transfer(char val){

	char inbuf[1];
	char outbuf[1];
	int status;

// struct spi_ioc_transfer xfer[1];

int fd_spi;
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        } // end if (fd_spi < 0)

// clearing buffers
	memset(inbuf, 0, sizeof inbuf);
	memset(outbuf, 0, sizeof outbuf);

// filling buffers
	outbuf[0] = val;	


	xfer[0].rx_buf = (unsigned long) inbuf;
	xfer[0].tx_buf = (unsigned long)outbuf;
	xfer[0].len = 1; /* Length of  command to write*/

//	xfer[0].speed_hz =1000000; //speed


// sending message
 
	status = ioctl(fd_spi, SPI_IOC_MESSAGE(1), xfer);
    	if (status < 0)
        {
        perror("SPI_IOC_MESSAGE");
        exit(EXIT_FAILURE);
        } //end if (status < 0)
	close(fd_spi);

	return inbuf[0];

} // end transfer(char val)




/*********************** RPI STYLE *************************************/
/***********************************************************************/
// rpi specific, uses either _CS0 or _CS1


// initialises SPI with default parameters speed=4MHz, msb first, mode = 0
// rpi specific, uses either _CS0 or _CS1
void SPI::begin(int _csPin) {


int fd_spi;
__u8    mode, lsb, bits;
__u32 	speed;

        bits = 8; // 8 bits
	speed = 6500000;
        
        if (!_csPin){
        fd_spi = open("/dev/spidev0.0", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.0");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
        xfer0[0].cs_change = 0, /* Keep CS activated if = 1 */
		xfer0[0].delay_usecs = 0, //delay in us
		xfer0[0].speed_hz = speed, //speed
        xfer0[0].bits_per_word = bits; // bits per word 8

        } else {
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
        xfer1[0].cs_change = 0, /* Keep CS activated if = 1 */
		xfer1[0].delay_usecs = 0, //delay in us
		xfer1[0].speed_hz = speed, //speed
        xfer1[0].bits_per_word = bits; // bits per word 8
             
        }
        
        
	mode = SPI_MODE_0;
	ioctl(fd_spi, SPI_IOC_WR_MODE, &mode);

	lsb = 0; // MSB first
	ioctl(fd_spi, SPI_IOC_WR_LSB_FIRST, &lsb);

	close(fd_spi);


} // end begin(int _csPin)

// spi transfer
// rpi specific, uses either _CS0 or _CS1
char SPI::transfer(int _csPin, char val){

	char inbuf[1];
	char outbuf[1];
        int fd_spi;
        
        // preparing buffers
	memset(inbuf, 0, sizeof inbuf);
	memset(outbuf, 0, sizeof outbuf);
	outbuf[0] = val;
        
// sending message
        
        if (!_csPin){
        fd_spi = open("/dev/spidev0.0", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.0");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
                
        xfer0[0].rx_buf = (unsigned long) inbuf;
	xfer0[0].tx_buf = (unsigned long)outbuf;
	xfer0[0].len = 1; /* Length of  command to write*/
                
        if (ioctl(fd_spi, SPI_IOC_MESSAGE(1), xfer0) < 0)
        {
        perror("SPI_IOC_MESSAGE");
        exit(EXIT_FAILURE);
        } //end if ioctl...

        } else {
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
                
        xfer1[0].rx_buf = (unsigned long) inbuf;
	xfer1[0].tx_buf = (unsigned long)outbuf;
	xfer1[0].len = 1; /* Length of  command to write*/
                
        if (ioctl(fd_spi, SPI_IOC_MESSAGE(1), xfer1) < 0)
        {
        perror("SPI_IOC_MESSAGE");
        exit(EXIT_FAILURE);
        } //end if ioctl...

        }// end if (!_csPin)
        
// terminating

        close(fd_spi);
	return inbuf[0];

} // end transfer(int _csPin, char val)

// spi transfer
// rpi specific, uses either _CS0 or _CS1
void SPI::transfer(int _csPin, char *buffer, int length){

	char inbuf[length];
	char outbuf[length];
        int fd_spi;
        int i;
        
        // preparing buffers
	memset(inbuf, 0, sizeof inbuf);
	memset(outbuf, 0, sizeof outbuf);
	for (i=0; i<length; i++){
                outbuf [i] = buffer[i];
        }
        
// sending message
        
        if (!_csPin){
        fd_spi = open("/dev/spidev0.0", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.0");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
                
        xfer0[0].rx_buf = (unsigned long) inbuf;
	xfer0[0].tx_buf = (unsigned long)outbuf;
	xfer0[0].len = length; //Length of  command to write
                
        if (ioctl(fd_spi, SPI_IOC_MESSAGE(1), xfer0) < 0)
        {
        perror("SPI_IOC_MESSAGE");
        exit(EXIT_FAILURE);
        } //end if ioctl...

        } else {
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
                
        xfer1[0].rx_buf = (unsigned long) inbuf;
	xfer1[0].tx_buf = (unsigned long)outbuf;
	xfer1[0].len = length; // Length of  command to write
                
        if (ioctl(fd_spi, SPI_IOC_MESSAGE(1), xfer1) < 0)
        {
        perror("SPI_IOC_MESSAGE");
        exit(EXIT_FAILURE);
        } //end if ioctl...

        }// end if (!_csPin)
        
// terminating

        close(fd_spi);
//	buffer= inbuf;
	for (i=0; i<length; i++){
                buffer [i] =inbuf[i];
        }
} // end transfer(char val)

// set clock divider

void SPI::setClockDivider(int _csPin, int div){

__u32 speed;

int fd_spi;


if ((div <1) || (div > 255)) {

	if ((div <300) || (div > 306)) {
	div = 21;
	} else {
			
			switch(div){
			case 300:
			div = 11;
			break;
			case 301:
			div = 21;
			break;
			case 302:
			div = 42;
			break;
			case 303:
			div = 84;
			break;
			case 304:
			div= 168;
			break;
			case 305:
			div = 336;
			break;
			case 306:
			div = 672;
			break;
			}//end switch(div)

			
	}// end if ((div <300) || (div > 306))

} // end if ((div <1) || (div > 255))

		div = div*13/21;
		speed = (__u32)(84000000 / div);
	
       if (!_csPin){
        fd_spi = open("/dev/spidev0.0", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.0");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
 		xfer0[0].speed_hz = speed; //speed


        } else {
        fd_spi = open("/dev/spidev0.1", O_RDWR);
        if (fd_spi < 0) {
                perror("/dev/spidev0.1");
                exit(EXIT_FAILURE);
        }// end if (fd_spi < 0)
 		xfer1[0].speed_hz = speed; //speed

             
        }


close(fd_spi);


//fprintf(stdout,"div= %d\n", div);
//fprintf(stdout,"speed= %d\n", speed);

} // end setClockDivider(int int )

