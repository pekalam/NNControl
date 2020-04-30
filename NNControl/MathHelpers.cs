using System;

namespace NeuralNetworkControl
{
    internal static class MathHelpers
    {
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float) Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static float Distance((float x1, float y1) p1, (float x2, float y2) p2)
        {
            return Distance(p1.x1, p1.y1, p2.x2, p2.y2);
        }
    }
}