using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeuralNetworkControl.Abstraction;
using NeuralNetworkControl.Model;
using NNLib;

namespace NeuralNetworkControl.Adapter
{
    public interface INeuralNetworkModelAdapter : INotifyPropertyChanged
    {
        NeuralNetworkModel NeuralNetworkModel { get; }
        IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
    }
}