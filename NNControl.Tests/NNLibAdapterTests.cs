using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NNLib;
using NNLib.ActivationFunction;
using NNLib.MLP;
using NNLibAdapter;
using Xunit;

namespace NNControl.Tests
{
    public class NNLibAdapterTests
    {
        private void CheckStructure(NNLibModelAdapter adapter, INetwork network)
        {
            for (int j = 0; j < network.TotalLayers; j++)
            {
                if (j == 0)
                {
                    adapter.LayerModelAdapters[0].LayerModel.NeuronModels.Count.Should()
                        .Be(network.BaseLayers[0].InputsCount);
                }
                else
                {
                    adapter.LayerModelAdapters[j+1].LayerModel.NeuronModels.Count.Should()
                        .Be(network.BaseLayers[j].NeuronsCount);
                }
            }
        }

        [Fact]
        public void Network_structure_change_changes_structure_of_model()
        {
            var net = MLPNetwork.Create(1, (1, new LinearActivationFunction()));
            var clone = net.Clone();
            var adapter = new NNLibModelAdapter(net);

            //1 1
            CheckStructure(adapter, net);

            net.InsertAfter(0);
            //1 1 1
            CheckStructure(adapter, net);


            net.InsertAfter(1);
            //1 1 1 1
            CheckStructure(adapter, net);


            net.BaseLayers[0].InputsCount++;
            //2 1 1 1
            CheckStructure(adapter, net);

            net.BaseLayers[0].NeuronsCount = 3;
            //2 3 1 1
            CheckStructure(adapter, net);

            net.BaseLayers[1].NeuronsCount = 4;
            //2 3 4 1
            CheckStructure(adapter, net);

            net.BaseLayers[2].NeuronsCount = 5;
            //2 3 4 5
            CheckStructure(adapter, net);

            net.RemoveLayer(2);
            //2 3 4
            CheckStructure(adapter, net);

            net.BaseLayers[0].NeuronsCount = 5;
            //2 5 4
            CheckStructure(adapter, net);

            net.InsertBefore(1);
            //2 5 5 4
            CheckStructure(adapter, net);

            net.InsertBefore(1);
            //2 5 5 5 4
            CheckStructure(adapter, net);

            net.BaseLayers[1].NeuronsCount = 1;
            //2 5 1 5 4
            CheckStructure(adapter, net);

            net.InsertBefore(2);
            //2 5 1 1 5 4
            CheckStructure(adapter, net);

            net.InsertBefore(0);
            //2 2 5 1 1 5 4
            CheckStructure(adapter, net);

            net.BaseLayers[0].InputsCount++;
            //3 2 5 1 1 5 4
            CheckStructure(adapter, net);

            adapter.SetNeuralNetwork(clone);
            CheckStructure(adapter, clone);

            clone.BaseLayers[0].InputsCount++;
            CheckStructure(adapter, clone);


            adapter.SetNeuralNetwork(net);
            CheckStructure(adapter, net);
        }
    }
}
