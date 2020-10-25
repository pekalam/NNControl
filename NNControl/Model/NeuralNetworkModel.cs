using System;
using System.Collections.ObjectModel;

namespace NNControl.Model
{
    public class NeuronSettings
    {
        public string Color = "#2C3E50";
        public string InputColor = "#000000";
        public string SelectedColor = "#4C4CFC";
        public string HighlightedColor = "#FFFF00";
        public string LabelColor = "#000000";
    }

    public class SynapseSettings
    {
        public string Color = "#000000";
        public string LabelColor = "#000000";
        public string SelectedColor = "#FFFFFF";
        public string ArrowColor = "#000000";
        public float StrokeWidth = 1.45f;
        public float ArrowLength = 3.2f;
    }

    public class NeuralNetworkModel
    {
        private string _backgroundColor = "#F00FFF";

        public int NeuronRadius = 10;
        public int LayerXSpaceBetween = 25;
        public int LayerYSpaceBetween = 15;
        public Box Padding = new Box() {Left = 40, Right = 40, Top = 40, Bottom = 40};

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                BackgroundColorChanged?.Invoke(value);
            }
        }

        public NeuronSettings NeuronSettings = new NeuronSettings();
        public SynapseSettings SynapseSettings = new SynapseSettings();


        public ObservableCollection<LayerModel> NetworkLayerModels = new ObservableCollection<LayerModel>();
  

        public event Action<string> BackgroundColorChanged;
    }
}