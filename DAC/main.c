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


int8_t terminal[30];
uint8_t itr = 0;
int8_t ready = 0;
int8_t type;
uint8_t counter = 0;
int8_t state = 10;

probka_t probka = {0};
float32_t probka_napiecie = 0;
parametry_sygnalu_t sygnalParam;

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


int main()
{
	//XRAM
	BitSet(CFG831, 0);
	ET1 = 1;
	EA = 1;
	// UART INTERRUPT
	ES = 1;
	DACCON = 0x7F;
	TMOD = 0x10;
	
	
	REN = 1;
	SM0 = 0x00;
	SM1 = 0x01;
	
	//TIMER3 SETTINGS
	T3CON = 0x85;
	T3FD = 0x08;
	
	//INTERRUPT PRIORITY
	PS=1;
	PT1=0;
	
		//DEFAULTS
	type = 'p';
	setDefaultParameters();

	T1_Set(OKRES)
	TR1 = 1;
	while(1)
	{
		getParameters();
	};
	
}