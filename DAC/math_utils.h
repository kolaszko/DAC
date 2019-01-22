#ifndef _MATH_UTILS_H
#define _MATH_UTILS_H

#include "custom_types.h"
#include "std_types.h"


float32_t modulo(float32_t a, float32_t b);
/**
 * @brief Generates single signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GenerateTrapezoid(parametry_sygnalu_t* syg);
/**
 * @brief Generates single signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GenerateTrojkat(parametry_sygnalu_t* syg);
/**
 * @brief Generates single signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GeneratePila(parametry_sygnalu_t* syg);
#endif