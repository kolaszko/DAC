#include "data_integrity.h"

int8_t validateParameters()
{
	if(sygnalParam.amplituda + sygnalParam.offset > 5.0) return 2;
	if(sygnalParam.amplituda > 5.0) return 3;
	switch(type)
	{
		case 'p' :
		{
			if(sygnalParam.okres != sygnalParam.rosnace + sygnalParam.opadajace) return 1;
			break;
		}
		case 't' :
		{
			if(sygnalParam.okres != sygnalParam.rosnace + sygnalParam.opadajace) return 1;
			break;
		}
		case 'z' :
		{
			if(sygnalParam.okres != sygnalParam.rosnace + sygnalParam.opadajace + sygnalParam.stop) return 1;
			break;
		}
	}
}


void getParameters()
{
	if(ready == 1)
	{
		TR1 = 0;
		counter = sscanf(terminal, "%c %f %f %f %f %f %f" , &type, &sygnalParam.okres, &sygnalParam.amplituda, &sygnalParam.offset, &sygnalParam.rosnace, &sygnalParam.opadajace, &sygnalParam.stop);
		debugParameters();
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