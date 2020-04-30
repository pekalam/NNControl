using System.Collections.Generic;
using System.ComponentModel;
using NNControl.Model;

namespace NNControl.Adapter
{
    public interface INeuralNetworkModelAdapter : INotifyPropertyChanged
    {
        NeuralNetworkModel NeuralNetworkModel { get; }
        IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
    }
}