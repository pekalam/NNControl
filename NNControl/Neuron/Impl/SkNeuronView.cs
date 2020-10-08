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
        private int _scale = -1;

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


        public override void OnPositionSet()
        {
            ReferenceRect.Left = X - Radius;
            ReferenceRect.Right = X + Radius;
            ReferenceRect.Top = Y - Radius;
            ReferenceRect.Bottom = Y + Radius;
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

        public override void ResetColor(string hexColor)
        {
            var col = SKColor.Parse(hexColor);
            _neuronPainter.SetColor(ref col);
            _scale = -1;
        }

        public override void SetColor(int scale)
        {
            if (scale != _scale)
            {
                _neuronPainter.SetColor(ref ScaleColorManager.FromScale(scale));
                _scale = scale;
            }
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SKCanvas canvas)
        {
            _neuronPainter.Draw(network, layer, this, canvas);
        }

        public override bool Contains(float x, float y)
        {
            return ReferenceRect.Contains(x, y);
        }
    }
}