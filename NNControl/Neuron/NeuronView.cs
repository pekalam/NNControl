using NNControl.Layer;
using NNControl.Model;
using NNControl.Synapse;
using System.Collections.Generic;

namespace NNControl.Neuron
{
    public abstract class NeuronView
    {
        public float X;
        public float Y;


        public int NumberInLayer;
        public int Number;
        public bool Excluded;
        public readonly List<SynapseView> Synapses = new List<SynapseView>(500);
        public readonly List<SynapseView> ConnectedSynapses = new List<SynapseView>(500);
        public LayerView Layer;
        public NeuronModel NeuronModel;

        public abstract SynapseView CreateSynapseImpl();
        public abstract bool Contains(float x, float y);
        public abstract void OnPositionSet();
        public abstract void OnRepositioned();
        public abstract void ResetColor(string hexColor);
        public abstract void SetColor(int scale);
    }
}