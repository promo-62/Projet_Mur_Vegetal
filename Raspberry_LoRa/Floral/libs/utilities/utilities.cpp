
/****************************************************************************/
/*
For direct access (i.e. non sysfile) to pins SEL0..2 and RX_RESETPIN

see : https://www.iot-programmer.com/index.php/books/22-raspberry-pi-and-the-iot-in-c/chapters-raspberry-pi-and-the-iot-in-c/59-raspberry-pi-and-the-iot-in-c-memory-mapped-gpio?start=2

 */
volatile unsigned * map_peripherals(void) {
	void *gpio_map;
	int  mem_fd;

   /* open /dev/mem */
   if ((mem_fd = open("/dev/mem", O_RDWR|O_SYNC) ) < 0) {
      perror("opening /dev/mem");
      exit(-1);
   }
 
   /* mmap GPIO */
   gpio_map = mmap(
      NULL,             //Any adddress in our space will do
      BLOCK_SIZE,       //Map length
      PROT_READ|PROT_WRITE,// Enable reading & writting to mapped memory
      MAP_SHARED,       //Shared with other processes
      mem_fd,           //File to map
      GPIO_BASE         //Offset to GPIO peripheral
   );
 
   
 
   if (gpio_map == MAP_FAILED) {
      perror("mmap(/dev/mem)");//errno also set!
      exit(-1);
   }
 
   // Always use volatile pointer!
 //  gpio = (volatile unsigned *)gpio_map;
	return (volatile unsigned *)gpio_map;
 
	close(mem_fd); //No need to keep mem_fd open after mmap
 
 
} // end map_peripherals

/****************************************************************************/
/*
defines pin mode
pin = gpio pin, mode = 0|1|2
returns none
*/
void pinMode(int pin, int mode){

	switch(mode){

		case 0:
		INP_GPIO(pin); // must use INP_GPIO before we can use OUT_GPIO
		OUT_GPIO(pin);
		break;

		case 1:
		INP_GPIO(pin);
		GPIO_PULL = 0;
		break;

		case 2:
		INP_GPIO(pin);
		GPIO_PULL = 2;
		break;

		default:
		INP_GPIO(pin);
		GPIO_PULL = 0;
		break;

	}

	/****************************************************************************/
	// sets pull-up/float
	usleep(10);

	if (pin>= 32)
	GPIO_PULLCLK1 = (1 << (pin % 32));
	else
	GPIO_PULLCLK0 = (1 << pin);

	usleep(10);

	GPIO_PULL = 0;

	if (pin>= 32)
	GPIO_PULLCLK1 = 0;
	else
	GPIO_PULLCLK0 = 0;

/****************************************************************************/
}// end pinMode(int pin, int mode)

/*
defines pin value
pin = gpio pin, value = 0|1
returns none
*/
void digitalWrite(int pin, int value){

	switch(value){

		case 0:
		if (pin>= 32)
		GPIO_CLR_EXT = (1 << (pin % 32));
		else
		GPIO_CLR = (1 << pin);
		break;

		case 1:
		if (pin>= 32)
		GPIO_SET_EXT = (1 << (pin % 32));
		else
		GPIO_SET = (1 << pin);
		break;

		default:
		// no change
		break;
	}

} // end void digitalWrite(int pin, int mode)

/****************************************************************************/
/*
gets pin value
pin = gpio pin
returns 0|1
*/
int digitalRead(int pin){

  if (GET_GPIO(pin)) // !=0 <-> bit is 1 <- port is HIGH=3.3V
    return 1;
  else // port is LOW=0V
    return 0;

} // end void digitalRead(int pin)


/****************************************************************************/
int createInterruptFile(int pin, int edge){

	int fdreturned;
	char filename[255] ="";
	char pinval[3]="";
	FILE * fd;

	sprintf(filename, "/sys/class/gpio/gpio%d",pin);
	strcat(filename, "/edge");

	if ((fd = fopen(filename, "r+")) == NULL) {
		fd = fopen("/sys/class/gpio/export", "w");
		sprintf(pinval, "%d",pin);
		fprintf(fd, "%s", pinval);
		fclose(fd);
		sprintf(filename, "/sys/class/gpio/gpio%d", pin);
		strcat(filename, "/edge");
		fd = fopen(filename, "r+");

		switch(edge){
		case 0:	
		fprintf(fd, "falling");
		break;
		case 1:	
		fprintf(fd, "rising");
		break;
		default:
		fprintf(fd, "falling");
		break;
		}

		fclose(fd);

	} else {

		switch(edge){
		case 0:	
		fprintf(fd, "falling");
		break;
		case 1:	
		fprintf(fd, "rising");
		break;
		default:
		fprintf(fd, "falling");
		break;
		}

	fclose(fd);
	}

	sprintf(filename, "/sys/class/gpio/gpio%d", pin);
	strcat(filename, "/value");
	if ((fdreturned = open(filename, O_RDONLY)) < 0) {
		perror("value file: ");
		return -1;
		exit(EXIT_FAILURE);
	} else {
		return fdreturned;
	}

}// end int createInterruptFile(int, int);

