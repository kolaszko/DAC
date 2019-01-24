#ifndef _DATA_INTEGRITY_H
#define _DATA_INTEGRITY_H

#include "custom_types.h"
#include "std_types.h"
#include "uart_io.h"

#include "aduc831.h"

#include <stdio.h>

#define OKRES 50

extern int8_t terminal[30];
extern uint8_t itr;
extern int8_t ready;
extern int8_t type;
extern uint8_t counter;
extern int8_t state;

extern probka_t probka;
extern float32_t probka_napiecie;
extern parametry_sygnalu_t sygnalParam;

/**
 * @brief Validates correctness of given parameters.
 * 
 * @return void
 */
void validateParameters();
/**
 * @brief Getting parameters of signal from UART filled terminal. 
 * 
 * @return void
 */
void getParameters();
/**
 * @brief Sending current parameters of signal through UART.
 * 
 * @return void
 */
void debugParameters();
/**
 * @brief Sending current state of program/data correctness through UART.
 * 
 * @return void
 */
void sendState();
/**
 * @brief Setting default parameters of signal.
 * 
 * @return void
 */
void setDefaultParameters();
/**
 * @brief Sending start parameters.
 * 
 * @return void
 */
void sendStartParameters();

#endif