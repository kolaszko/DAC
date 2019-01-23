#ifndef _MATH_UTILS_H
#define _MATH_UTILS_H

#include "custom_types.h"
#include "std_types.h"

/**
 * @brief Floating point modulo operation. Takes two float32_t numbers.
 * 
 * @param a The nominator float32_t number
 * @param b The denominator float32_t number
 * @return float32_t The result of operation.
 */
float32_t modulo(float32_t a, float32_t b);
/**
 * @brief Generates single trapezoid signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GenerateTrapezoid(parametry_sygnalu_t* syg);
/**
 * @brief Generates single triangular signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GenerateTrojkat(parametry_sygnalu_t* syg);
/**
 * @brief Generates single saw signal sample.
 * 
 * @param parametry_sygnalu_t* syg Signal parameters pointer
 * @return float32_t Calculated sample.
 */
float32_t GeneratePila(parametry_sygnalu_t* syg);
#endif