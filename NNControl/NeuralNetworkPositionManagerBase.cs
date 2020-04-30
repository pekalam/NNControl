using NNControl.Layer;
using NNControl.Neuron;
using NeuralNetworkViewAbstraction = NNControl.Network.NeuralNetworkViewAbstraction;

namespace NNControl
{
    public abstract class NeuralNetworkPositionManagerBase
    {
        public abstract float GetLayerX(NeuralNetworkViewAbstraction network, int layerNum);
        public abstract float GetLayerY(NeuralNetworkViewAbstraction network, int layerNum);
        public abstract float GetNeuronX(NeuralNetworkViewAbstraction network, LayerViewImpl layer,
            NeuronViewImpl neuron);
        public abstract float GetNeuronY(NeuralNetworkViewAbstraction network, LayerViewImpl layer,
            NeuronViewImpl neuron);
        public abstract void InvokeActionsAfterPositionsSet(NeuralNetworkViewAbstraction network);
    }
}