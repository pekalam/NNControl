using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using NNLib.ActivationFunction;
using NNLib.Data;
using NNLib.LossFunction;
using NNLib.MLP;
using NNLib.Training.GradientDescent;
using NNLibAdapter;
using NNLibAdapterTest.Annotations;

namespace NNLibAdapterTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MLPNetwork _network;
        private MLPTrainer _trainer;
        private NNLibModelAdapter _adapter;
        private NNLibModelAdapter _previous;

        public MainWindow()
        {
            var trainingSets =
                new SupervisedTrainingData(SupervisedTrainingSamples.FromArrays(new[]
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

            _network = new MLPNetwork(new PerceptronLayer(2, 4, new SigmoidActivationFunction()),
                new PerceptronLayer(4, 1, new SigmoidActivationFunction()));

            _trainer = new MLPTrainer(_network, trainingSets, new GradientDescentAlgorithm(new GradientDescentParams()), new QuadraticLossFunction());

            Adapter = _previous = new NNLibModelAdapter(_network);
            //Adapter.SetInputLabels(new []{"inp 1", "inp 2"});
            //Adapter.SetOutputLabels(new[] { "out 1" });
            Adapter.NeuralNetworkModel.BackgroundColor = "#FFFFFF";
            DataContext = this;
            InitializeComponent();
            NCount.Text = _network.Layers[0].NeuronsCount.ToString();
            NCountl2.Text = _network.Layers[1].NeuronsCount.ToString();
        }

        public NNLibModelAdapter Adapter
        {
            get => _adapter;
            set
            {
                _adapter = value;
                OnPropertyChanged();
            }
        }

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
            _network.Layers[0].NeuronsCount++;
        }

        private void NCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var n = Convert.ToInt32(NCount.Text);
                _network.Layers[0].NeuronsCount = n;
            }
            catch (Exception)
            {

            }
        }

        private void RmMid_Click(object sender, RoutedEventArgs e)
        {
            var ind = Convert.ToInt32(BeforeInd.Text);
            _network.RemoveLayer(_network.Layers[ind]);
        }

        private void Before_Click(object sender, RoutedEventArgs e)
        {
            var ind = Convert.ToInt32(BeforeInd.Text);
            _network.InsertBefore(ind);
        }

        private void After_Click(object sender, RoutedEventArgs e)
        {
            var ind = Convert.ToInt32(AfterInd.Text);
            _network.InsertAfter(ind);
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            var layer = new PerceptronLayer(_network.Layers[^1].NeuronsCount, 1, new LinearActivationFunction());
            _network.AddLayer(layer);
        }

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            var stw = new Stopwatch();
            long total = 0L;
            stw.Restart();

            _network.Layers[0].NeuronsCount = 1000;

            stw.Stop();
            Debug.WriteLine("Adapter elapsed: " + stw.Elapsed);
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

        private void NCountl2_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var n = Convert.ToInt32(NCountl2.Text);
                _network.Layers[1].NeuronsCount = n;
            }
            catch (Exception)
            {

            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var net = new MLPNetwork(new []
            {
                new PerceptronLayer(1, 2, new SigmoidActivationFunction()), 
                new PerceptronLayer(2, 100, new SigmoidActivationFunction()), 
                new PerceptronLayer(100, 1, new SigmoidActivationFunction()), 
            });

            Adapter.SetNeuralNetwork(net);
        }

        private void ChangeModel_OnClick(object sender, RoutedEventArgs e)
        {
            if (Adapter != _previous)
            {
                Adapter = _previous;
                return;
            }

            var net = new MLPNetwork(new[]
            {
                new PerceptronLayer(1, 2, new SigmoidActivationFunction()),
                new PerceptronLayer(2, 3, new SigmoidActivationFunction()),
                new PerceptronLayer(3, 1, new SigmoidActivationFunction()),
            });

            Adapter = new NNLibModelAdapter(net);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}