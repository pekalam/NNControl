using NNControl.Layer.Impl;
using NNControl.Network.Impl;
using NNControl.Synapse;
using NNControl.Synapse.Impl;
using SkiaSharp;

namespace NNControl.Neuron.Impl
{
    internal class SkNeuronView : NeuronView
    {
        private readonly SkNeuronPainter _neuronPainter;

        private float _x;
        private float _y;

        private int Radius => Layer.Network.NeuralNetworkModel.NeuronRadius;

        internal SKRect ReferenceRect;

        public SkNeuronView(SkNeuronPainter neuronPainter)
        {
            _neuronPainter = neuronPainter;
        }

        public override SynapseView CreateSynapseImpl()
        {
            return new SkSynapseView(new SkSynapsePainter(Layer.Network.NeuralNetworkModel.SynapseSettings));
        }

        public override float X
        {
            get => _x;
            set
            {
                _x = value;
                ReferenceRect.Left = X - Radius;
                ReferenceRect.Right = X + Radius;
            }
        }
        public override float Y
        {
            get => _y;
            set
            {
                _y = value;

                ReferenceRect.Top = Y - Radius;
                ReferenceRect.Bottom = Y + Radius;
            }
        }

        public override void OnRepositioned()
        {
            if (Layer.Network.Zoom == 0)
            {
                ReferenceRect.Left = X - Radius;
                ReferenceRect.Right = X + Radius;
                ReferenceRect.Top = Y - Radius;
                ReferenceRect.Bottom = Y + Radius;
            }
        }

        public override void SetColor(string hexColor)
        {
            _neuronPainter.SetColor(SKColor.Parse(hexColor));
        }

        public override void SetColor(int scale)
        {
            _neuronPainter.SetColor(ScaleColorManager.FromScale(scale));
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer)
        {
            _neuronPainter.Draw(network, layer, this);
        }

        public override bool Contains(float x, float y)
        {
            return ReferenceRect.Contains(x, y);
        }
    }
}