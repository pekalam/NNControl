using System.Windows;
using System.Windows.Controls;
using NeuralNetworkControl.Abstraction;

namespace NeuralNetworkControl.Overlays
{
    /// <summary>
    /// Interaction logic for ActionMenuOverlay.xaml
    /// </summary>
    public partial class ActionMenuOverlay : UserControl
    {
        private static double ZoomStepActionMenu = 0.1;

        private NeuralNetworkControl _control;

        public ActionMenuOverlay(NeuralNetworkControl control)
        {
            _control = control;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _control.Reposition();
        }

        private void ZoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            _control.Zoom(ZoomStepActionMenu);
        }

        private void ZoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            _control.Zoom(-ZoomStepActionMenu);
        }
    }
}
