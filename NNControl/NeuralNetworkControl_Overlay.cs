﻿using NNControl.Overlays;
using System.Windows;
using System.Windows.Controls;

namespace NNControl
{
    public partial class NeuralNetworkControl
    {
        private NeuronOverlay _neuronOverlay;

        private void InitOverlay()
        {
            _neuronOverlay = new NeuronOverlay()
            {
                Width = 350,
                Height = 200,
                Visibility = Visibility.Collapsed,
            };
            _neuronOverlay.SetValue(Panel.ZIndexProperty, 1);
            overlayCanvas.Children.Add(_neuronOverlay);
        }


        private void ShowNeuronOverlay(Point p)
        {
            var neuronRadius = ModelAdapter.NeuralNetworkModel.NeuronRadius * 2 * (_networkController.View.Zoom + 1);
            _neuronOverlay.Visibility = Visibility.Visible;
            
            Canvas.SetLeft(_neuronOverlay, p.X + neuronRadius);
            Canvas.SetTop(_neuronOverlay, p.Y);
        }


        private void HideNeuronOverlay()
        {
            if (_neuronOverlay.Visibility != Visibility.Collapsed)
            {
                _neuronOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}