using System.Collections.Generic;
using NeuralNetworkControl.Model;

namespace NeuralNetworkControl.Impl
{
    public abstract class NeuralNetworkViewImpl
    {
        public virtual NeuralNetworkModel NeuralNetworkModel { get; set; }
        public List<LayerViewImpl> Layers { get; } = new List<LayerViewImpl>();
        public List<NeuronViewImpl> SelectedNeuron { get; } = new List<NeuronViewImpl>();
        public virtual double Zoom { get; internal set; } = 0;
        public int NeuronsCount { get; internal set; }
        public SynapseViewImpl SelectedSynapse { get; set; }

        public abstract LayerViewImpl CreateLayerInstance();
    
        public abstract void DrawAndSave();
        public abstract void DrawFromSaved();
        public abstract void DrawExcluded();
    }
}