/****************************************************************************/
/*
Permit to select the mode we want to the LoRa pin

pin that are connected to the LoRa are in mu
 */
void hardwareSetup(int resetpin){

	// pins init

	pinMode(resetpin, INPUT);

	// modules reset 
	pinMode(resetpin, OUTPUT);
	digitalWrite(resetpin, HIGH);
	usleep(50000);
	pinMode(resetpin, INPUT);
	usleep(50000);


}// end  hardwareSetup(int, int, int, int);

/*
Reset all pin and unselect all mode
 */
void hardwareReset(){


	// pins init

	pinMode(TX_RESETPIN, INPUT);

	// modules reset 
	pinMode(TX_RESETPIN, OUTPUT);
	digitalWrite(TX_RESETPIN, LOW);
	usleep(50000);
	pinMode(TX_RESETPIN, INPUT);
	usleep(50000);


	// pins init

	pinMode(RX_RESETPIN, INPUT);

	// modules reset 
	pinMode(RX_RESETPIN, OUTPUT);
	digitalWrite(RX_RESETPIN, LOW);
	usleep(50000);
	pinMode(RX_RESETPIN, INPUT);
	usleep(50000);

}

/****************************************************************************/
/*
Write to a register using the SPI librairies
 */
void writeRegister(__u8 address, __u8 data) {

	bitSet(address, 7);     // Bit 7 set to write in registers
	
	char buffer[2];
	buffer[0]= address;
	buffer[1]= data;

	spi.transfer(SPI0, buffer, 2);

	
}// end writeRegister()


/****************************************************************************/

__u8 readRegister(__u8 address) {

	bitClear(address, 7);   // Bit 7 cleared to read from registers
	
	char buffer[2];
	buffer[0]= (char) address;
	buffer[1]= (char) 0x55;

	spi.transfer(SPI0, buffer, 2);
	return (__u8) buffer[1];
	

}// end readRegister()



/****************************************************************************/


/*
Init the LoRa module

mode permit to diffentiate some register and value according to witch mode we want
 */
void initModule(int mode) {

	// module in lora mode
	writeRegister(REG_OP_MODE, FSK_SLEEP_MODE); // to change  bit 7!!
	writeRegister(REG_OP_MODE, LORA_SLEEP_MODE);
	writeRegister(REG_OP_MODE, LORA_STANDBY_MODE); // to fill-in the fifo
	usleep(100 * 1000);

	writeRegister(REG_FIFO, 0x00);


	if (mode == TX_MODE){
		int pout = 0;
		writeRegister(REG_PA_CONFIG, 0x80 | pout);
	}else{
		writeRegister(REG_PA_CONFIG, 0x01); // out=RFIO, Pout=0dBm
	}

	writeRegister(REG_PA_RAMP, 0x19); // low cons PLL TX&RX, 40us

	writeRegister(REG_OCP, 0b00101011); //OCP enabled, 100mA

	writeRegister(REG_LNA, 0b00100011); // max gain, BOOST on

	writeRegister(REG_FIFO_ADDR_PTR, 0x00); // pointeur pour l'accès à la FIFO via SPI (read or write)
	writeRegister(REG_FIFO_TX_BASE_ADDR, 0x80); // top half
	writeRegister(REG_FIFO_RX_BASE_ADDR, 0x00); // bottom half

	writeRegister(REG_IRQ_FLAGS_MASK, 0x00);  // toutes IRQ actives

	writeRegister(REG_IRQ_FLAGS, 0xff);  // RAZ de toutes IRQ

	// en mode Explicit Header, CRC enable ou disable n'a aucune importance pour RX, tout dépend de la config TX
	//writeRegister(REG_MODEM_CONFIG1, 0b10001000); //BW=500k, CR=4/5, explicit header, CRC disable, LDRO disabled
	//writeRegister(REG_MODEM_CONFIG1, 0b00001001); //BW=125k, CR=4/5, explicit header, CRC disable, LDRO enabled
	//writeRegister(REG_MODEM_CONFIG1, 0b00100001); //BW=125k, CR=4/8, explicit header, CRC disable, LDRO enabled
	writeRegister(REG_MODEM_CONFIG1, 0b00100011); //BW=125k, CR=4/8, explicit header, CRC enable, LDRO enabled
	//writeRegister(REG_MODEM_CONFIG1, 0b10001010); //BW=500k, CR=4/5, explicit header, CRC enable, LDRO disabled

	writeRegister(REG_MODEM_CONFIG2, 0b11000111); // SF=12, normal TX mode, AGC auto on, RX timeout MSB = 11

	writeRegister(REG_SYMB_TIMEOUT_LSB, 0xff);  // max timeout

	writeRegister(REG_PREAMBLE_MSB_LORA, 0x00); // default value
	writeRegister(REG_PREAMBLE_LSB_LORA, 0x08);

	writeRegister(REG_MAX_PAYLOAD_LENGTH, 0x80); // half the FIFO

	writeRegister(REG_HOP_PERIOD, 0x00); // freq hopping disabled

	writeRegister(REG_DETECT_OPTIMIZE, 0xc3); // pour SF=12

	writeRegister(REG_INVERT_IQ, 0x27); // default value, IQ not inverted

	writeRegister(REG_DETECTION_THRESHOLD, 0x0A); // pour SF=12

	writeRegister(REG_SYNC_WORD, 0x12);   // default value
}

