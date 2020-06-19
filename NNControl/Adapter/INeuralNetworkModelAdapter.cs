using NNControl.Model;
using NNControl.Network;
using System.Collections.Generic;
using System.ComponentModel;

namespace NNControl.Adapter
{
    public interface INeuralNetworkModelAdapter : INotifyPropertyChanged
    {
        NeuralNetworkModel NeuralNetworkModel { get; }
        IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
        NeuralNetworkController Controller { get; set; }
    }
}