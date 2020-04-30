using NNControl.Synapse;

namespace NNControl.Model
{
    public class LayerModel
    {
        public ObservableRangeCollection<NeuronModel> NeuronModels { get; set; } = new ObservableRangeCollection<NeuronModel>();
    }
}