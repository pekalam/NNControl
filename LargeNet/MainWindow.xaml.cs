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
            Adapter = new DefaultNeuralNetworkModelAdapter(40,40,20,10,1);
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
                    var l = rnd.Next(0, control.Controller.Layers.Count);
                    var n = rnd.Next(0, control.Controller.Layers[l].Neurons.Count);
                    var s = rnd.Next(0, control.Controller.Layers[l].Neurons[n].TotalSynapses);

                    Dispatcher.Invoke(() =>
                    {
                        var b = new byte[3];
                        rnd.NextBytes(b);
                        var str = BitConverter.ToString(b).Replace("-", "");
                        control.Controller.Color.SetNeuronColor(l, n, "#" + str);
                        control.Controller.Color.SetSynapseColor(l, n, s, "#" + str);
                        control.Controller.Color.ApplyColors();
                    });

                    await Task.Delay(200);
                }
            });
        }

        public DefaultNeuralNetworkModelAdapter Adapter { get; set; }
    }
}