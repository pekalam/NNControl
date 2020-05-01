using NNControl.Layer;
using NNControl.Network;
using NNControl.Neuron;

namespace NNControl
{
    public abstract class NeuralNetworkPositionManagerBase
    {
        public abstract float GetLayerX(NeuralNetworkController network, int layerNum);
        public abstract float GetLayerY(NeuralNetworkController network, int layerNum);
        public abstract float GetNeuronX(NeuralNetworkController network, LayerView layer,
            NeuronView neuron);
        public abstract float GetNeuronY(NeuralNetworkController network, LayerView layer,
            NeuronView neuron);
        public abstract void InvokeActionsAfterPositionsSet(NeuralNetworkController network);
    }
}