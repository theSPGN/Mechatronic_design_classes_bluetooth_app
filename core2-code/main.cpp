// Bluetooth hc-05 connection code:
// https://github.com/husarion/hFramework/blob/master/examples/core2/Serial_serial_config.cpp

// Wiring diagrams (hSens3)
// https://husarion.com/manuals/core2/#hardware-hext
#include <hFramework.h>

// connect hExt UART TX pin to hSens3 UART RX pin

void hMain()
{
	char received_data[20];
	char data_to_send[] = {"example"};
	sys.setSysLogDev(&devNull);								// turn off system logs on Serial
	sys.setLogDev(&Serial);									// setting USB-serial as a default printf output
	hExt.serial.init(19200, Parity::None, StopBits::One);	// configure hExt serial with baudrate == 19200, none parity and with one stop bit
	hSens3.selectSerial();									// switch hSens3 to serial mode with default settings
	hSens3.serial.init(19200, Parity::None, StopBits::One); // configure hSens3 serial with baudrate == 19200,
	for (;;)
	{
		hExt.serial.write(data_to_send, sizeof(data_to_send));
		if (hSens3.serial.read(received_data, sizeof(data_to_send), 500))
		{
			hLED2.toggle();
			printf("hSens3.serial has received data: %s\r\n", received_data);
		}
		else
		{
			printf("no data received - check connections!\r\n");
		}
		sys.delay(500);
	}
}