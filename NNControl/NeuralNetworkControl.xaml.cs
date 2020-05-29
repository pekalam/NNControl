using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NNControl.Adapter;
using NNControl.Network;
using NNControl.Network.Impl;
using NNControl.Overlays;


[assembly: InternalsVisibleTo("NNControl.Tests")]
namespace NNControl
{
    /// <summary>
    /// Interaction logic for NeuralNetworkControl.xaml
    /// </summary>
    public partial class NeuralNetworkControl : UserControl
    {
        private static double ZoomStepWheel = 0.05;

        private readonly INeuralNetworkModelAdapter _defaultAdapter = new DefaultNeuralNetworkModelAdapter(2,4,5,1);

        private readonly NeuralNetworkController _networkController;

        public NeuralNetworkControl()
        {
            SizeChanged += NeuralNetworkControl_SizeChanged;
            _networkController = new NeuralNetworkController(new SkNeuralNetworkView());
            _networkController.OnRequestRedraw += () => canvas.InvalidateVisual();

            InitStateMachine();
            InitializeComponent();
            InitActionMenuOverlay();
            InitOverlay();
        }

        public NeuralNetworkController Controller => _networkController;


        private void NeuralNetworkControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(_actionMenuOverlay, rootGrid.ActualWidth - 60);
            _networkController.SetCanvasSz((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            // _networkView.ForceDraw((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            _networkController.Reposition();
        }

        private void canvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SkNetworkPaintContextHolder.Context = new SkNetworkPaintContext()
            {
                e = e
            };

            if (_networkController.NeuralNetworkModel != null)
            {
                _networkController.Draw((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            }
            else
            {
                _networkController.SetCanvasSz((float)canvas.ActualWidth, (float)canvas.ActualHeight);
                SetValue(ModelAdapterProperty, _defaultAdapter);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _clickedPoint = e.GetPosition(canvas);
                _stateMachine.Next(Triggers.MV_LB);
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                var p = e.GetPosition(canvas);
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    _clickedPoint = e.GetPosition(canvas);
                    _stateMachine.Next(Triggers.RBx1);
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    _actionMenuOverlay.HideSettings();
                }

                if (e.ClickCount == 1 && !Keyboard.IsKeyDown(Key.LeftShift))
                {
                    _clickedPoint = e.GetPosition(canvas);
                    _stateMachine.Next(Triggers.LBx1);
                }
                else if (e.ClickCount == 1 && Keyboard.IsKeyDown(Key.LeftShift))
                {
                    _clickedPoint = e.GetPosition(canvas);
                    _stateMachine.Next(Triggers.CTRL_LB);
                }
                else if (e.ClickCount == 2)
                {
                    _clickedPoint = e.GetPosition(canvas);
                    _stateMachine.Next(Triggers.LBx2);
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _stateMachine.Next(Triggers.LB_UP);
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                Zoom(-ZoomStepWheel);
            }
            else
            {
                Zoom(ZoomStepWheel);
            }
        }

        private void rootGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            _stateMachine.Next(Triggers.MOUSE_OUT);
        }
    }
}