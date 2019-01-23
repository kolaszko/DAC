#include "libs.h"
#include "aduc831.h"

#define ca_Vref 5.0
#define ca_Resolution 12
#define ca_Maximum_Value ((0x000001ul<<ca_Resolution)-1)
#define F_OSC 11058000
#define pars 12
#define t_resol 16

#define Tx_Tau(dzielnik) (float)((float)dzielnik/F_OSC)
#define Tx_N(czas_ms, dzielnik) (unsigned int)((float)czas_ms/Tx_Tau(dzielnik)/1000.0)
#define T1_Rejestr(czas_ms) ((0x000001ul<<t_resol)-Tx_N(czas_ms,pars))
#define T1_Set(czas_ms) TL1 = T1_Rejestr(czas_ms);TH1 = T1_Rejestr(czas_ms)>>8;

/*! 
@var int8_t terminal[30] 
@brief Input from user.  
*/
int8_t terminal[30];
/*! 
@var uint8_t itr 
@brief Iterator used in UART Interrupt.  
*/
uint8_t itr = 0;
/*! 
@var int8_t ready 
@brief Flag setted by UART Interrupt when input from user was read. 
*/
int8_t ready = 0;
/*! 
@var int8_t type 
@brief Represents currently emitted signal. 
 * p - saw
 * t - triangle
 * z - trapezoid
 * 
*/
int8_t type;
/*! 
@var uint8_t counter 
@brief Counter setted by sscanf function during reading signal parameters. 
*/
uint8_t counter = 0;
/*! 
@var int8_t state 
@brief Represents current status. 
 * 0 - correct working of application
 * 1 - error: problem with given time variables
 * 2 - error: problem with given amplitude
 * 3 - error: problem with given offset
 * 4 - error: bad argument count
 * 
*/
int8_t state = 10;

probka_t probka = {0};
float32_t probka_napiecie = 0;
parametry_sygnalu_t sygnalParam;

/**
 * @brief UART interrupt to fill terminal.
 *
 * line of data specified as:
 * char type of plot, float amplitude, float offset
 * float rosnace, float opadajace, float stop '!' . Formatted without ',' delimiter
 */
void uartInterrupt () interrupt 4
{
	char buffer;
	if(RI)
	{
		buffer = _getKey();
		if(buffer == '!')
		{			
			itr = 0;
			ready = 1;
		}
		else if(itr > 29)
		{
			itr = 0;
		}
		else
		{
			terminal[itr] = buffer;
			itr++;
		}
		
		RI=0;
	}
	return;
}

/**
 * @brief Timer interrupt function.
 * Generates signal samples and instructs analog outputs to emit signal.
 *
 */
void timer1() interrupt 3
{
	T1_Set(OKRES);
	sygnalParam.t += sygnalParam.delta_t;
	if(sygnalParam.t > sygnalParam.okres)sygnalParam.t = sygnalParam.delta_t;
	switch (type){
		case 'p':
		{
			probka_napiecie = GeneratePila(&sygnalParam);
			probka_napiecie = (probka_napiecie>ca_Vref)? ca_Vref : probka_napiecie;
			probka_napiecie = (probka_napiecie < 0)? 0: probka_napiecie;
			probka.wartosc = (uint16_t)(probka_napiecie/ca_Vref * (float32_t)ca_Maximum_Value);
			break;
		}
		case 't':
		{
			probka_napiecie = GenerateTrojkat(&sygnalParam);
			probka_napiecie = (probka_napiecie>ca_Vref)? ca_Vref : probka_napiecie;
			probka.wartosc = (uint16_t)(probka_napiecie* (1.0 / (1.0 * ca_Vref ))* (float32_t)ca_Maximum_Value);
			break;
		}
		case 'z':
		{
			probka_napiecie = GenerateTrapezoid(&sygnalParam);
			probka_napiecie = (probka_napiecie>ca_Vref)? ca_Vref : probka_napiecie;
			probka.wartosc = (uint16_t)(probka_napiecie* (1.0 / (1.0 * ca_Vref ))* (float32_t)ca_Maximum_Value);
			break;
		}
	}
	DAC0H = probka.slowo.bajt_gorny;
	DAC0L = probka.slowo.bajt_dolny;
}

/**
 * @brief Program entry point.
 * 
 * @return int
 */
int main()
{
	// XRAM AVAILABLE
	BitSet(CFG831, 0);
	// INTERRUPTS FLAGS
	ET1 = 1;
	EA = 1;
	// UART INTERRUPT
	ES = 1;
	
	// DAC register configuration, all values are set short of MODE = 0
	// which sets the DAC into 12bits mode.
	// Mode = 0; RNGx = 11; CLRx = 11; PDx = 11;
	DACCON = 0x7F;
	//TIMER 1 SETTING
	TMOD = 0x10;

	REN = 1;
	SM0 = 0x00;
	SM1 = 0x01;
	
	// TIMER3 SETTINGS - BAUDRATE 9600
	T3CON = 0x85;
	T3FD = 0x08;
	
	//INTERRUPT PRIORITY
	PS=1;
	PT1=0;
	
	//DEFAULTS OF SIGNAL
	setDefaultParameters();

	T1_Set(OKRES)
	TR1 = 1;
	while(1)
	{
		getParameters();
	};
	
}