using NNControl.Layer.Impl;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;
using SkiaSharp;
using System;

namespace NNControl.Synapse.Impl
{
    internal class SkSynapseView : SynapseView
    {
        private readonly SkSynapsePainter _synapsePainter;
        private int _scale = -1;

        public SkSynapseView(SkSynapsePainter synapsePainter)
        {
            _synapsePainter = synapsePainter;
        }

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

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron)
        {
            _synapsePainter.Draw(network, layer, neuron, this);
        }


        public void DrawSynapseLabel(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            SkSynapseView synapse)
        {
            _synapsePainter.DrawSynapseLabel(network, layer, neuron, synapse);
        }

        public override void ResetColor(string hexColor)
        {
            _synapsePainter.SetColor(SKColor.Parse(hexColor));
            _scale = -1;
        }

        public override void SetColor(int scale)
        {
            if (scale != _scale)
            {
                _synapsePainter.SetColor(ScaleColorManager.FromScale(scale));
                _scale = scale;
            }
        }
    }
}