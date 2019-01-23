#ifndef _CUSTOM_TYPES_H
#define _CUSTOM_TYPES_H

#include "std_types.h"

/**
 * @brief Structure representing signal parameters.
 * Okres - singal period interval.
 * Amplituda - signal amplitude.
 * Offset - generating signal offset.
 * t - should not be set, accumulates time for inner calculation
 * delta_t - should not be set, calculated during initialization
 * rosnace - time of signal's rising slope
 * opadajace - time of signal's declining slope.
 * stop - interval time of holding trapezoid amplitude value.
 */
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

/**
 * @brief Union representing sample value in register memory.
 * 
 */
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