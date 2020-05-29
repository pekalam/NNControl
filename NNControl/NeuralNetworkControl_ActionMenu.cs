using System.Windows;
using System.Windows.Controls;
using NNControl.Overlays;

namespace NNControl
{
    public partial class NeuralNetworkControl
    {
        private ActionMenuOverlay _actionMenuOverlay;

        private void InitActionMenuOverlay()
        {
            _actionMenuOverlay = new ActionMenuOverlay(this)
            {
                Visibility = Visibility.Visible,
            };
            _actionMenuOverlay.SetValue(Panel.ZIndexProperty, 1);
            Canvas.SetTop(_actionMenuOverlay, 0);
            overlayCanvas.Children.Add(_actionMenuOverlay);
        }
    }
}