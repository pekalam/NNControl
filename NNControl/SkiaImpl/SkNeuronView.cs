﻿using NeuralNetworkControl.Impl;
using SkiaSharp;

namespace NeuralNetworkControl.SkiaImpl
{
    internal class SkNeuronView : NeuronViewImpl
    {
        private float _x;
        private float _y;

        private int Radius => Layer.Network.NeuralNetworkModel.NeuronRadius;

        public override float X
        {
            get => _x;
            set
            {
                _x = value;

                ReferenceRect.Left = (X - Radius) * (float)(Layer.Network.Zoom + 1);
                ReferenceRect.Right = (X + Radius) * (float)(Layer.Network.Zoom + 1);
            }
        }
        public override float Y
        {
            get => _y;
            set
            {
                _y = value;

                ReferenceRect.Top = (Y - Radius) * (float)(Layer.Network.Zoom + 1);
                ReferenceRect.Bottom = (Y + Radius) * (float)(Layer.Network.Zoom + 1);
            }
        }

        public override void OnRepositioned()
        {
            if (Layer.Network.Zoom == 0)
            {
                ReferenceRect.Left = X - Radius;
                ReferenceRect.Right = X + Radius;
                ReferenceRect.Top = Y - Radius;
                ReferenceRect.Bottom = Y + Radius;
            }
        }

        public override void OnZoomChanged()
        {
            ReferenceRect.Left = (X - Radius) * (float)(Layer.Network.Zoom + 1);
            ReferenceRect.Right = (X + Radius) * (float)(Layer.Network.Zoom + 1);
            ReferenceRect.Top = (Y - Radius) * (float)(Layer.Network.Zoom + 1);
            ReferenceRect.Bottom = (Y + Radius) * (float)(Layer.Network.Zoom + 1);
        }

        public override bool Contains(float x, float y)
        {
            return ReferenceRect.Contains(x, y);
        }

        public override SynapseViewImpl CreateSynapseImpl()
        {
            return new SkSynapseView();
        }

        internal SKRect ReferenceRect;
    }
}