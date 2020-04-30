using System.ComponentModel;
using NeuralNetworkControl.Model;

namespace NeuralNetworkControl.Adapter
{
    public interface ILayerModelAdapter : INotifyPropertyChanged
    {
        LayerModel LayerModel { get; }
    }

    public class NetworkAdapter
    {
        public INeuralNetworkModelAdapter Impl { get; set; }
    }
}