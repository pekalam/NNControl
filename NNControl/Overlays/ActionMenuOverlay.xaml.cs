using System.Windows;
using System.Windows.Controls;

namespace NNControl.Overlays
{
    /// <summary>
    /// Interaction logic for ActionMenuOverlay.xaml
    /// </summary>
    public partial class ActionMenuOverlay : UserControl
    {
        private static double ZoomStepActionMenu = 0.1;

        private readonly NeuralNetworkControl _control;

        public ActionMenuOverlay(NeuralNetworkControl control)
        {
            _control = control;
            _control.PropertyChanged += _control_PropertyChanged;
            InitializeComponent();
        }


        private void _control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(NeuralNetworkControl.ModelAdapter) && _control.ModelAdapter.NeuralNetworkModel != null)
            {

                vSlider.Minimum = _control.ModelAdapter.NeuralNetworkModel.NeuronRadius;
                vSlider.Maximum = 500;
                vSlider.Value = _control.ModelAdapter.NeuralNetworkModel.LayerYSpaceBetween;


                hSlider.Minimum = _control.ModelAdapter.NeuralNetworkModel.NeuronRadius;
                hSlider.Maximum = 500;
                hSlider.Value = _control.ModelAdapter.NeuralNetworkModel.LayerXSpaceBetween;
            }
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

        private void HSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _control.ModelAdapter.NeuralNetworkModel.LayerXSpaceBetween = (int)e.NewValue;
            _control.UpdatePositionParameters();
        }

        private void VSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _control.ModelAdapter.NeuralNetworkModel.LayerYSpaceBetween = (int) e.NewValue;
            _control.UpdatePositionParameters();
        }

        public void HideSettings()
        {
            if (settings.Visibility == Visibility.Visible)
            {
                settings.Visibility = Visibility.Collapsed;
                Canvas.SetLeft(this, Canvas.GetLeft(this) + settings.MinWidth);
            }
        }

        public void ShowSettings()
        {
            if (settings.Visibility == Visibility.Collapsed)
            {
                settings.Visibility = Visibility.Visible;
                Canvas.SetLeft(this, Canvas.GetLeft(this) - settings.MinWidth);
            }
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (settings.Visibility == Visibility.Visible)
            {
                HideSettings();
            }
            else if(settings.Visibility == Visibility.Collapsed)
            {
                ShowSettings();
            }
        }
    }
}
