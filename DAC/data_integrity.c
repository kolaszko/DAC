#include "data_integrity.h"

void validateParameters()
{
	if(sygnalParam.amplituda > 5.0)
	{
		sygnalParam.amplituda = 3.0;
		state = 2;
		return;
	}
	if(sygnalParam.amplituda + sygnalParam.offset > 5.0) 
	{
		sygnalParam.offset = 0;
		state = 3;
		return;
	}
	switch(type)
	{
		case 'p' :
		{
			if(counter != 7)
			{
				state = 4;
				return;
			}
			break;
		}
		case 't' :
		{
			if(sygnalParam.okres != sygnalParam.rosnace + sygnalParam.opadajace)
			{
				state = 1;
				return;
			}
			if(counter != 7)
			{
				state = 4;
				return;
			}
			break;
		}
		case 'z' :
		{
			if(sygnalParam.okres != sygnalParam.rosnace + sygnalParam.opadajace + sygnalParam.stop)
			{
				state = 1;
				return;
			}
			if(counter != 7)
			{
				state = 4;
				return;
			}
			break;
		}
	}
	state = 0;
}


void getParameters()
{
	if(ready == 1)
	{
		int8_t isReset = 0;
		TR1 = 0;
		counter = sscanf(terminal, "%c %f %f %f %f %f %f" , &type, &sygnalParam.okres, &sygnalParam.amplituda, &sygnalParam.offset, &sygnalParam.rosnace, &sygnalParam.opadajace, &sygnalParam.stop);
		sscanf(terminal, "%c", &isReset);
		if(isReset == 'R')
		{
			setDefaultParameters();
		}
		debugParameters();
		validateParameters();
		sendState();
		ready = 0;
		TR1 = 1;
	}
}

void debugParameters()
{
	int8_t info[35];
	sprintf(info, "%c %.2f %.2f %.2f %.2f %.2f %.2f \r\n" , type, sygnalParam.okres, sygnalParam.amplituda, sygnalParam.offset, sygnalParam.rosnace, sygnalParam.opadajace, sygnalParam.stop);
	UART_puts(info);
}

void sendState()
{
	int8_t stateInfo[6];
	sprintf(stateInfo, "%i \r\n", (int)state);
	UART_puts(stateInfo);
}

void setDefaultParameters()
{
	type = 'p';
	sygnalParam.okres = 3.0;
	sygnalParam.amplituda = 3.0;
	sygnalParam.offset = 1;
	sygnalParam.t = 0.0;
	sygnalParam.rosnace = 0.001;
	sygnalParam.opadajace = 0.001;
	sygnalParam.stop = 1.5;
	sygnalParam.delta_t = ((float32_t)OKRES/1000.0);
}