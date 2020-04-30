﻿using System.Collections.ObjectModel;
using NeuralNetworkControl.Abstraction;

namespace NeuralNetworkControl.Model
{
    public class NeuralNetworkModel
    {
        public int NeuronRadius { get; set; } = 10;
        public int LayerXSpaceBetween { get; set; } = 15;
        public int LayerYSpaceBetween { get; set; } = 15;
        public Box Padding { get; } = new Box() {Left = 40, Right = 40, Top = 40, Bottom = 40};

        public string BackgroundColor { get; set; } = "#FFFFFF";
        public string NeuronColor { get; set; } = "#2C3E50";
        public string SynapseColor { get; set; } = "#000000";
        public string SynapseLabelColor { get; set; } = "#000000";


        public ObservableCollection<LayerModel> NetworkLayerModels { get; set; } = new ObservableCollection<LayerModel>();
    }
}