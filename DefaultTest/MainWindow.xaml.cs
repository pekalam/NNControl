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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.Random;

namespace DefaultTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();


        public MainWindow()
        {
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
                        var b = rnd.NextBytes(3);
                        var str = BitConverter.ToString(b).Replace("-", "");
                        control.Controller.SetNeuronColor(l, n, "#" + str);
                        control.Controller.SetSynapseColor(l,n,s, "#" + str);
                    });
                    await Task.Delay(200);
                }
            });
        }
    }
}
