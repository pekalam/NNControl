using System;
using NeuralNetworkControl.Impl;

namespace NeuralNetworkControl.SkiaImpl
{
    internal class SkSynapseView : SynapseViewImpl
    {
        public override bool Contains(float x, float y)
        {
            var n2 = Neuron2 as SkNeuronView;
            var n1 = Neuron1 as SkNeuronView;

            if (n1.ReferenceRect.Left <= n2.ReferenceRect.Left)
            {
                if (!(x >= (n1.ReferenceRect.Right) && x <= (n2.ReferenceRect.Left)))
                {
                    if (n1.ReferenceRect.Bottom < n2.ReferenceRect.Top)
                    {
                        if (!(y >= (n1.ReferenceRect.Bottom) && y <= (n2.ReferenceRect.Top)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!(y >= (n2.ReferenceRect.Bottom) && y <= (n1.ReferenceRect.Top)))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (!(x < (n1.ReferenceRect.Right) && x > (n2.ReferenceRect.Left)))
                {
                    if (n1.ReferenceRect.Bottom <= n2.ReferenceRect.Top)
                    {
                        if (!(y > (n1.ReferenceRect.Bottom) && y < (n2.ReferenceRect.Top)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!(y > (n2.ReferenceRect.Bottom) && y < (n1.ReferenceRect.Top)))
                        {
                            return false;
                        }
                    }
                }
            }


            var a = ((n2.Y * (Neuron1.Layer.Network.Zoom + 1)) - (n1.Y * (Neuron1.Layer.Network.Zoom + 1))) / ((n2.X * (Neuron1.Layer.Network.Zoom + 1)) - (n1.X * (Neuron1.Layer.Network.Zoom + 1)));
            var b = (n2.Y * (Neuron1.Layer.Network.Zoom + 1)) - a * (n2.X * (Neuron1.Layer.Network.Zoom + 1));

            if (Math.Abs(a * x + b - y) < 2)
            {
                return true;
            }

            return false;
        }
    }
}