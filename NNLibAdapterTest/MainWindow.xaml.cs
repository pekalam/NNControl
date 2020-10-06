using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NNLib;
using NNLib.Common;
using NNLibAdapter;

namespace NNLibAdapterTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MLPNetwork _network;
        private MLPTrainer _trainer;

        public MainWindow()
        {
            var trainingSets =
                new SupervisedTrainingSets(SupervisedSet.FromArrays(new[]
                {
                    new[] {0d, 0d},
                    new[] {0d, 1d},
                    new[] {1d, 0d},
                    new[] {1d, 1d}
                }, new[]
                {
                    new[] {0d},
                    new[] {0d},
                    new[] {0d},
                    new[] {1d},
                }));

            _network = new MLPNetwork(new PerceptronLayer(2, 2, new SigmoidActivationFunction()),
                new PerceptronLayer(2, 15, new SigmoidActivationFunction()),
                new PerceptronLayer(15, 1, new SigmoidActivationFunction()));

            _trainer = new MLPTrainer(_network, trainingSets, new GradientDescentAlgorithm(new GradientDescentParams()), new QuadraticLossFunction());

            Adapter = new NNLibAdapter.NNLibModelAdapter();
            Adapter.SetNeuralNetwork(_network);
            Adapter.SetInputLabels(new []{"inp 1", "inp 2"});
            Adapter.SetOutputLabels(new[] { "out 1" });
            DataContext = this;
            InitializeComponent();
            NCount.Text = _network.Layers[0].NeuronsCount.ToString();
        }

        public NNLibModelAdapter Adapter { get; set; }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

        }

        private void Highlihght_OnClick(object sender, RoutedEventArgs e)
        {
            control.Controller.ClearHighlight();
            control.Controller.HighlightLayer(1);
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            Adapter.LayerModelAdapters[1].SetNeuronsCount(_network.Layers[0].NeuronsCount+1);
            _network.Layers[0].NeuronsCount++;
        }

        private void NCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var n = Convert.ToInt32(NCount.Text);
                Adapter.LayerModelAdapters[1].SetNeuronsCount(n);
                _network.Layers[0].NeuronsCount = n;
            }
            catch (Exception)
            {

            }
        }

        private void RmMid_Click(object sender, RoutedEventArgs e)
        {
            Adapter.RemoveLayer(1);
            _network.RemoveLayer(_network.Layers[1]);
        }

        private void Before_Click(object sender, RoutedEventArgs e)
        {
            var ind = Convert.ToInt32(BeforeInd.Text);

            Adapter.InsertBefore(ind,_network.InsertBefore(ind));   
        }

        private void After_Click(object sender, RoutedEventArgs e)
        {
            var ind = Convert.ToInt32(AfterInd.Text);

            Adapter.InsertAfter(ind, _network.InsertAfter(ind));
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            var layer = new PerceptronLayer(_network.Layers[^1].NeuronsCount, 1, new LinearActivationFunction());
            _network.AddLayer(layer);
            Adapter.AddLayer(layer);
        }

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            var stw = new Stopwatch();
            long total = 0L;
            stw.Restart();

            Adapter.LayerModelAdapters[1].SetNeuronsCount(1000);
            stw.Stop();
            Debug.WriteLine("Adapter elapsed: " + stw.Elapsed);
            total += stw.ElapsedMilliseconds;

            stw.Restart();
            _network.Layers[0].NeuronsCount = 1000;
            stw.Stop();
            Debug.WriteLine("NNLib elapsed: " + stw.Elapsed);
            Debug.WriteLine("Total elapsed: " + total);
        }

        private void Animate_OnClick(object sender, RoutedEventArgs e)
        {
            Adapter.ColorAnimation.SetupTrainer(_trainer);
            Task.Run(async () =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _trainer.DoEpoch();
                        //Debug.WriteLine(_trainer.Error);
                        //Adapter.UpdateWeights(control);
                    });
            
                    await Task.Delay(10);
                }
            
            });
        }
    }
}