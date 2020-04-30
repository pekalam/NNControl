using System;

namespace NeuralNetworkControl.Abstraction
{
    internal static class Check
    {
        public static void NotNull(object obj, string exceptionMsg = "")
        {
            if (obj == null)
            {
                throw new NullReferenceException(exceptionMsg);
            }
        }
    }
}