using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using NNControl.Model;
using NNControl.Network;
using NNControl.Network.Impl;
using NNControl.Synapse;
using Xunit;

namespace NeuralNetworkControl.Tests
{
    public class NeuralNetworkViewAbstraction_ElementsNumbering
    {
        private NeuralNetworkController nnc;

        public NeuralNetworkViewAbstraction_ElementsNumbering()
        {
            nnc = new NeuralNetworkController(new SkNeuralNetworkView());
            nnc.OnRequestRedraw += () => { };
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
            nnc = new NeuralNetworkController(new SkNeuralNetworkView());
            nnc.OnRequestRedraw += () => requestRedrawCount++;
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
