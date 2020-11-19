using NNControl.Adapter;
using NNControl.Model;
using NNLib;
using System.ComponentModel;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace NNLibAdapter
{
    public class NNLibLayerAdapter : ILayerModelAdapter
    {
        public NNLibLayerAdapter(LayerModel layerModel, Layer? layer)
        {
            LayerModel = layerModel;
            Layer = layer;
            if (layer != null)
            {
                layer.NeuronsCountChanged += LayerOnNeuronsCountChanged;
            }
        }

        private void LayerOnNeuronsCountChanged(Layer obj)
        {
            SetNeuronsCount(obj.NeuronsCount);
        }

        public LayerModel LayerModel { get; }
        public Layer? Layer { get; }


        public void SetNeuronsCount(int neuronsCount)
        {
            int layerNeuronsCount = LayerModel.NeuronModels.Count;

            if (neuronsCount > layerNeuronsCount)
            {
                var newNeuronModels = new NeuronModel[neuronsCount - layerNeuronsCount];
                for (int i = 0; i < neuronsCount - layerNeuronsCount; i++)
                {
                    newNeuronModels[i] = new NeuronModel();
                }
                LayerModel.NeuronModels.AddRange(newNeuronModels);
            }
            else
            {
                LayerModel.NeuronModels.RemoveRange(LayerModel.NeuronModels.Where((_, ind) => ind >= neuronsCount).ToArray());
            }

        }

        public event PropertyChangedEventHandler PropertyChanged = null!;
    }
}
