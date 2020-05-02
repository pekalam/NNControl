using System.ComponentModel;
using NNControl.Model;

namespace NNControl.Adapter
{
    public interface ILayerModelAdapter : INotifyPropertyChanged
    {
        LayerModel LayerModel { get; }
        void SetNeuronsCount(int neuronsCount);
    }
}