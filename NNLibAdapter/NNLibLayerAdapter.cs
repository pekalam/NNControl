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
        private string[]? _labels;

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

            if (_labels != null && _labels.Length == LayerModel.NeuronModels.Count)
            {
                for (int i = 0; i < LayerModel.NeuronModels.Count; i++)
                {
                    LayerModel.NeuronModels[i].Label = _labels[i];
                }
            }
        }

        internal void AttachLabels(string[] labels)
        {
            _labels = labels;

            if (labels.Length == LayerModel.NeuronModels.Count)
            {
                for (int i = 0; i < LayerModel.NeuronModels.Count; i++)
                {
                    LayerModel.NeuronModels[i].Label = labels[i];
                }
            }
        }

        internal void ClearLabels()
        {
            _labels = null;

            for (int i = 0; i < LayerModel.NeuronModels.Count; i++)
            {
                LayerModel.NeuronModels[i].Label = "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = null!;
    }
}
