/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System;
using System.Collections;

namespace Nirge.Core
{
    public class CArrayLinkedList<T> : IEnumerable<T>, IEnumerable
    {
        class CNode
        {
            T _val;
            int _prev;
            int _next;
            bool _use;

            public T Val
            {
                get
                {
                    return _val;
                }
                set
                {
                    _val = value;
                }
            }

            public int Prev
            {
                get
                {
                    return _prev;
                }
                set
                {
                    _prev = value;
                }
            }

            public int Next
            {
                get
                {
                    return _next;
                }
                set
                {
                    _next = value;
                }
            }

            public bool Use
            {
                get
                {
                    return _use;
                }
                set
                {
                    _use = value;
                }
            }

            public CNode()
            {
                Clear();
            }

            public void Clear()
            {
                _val = default(T);
                _prev = _next = -1;
                _use = false;
            }
        }

        CNode[] _nodes;
        int _head;
        int _tail;
        int _free;
        int _count;

        public int Capacity
        {
            get
            {
                return _nodes.Length;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _count == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return _count == _nodes.Length;
            }
        }

        public IEnumerable<int> Indexs
        {
            get
            {
                for (int i = _head; i != -1;)
                {
                    var v = _nodes[i].Next;
                    yield return i;
                    i = v;
                }
            }
        }

        public CArrayLinkedList(int capacity)
        {
            if (capacity == 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _nodes = new CNode[capacity];
            for (int i = 0; i < capacity; ++i)
                _nodes[i] = new CNode();
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < Capacity; ++i)
            {
                _nodes[i].Val = default(T);
                _nodes[i].Prev = -1;
                _nodes[i].Next = i + 1;
                _nodes[i].Use = false;
            }
            _nodes[Capacity - 1].Next = -1;

            _head = _tail = -1;
            _free = 0;
            _count = 0;
        }

        public T GetFirst()
        {
            if (_count == 0)
                return default(T);
            return _nodes[_head].Val;
        }

        public T GetLast()
        {
            if (_count == 0)
                return default(T);
            return _nodes[_tail].Val;
        }

        public int AddLast(T val)
        {
            if (_count == Capacity)
                return -1;

            if (_count == 0)
            {
                _head = _tail = _free;
                _nodes[_tail].Prev = -1;
            }
            else
            {
                _nodes[_tail].Next = _free;
                _nodes[_free].Prev = _tail;
                _tail = _free;
            }
            _free = _nodes[_tail].Next;
            _nodes[_tail].Next = -1;

            _nodes[_tail].Val = val;
            _nodes[_tail].Use = true;

            ++_count;
            return _tail;
        }

        public void Remove(int index)
        {
            if (_count == 0)
                throw new InvalidOperationException(nameof(index));
            if (index < 0 || index >= Capacity)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (!_nodes[index].Use)
                throw new InvalidOperationException(nameof(index));

            erase(index);
        }
        public void Remove(T val)
        {
            if (_count == 0)
                throw new InvalidOperationException(nameof(val));

            for (int i = _head; i != -1; i = _nodes[i].Next)
            {
                if (_nodes[i].Use
                        && (object)val == (object)_nodes[i].Val)
                {
                    erase(i);
                    return;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(val));
        }

        void erase(int i)
        {
            if (_count == 1)
            {
                _head = -1;
                _tail = -1;
            }
            else
            {
                if (i == _head)
                {
                    _head = _nodes[i].Next;
                    _nodes[_head].Prev = -1;
                }
                else if (i == _tail)
                {
                    _tail = _nodes[i].Prev;
                    _nodes[_tail].Next = -1;
                }
                else
                {
                    _nodes[_nodes[i].Prev].Next = _nodes[i].Next;
                    _nodes[_nodes[i].Next].Prev = _nodes[i].Prev;
                }
            }

            _nodes[i].Use = false;
            _nodes[i].Next = _free;
            _free = i;

            --_count;
        }

        public T PopFirst()
        {
            if (_count == 0)
                return default(T);

            int i = _head;

            if (_count == 1)
            {
                _head = -1;
                _tail = -1;
            }
            else
            {
                _head = _nodes[i].Next;
                _nodes[_head].Prev = -1;
            }

            _nodes[i].Use = false;
            _nodes[i].Next = _free;
            _free = i;

            --_count;
            return _nodes[i].Val;
        }

        public T GetVal(int index)
        {
            if (_count == 0)
                throw new InvalidOperationException(nameof(index));
            if (index < 0 || index >= Capacity)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (!_nodes[index].Use)
                throw new InvalidOperationException(nameof(index));

            return _nodes[index].Val;
        }

        public bool TryGetVal(int index, out T val)
        {
            if (_count > 0
                && index >= 0 && index < Capacity
                && _nodes[index].Use)
            {
                val = _nodes[index].Val;
                return true;
            }
            else
            {
                val = default(T);
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _head; i != -1;)
            {
                var v = _nodes[i].Next;
                yield return _nodes[i].Val;
                i = v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