/****************************************************************************/
/*
 Function that define an 32 bit value containing REG_FRF_MSB, REG_FRF_MID & REG_FRF_LSB 
 */
void setChanHex(void){

	// sets the 32 bit value containing the frequency value (REG_FRF_MSB, REG_FRF_MID & REG_FRF_LSB)
	chanHex[0] = 0xD84CCC; // channel 10, freq = 865.20MHz
	chanHex[1] = 0xD86000; // channel 11, freq = 865.50MHz
	chanHex[2] = 0xD87333; // channel 12, freq = 865.80MHz
	chanHex[3] = 0xD88666; // channel 13, freq = 866.10MHz
	chanHex[4] = 0xD89999; // channel 14, freq = 866.40MHz
	// Pour le projet on utilise ce channel
	chanHex[5] = 0xD8ACCC; // channel 15, freq = 866.70MHz
	chanHex[6] = 0xD8C000; // channel 16, freq = 867.00MHz
	chanHex[7] = 0xD90000; // channel 16, freq = 868.00MHz

}

/****************************************************************************/

void setLoraChannel(int channel){

  writeRegister(REG_FRF_LSB, chanHex[channel] & 0x000000ff);

  writeRegister(REG_FRF_MID, (chanHex[channel] >> 8) & 0x000000ff);
  
  writeRegister(REG_FRF_MSB, (chanHex[channel] >> 16) & 0x000000ff);
}

/****************************************************************************/

__u32 getLoraChannel(void){
  __u32 channel;

  channel = readRegister(REG_FRF_MSB)*65536 + readRegister(REG_FRF_MID)*256 + readRegister(REG_FRF_LSB);
  return channel;
}

/****************************************************************************/

void printLoraChannelToStdout(__u32 channel){
	switch(channel){
		case 0xD84CCC:
			fprintf(stdout,"channel 10, freq = 865.20MHz");
			break;
		case 0xD86000:
			fprintf(stdout,"channel 11, freq = 865.50MHz");
			break;
		case 0xD87333:
			fprintf(stdout,"channel 12, freq = 865.80MHz");
			break;
		case 0xD88666:
			fprintf(stdout,"channel 13, freq = 866.10MHz");
			break;
		case 0xD89999:
			fprintf(stdout,"channel 14, freq = 866.40MHz");
			break;
		case 0xD8ACCC:
			fprintf(stdout,"channel 15, freq = 866.70MHz");
			break;
		case 0xD8C000:
			fprintf(stdout,"channel 16, freq = 867.00MHz");
			break;
		case 0xD90000:
			fprintf(stdout,"channel 17, freq = 868.00MHz");
			break;
		default:
			fprintf(stdout,"!ERROR! unknown channel");
			break;
	}
	fprintf(stdout,"\n");

}

/****************************************************************************/

