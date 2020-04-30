using NeuralNetworkControl.Abstraction;
using NeuralNetworkControl.Impl;

namespace NeuralNetworkControl
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