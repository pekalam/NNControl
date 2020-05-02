using System;
using System.Collections.Generic;
using NNControl.Layer.Impl;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;
using SkiaSharp;

namespace NNControl.Synapse.Impl
{
    internal static class ScaleColorManager
    {
        private const int ScaleMax = 256;

        private static SKColor[] _colors = new SKColor[ScaleMax];

        static ScaleColorManager()
        {
            float h = 0;
            float s = 100;
            float v = 100;

            var step = 100f / (ScaleMax / 2f);
            for (int i = 0; i < ScaleMax / 2; i++)
            {
                _colors[i] = SKColor.FromHsv(h,s,v);

                s -= step;
            }

            s = 0;
            h = 232;

            for (int i = ScaleMax / 2; i < ScaleMax; i++)
            {
                _colors[i] = SKColor.FromHsv(h, s, v);

                s += step;
            }
        }

        public static SKColor FromScale(int scale) => _colors[scale];
    }

    internal class SkSynapseView : SynapseView
    {
        private SkSynapsePainter _synapsePainter;

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

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SkSynapseView synapse)
        {
            _synapsePainter.Draw(network, layer, neuron, synapse);
        }


        public void DrawSynapseLabel(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            SkSynapseView synapse)
        {
            _synapsePainter.DrawSynapseLabel(network, layer, neuron, synapse);
        }

        public override void SetColor(string hexColor)
        {
            _synapsePainter.SetColor(SKColor.Parse(hexColor));
        }

        public override void SetColor(int scale)
        {
            _synapsePainter.SetColor(ScaleColorManager.FromScale(scale));
        }
    }
}