void printLoraChannelToFile(__u32 channel, FILE * fd){
		switch(channel){
			case 0xD84CCC:
				fprintf(fd,"channel 10, freq = 865.20MHz");
				break;
			case 0xD86000:
				fprintf(fd,"channel 11, freq = 865.50MHz");
				break;
			case 0xD87333:
				fprintf(fd,"channel 12, freq = 865.80MHz");
				break;
			case 0xD88666:
				fprintf(fd,"channel 13, freq = 866.10MHz");
				break;
			case 0xD89999:
				fprintf(fd,"channel 14, freq = 866.40MHz");
				break;
			case 0xD8ACCC:
				fprintf(fd,"channel 15, freq = 866.70MHz");
				break;
			case 0xD8C000:
				fprintf(fd,"channel 16, freq = 867.00MHz");
				break;
			case 0xD90000:
				fprintf(fd,"channel 17, freq = 868.00MHz");
				break;
			default:
				fprintf(fd,"!ERROR! unknown channel");
				break;
		}

}

/****************************************************************************/

void checkMessageLoRa(int rxindex){

	int j;
	int msgok = 1; // message received by default
	struct timeval tstart, tcurr;
	long delta;
	__u8 regVal;
	__u8 reg_val_table[255] = {0};
	int rssi;
	int nb_rec_bytes;

	char filename[255] ="";

	regVal = readRegister(REG_IRQ_FLAGS); // reads interruption flags
	
	
	if(bitRead(regVal, 4) == 0x01){ // here, we have a valid header

		lastMessageIndex[rxindex]++;
		
		//Open the log file & index file of current channel
		sprintf(filename, "/tmp/channel%2d.log",10+rxindex);
			if ((fd_log = fopen(filename, "a+")) < 0) {
				perror("log file:");
				exit(EXIT_FAILURE);
			}
		
		sprintf(filename, "/tmp/channel%2d.index",10+rxindex);
			if ((fd_index = fopen(filename, "w+")) < 0) {
				perror("index file:");
				exit(EXIT_FAILURE);
			}

		fprintf(fd_index,"%09u\n", lastMessageIndex[rxindex]);

		fclose(fd_index);

		fprintf(fd_log,"%09d: Valid header received on ", lastMessageIndex[rxindex]);
		printLoraChannelToFile(getLoraChannel(), fd_log);
		fprintf(fd_log,"\nTime stamp: ");	

		// reads and prints the time at which the event occured
		//get_date(fd_log);

			// Step 5a: waits for RXdone before timeout
		if (!bitRead(regVal, 6)){
			while ((!bitRead(regVal,6)) && (!bitRead(regVal,6))){
				regVal = readRegister(REG_IRQ_FLAGS);
			}
		}

		if (!bitRead(regVal,6)) msgok=0;	// no message before timeout


		if (msgok){
			fprintf(fd_log,"******************   Message received on %d : %d  ******************\n",10 +rxindex,lastMessageIndex[rxindex]);
			fprintf(stdout,"******************   Message received on %d : %d  ******************\n",10 +rxindex,lastMessageIndex[rxindex]);
	
			// Step 5b: test CRC

			if (bitRead(regVal, 5)){
				fprintf(fd_log,"|------[+] !WARNING! Payload CRC error\n");
				fprintf(stdout,"|------[+] !WARNING! Payload CRC error\n");
			} else {
				fprintf(fd_log,"|------[+] Payload CRC OK (INFO: test relevance depends on transmitter configuration)\n");
				fprintf(stdout,"|------[+] Payload CRC OK (INFO: test relevance depends on transmitter configuration)\n");
			}

			nb_rec_bytes= readRegister(REG_RX_NB_BYTES);

			fprintf(fd_log,"|------[+] Number of bytes received = %d\n", nb_rec_bytes);
			fprintf(stdout,"|------[+] Number of bytes received = %d\n", nb_rec_bytes);

			// step 6: get data
			writeRegister(REG_FIFO_ADDR_PTR, readRegister(REG_FIFO_RX_CURRENT_ADDR)); //REG_FIFO_RX_CURRENT_ADDR -> FifoAddrPtr
			
			for (j = 0; j < nb_rec_bytes; j++){
				reg_val_table[j] = readRegister(REG_FIFO);
			} // end for (j = 0; j < nb_rec_bytes; j++)

			fprintf(fd_log, "|------[+] Message as hexa:");
			fprintf(stdout, "|------[+] Message as hexa:");
			for (j = 0; j < nb_rec_bytes; j++){
				fprintf(fd_log, " %x |", reg_val_table[j]);
				fprintf(stdout, " %x |", reg_val_table[j]);
			} // end for (j = 0; j < nb_rec_bytes; j++)

			fprintf(fd_log, "\n");
			fprintf(stdout, "\n");
			
			__u8 * msgTx = (__u8 *) malloc(BUFLEN);
			client.sendToServer(reg_val_table,msgTx); // send a socket message
			sendTxMessage(msgTx);

			fprintf(fd_log, "\n");
			fprintf(stdout, "\n");
		} else {
			fprintf(fd_log,"[+] ERROR: Valid header received but no message received before %d microseconds timeout\n", rxTimeout);
		}// end if (msgok)

		
		fclose(fd_log);
		
		writeRegister(REG_IRQ_FLAGS, 0xff);  //  IRQ reset for this receiver anyway
	} // end if(bitRead(reg_val, 4) == 0x01) 

}// end getReceiver2(int rxindex, int rxTimeout)


