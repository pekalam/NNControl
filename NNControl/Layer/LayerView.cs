using NNControl.Model;
using NNControl.Network;
using NNControl.Neuron;
using System.Collections.Generic;

namespace NNControl.Layer
{
    public abstract class LayerView
    {
        public List<NeuronView> Neurons = new List<NeuronView>();
        public List<NeuronView> HighlightedNeurons = new List<NeuronView>();
        public LayerModel LayerModel;
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        public LayerView PreviousLayer;
        public NeuralNetworkView Network;
        public int Number;

        public abstract NeuronView CreateNeuronInstance();
        public abstract void OnRepositioned();
        public abstract void OnZoomChanged();
    }
}