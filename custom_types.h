#ifndef _CUSTOM_TYPES_H
#define _CUSTOM_TYPES_H

#include "std_types.h"

typedef struct
{
	float32_t okres;
	float32_t amplituda;
	float32_t offset;
	float32_t t;
	float32_t delta_t;
	float32_t rosnace;
	float32_t opadajace;
	float32_t stop;
}parametry_sygnalu_t;

typedef union
{
	uint16_t wartosc;
	struct
	{
		uint8_t bajt_gorny;
		uint8_t bajt_dolny;
	}slowo;
}probka_t;

#endif