using System.Collections.ObjectModel;
using NeuralNetworkControl.Abstraction;

namespace NeuralNetworkControl.Model
{
    public class NeuronModel
    {
        public SynapsesLabelsCollection SynapsesLabels { get; internal set; }
    }
}