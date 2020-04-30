using System.ComponentModel;
using NNControl.Model;

namespace NNControl.Adapter
{
    public class DefaultLayerModelAdapter: ILayerModelAdapter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LayerModel LayerModel { get; set; }
    }
}