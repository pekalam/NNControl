using NNControl.Model;
using NNControl.Network;
using NNControl.Neuron;
using System.Collections.Generic;

namespace NNControl.Layer
{
    public abstract class LayerView
    {
        public List<NeuronView> Neurons = new List<NeuronView>(500);
        public List<NeuronView> HighlightedNeurons = new List<NeuronView>(500);
        public LayerModel LayerModel;
        public float X;
        public float Y;

        public LayerView PreviousLayer;
        public NeuralNetworkView Network;
        public int Number;

        public abstract NeuronView CreateNeuronInstance();
    }
}