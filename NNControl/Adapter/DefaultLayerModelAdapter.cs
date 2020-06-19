using NNControl.Model;
using System.ComponentModel;

namespace NNControl.Adapter
{
    public class DefaultLayerModelAdapter: ILayerModelAdapter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LayerModel LayerModel { get; set; }
        public void SetNeuronsCount(int neuronsCount)
        {
            throw new System.NotImplementedException();
        }
    }
}