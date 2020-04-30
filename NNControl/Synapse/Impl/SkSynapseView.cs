using System;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;

namespace NNControl.Synapse.Impl
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


            var a = (n2.Y - n1.Y) / (n2.X - n1.X);
            var b = n2.Y - a * n2.X;

            if (Math.Abs(a * x + b - y) < 10)
            {
                return true;
            }

            return false;
        }
    }
}