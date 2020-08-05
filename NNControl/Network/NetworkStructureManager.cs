using System.Collections.Generic;
using System.Collections.Specialized;
using NNControl.Layer;
using NNControl.Model;

namespace NNControl.Network
{
    internal class NetworkStructureManager
    {
        internal readonly NeuralNetworkView View;
        internal readonly List<LayerController> LayerControllers = new List<LayerController>();

        private NeuralNetworkController _networkController;

        public NetworkStructureManager(NeuralNetworkView view, NeuralNetworkController networkController)
        {
            View = view;
            _networkController = networkController;
        }

        public void SetNeuralNetworkModel(NeuralNetworkModel model)
        {
            LayerControllers.Clear();
            View.Layers.Clear();
            View.NeuralNetworkModel = model;


            model.NetworkLayerModels.CollectionChanged += LayerModelsOnCollectionChanged;
        }

        private void LayerModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AddNewLayers(e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                LayerController next = e.OldStartingIndex < LayerControllers.Count - 1 ? LayerControllers[e.OldStartingIndex + 1] : null;
                next?.RemoveSynapsesFromPrevious();

                LayerController toRemove = LayerControllers[e.OldStartingIndex];
                //toRemove.RemoveSynapsesFromPrevious();
                toRemove.RemoveNeurons();

                View.Layers.RemoveAt(e.OldStartingIndex);
                LayerControllers.RemoveAt(e.OldStartingIndex);

                for (int i = 0; i < LayerControllers.Count; i++)
                {
                    LayerControllers[i].View.Number = i;
                    View.Layers[i].PreviousLayer = i > 0 ? View.Layers[i - 1] : null;
                    LayerControllers[i].PreviousLayer = i > 0 ? LayerControllers[i - 1] : null;
                    LayerControllers[i].NextLayer = i < LayerControllers.Count - 1 ? LayerControllers[i + 1] : null;
                }

                next?.AddSynapsesToPrevious();
            }
        }

        public void CreateNewLayer(int layerNum)
        {
            var newLayerView = View.CreateLayerInstance();
            View.Layers.Add(newLayerView);

            var previousLayer = layerNum == 0 ? null : LayerControllers[layerNum - 1];
            var newAbstractLayer = new LayerController(previousLayer, layerNum, newLayerView, _networkController);
            if (previousLayer != null)
            {
                previousLayer.NextLayer = newAbstractLayer;
            }

            LayerControllers.Add(newAbstractLayer);


            View.NeuralNetworkModel.NetworkLayerModels[layerNum].NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;

            foreach (var neuron in newLayerView.Neurons)
            {
                neuron.Number = View.NeuronsCount++;
            }
        }

        public void AddNewLayers(int collectionStartingIndex = 0)
        {
            for (int i = collectionStartingIndex; i < View.NeuralNetworkModel.NetworkLayerModels.Count; i++)
            {
                CreateNewLayer(i);
            }
        }

        private void ResetNeuronsNumbers()
        {
            int n = 0;
            foreach (var layer in View.Layers)
            {
                int l = 0;
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Number = n++;
                    neuron.NumberInLayer = l++;
                }
            }

            View.NeuronsCount = n;
        }

        private void NeuronModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetNeuronsNumbers();
        }
    }
}