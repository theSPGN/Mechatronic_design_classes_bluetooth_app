// Bluetooth hc-05 connection code:
// https://github.com/husarion/hFramework/blob/master/examples/core2/Serial_serial_config.cpp

// Wiring diagrams (hSens3)
// https://husarion.com/manuals/core2/#hardware-hext

#include <hFramework.h>

int pieces = 7;

// TODO: manual control (verify directions of motors and it's power to control it safety)
void go_forward()
{
	hMot2.rotAbs(0, 100, true, INFINITE);
	hMot1.setPower(500);
}

void go_backward()
{
	hMot2.rotAbs(0, 100, true, INFINITE);
	hMot1.setPower(-500);
}

void go_left()
{
	hMot2.rotAbs(90, 100, true, INFINITE);
	hMot1.setPower(500);
}

void go_right()
{
	hMot2.rotAbs(-90, 100, true, INFINITE);
	hMot1.setPower(500);
}

void place_domino_piece()
{
	hMot3.rotAbs(90, 200, true, INFINITE);
	hMot3.rotAbs(-90, 200, true, INFINITE);
	hMot4.rotAbs(90, 200, true, INFINITE);
	hMot4.rotAbs(-90, 200, true, INFINITE);
}

// TODO: automatic building functions (some kind of shape or curve line)
void automatic_build_1(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void automatic_build_2(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void automatic_build_3(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void automatic_build_4(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void automatic_build_5(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void automatic_build_6(bool &busy)
{
	busy = true;
	for (int piece = 0; piece < pieces; piece++)
	{
		continue;
	}
	busy = false;
}

void hMain()
{
	// creating a variable for messages
	char received_data[1];

	// creating a variable that will block other functions after fcn callback
	bool busy = false;

	// turn off system logs on Serial
	sys.setSysLogDev(&devNull);

	// setting USB-serial as a default printf output
	sys.setLogDev(&Serial);

	// configure hMot(1-3) polarity
	hMot1.setMotorPolarity(Polarity::Normal);
	hMot2.setMotorPolarity(Polarity::Normal);
	hMot3.setMotorPolarity(Polarity::Normal);
	hMot4.setMotorPolarity(Polarity::Normal);

	// configure hMot(1-3) encoder polarity
	hMot1.setEncoderPolarity(Polarity::Reversed);
	hMot2.setEncoderPolarity(Polarity::Reversed);
	hMot3.setEncoderPolarity(Polarity::Reversed);
	hMot4.setEncoderPolarity(Polarity::Reversed);

	// Reset encoder count to zero
	hMot1.resetEncoderCnt();
	hMot2.resetEncoderCnt();
	hMot3.resetEncoderCnt();
	hMot4.resetEncoderCnt();

	// configure hSens3 serial with hc-05 module parameters
	hExt.serial.init(9600, Parity::None, StopBits::One);
	for (;;)
	{
		received_data[0] = '0';
		// signing serial data from hSens3 to the received_data memory cell
		if (hExt.serial.read(&received_data, sizeof(received_data), 0) && busy == false)
		{
			printf("%s", received_data);

			switch (received_data[0])
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
			hMot1.setPower(0);
		}
	}
}