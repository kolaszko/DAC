#include "uart_io.h"
#include <reg51.h>
char _getKey()
{
	char c;
	while(!RI);
	c = SBUF;
	RI = 0;
	return c;
}

void UART_putchar(char c)
{
	SBUF = c;
	while(!TI){};
	TI = 0;
}
void UART_puts(char * str)
{
	while(*str != '\0')
	{
		UART_putchar(*str);
		str++;
	}
}