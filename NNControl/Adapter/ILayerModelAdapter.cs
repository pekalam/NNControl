using System.ComponentModel;
using NNControl.Model;

namespace NNControl.Adapter
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