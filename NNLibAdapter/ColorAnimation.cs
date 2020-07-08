using NNControl.Network;
using NNLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Threading;

namespace NNLibAdapter
{
    public class ColorAnimation
    {
        private readonly ReplaySubject<int> _sub = new ReplaySubject<int>(10);
        private IDisposable? _epochSubscr;
        private Action<Action>? _colorUpdateHighOrder;

        private NeuralNetworkController _controller;
        private MLPTrainer _trainer;

        public ColorAnimation(NeuralNetworkController controller)
        {
            _controller = controller;
        }

        public void SetupTrainer(MLPTrainer trainer)
        {
            _trainer = trainer;
            _trainer.EpochEnd += UpdateColors;
        }

        public void SetupTrainer(MLPTrainer trainer, TimeSpan delay)
        {
            _trainer = trainer;
            _trainer.EpochEnd += TrainerOnEpochEnd;

            _epochSubscr = _sub
                .Buffer(timeSpan: delay, count: 20)
                .DelaySubscription(delay)
                .SubscribeOn(Scheduler.Default)
                .Subscribe(ints =>
                {
                    if (ints.Count == 0)
                    {
                        return;
                    }
                    UpdateColors();
                });
        }

        public void SetupTrainer(MLPTrainer trainer, TimeSpan delay,ref Action epochEndAction, Action<Action> colorUpdateHighOrder)
        {
            _trainer = trainer;
            _colorUpdateHighOrder = colorUpdateHighOrder;

            epochEndAction += UpdateColors;
        }

        private void TrainerOnEpochEnd()
        {
            _sub.OnNext(0);
        }

        public void StopAnimation(bool resetColors)
        {
            //flush
            UpdateColors();
            _epochSubscr?.Dispose();
            _trainer.EpochEnd -= TrainerOnEpochEnd;
            if (resetColors)
            {
                _controller.Color.ResetColorsToDefault();
            }
        }


        private void UpdateColors()
        {
            var layers = _trainer.Network.Layers;
            SetColors(layers);
        }


        private (double mingz, double maxgz, double minlz, double maxlz) FindMinMax(Layer layer)
        {
            double mingz = 0, maxgz = 0, minlz = 0, maxlz = 0;

            for (int i = 0; i < layer.Weights.RowCount; i++)
            {
                for (int j = 0; j < layer.Weights.ColumnCount; j++)
                {
                    var el = layer.Weights[i, j];
                    if (el < 0)
                    {
                        if (el < minlz)
                        {
                            minlz = el;
                        }

                        if (el > maxlz)
                        {
                            maxlz = el;
                        }
                    }
                    else
                    {
                        if (el < mingz)
                        {
                            mingz = el;
                        }

                        if (el > maxgz)
                        {
                            maxgz = el;
                        }
                    }
                }
            }




            return (mingz, maxgz, minlz, maxlz);
        }

        private (double mingz, double maxgz, double minlz, double maxlz) FindMinMaxB(Layer layer)
        {
            double mingz = 0, maxgz = 0, minlz = 0, maxlz = 0;

            for (int i = 0; i < layer.Biases.RowCount; i++)
            {
                var el = layer.Biases[i, 0];
                if (el < 0)
                {
                    if (el < minlz)
                    {
                        minlz = el;
                    }

                    if (el > maxlz)
                    {
                        maxlz = el;
                    }
                }
                else
                {
                    if (el < mingz)
                    {
                        mingz = el;
                    }

                    if (el > maxgz)
                    {
                        maxgz = el;
                    }
                }
            }


            return (mingz, maxgz, minlz, maxlz);
        }

        private static int i = 128;

        private int Scale(double x, double minx, double maxx, int miny, int maxy, int start = 0)
        {
            // var r = new Random().Next(0, 6);
            // return r == 1 ? 126 : (r == 2 ? 128 : (r == 3 ? 127 : (r == 4 ? 0 : (r == 5 ? 254 : 255))));


            if (!double.IsFinite(x) || x == 0.0d)
            {
                return 127;
            }

            var s = (int) ((x - minx) * (maxy - miny) / (maxx - minx)) + start;
            if (s > start + maxy)
            {
                s = start + maxy;
            }
            else if (s < miny + start)
            {
                s = start + miny;
            }
            //Debug.WriteLine($"{s} for {x} max:{maxx} min: {minx}");
            // if (s >= 126 || s <= 128)
            // {
            //     Debug.WriteLine($"white for {x} max:{maxx} min: {minx}");
            // }
            return s;
        }

        protected virtual void SetColors(IReadOnlyList<PerceptronLayer> layers)
        {
            var color = _controller.Color;

            for (int i = 0; i < layers.Count; i++)
            {
                var (mingz, maxgz, minlz, maxlz) = FindMinMax(layers[i]);

                for (int j = 0; j < layers[i].Weights.RowCount; j++)
                {
                    for (int k = 0; k < layers[i].Weights.ColumnCount; k++)
                    {
                        var el = layers[i].Weights[j, k];
                        if (el < 0)
                        {
                            color.SetSynapseColor(i + 1, j, k, Scale(el, minlz, 0, 0, 126));
                        }
                        else
                        {
                            color.SetSynapseColor(i + 1, j, k, Scale(el, 0, maxgz, 0, 126, 128));
                        }
                    }
                }
            }


            for (int i = 0; i < layers.Count; i++)
            {
                var (minbgz, maxbgz, minblz, maxblz) = FindMinMaxB(layers[i]);

                for (int j = 0; j < layers[i].Biases.RowCount; j++)
                {
                    var el = layers[i].Biases[j, 0];
                    if (el < 0)
                    {
                        color.SetNeuronColor(i + 1, j, Scale(el, minblz, 0, 0, 126));
                    }
                    else
                    {
                        color.SetNeuronColor(i + 1, j, Scale(el, 0, maxbgz, 0, 126, 128));
                    }
                }
            }


            if (_colorUpdateHighOrder != null)
            {
                _colorUpdateHighOrder(() => color.ApplyColors());
            }
            else
            {
                Application.Current?.Dispatcher.InvokeAsync(() => color.ApplyColors(), DispatcherPriority.Background);
            }

        }
    }
}