using SMProject.Utils;

namespace SMProject
{
    public enum SignalType
    {
        [DataFrameRepresentation("t")] Triangle,
        [DataFrameRepresentation("r")] Rectangular,
        [DataFrameRepresentation("p")] SawSignal,
        [DataFrameRepresentation("s")] Sinusoid
    }
}