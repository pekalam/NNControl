using NNControl.Model;
using System.ComponentModel;

namespace NNControl.Adapter
{
    public interface ILayerModelAdapter : INotifyPropertyChanged
    {
        LayerModel LayerModel { get; }
        void SetNeuronsCount(int neuronsCount);
    }
}