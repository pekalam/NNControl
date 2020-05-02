using System.Collections.Generic;
using System.ComponentModel;
using NNControl.Model;
using NNControl.Network;

namespace NNControl.Adapter
{
    public interface INeuralNetworkModelAdapter : INotifyPropertyChanged
    {
        NeuralNetworkModel NeuralNetworkModel { get; }
        IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
        NeuralNetworkController Controller { get; set; }
    }
}