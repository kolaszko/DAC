﻿using System.Collections.Generic;

namespace SMProject
{
    public class CircularBuffer<T>
    {
        public readonly Queue<T> DataBuffer;
        public int Length;

        private bool directionSwitcher;

        public CircularBuffer(int length)
        {
            DataBuffer = new Queue<T>();
            Length = length;
        }


        public void Add(T data)
        {
            if (DataBuffer.Count == Length) directionSwitcher = true;

            if (DataBuffer.Count == 0) directionSwitcher = false;

            if (!directionSwitcher)
                DataBuffer.Enqueue(data);
            else
                DataBuffer.Dequeue();
        }

        public void Clear()
        {
            this.DataBuffer.Clear();
        }
    }
}