#include "math_utils.h"

float32_t modulo(float32_t a, float32_t b)
{
	int16_t result = (int16_t)(a/b);
	return  a - (float32_t)(result ) *b;
}

float32_t GenerateTrapezoid(parametry_sygnalu_t* syg)
{
	// Setting values of signal in fixed declaration outside of given reference, in order to save CODE memory.
	float32_t time, elapsedInterval,result;
	time = modulo(syg->t,syg->okres);
    
	if(time > syg->rosnace + syg->stop )
	{
        elapsedInterval = syg->rosnace + syg->stop;
		result = -syg->amplituda  * 1.0 / syg->opadajace *(time - elapsedInterval) + syg->amplituda + syg->offset;
        
	}
    else if(time < syg->rosnace)
    {
        result = syg->amplituda*time/(syg->rosnace) + syg->offset;
        
    }
    else
    {
        result = syg->amplituda + syg->offset;
    }
    
    return result;
}

float32_t GenerateTrojkat(parametry_sygnalu_t* syg)
{
	// Setting values of signal in fixed declaration outside of given reference, in order to save CODE memory.
	float32_t time, result;
	time = modulo(syg->t,syg->okres);
	if(time > syg->rosnace )
	{
		result = -syg->amplituda  * 1.0 / (syg->opadajace) *(time - syg->rosnace) + syg->amplituda + syg->offset;
        return result;
	}
	else
	{
		result =   syg->amplituda*time/(syg->rosnace) + syg->offset;
	}
		
	
    return result;
}

float32_t GeneratePila(parametry_sygnalu_t* syg)
{
	return syg->amplituda*(modulo(syg->t, (syg->okres)))/(syg->okres) + syg->offset;
}