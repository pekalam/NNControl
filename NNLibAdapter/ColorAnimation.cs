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
using Microsoft.VisualBasic;

namespace NNLibAdapter
{
    public enum ColorAnimationMode
    {
        Layer,Network,
    }

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

        public ColorAnimationMode AnimationMode { get; set; }
        public bool IsAnimating { get; set; }

        public void SetupTrainer(MLPTrainer trainer)
        {
            _trainer = trainer;
            _trainer.EpochEnd += UpdateColors;
            IsAnimating = true;
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
            IsAnimating = true;
        }

        public void SetupTrainer(MLPTrainer trainer,ref Action epochEndAction, Action<Action> colorUpdateHighOrder)
        {
            _trainer = trainer;
            _colorUpdateHighOrder = colorUpdateHighOrder;

            epochEndAction += UpdateColors;
            IsAnimating = true;
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
            IsAnimating = false;
        }


        private void UpdateColors()
        {
            var layers = _trainer.Network.Layers;
            if(AnimationMode == ColorAnimationMode.Layer)
            {
                SetColorsPerLayer(layers);
            }
            else
            {
                SetColorsPerNetwork(layers);
            }
        }


        private (double mingz, double maxgz, double minlz, double maxlz) FindMinMax(Layer layer)
        {
            double mingz = 0, maxgz = 0, minlz = 0, maxlz = 0;

            for (int i = 0; i < layer.Weights.RowCount; i++)
            {
                for (int j = 0; j < layer.Weights.ColumnCount; j++)
                {
                    var el = layer.Weights.At(i, j);
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
                var el = layer.Biases.At(i, 0);
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

        public void SetColorsPerLayer(IReadOnlyList<PerceptronLayer> layers)
        {
            var color = _controller.Color;

            for (int i = 0; i < layers.Count; i++)
            {
                var (_, maxgz, minlz, _) = FindMinMax(layers[i]);
                var (_, maxbgz, minblz, _) = FindMinMaxB(layers[i]);

                for (int j = 0; j < layers[i].Weights.ColumnCount; j++)
                {
                    for (int k = 0; k < layers[i].Weights.RowCount; k++)
                    {
                        if (layers[i].Weights.At(k, j) < 0)
                        {
                            color.SetSynapseColor(i + 1, k, j, Scale(layers[i].Weights.At(k, j), minlz, 0, 0, 126));
                        }
                        else
                        {
                            color.SetSynapseColor(i + 1, k, j, Scale(layers[i].Weights.At(k, j), 0, maxgz, 0, 126, 128));
                        }
                    }
                }

                for (int j = 0; j < layers[i].Biases.RowCount; j++)
                {
                    if (layers[i].Biases.At(j, 0) < 0)
                    {
                        color.SetNeuronColor(i + 1, j, Scale(layers[i].Biases.At(j, 0), minblz, 0, 0, 126));
                    }
                    else
                    {
                        color.SetNeuronColor(i + 1, j, Scale(layers[i].Biases.At(j, 0), 0, maxbgz, 0, 126, 128));
                    }
                }
            }


            if (_colorUpdateHighOrder != null)
            {
                _colorUpdateHighOrder(_controller.Color.ApplyColors);
            }
            else
            {
                Application.Current?.Dispatcher.InvokeAsync(_controller.Color.ApplyColors, DispatcherPriority.Background);
            }

        }

        public void SetColorsPerNetwork(IReadOnlyList<PerceptronLayer> layers)
        {
            var color = _controller.Color;
            double mingz=double.MinValue, maxgz=double.MinValue, minlz=double.MinValue, maxlz=double.MaxValue;
            double minbgz = double.MinValue, maxbgz = double.MaxValue, minblz = double.MinValue, maxblz = double.MaxValue;

            for (int i = 0; i < layers.Count; i++)
            {
                var (nmingz, nmaxgz, nminlz, nmaxlz) = FindMinMax(layers[i]);
                if (nmingz < mingz) mingz = nmingz;
                if (nmaxgz > maxgz) maxgz = nmaxgz;
                if (nminlz < minlz) minlz = nminlz;
                if (nmaxlz > maxlz) maxlz = nmaxlz;


                var (nminbgz, nmaxbgz, nminblz, nmaxblz) = FindMinMaxB(layers[i]);
                if (nminbgz < minbgz) minbgz = nminbgz;
                if (nmaxbgz > maxbgz) maxbgz = nmaxbgz;
                if (nminblz < minblz) minblz = nminblz;
                if (nmaxblz > maxblz) maxblz = nmaxblz;
            }

            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].Weights.ColumnCount; j++)
                {
                    for (int k = 0; k < layers[i].Weights.RowCount; k++)
                    {
                        if (layers[i].Weights.At(k, j) < 0)
                        {
                            color.SetSynapseColor(i + 1, k, j, Scale(layers[i].Weights.At(k, j), minlz, 0, 0, 126));
                        }
                        else
                        {
                            color.SetSynapseColor(i + 1, k, j, Scale(layers[i].Weights.At(k, j), 0, maxgz, 0, 126, 128));
                        }
                    }
                }

                for (int j = 0; j < layers[i].Biases.RowCount; j++)
                {
                    if (layers[i].Biases.At(j, 0) < 0)
                    {
                        color.SetNeuronColor(i + 1, j, Scale(layers[i].Biases.At(j, 0), minblz, 0, 0, 126));
                    }
                    else
                    {
                        color.SetNeuronColor(i + 1, j, Scale(layers[i].Biases.At(j, 0), 0, maxbgz, 0, 126, 128));
                    }
                }
            }

            if (_colorUpdateHighOrder != null)
            {
                _colorUpdateHighOrder(_controller.Color.ApplyColors);
            }
            else
            {
                Application.Current?.Dispatcher.InvokeAsync(_controller.Color.ApplyColors, DispatcherPriority.Background);
            }
        }
    }
}