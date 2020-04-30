using System.Collections.ObjectModel;
using NeuralNetworkControl.Abstraction;

namespace NeuralNetworkControl.Model
{
    public class LayerModel
    {
        public ObservableRangeCollection<NeuronModel> NeuronModels { get; set; } = new ObservableRangeCollection<NeuronModel>();
    }
}