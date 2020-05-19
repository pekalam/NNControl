using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using NNControl.Network;
using NNLib;
using NNLib.ActivationFunction;
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
            SetColors(network.Layers);
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        protected async override void OnContentRendered(EventArgs e)
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
                            layer.Weights[r, c] = rnd.NextDouble() * rnd.Next(-100, 100);
                        }
                    }

                    for (int r = 0; r < layer.Biases.RowCount; r++)
                    {
                        layer.Biases[r, 0] = rnd.NextDouble() * rnd.Next(-100, 100);
                    }
                }

                _colorAnimation.CallSetColors(_network);

                await Task.Delay(100);
            }
        }

        public NNLibModelAdapter Adapter { get; set; }
    }
}
