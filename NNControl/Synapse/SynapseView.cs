using NNControl.Neuron;

namespace NNControl.Synapse
{
    public abstract class SynapseView
    {
        public int NumberInNeuron { get; set; }
        public bool Excluded { get; set; }
        public NeuronView Neuron1 { get; set; }
        public NeuronView Neuron2 { get; set; }

        public (float x, float y) ArrowLeftEnd { get; set; }
        public (float x, float y) ArrowRightEnd { get; set; }
        public (float x, float y) ArrowEnd { get; set; }
        public (float x, float y) ArrowBeg { get; set; }


        public abstract bool Contains(float x, float y);
        public abstract void SetColor(string hexColor);
    }
}