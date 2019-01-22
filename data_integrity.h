#ifndef _DATA_INTEGRITY_H
#define _DATA_INTEGRITY_H

#include "custom_types.h"
#include "std_types.h"
#include "uart_io.h"

#include "aduc831.h"

#include <stdio.h>

extern int8_t terminal[30];
extern uint8_t itr;
extern int8_t ready;
extern int8_t type;
extern uint8_t counter;
extern int8_t state;

extern probka_t probka;
extern float32_t probka_napiecie;
extern parametry_sygnalu_t sygnalParam;

int8_t validateParameters();
void getParameters();
void debugParameters();

#endif