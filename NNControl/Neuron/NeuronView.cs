using NNControl.Layer;
using NNControl.Model;
using NNControl.Synapse;
using System.Collections.Generic;

namespace NNControl.Neuron
{
    public abstract class NeuronView
    {
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        
        public int NumberInLayer { get; set; }
        public int Number { get; set; }
        public bool Excluded { get; set; }
        public readonly List<SynapseView> Synapses = new List<SynapseView>();
        public readonly List<SynapseView> ConnectedSynapses = new List<SynapseView>();
        public LayerView Layer { get; set; }
        public NeuronModel NeuronModel { get; set; }

        public abstract SynapseView CreateSynapseImpl();
        public abstract bool Contains(float x, float y);
        public abstract void OnRepositioned();
        public abstract void OnZoomChanged();
        public abstract void SetColor(string hexColor);
        public abstract void SetColor(int scale);
    }
}