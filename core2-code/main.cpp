// Bluetooth hc-05 connection code:
// https://github.com/husarion/hFramework/blob/master/examples/core2/Serial_serial_config.cpp

// Wiring diagrams (hSens3)
// https://husarion.com/manuals/core2/#hardware-hext

#include <hFramework.h>

int pieces = 7;

// TODO: manual control (verify directions of motors and it's power to control it safety)
void go_forward()
{
	hMot1.setPower(500);
	hMot2.setPower(500);
}

void go_backward()
{
	hMot1.setPower(-500);
	hMot2.setPower(-500);
}

void go_left()
{
	hMot1.setPower(-500);
	hMot2.setPower(500);
}

void go_right()
{
	hMot1.setPower(500);
	hMot2.setPower(-500);
}

void place_domino_piece()
{
	hMot3.rotRel(90, 200, false, 100);
	sys.delay(1000);
	hMot3.rotRel(-90, 200, false, 100);
}

// TODO: automatic building functions (some kind of shape or curve line)
void automatic_build_1(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(180, 500);
		hMot2.rotRel(180, 500, true);
		place_domino_piece();
	}
	busy = false;
}

void automatic_build_2(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(-180, 500);
		hMot2.rotRel(-180, 500, true);
	}
	busy = false;
}

void automatic_build_3(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(100, 500);
		hMot2.rotRel(180, 500, true);
		place_domino_piece();
	}
	busy = false;
}

void automatic_build_4(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(-100, 500);
		hMot2.rotRel(-180, 500, true);
	}
	busy = false;
}

void automatic_build_5(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(180, 500);
		hMot2.rotRel(100, 500, true);
		place_domino_piece();
	}
	busy = false;
}

void automatic_build_6(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		hMot1.rotRel(-180, 500);
		hMot2.rotRel(-100, 500, true);
	}
	busy = false;
}

void hMain()
{
	// creating a variable for messages
	char received_data;

	// creating a variable that will block other functions after fcn callback
	bool busy = false;

	// turn off system logs on Serial
	sys.setSysLogDev(&devNull);

	// setting USB-serial as a default printf output
	sys.setLogDev(&Serial);

	// switch hSens3 to serial mode with default settings
	hSens3.selectSerial();

	// configure hSens3 serial with hc-05 module parameters
	hSens3.serial.init(9600, Parity::None, StopBits::One);

	// configure hMot(1-3) polarity
	hMot1.setMotorPolarity(Polarity::Normal);
	hMot2.setMotorPolarity(Polarity::Normal);
	hMot3.setMotorPolarity(Polarity::Normal);

	// configure hMot(1-3) encoder polarity
	hMot1.setEncoderPolarity(Polarity::Reversed);
	hMot2.setEncoderPolarity(Polarity::Reversed);
	hMot3.setEncoderPolarity(Polarity::Reversed);

	for (;;)
	{
		// signing serial data from hSens3 to the received_data memory cell
		if (hSens3.serial.read(&received_data, sizeof(received_data), 500) && busy == false)
		{
			printf("hSens3.serial has received data: %s\r\n", received_data);

			switch (received_data)
			{
			case 'a':
				go_left();
				break;
			case 'b':
				go_forward();
				break;
			case 'c':
				go_right();
				break;
			case 'd':
				go_backward();
				break;
			case 'x':
				place_domino_piece();
				break;
			case 'q':
				automatic_build_1(busy);
				break;
			case 'w':
				automatic_build_2(busy);
				break;
			case 'e':
				automatic_build_3(busy);
				break;
			case 'r':
				automatic_build_4(busy);
				break;
			case 't':
				automatic_build_5(busy);
				break;
			case 'y':
				automatic_build_6(busy);
				break;
			default:
				break;
			}
		}
		else
		{
			hMot1.stop();
			hMot2.stop();
			printf("no data received - check connections!\r\n");
		}
		sys.delay(10);
	}
}