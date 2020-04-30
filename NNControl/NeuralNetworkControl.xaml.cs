using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeuralNetworkControl.Abstraction;
using NeuralNetworkControl.Adapter;
using NeuralNetworkControl.Model;
using NeuralNetworkControl.SkiaImpl;
using NNLib;
using NNLib.ActivationFunction;


[assembly: InternalsVisibleTo("NeuralNetworkControl.Tests")]
namespace NeuralNetworkControl
{
    public class DefaultLayerModelAdapter: ILayerModelAdapter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LayerModel LayerModel { get; set; }
    }

    public class DefaultNeuralNetworkModelAdapter : INeuralNetworkModelAdapter
    {
        public DefaultNeuralNetworkModelAdapter(params int[] netsz)
        {
            NeuralNetworkModel = new NeuralNetworkModel();
            NeuralNetworkModel.NetworkLayerModels = new ObservableCollection<LayerModel>();
            foreach (var sz in netsz)
            {
                var l = new LayerModel();
                l.NeuronModels = new ObservableRangeCollection<NeuronModel>();
                for (int i = 0; i < sz; i++)
                {
                    l.NeuronModels.Add(new NeuronModel());
                }
                NeuralNetworkModel.NetworkLayerModels.Add(l);
            }

            var layerAdapters = new List<DefaultLayerModelAdapter>();

            foreach (var layerModel in NeuralNetworkModel.NetworkLayerModels)
            {
                layerAdapters.Add(new DefaultLayerModelAdapter()
                {
                    LayerModel = layerModel,
                });
            }

            LayerModelAdapters = layerAdapters;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public NeuralNetworkModel NeuralNetworkModel { get; }
        public IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
    }


    /// <summary>
    /// Interaction logic for NeuralNetworkControl.xaml
    /// </summary>
    public partial class NeuralNetworkControl : UserControl
    {
        private static double ZoomStepWheel = 0.1;

        private readonly INeuralNetworkModelAdapter _defaultAdapter = new DefaultNeuralNetworkModelAdapter(2,4,5,1);

        private readonly NeuralNetworkViewAbstraction _networkView;

        public NeuralNetworkControl()
        {
            this.SizeChanged += NeuralNetworkControl_SizeChanged;
            _networkView = new NeuralNetworkViewAbstraction(new SkNeuralNetworkView());
            _networkView.OnRequestRedraw += () => canvas.InvalidateVisual();
            InitDependencyProperties();

            InitStateMachine();
            InitializeComponent();
            InitOverlay();
            InitActionMenuOverlay();

            SetValue(ModelAdapterProperty, _defaultAdapter);
        }

        private void NeuralNetworkControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(_actionMenuOverlay, rootGrid.ActualWidth - _actionMenuOverlay.Width * 2);
            _networkView.ForceDraw();
        }

        private void canvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            SkNetworkPaintContextHolder.Context = new SkNetworkPaintContext()
            {
                e = e
            };

            _networkView.Draw();
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
                if (p.X > rootGrid.ActualWidth - _actionMenuOverlay.Width && p.X <= rootGrid.ActualWidth)
                {
                    ShowActionMenuOverlay();
                }
                else
                {
                    HideActionMenuOverlay();
                }
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
            HideActionMenuOverlay();
        }
    }
}