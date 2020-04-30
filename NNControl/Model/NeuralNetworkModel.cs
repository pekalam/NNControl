using System.Collections.ObjectModel;

namespace NNControl.Model
{
    public class NeuralNetworkModel
    {
        public int NeuronRadius { get; set; } = 10;
        public int LayerXSpaceBetween { get; set; } = 15;
        public int LayerYSpaceBetween { get; set; } = 15;
        public Box Padding { get; } = new Box() {Left = 40, Right = 40, Top = 40, Bottom = 40};

        public string BackgroundColor { get; set; } = "#F00FFF";
        public string NeuronColor { get; set; } = "#2C3E50";
        public string SynapseColor { get; set; } = "#000000";
        public string SynapseLabelColor { get; set; } = "#000000";


        public ObservableCollection<LayerModel> NetworkLayerModels { get; set; } = new ObservableCollection<LayerModel>();
    }
}