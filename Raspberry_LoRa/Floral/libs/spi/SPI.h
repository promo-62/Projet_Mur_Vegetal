#ifndef SPI_H
#define SPI_H

/******************************************************************************
 * Includes
 ******************************************************************************/
#include <linux/types.h>
#include <linux/spi/spidev.h>

#include <sys/ioctl.h>

/******************************************************************************
 * Definitions & Declarations
 *****************************************************************************/

#define SPI0 0
#define SPI1 1

#define LSBFIRST 1
#define MSBFIRST 0
      
#define SPI_MODE0 0
#define SPI_MODE1 1 
#define SPI_MODE2 2
#define SPI_MODE3 3

#define SPI_CLOCK_DIV2 300
#define SPI_CLOCK_DIV4 301
#define SPI_CLOCK_DIV8 302
#define SPI_CLOCK_DIV16 303
#define SPI_CLOCK_DIV32 304
#define SPI_CLOCK_DIV64 305
#define SPI_CLOCK_DIV128 306

// struct spi_ioc_transfer xfer[1];

/******************************************************************************
 * Function prototypes & macros
 *****************************************************************************/



class SPI {

public:
	SPI();
// arduino compatible	
	void begin(void);
	void setBitOrder(int);
	void setClockDivider(int);
	void setDataMode(int);
	char transfer(char);

// rpi style	
	void begin(int);
	void setClockDivider(int, int);
	char transfer(int, char);
	void transfer(int, char *, int);
};


#endif
