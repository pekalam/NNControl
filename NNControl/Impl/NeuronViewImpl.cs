using System.Collections.Generic;
using NeuralNetworkControl.Model;

namespace NeuralNetworkControl.Impl
{
    public abstract class NeuronViewImpl
    {
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        
        public int NumberInLayer { get; set; }
        public int Number { get; set; }
        public bool Excluded { get; set; }
        public readonly List<SynapseViewImpl> Synapses = new List<SynapseViewImpl>();
        public readonly List<SynapseViewImpl> ConnectedSynapses = new List<SynapseViewImpl>();
        public LayerViewImpl Layer { get; set; }
        public NeuronModel NeuronModel { get; set; }

        public abstract SynapseViewImpl CreateSynapseImpl();
        public abstract bool Contains(float x, float y);
        public abstract void OnRepositioned();
        public abstract void OnZoomChanged();
    }
}