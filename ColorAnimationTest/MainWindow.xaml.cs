﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using NNControl.Network;
using NNLib;
using NNLibAdapter;

namespace ColorAnimationTest
{
    public class TestColorAnimation : ColorAnimation
    {
        public TestColorAnimation(NeuralNetworkController controller) : base(controller)
        {
        }

        public void CallSetColors(MLPNetwork network)
        {
            SetColorsPerLayer(network.Layers);
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int i = 0;
        private MLPNetwork _network;
        private TestColorAnimation _colorAnimation;

        public MainWindow()
        {

            _network = new MLPNetwork(new PerceptronLayer(2, 2, new SigmoidActivationFunction()),
                new PerceptronLayer(2, 15, new SigmoidActivationFunction()),
                new PerceptronLayer(15, 1, new SigmoidActivationFunction()));

            Adapter = new NNLibModelAdapter();
            Adapter.SetNeuralNetwork(_network);
            DataContext = this;

            InitializeComponent();
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            _colorAnimation = new TestColorAnimation(Adapter.Controller);
            var rnd = new Random();

            while (true)
            {
                foreach (var layer in _network.Layers)
                {
                    for (int r = 0; r < layer.Weights.RowCount; r++)
                    {
                        for (int c = 0; c < layer.Weights.ColumnCount; c++)
                        {
                            if (i < 4)
                            {
                                if (Adapter.ColorAnimation.AnimationMode == ColorAnimationMode.Layer)
                                {
                                    layer.Weights[r, c] = rnd.NextDouble() * rnd.Next(-101, 100);
                                }
                                else
                                {
                                    if (layer.IsOutputLayer)
                                    {
                                        layer.Weights[r, c] = rnd.NextDouble() * rnd.Next(-101, 100);
                                    }
                                    else
                                    {
                                        layer.Weights[r, c] = 0.000001;
                                    }
                                }
                            }

                            if (i > 4)
                            {
                                layer.Weights[r, c] = c switch
                                {
                                    0 => 0.01,
                                    1 => 0,
                                    _ => 100
                                };
                            }

                            if (i == 8)
                            {
                                i = 0;
                            }
                        }
                    }

                    for (int r = 0; r < layer.Biases.RowCount; r++)
                    {
                        layer.Biases[r, 0] = r switch
                        {
                            0 => -0.1275,
                            1 => -0.15,
                            2 => 0.42,
                            3 => -0.308,
                            4 => -2.44,
                            _ => 10
                        };
                    }
                }

                _colorAnimation.CallSetColors(_network);

                await Task.Delay(1000);
                i++;
            }
        }

        public NNLibModelAdapter Adapter { get; set; }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            Adapter.ColorAnimation.AnimationMode = ColorAnimationMode.Network;
        }

        private void Mode_Unchecked(object sender, RoutedEventArgs e)
        {
            Adapter.ColorAnimation.AnimationMode = ColorAnimationMode.Layer;
        }
    }
}
