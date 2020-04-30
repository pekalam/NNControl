using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeuralNetworkControl.Adapter;
using NeuralNetworkControl.Model;
using NNLib;

namespace NNLibAdapter
{
    public class LayerModelAdapter : ILayerModelAdapter
    {
        public LayerModelAdapter(LayerModel layerModel, Layer layer)
        {
            LayerModel = layerModel;
            Layer = layer;
        }

        public LayerModel LayerModel { get; }
        public Layer Layer { get; }


        public void SetNeuronsCount(int neuronsCount)
        {
            int layerNeuronsCount = 0;


            if (Layer != null)
            {
                if (neuronsCount == Layer.NeuronsCount)
                {
                    return;
                }

                if (neuronsCount <= 0)
                {
                    throw new ArgumentException("Invalid neurons count");
                }
                layerNeuronsCount = Layer.NeuronsCount;
            }
            else
            {
                layerNeuronsCount = LayerModel.NeuronModels.Count;
            }

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
                var newNeuronModels = new NeuronModel[layerNeuronsCount - neuronsCount];
                for (int i = 0; i < layerNeuronsCount - neuronsCount; i++)
                {
                    LayerModel.NeuronModels.RemoveAt(LayerModel.NeuronModels.Count - 1);
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
