using NNControl.Layer;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;
using System.Collections.Generic;

namespace NNControl.Network
{
    public abstract class NeuralNetworkView
    {
        public virtual NeuralNetworkModel NeuralNetworkModel { get; set; }
        public List<LayerView> Layers = new List<LayerView>();
        public List<NeuronView> SelectedNeuron = new List<NeuronView>();
        public virtual float Zoom { get; internal set; } = 0;
        public int NeuronsCount;
        public SynapseView SelectedSynapse;

        public abstract (float x, float y) ToCanvasPoints(float x, float y);

        public abstract LayerView CreateLayerInstance();
            
        public abstract void DrawAndSave();
        public abstract void DrawFromSaved();
        public abstract void DrawExcluded();
        public abstract void DrawDirectly();
    }
}