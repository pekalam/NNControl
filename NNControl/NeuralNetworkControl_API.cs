using System.Threading.Tasks;
using System.Windows;
using NNControl.Adapter;
using NNControl.Network;
using NNControl.Neuron;
using StateMachineLib;

namespace NNControl
{
    public partial class NeuralNetworkControl
    {
        private void InitDependencyProperties()
        {
            if (ModelAdapterProperty == null)
            {
                ModelAdapterProperty = DependencyProperty.Register(
                    nameof(ModelAdapter), typeof(INeuralNetworkModelAdapter), typeof(NeuralNetworkControl),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                        OnModelAdapterChanged));
            }

        }

        public static readonly DependencyProperty ShowVisProperty = DependencyProperty.Register(
            nameof(ShowVis), typeof(bool), typeof(NeuralNetworkControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) =>
                {
                    if ((bool) args.NewValue)
                    {
                        (o as NeuralNetworkControl).StartVis();
                    }
                }));

        public bool ShowVis
        {
            get { return (bool) GetValue(ShowVisProperty); }
            set { SetValue(ShowVisProperty, value); }
        }


        public static DependencyProperty ModelAdapterProperty;

        public INeuralNetworkModelAdapter ModelAdapter
        {
            get => (INeuralNetworkModelAdapter) GetValue(ModelAdapterProperty);
            set => SetValue(ModelAdapterProperty, value);
        }

        private void OnModelAdapterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adapter = d.GetValue(ModelAdapterProperty) as INeuralNetworkModelAdapter;
            if (adapter == null)
            {
                return;
            }
            adapter.PropertyChanged += Adapter_PropertyChanged;
            if (adapter.NeuralNetworkModel != null)
            {
                (d as NeuralNetworkControl)._networkView.NeuralNetworkModel = adapter.NeuralNetworkModel;
            }
        }

        private void Adapter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnAdapterPropertyChanged(sender as INeuralNetworkModelAdapter, e.PropertyName);
        }

        private void OnAdapterPropertyChanged(INeuralNetworkModelAdapter adapter, string propertyName)
        {
            if (propertyName == nameof(adapter.NeuralNetworkModel))
            {
                _networkView.NeuralNetworkModel = adapter.NeuralNetworkModel;
            }
        }

        private async void StartVis()
        {
            if (vis == null)
            {
                vis = new StateMachineVis<Triggers, States>(_stateMachine, loggingEnabled: true);

                await Task.Delay(2000);
                vis.Start(@"StateMachineLibVis.exe", "-c graphViz -l 0");
                _networkView.StartVis();
            }
        }

        public void UpdateSynapseLabels()
        {
            _networkView.RequestRedraw(NeuralNetworkController.ViewTrig.FORCE_DRAW_EXCLUDED);
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

        public void Zoom(double zoom)
        {
            _networkView.SetZoom(_networkView.View.Zoom + zoom);
            _stateMachine.Next(Triggers.ZOOM);
        }

        public void Reposition()
        {
            _networkView.Reposition();
            _stateMachine.Next(Triggers.REPOSITION);
        }
    }


    public class NeuronClickedEventArgs : RoutedEventArgs
    {
        public NeuronClickedEventArgs(ILayerModelAdapter layerAdapter, int neuronIndex) : base(NeuralNetworkControl.NeuronClickEvent)
        {
            LayerAdapter = layerAdapter;
            NeuronIndex = neuronIndex;
        }

        public ILayerModelAdapter LayerAdapter { get; }
        public int NeuronIndex { get; }
    }
}