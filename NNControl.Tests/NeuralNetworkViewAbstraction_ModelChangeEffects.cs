using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using NeuralNetworkControl.Abstraction;
using NeuralNetworkControl.Model;
using NeuralNetworkControl.SkiaImpl;
using Xunit;

namespace NeuralNetworkControl.Tests
{
    public class NeuralNetworkViewAbstraction_ModelChangeEffects
    {
        private NeuralNetworkViewAbstraction nnc;
        private int requestRedrawCount;

        public NeuralNetworkViewAbstraction_ModelChangeEffects()
        {
            nnc = new NeuralNetworkViewAbstraction(new SkNeuralNetworkView());
            nnc.OnRequestRedraw += () => requestRedrawCount++;
        }

        [Fact]
        public void Test1()
        {
            nnc.NeuralNetworkModel = new NeuralNetworkModel()
            {
            };

            nnc.Layers.Count.Should().Be(0);
            nnc.Impl.Layers.Count.Should().Be(0);
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
            nnc.Impl.Layers.Count.Should().Be(0);
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
            nnc.Impl.Layers.Count.Should().Be(1);
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
            nnc.Impl.Layers.Count.Should().Be(1);

            nnc.Impl.Layers[0].Neurons.Count.Should().Be(1);
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
            nnc.Impl.Layers.Count.Should().Be(2);

            nnc.Impl.Layers[1].Neurons.Count.Should().Be(2);
            nnc.Layers[1].Neurons.Count.Should().Be(2);

            nnc.Layers[1].PreviousLayer.Should().Be(nnc.Layers[0]);
            nnc.Impl.Layers[1].PreviousLayer.Should().Be(nnc.Impl.Layers[0]);

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
