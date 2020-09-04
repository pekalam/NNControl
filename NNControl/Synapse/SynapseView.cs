using NNControl.Neuron;

namespace NNControl.Synapse
{
    public abstract class SynapseView
    {
        public int NumberInNeuron;
        public bool Excluded;
        public NeuronView Neuron1;
        public NeuronView Neuron2;

        public (float x, float y) ArrowLeftEnd;
        public (float x, float y) ArrowRightEnd;
        public (float x, float y) ArrowEnd;
        public (float x, float y) ArrowBeg;


        public abstract bool Contains(float x, float y);
        public abstract void ResetColor(string hexColor);
        public abstract void SetColor(int scale);
    }
}