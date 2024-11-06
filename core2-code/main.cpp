#include <hFramework.h>

void hMain()
{
	// Set the log device to the default serial port
	sys.setLogDev(&Serial);

	// Initialize motor H1
	hMot1.setPower(0); // Ensure motor is off initially

	while (true)
	{
		// Check if data is available on the primary serial port
		if (Serial.available())
		{
			// Read the data from the serial port
			char data = Serial.getch();

			// Print the data to the serial monitor
			Serial.printf("Received: %c\n", data);

			// Check if the received data is 'a'
			if (data == 'a')
			{
				// Turn on motor H1
				hMot1.setPower(500); // Set motor power (adjust as needed)
			}
			else
			{
				// Turn off motor H1 if any other character is received
				hMot1.setPower(0);
			}
		}
		sys.delay(100);
	}
}
