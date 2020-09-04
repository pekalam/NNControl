using System;
using System.Runtime.CompilerServices;
using NNControl.Neuron;

namespace NNControl.Synapse
{
    public class SynapseData
    {
        private const int DefaultSz = 100_000;
        private const int ResizeSz = 100_000;

        private int _count;

        /// <summary>
        /// Contains data required to draw for all synapses.
        /// Layout:
        /// [ X,Y, ArrowBeg.x, ArrowBeg.y, ArrowEnd.x, ArrowEnd.y, ArrowLeftEnd.x, ArrowLeftEnd.y, ArrowRightEnd.x, ArrowRightEnd.y, X, Y...]
        /// </summary>
        public float[] Data = new float[DefaultSz];


        /// <summary>
        /// Returns id of added
        /// </summary>
        /// <returns></returns>
        public int AddSynapse()
        {
            if (_count * 10 >= Data.Length)
            {
                var newData = new float[Data.Length + ResizeSz];
                Array.Copy(Data, newData, Data.Length);
                Data = newData;
            }

            return _count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetX(int id) => Data[id*10];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetX(int id, float x) => Data[id*10] = x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetY(int id) => Data[id*10 + 1];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetY(int id, float y) => Data[id*10 + 1] = y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowBegX(int id) => Data[id*10 + 2];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowBegX(int id, float x) => Data[id*10 + 2] = x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowBegY(int id) => Data[id*10 + 3];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowBegY(int id, float x) => Data[id*10 + 3] = x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowEndX(int id) => Data[id*10 + 4];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowEndX(int id, float x) => Data[id*10 + 4] = x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowEndY(int id) => Data[id*10 + 5];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowEndY(int id, float x) => Data[id*10 + 5] = x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowLeftX(int id) => Data[id*10 + 6];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowLeftX(int id, float x) => Data[id*10 + 6] = x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowLeftY(int id) => Data[id*10 + 7];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowLeftY(int id, float x) => Data[id*10 + 7] = x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowRightX(int id) => Data[id*10 + 8];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowRightX(int id, float x) => Data[id*10 + 8] = x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetArrowRightY(int id) => Data[id*10 + 9];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SetArrowRightY(int id, float x) => Data[id*10 + 9] = x;

    }

    public abstract class SynapseView
    {
        public int NumberInNeuron;
        public bool Excluded;
        public NeuronView Neuron1;
        public NeuronView Neuron2;

        public int Id;
        public SynapseData SynapseData;

        public (float x, float y) ArrowLeftEnd;
        public (float x, float y) ArrowRightEnd;
        public (float x, float y) ArrowEnd;
        public (float x, float y) ArrowBeg;


        public abstract bool Contains(float x, float y);
        public abstract void ResetColor(string hexColor);
        public abstract void SetColor(int scale);
    }
}