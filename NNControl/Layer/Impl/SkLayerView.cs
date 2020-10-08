using NNControl.Neuron;
using NNControl.Neuron.Impl;

namespace NNControl.Layer.Impl
{
    internal class SkLayerView : LayerView
    {
        public override NeuronView CreateNeuronInstance()
        {
            var neuron = new SkNeuronView(new SkNeuronPainter(Network.NeuralNetworkModel.NeuronSettings));
            return neuron;
        }
    }
}