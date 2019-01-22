using SMProject.Utils;

namespace SMProject
{
    public enum SignalType
    {
        [DataFrameRepresentation("T")] Triangle,
        [DataFrameRepresentation("R")] Rectangular,
        [DataFrameRepresentation("P")] SawSignal,
        [DataFrameRepresentation("S")] Sinusoid
    }
}