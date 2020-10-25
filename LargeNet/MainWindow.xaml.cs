using System;
using System.Collections.Generic;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NNControl;
using NNControl.Adapter;

namespace LargeNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();


        public MainWindow()
        {
            Adapter = new DefaultNeuralNetworkModelAdapter(100,100, 100,20,10,1);
            DataContext = this;
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Task.Run(async () =>
            {
                while (true)
                {
                    // var l = rnd.Next(0, control.Controller.Layers.Count);
                    // var n = rnd.Next(0, control.Controller.Layers[l].Neurons.Count);
                    // var s = rnd.Next(0, control.Controller.Layers[l].Neurons[n].TotalSynapses);

                    var b = new byte[3];
                    for (int i = 0; i < control.Controller.Layers.Count; i++)
                    {
                        for (int j = 0; j < control.Controller.Layers[i].Neurons.Count; j++)
                        {
                            for (int k = 0; k < control.Controller.Layers[i].Neurons[j].TotalSynapses; k++)
                            {
                                control.Controller.Color.SetNeuronColor(i, j, rnd.Next(0, 256));
                                control.Controller.Color.SetSynapseColor(i, j, k, rnd.Next(0, 256));
                            }
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        control.Controller.Color.ApplyColors();
                    }, DispatcherPriority.Background);

                    await Task.Delay(33);
                }
            });
        }

        public DefaultNeuralNetworkModelAdapter Adapter { get; set; }
    }
}