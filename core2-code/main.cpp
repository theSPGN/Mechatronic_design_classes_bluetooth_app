// Bluetooth hc-05 connection code:
// https://github.com/husarion/hFramework/blob/master/examples/core2/Serial_serial_config.cpp

// Wiring diagrams (hSens3)
// https://husarion.com/manuals/core2/#hardware-hext

#include <hFramework.h>
#include <DistanceSensor.h>
#define PIECES 10
#define DISTANCE_BETWEEN 180
#define ROTATE_WHEEL_ANGLE 35
#define ROT_MOVE_ANGLE 50
#define TIME_DIFF 350000
using namespace hModules;

int dist = 100;
// turn on sensor
DistanceSensor sens(hSens6);

void encoder()
{
	while (true)
	{
		Serial.printf("pos1: %d\n", hMot1.getEncoderCnt());
		Serial.printf("pos2: %d\n", hMot2.getEncoderCnt());
		Serial.printf("pos3: %d\n", hMot3.getEncoderCnt());
		Serial.printf("pos4: %d\n", hMot4.getEncoderCnt());
		dist = sens.getDistance();
		Serial.printf("sensor: %d\n", dist);
		sys.delay(100);
	}
}

void go_forward()
{
	hMot2.rotAbs(0, 500, false, INFINITE);
	// hMot1.rotRel(-ROT_MOVE_ANGLE, 1000, true, INFINITE);
	hMot1.setPower(-1000);
}

void go_backward()
{
	hMot2.rotAbs(0, 500, false, INFINITE);
	// hMot1.rotRel(ROT_MOVE_ANGLE, 1000, true, INFINITE);
	hMot1.setPower(1000);
}

void go_left()
{
	hMot2.rotAbs(-ROTATE_WHEEL_ANGLE, 500, false, INFINITE);
	// hMot1.rotRel(-ROT_MOVE_ANGLE, 1000, true, INFINITE);
	hMot1.setPower(-1000);
}

void go_right()
{
	hMot2.rotAbs(ROTATE_WHEEL_ANGLE, 500, false, INFINITE);
	// hMot1.rotRel(-ROT_MOVE_ANGLE, 1000, true, INFINITE);
	hMot1.setPower(-1000);
}

void place_domino_piece()
{
	hMot3.resetEncoderCnt();
	hMot4.resetEncoderCnt();
	hMot1.setPower(0);
	hMot3.rotAbs(350, 500, true, INFINITE);
	hMot3.rotAbs(0, 1000, true, INFINITE);
	hMot4.rotAbs(600, 400, true, INFINITE);
	hMot4.rotAbs(0, 400, true, INFINITE);
}

void automatic_build_1()
{
	for (int piece = 0; piece < PIECES; piece++)
	{
		dist = sens.getDistance();
		if (dist < 60 && dist != -1)
			break;
		place_domino_piece();
		hMot1.rotRel(-DISTANCE_BETWEEN, 500, true, INFINITE);
	}
}

void automatic_build_2()
{
	hMot1.rotRel(DISTANCE_BETWEEN + 30, 500, true, INFINITE);
}

void automatic_build_3()
{
	hMot2.rotAbs(ROTATE_WHEEL_ANGLE, 500, true, INFINITE);
	automatic_build_1();
	hMot2.rotAbs(0, 500, true, INFINITE);
}

void automatic_build_4()
{
	hMot2.rotAbs(ROTATE_WHEEL_ANGLE, 500, true, INFINITE);
	automatic_build_2();
	hMot2.rotAbs(0, 500, true, INFINITE);
}

void automatic_build_5()
{
	hMot2.rotAbs(-ROTATE_WHEEL_ANGLE, 500, true, INFINITE);
	automatic_build_1();
	hMot2.rotAbs(0, 500, true, INFINITE);
}

void automatic_build_6()
{
	hMot2.rotAbs(-ROTATE_WHEEL_ANGLE, 500, true, INFINITE);
	automatic_build_2();
	hMot2.rotAbs(0, 500, true, INFINITE);
}

void hMain()
{
	// time difference calculate
	int start_ride_time, end_ride_time;

	// encoder monitoring
	// sys.taskCreate(encoder);

	// creating a structures for time storage
	// struct timespec time_now, btn_drive_pressed_time;
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

	// reset encoder count to zero
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
				start_ride_time = sys.getUsTimVal();
				go_left();
				break;
			case 'b':
				start_ride_time = sys.getUsTimVal();
				go_forward();
				break;
			case 'c':
				start_ride_time = sys.getUsTimVal();
				go_right();
				break;
			case 'd':
				start_ride_time = sys.getUsTimVal();
				go_backward();
				break;
			case 'x':
				place_domino_piece();
				break;
			case 'q':
				automatic_build_1();
				break;
			case 'w':
				automatic_build_2();
				break;
			case 'e':
				automatic_build_3();
				break;
			case 'r':
				automatic_build_4();
				break;
			case 't':
				automatic_build_5();
				break;
			case 'y':
				automatic_build_6();
				break;
			default:
				break;
			}
		}
		else
		{
			end_ride_time = sys.getUsTimVal();
			if (end_ride_time - start_ride_time > TIME_DIFF)
				hMot1.setPower(0);
			continue;
		}
	}
}