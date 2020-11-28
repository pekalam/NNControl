using NNControl.Adapter;
using NNControl.Network;
using NNControl.Neuron;
using StateMachineLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace NNControl
{
    public partial class NeuralNetworkControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ShowVisProperty = DependencyProperty.Register(
            nameof(ShowVis), typeof(bool), typeof(NeuralNetworkControl), new FrameworkPropertyMetadata(default(bool),
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    if ((bool) args.NewValue)
                    {
                        (o as NeuralNetworkControl)!.StartVis();
                    }
                }));

        public bool ShowVis
        {
            get => (bool) GetValue(ShowVisProperty);
            set => SetValue(ShowVisProperty, value);
        }


        public static DependencyProperty ModelAdapterProperty = DependencyProperty.Register(
            nameof(ModelAdapter), typeof(INeuralNetworkModelAdapter), typeof(NeuralNetworkControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnModelAdapterChanged));

        public INeuralNetworkModelAdapter ModelAdapter
        {
            get => (INeuralNetworkModelAdapter) GetValue(ModelAdapterProperty);
            set => SetValue(ModelAdapterProperty, value);
        }

        private static void OnModelAdapterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d.GetValue(ModelAdapterProperty) is INeuralNetworkModelAdapter adapter))
            {
                return;
            }

            if (e.OldValue is INeuralNetworkModelAdapter oldAdapter)
            {
                oldAdapter.Detach();
            }

            var control = (d as NeuralNetworkControl);

            adapter.Controller = control.Controller;
            control.SetAdapterHandlers(adapter);
            
            if (adapter.NeuralNetworkModel != null)
            {
                control.Controller.NeuralNetworkModel = adapter.NeuralNetworkModel;
            }

            control.OnPropertyChanged(nameof(ModelAdapter));
        }

        private void SetAdapterHandlers(INeuralNetworkModelAdapter adapter)
        {
            adapter.PropertyChanged -= AdapterOnPropertyChanged;
            adapter.PropertyChanged += AdapterOnPropertyChanged;
        }

        private void AdapterOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ModelAdapter.NeuralNetworkModel))
            {
                Controller.NeuralNetworkModel = ModelAdapter.NeuralNetworkModel;
            }
        }

        private async void StartVis()
        {
            if (vis == null)
            {
                vis = new StateMachineVis<Triggers, States>(_stateMachine);

                await Task.Delay(2000);
                vis.Start(@"StateMachineLibVis.exe", "-c graphViz -l 0");
                Controller.StartVis();
            }
        }

        public void UpdateSynapseLabels()
        {
            Controller.RequestRedraw(NeuralNetworkController.ViewTrig.FORCE_DRAW_EXCLUDED);
        }

        public static readonly RoutedEvent NeuronClickEvent =
            EventManager.RegisterRoutedEvent(nameof(NeuronClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(NeuralNetworkControl));

        public event RoutedEventHandler NeuronClick
        {
            add => AddHandler(NeuronClickEvent, value);
            remove => RemoveHandler(NeuronClickEvent, value);
        }

        protected virtual void RaiseNeuronClickEvent(NeuronView neuron)
        {
            var layerAdapter = ModelAdapter.LayerModelAdapters[neuron.Layer.Number];

            var args = new NeuronClickedEventArgs(layerAdapter, neuron.NumberInLayer);
            RaiseEvent(args);
        }

        public void Zoom(float zoom)
        {
            Controller.SetZoom(zoom);
        }

        public void Reposition()
        {
            Controller.Reposition();
        }

        public void UpdatePositionParameters()
        {
            Controller.UpdatePositionParameters();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class NeuronClickedEventArgs : RoutedEventArgs
    {
        public NeuronClickedEventArgs(ILayerModelAdapter layerAdapter, int neuronIndex) : base(NeuralNetworkControl
            .NeuronClickEvent)
        {
            LayerAdapter = layerAdapter;
            NeuronIndex = neuronIndex;
        }

        public ILayerModelAdapter LayerAdapter { get; }
        public int NeuronIndex { get; }
    }
}