using NNControl.Model;
using NNControl.Network;
using NNControl.Neuron;
using System.Collections.Generic;

namespace NNControl.Layer
{
    public abstract class LayerView
    {
        public List<NeuronView> Neurons { get; } = new List<NeuronView>();
        public List<NeuronView> HighlightedNeurons { get; } = new List<NeuronView>();
        public LayerModel LayerModel { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        public LayerView PreviousLayer { get; set; }
        public NeuralNetworkView Network { get; set; }
        public int Number { get; set; }

        public abstract NeuronView CreateNeuronInstance();
        public abstract void OnRepositioned();
        public abstract void OnZoomChanged();
    }
}