using System.Collections.ObjectModel;

namespace NNControl.Model
{
    public class NeuronSettings
    {
        public string Color { get; set; } = "#2C3E50";
        public string InputColor { get; set; } = "#000000";
        public string SelectedColor { get; set; } = "#4C4CFC";
    }

    public class SynapseSettings
    {
        public string Color { get; set; } = "#000000";
        public string LabelColor { get; set; } = "#000000";
        public string SelectedColor { get; set; } = "#FFFFFF";
        public string LargetNetColor { get; set; } = "#ffb0e3";
        public string ArrowColor { get; set; } = "#000000";
        public float StrokeWidth { get; set; } = 1.45f;
        public float ArrowLength { get; set; } = 3.2f;
    }

    public class NeuralNetworkModel
    {
        public int NeuronRadius { get; set; } = 10;
        public int LayerXSpaceBetween { get; set; } = 15;
        public int LayerYSpaceBetween { get; set; } = 15;
        public Box Padding { get; } = new Box() {Left = 40, Right = 40, Top = 40, Bottom = 40};

        public string BackgroundColor { get; set; } = "#F00FFF";

        public NeuronSettings NeuronSettings { get; set; } = new NeuronSettings();
        public SynapseSettings SynapseSettings { get; set; } = new SynapseSettings();


        public ObservableCollection<LayerModel> NetworkLayerModels { get; set; } = new ObservableCollection<LayerModel>();
    }
}