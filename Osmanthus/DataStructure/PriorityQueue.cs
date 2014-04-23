using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmanthus.DataStructure
{
    /// <summary>
    /// A generic largest first priority queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        private const int DefaultCapacity = 8;

        private IComparer<T> _comparer;
        private T[] _heap;

        /// <summary>
        /// The capacity the queue holds
        /// </summary>
        public int Capacity { get { return _heap.Length; } }
        /// <summary>
        /// Current data items' count in the priority queue
        /// </summary>
        public int Count { get; internal set; }
        /// <summary>
        /// The largest data item int the priority queue
        /// </summary>
        public T Top { get { return _heap[0]; } }
        /// <summary>
        /// If the queue is empty
        /// </summary>
        public bool IsEmpty { get { return Count == 0; } }

        #region Ctors.
        /// <summary>
        /// A priority queue with default comparer of T, and its initial capacity is 8
        /// </summary>
        public PriorityQueue()
            : this(DefaultCapacity, null)
        { }
        /// <summary>
        /// A priority queue with default comparer of T
        /// </summary>
        /// <param name="capacity">The initial capacity of the priority queue</param>
        public PriorityQueue(int capacity)
            : this(capacity, null)
        { }
        /// <summary>
        /// A priority queue with default capacity 8
        /// </summary>
        /// <param name="comparer">The comparer of the data items</param>
        public PriorityQueue(IComparer<T> comparer)
            : this(DefaultCapacity, comparer)
        { }
        /// <summary>
        /// A priority queue
        /// </summary>
        /// <param name="capacity">The initial capacity of the priority queue</param>
        /// <param name="comparer">The comparer of the data items</param>
        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _heap = new T[capacity <= 0 ? DefaultCapacity : capacity];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Push a new item into the queue
        /// </summary>
        /// <param name="val">Data</param>
        public void Push(T val)
        {
            if (Count >= _heap.Length)
                Array.Resize(ref _heap, _heap.Length << 1);
            _heap[Count] = val;
            SiftUp(Count++);
        }

        /// <summary>
        /// Pop and return the largest item from the queue
        /// </summary>
        /// <returns>The largest value in the queue, if the queue is empty, this will be random</returns>
        public T Pop()
        {
            T val = Top;
            _heap[0] = _heap[--Count];
            SiftDown(0);
            return val;
        }

        /// <summary>
        /// Remove all items from the queue
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }
        #endregion

        #region Internal Functions
        /// <summary>
        /// Adjust the heap from the i-th item to up
        /// </summary>
        /// <param name="i"></param>
        private void SiftUp(int i)
        {
            T val = _heap[i];
            for (int j = (i - 1) >> 1; i > 0 && _comparer.Compare(val, _heap[j]) > 0; i = j, j = (j - 1) >> 1)
                _heap[i] = _heap[j];
            _heap[i] = val;
        }

        /// <summary>
        /// Adjust the heap from the i-th item to down
        /// </summary>
        /// <param name="i"></param>
        private void SiftDown(int i)
        {
            T val = _heap[i];
            for (int j = i << 1 | 1; j < Count; i = j, j = j << 1 | 1)
            {
                if (j + 1 < Count && _comparer.Compare(_heap[j + 1], _heap[j]) > 0)
                    ++j;
                if (_comparer.Compare(val, _heap[j]) >= 0)
                    break;
                _heap[i] = _heap[j];
            }
            _heap[i] = val;

        }
        #endregion
    }
}
