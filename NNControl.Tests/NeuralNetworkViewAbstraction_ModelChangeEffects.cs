using System;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NNControl.Model;
using NNControl.Network;
using NNControl.Network.Impl;
using NNControl.Synapse;
using Xunit;

namespace NeuralNetworkControl.Tests
{
    public class NetworkStructureManagerTests
    {
        private NeuralNetworkController nnc;
        private NeuralNetworkModel _model;

        public NetworkStructureManagerTests()
        {
            nnc = new NeuralNetworkController(new SkNeuralNetworkView(), () => { });
            _model = nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                            new NeuronModel(),
                            new NeuronModel(),
                        }
                    },
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                            new NeuronModel(),
                        }
                    },
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                        }
                    },
                }
            };
        }


        public void ValidateNetwork()
        {
            //Net controller properties
            nnc.Layers.Count.Should().Be(_model.NetworkLayerModels.Count);

            //Net view properties
            nnc.View.Layers.Count.Should().Be(_model.NetworkLayerModels.Count);
            nnc.View.NeuronsCount.Should().Be(_model.NetworkLayerModels.Sum(m => m.NeuronModels.Count));


            int n=0;
            //Neurons
            for (int i = 0; i < _model.NetworkLayerModels.Count; i++)
            {
                var layer = nnc.Layers[i];
                //count
                layer.Neurons.Count.Should().Be(_model.NetworkLayerModels[i].NeuronModels.Count);
                
                for (int j = 0; j < _model.NetworkLayerModels[i].NeuronModels.Count; j++)
                {
                    var neuron = layer.Neurons[j];
                    //numbering
                    neuron.View.NumberInLayer.Should().Be(j);
                    neuron.View.Number.Should().Be(n);
                    neuron.Layer.Should().Be(layer);
                    n++;

                    if (i == 0)
                    {
                        neuron.Synapses.Count.Should().Be(0);
                        neuron.ConnectedSynapses.Count.Should().Be(_model.NetworkLayerModels[1].NeuronModels.Count);
                    }
                    if (i > 0)
                    {
                        neuron.Synapses.Count.Should().Be(_model.NetworkLayerModels[i - 1].NeuronModels.Count);

                        if (i < _model.NetworkLayerModels.Count - 1)
                        {
                            neuron.ConnectedSynapses.Count.Should().Be(_model.NetworkLayerModels[i + 1].NeuronModels.Count);
                        }
                    }
                   
                }
            }

            //Layer next prev
            for (int i = 0; i < _model.NetworkLayerModels.Count - 1; i++)
            {
                nnc.Layers[i].NextLayer.Should().Be(nnc.Layers[i + 1]);
            }
            for (int i = 1; i < _model.NetworkLayerModels.Count; i++)
            {
                nnc.Layers[i].PreviousLayer.Should().Be(nnc.Layers[i - 1]);
            }



        }

        [Fact]
        public void Net_valid_after_creation()
        {
            ValidateNetwork();
        }

        [Fact]
        public void Net_valid_after_layer_add()
        {
            _model.NetworkLayerModels.Add(new LayerModel()
            {
                NeuronModels = new ObservableRangeCollection<NeuronModel>()
                {
                    new NeuronModel(),
                    new NeuronModel(),
                }
            });

            ValidateNetwork();
        }


        [Fact]
        public void Net_valid_after_neurons_add()
        {
            _model.NetworkLayerModels[0].NeuronModels.Add(new NeuronModel());

            ValidateNetwork();

            _model.NetworkLayerModels[1].NeuronModels.Add(new NeuronModel());

            ValidateNetwork();


            _model.NetworkLayerModels[^1].NeuronModels.Add(new NeuronModel());

            ValidateNetwork();
        }


        [Fact]
        public void Net_valid_after_neurons_removal()
        {
            _model.NetworkLayerModels[0].NeuronModels.RemoveRange(_model.NetworkLayerModels[0].NeuronModels.Where((model, i1) => i1 < 1).ToArray());

            ValidateNetwork();
        }
    }


    public class NeuralNetworkViewAbstraction_ElementsNumbering
    {
        private NeuralNetworkController nnc;

        public NeuralNetworkViewAbstraction_ElementsNumbering()
        {
            nnc = new NeuralNetworkController(new SkNeuralNetworkView(), () => { });
        }

        [Fact]
        public void f()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                            new NeuronModel(),
                            new NeuronModel(),
                        }
                    },
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                            new NeuronModel(),
                        }
                    },
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel(),
                        }
                    },
                }
            };


            int nc = 0;
            for (int i = 0; i < nnc.View.Layers.Count; i++)
            {
                nnc.View.Layers[i].Number.Should().Be(i);
                for (int j = 0; j < nnc.View.Layers[i].Neurons.Count; j++)
                {
                    nnc.View.Layers[i].Neurons[j].NumberInLayer.Should().Be(j);
                    nnc.View.Layers[i].Neurons[j].Number.Should().Be(nc);
                    for (int k = 0; k < nnc.View.Layers[i].Neurons[j].Synapses.Count; k++)
                    {
                        nnc.View.Layers[i].Neurons[j].Synapses[k].NumberInNeuron.Should().Be(k);
                    }
                    nc++;
                }
            }
        }
    }


    public class NeuralNetworkViewAbstraction_ModelChangeEffects
    {
        private NeuralNetworkController nnc;
        private int requestRedrawCount;

        public NeuralNetworkViewAbstraction_ModelChangeEffects()
        {
            nnc = new NeuralNetworkController(new SkNeuralNetworkView(), () => requestRedrawCount++);
        }

        [Fact]
        public void Test1()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
            };

            nnc.Layers.Count.Should().Be(0);
            nnc.View.Layers.Count.Should().Be(0);
            requestRedrawCount.Should().Be(1);
        }

        [Fact]
        public void Test2()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
            };

            nnc.Layers.Count.Should().Be(0);
            nnc.View.Layers.Count.Should().Be(0);
            requestRedrawCount.Should().Be(1);
        }

        [Fact]
        public void Test3()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                }
            };

            nnc.Layers.Count.Should().Be(1);
            nnc.View.Layers.Count.Should().Be(1);
            requestRedrawCount.Should().Be(1);
        }


        [Fact]
        public void Test4()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel()
                        }
                    }
                }
            };

            nnc.Layers.Count.Should().Be(1);
            nnc.View.Layers.Count.Should().Be(1);

            nnc.View.Layers[0].Neurons.Count.Should().Be(1);
            nnc.Layers[0].Neurons.Count.Should().Be(1);
            requestRedrawCount.Should().Be(1);
        }



        [Fact]
        public void Test5()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel()
                        }
                    }
                }
            };

            nnc.NeuralNetworkModel.NetworkLayerModels.Add(new LayerModel()
            {
                NeuronModels = new ObservableRangeCollection<NeuronModel>()
                {
                    new NeuronModel(), new NeuronModel()
                }
            });

            nnc.Layers.Count.Should().Be(2);
            nnc.View.Layers.Count.Should().Be(2);

            nnc.View.Layers[1].Neurons.Count.Should().Be(2);
            nnc.Layers[1].Neurons.Count.Should().Be(2);

            nnc.Layers[1].PreviousLayer.Should().Be(nnc.Layers[0]);
            nnc.View.Layers[1].PreviousLayer.Should().Be(nnc.View.Layers[0]);

            requestRedrawCount.Should().Be(2);
        }


        [Fact]
        public void Test6()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
                NetworkLayerModels = new ObservableCollection<LayerModel>()
                {
                    new LayerModel()
                    {
                        NeuronModels = new ObservableRangeCollection<NeuronModel>()
                        {
                            new NeuronModel()
                        }
                    }
                }
            };

            var newModels = new NeuronModel[100];
            for (int i = 0; i < 100; i++)
            {
                newModels[i] = new NeuronModel();
            }
            nnc.NeuralNetworkModel.NetworkLayerModels[0].NeuronModels.AddRange(newModels);

            requestRedrawCount.Should().Be(2);
        }
    }
}
