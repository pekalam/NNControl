using NNControl.Network.Impl;
using NNControl.Synapse;
using NNControl.Synapse.Impl;
using SkiaSharp;

namespace NNControl.Neuron.Impl
{
    internal class SkNeuronView : NeuronViewImpl
    {
        private float _x;
        private float _y;

        private int Radius => Layer.Network.NeuralNetworkModel.NeuronRadius;

        public override float X
        {
            get => _x;
            set
            {
                _x = value;
                ReferenceRect.Left = (X - Radius);
                ReferenceRect.Right = (X + Radius);
            }
        }
        public override float Y
        {
            get => _y;
            set
            {
                _y = value;

                ReferenceRect.Top = (Y - Radius);
                ReferenceRect.Bottom = (Y + Radius);
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

        public override void OnZoomChanged()
        {
        }

        public override bool Contains(float x, float y)
        {
            return ReferenceRect.Contains(x, y);
        }

        public override SynapseViewImpl CreateSynapseImpl()
        {
            return new SkSynapseView();
        }

        internal SKRect ReferenceRect;
    }
}