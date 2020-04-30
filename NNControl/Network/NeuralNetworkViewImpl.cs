using System.Collections.Generic;
using NNControl.Layer;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;

namespace NNControl.Network
{
    public abstract class NeuralNetworkViewImpl
    {
        public virtual NeuralNetworkModel NeuralNetworkModel { get; set; }
        public List<LayerViewImpl> Layers { get; } = new List<LayerViewImpl>();
        public List<NeuronViewImpl> SelectedNeuron { get; } = new List<NeuronViewImpl>();
        public virtual double Zoom { get; internal set; } = 0;
        public int NeuronsCount { get; internal set; }
        public SynapseViewImpl SelectedSynapse { get; set; }

        public abstract (float x, float y) ToCanvasPoints(float x, float y);

        public abstract LayerViewImpl CreateLayerInstance();
    
        public abstract void DrawAndSave();
        public abstract void DrawFromSaved();
        public abstract void DrawExcluded();
    }
}