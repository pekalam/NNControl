using System;
using System.Diagnostics;
using System.Linq;
using NNControl.Adapter;
using NNControl.Network;
using NNControl.Network.Impl;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


[assembly: InternalsVisibleTo("NNControl.Tests")]
namespace NNControl
{
    /// <summary>
    /// Interaction logic for NeuralNetworkControl.xaml
    /// </summary>
    public partial class NeuralNetworkControl : UserControl
    {
        private static float ZoomStepWheel = 0.05f;
        
        private readonly INeuralNetworkModelAdapter _defaultAdapter = new DefaultNeuralNetworkModelAdapter(2,4,5,1);

        private Stopwatch _st = new Stopwatch();
        private double _measured;
        private int _meaInd = 0;
        private double _total = 0;
        private int _totalCount = 0;

        private SkNeuralNetworkView _skView = new SkNeuralNetworkView();

        public NeuralNetworkControl()
        {
            SizeChanged += NeuralNetworkControl_SizeChanged;
            Controller = new NeuralNetworkController(_skView, () =>
            {
                _st.Restart();
                canvas.InvalidateVisual();
                _st.Stop();

                _measured += _st.ElapsedTicks / 40.0d;
                _meaInd++;
                if (_meaInd == 40)
                {
                    _meaInd = 0;
                    Console.WriteLine("Mean: " + _measured);
                    _total += _measured / 30.0d;
                    _totalCount++;
                    _measured = 0;
                }

                if (_totalCount == 30)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Total: " + _total);
                    _totalCount = 0;
                    _total = 0;
                }
            });

            InitStateMachine();
            InitializeComponent();
            InitActionMenuOverlay();
            InitOverlay();
        }

        public NeuralNetworkController Controller { get; }


        private void NeuralNetworkControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(_actionMenuOverlay, rootGrid.ActualWidth - 60);
            Controller.SetCanvasSz((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            // _networkView.ForceDraw((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            Controller.Reposition();
        }

        private void canvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _skView.PaintArgs = e;

            if (Controller.NeuralNetworkModel != null)
            {
                Controller.Draw((float)canvas.ActualWidth, (float)canvas.ActualHeight);
            }
            else
            {
                Controller.SetCanvasSz((float)canvas.ActualWidth, (float)canvas.ActualHeight);
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