/****************************************************************************/
/*
select mode (Tx ou Rx)
0 -> Rx
1 -> Tx */
void selectLoRaMode(int mode){
	hardwareReset();

	switch (mode){
		case RX_MODE:
			// modules reset 
			hardwareSetup(RX_RESETPIN);	// RX_RESETPIN as input and resets the module

			//initModule(RX_MODE); // initialize  modules (TX & RX) but NOT the channel
			setLoraChannel(RX_CHANNEL-10); // initialize  the channel
	
			rxConfig();
			printLoraChannelToStdout(getLoraChannel());
			fprintf(stdout,"\n");
			break;
		case TX_MODE:
			// modules reset 
			hardwareSetup(TX_RESETPIN);	// TX_RESETPIN as input and resets the module

			initModule(TX_MODE);
			setLoraChannel(TX_CHANNEL - 10); // initialize  the channel 

			fprintf(stdout,"\n******************   SENDING MESSAGE   ******************");
			fprintf(stdout,"\n|------[+] TX configuration for ");printLoraChannelToStdout(getLoraChannel());
			break;
		
		default:
			break;
	}
}
void rxConfig(void){
	
	fprintf(stdout,"[+] Configuration for RX mode... ");
	
	// initModule() unmasks ALL interruptions by default
	/*
		writeRegister(REG_DIO_MAPPING1, 0x01); // DIO0=RxDone, DIO1=RxTimeout, DIO2=FhssChangeChannel, DIO3=ValidHeader
		writeRegister(REG_DIO_MAPPING2, 0x00); // default values

		writeRegister(REG_IRQ_FLAGS_MASK, 0x8f);  // only Rxdone, CRC error , valid header enabled
	*/

	writeRegister(REG_IRQ_FLAGS, 0xff);  // clears all IRQs

	// datasheet step 1: FifoRxBaseDddress -> FifoAddrPtr
	writeRegister(REG_FIFO_ADDR_PTR, readRegister(REG_FIFO_RX_BASE_ADDR));

	//datasheet Step2: done in init

	//datasheet Step3: RX continuous mode
	writeRegister(REG_OP_MODE, LORA_RX_CONTINUOUS_MODE);
	usleep(100 * 1000); // delay necessary to start oscillator & PLL  (SLEEP -> other mode)

	fprintf(stdout,"... done\n");

	fprintf(stdout,"[+] Configuration completed, waiting for messages on : ");
}

void sendTxMessage(__u8 message[]) {
	sleep(WAITING_CHANGE_MODE);
	
	selectLoRaMode(TX_MODE);
	
	int payloadLength = 13;

	// datasheet step 1: loads the FIFO
	writeRegister(REG_FIFO_ADDR_PTR, readRegister(REG_FIFO_TX_BASE_ADDR)); //FifoTxBaseDddress -> FifoAddrPtr
	writeRegister(REG_PAYLOAD_LENGTH_LORA, payloadLength); // defines payload length

	for (int i=0; i<payloadLength; i++){
		writeRegister(REG_FIFO, message[i]);
	}

	fprintf(stdout,"|------[+] Configuration completed, transmitting message...\n");


	//datasheet Step2: TX  mode
	writeRegister(REG_OP_MODE, LORA_TX_MODE);
	usleep(100000); // delay necessary to start oscillator & PLL  (SLEEP -> other mode)



	//datasheet Step3: wait for TX completion
	int status;
	while((status = bitRead(readRegister(REG_IRQ_FLAGS),3)) == 0){
	//	fprintf(stdout,"reg_irq_flag = 0x%02x\n", status);
	}
	// reset of interruption
	writeRegister(REG_IRQ_FLAGS, 0xff);

	fprintf(stdout,"|------[+] TX completed\n\n");

	selectLoRaMode(RX_MODE); //On retourne en Rx mode
}