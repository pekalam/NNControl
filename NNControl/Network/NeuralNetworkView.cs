using System.Collections.Generic;
using NNControl.Layer;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;

namespace NNControl.Network
{
    public abstract class NeuralNetworkView
    {
        public virtual NeuralNetworkModel NeuralNetworkModel { get; set; }
        public List<LayerView> Layers { get; } = new List<LayerView>();
        public List<NeuronView> SelectedNeuron { get; } = new List<NeuronView>();
        public List<NeuronView> HighlightedNeurons { get; } = new List<NeuronView>();
        public List<SynapseView> HighlightedSynapses { get; } = new List<SynapseView>();
        public virtual double Zoom { get; internal set; } = 0;
        public int NeuronsCount { get; internal set; }
        public SynapseView SelectedSynapse { get; set; }

        public abstract (float x, float y) ToCanvasPoints(float x, float y);

        public abstract LayerView CreateLayerInstance();
            
        public abstract void DrawAndSave();
        public abstract void DrawFromSaved();
        public abstract void DrawExcluded();
    }
}