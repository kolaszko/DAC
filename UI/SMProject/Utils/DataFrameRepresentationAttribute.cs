using System;

namespace SMProject.Utils
{
    public class DataFrameRepresentationAttribute : Attribute
    {
        public DataFrameRepresentationAttribute(string frameContractName)
        {
            FrameContractName = frameContractName;
        }

        public string FrameContractName { get; }
    }
}