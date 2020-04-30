using NNControl.Neuron;

namespace NNControl.Synapse
{
    public abstract class SynapseViewImpl
    {
        public int NumberInNeuron { get; set; }
        public bool Excluded { get; set; }
        public NeuronViewImpl Neuron1 { get; set; }
        public NeuronViewImpl Neuron2 { get; set; }

        public (float x, float y) ArrowLeftEnd { get; set; }
        public (float x, float y) ArrowRightEnd { get; set; }
        public (float x, float y) ArrowEnd { get; set; }

        public abstract bool Contains(float x, float y);
    }
}