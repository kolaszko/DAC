using SMProject.Utils;

namespace SMProject
{
    public enum SignalType
    {
        [DataFrameRepresentation("t")] Triangle,
        [DataFrameRepresentation("z")] Trapezoidal,
        [DataFrameRepresentation("p")] SawSignal
    }
}