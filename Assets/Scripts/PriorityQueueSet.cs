using System;
using System.Collections.Generic;
using C5;

public class PriorityQueueSet<T>
{
    Dictionary<T, IPriorityQueueHandle<ValueTuple<float, T>>> heapReferences;
    IntervalHeap<ValueTuple<float, T>> heap;

    public int Count => heap.Count;

    private class FloatTComparer : IComparer<ValueTuple<float, T>>
    {
        public int Compare(ValueTuple<float, T> x, ValueTuple<float, T> y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }

    public PriorityQueueSet()
    {
        heap = new IntervalHeap<ValueTuple<float, T>>(new FloatTComparer());
        heapReferences = new Dictionary<T, IPriorityQueueHandle<(float, T)>>();
    }

    public void Add(T key, float value)
    {
        ValueTuple<float, T> element = (value, key);
        if(!heapReferences.ContainsKey(key))
        {
            IPriorityQueueHandle<ValueTuple<float, T>> handle = null; 
            heap.Add(ref handle, element);
            heapReferences.Add(key, handle);
        }
        else
        {
            var heapHandle = heapReferences[key];
            heap.Find(heapHandle, out ValueTuple<float, T> heapElement);
            if(value < heapElement.Item1) 
            {
                IPriorityQueueHandle<ValueTuple<float, T>> handle = null; 
                heap.Delete(heapHandle);
                heapReferences.Remove(key);
                heap.Add(ref handle, element);
                heapReferences.Add(key, handle);
            }
        }
    }

    public T ExtractMin()
    {
        if(heap.Count == 0) return default(T);
        ValueTuple<float, T> minElement = heap.DeleteMin();
        heapReferences.Remove(minElement.Item2);
        return minElement.Item2;
    }
}
