using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
    /// <summary>
    /// An implementation of a min-Priority Queue using a heap.  Has O(1) .Contains()!
    /// See https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started for more information
    /// </summary>
    /// <typeparam name="T">The values in the queue.  Must extend the FastPriorityQueueNode class</typeparam>
    public sealed class FastPriorityQueue<T> : IFixedSizePriorityQueue<T, float>
        where T : FastPriorityQueueNode
    {
        private int m_NumNodes;
        private T[] m_Nodes;

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        public FastPriorityQueue(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
#endif

            m_NumNodes = 0;
            m_Nodes = new T[maxNodes + 1];
        }

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// O(1)
        /// </summary>
        public int Count => m_NumNodes;

        /// <summary>
        /// Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once Count == MaxSize),
        /// attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int MaxSize => m_Nodes.Length - 1;

        /// <summary>
        /// Removes every node from the queue.
        /// O(n) (So, don't do this often!)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            Array.Clear(m_Nodes, 1, m_NumNodes);
            m_NumNodes = 0;
        }

        /// <summary>
        /// Returns (in O(1)!) whether the given node is in the queue.
        /// If node is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(node) has been called
        /// O(1)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(T node)
        {
#if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.Queue != null && !Equals(node.Queue))
            {
                throw new InvalidOperationException("node.Contains was called on a node from another queue.  Please call originalQueue.ResetNode() first");
            }
            if(node.QueueIndex < 0 || node.QueueIndex >= m_Nodes.Length)
            {
                throw new InvalidOperationException("node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?");
            }
#endif

            return (m_Nodes[node.QueueIndex] == node);
        }

        /// <summary>
        /// Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken arbitrarily.
        /// If the queue is full, the result is undefined.
        /// If the node is already enqueued, the result is undefined.
        /// If node is or has been previously added to another queue, the result is undefined unless oldQueue.ResetNode(node) has been called
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Enqueue(T node, float priority)
        {
#if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if(m_NumNodes >= m_Nodes.Length - 1)
            {
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            }
            if (node.Queue != null && !Equals(node.Queue))
            {
                throw new InvalidOperationException("node.Enqueue was called on a node from another queue.  Please call originalQueue.ResetNode() first");
            }
            if (Contains(node))
            {
                throw new InvalidOperationException("Node is already enqueued: " + node);
            }
            node.Queue = this;
#endif

            node.Priority = priority;
            m_NumNodes++;
            m_Nodes[m_NumNodes] = node;
            node.QueueIndex = m_NumNodes;
            CascadeUp(node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeUp(T node)
        {
            //aka Heapify-up
            int parent;
            if(node.QueueIndex > 1)
            {
                parent = node.QueueIndex >> 1;
                var parentNode = m_Nodes[parent];
                if(HasHigherOrEqualPriority(parentNode, node))
                    return;

                //PathNode has lower priority value, so move parent down the heap to make room
                m_Nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            }
            else
            {
                return;
            }
            while(parent > 1)
            {
                parent >>= 1;
                var parentNode = m_Nodes[parent];
                if(HasHigherOrEqualPriority(parentNode, node))
                    break;

                //PathNode has lower priority value, so move parent down the heap to make room
                m_Nodes[node.QueueIndex] = parentNode;
                parentNode.QueueIndex = node.QueueIndex;

                node.QueueIndex = parent;
            }
            m_Nodes[node.QueueIndex] = node;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeDown(T node)
        {
            //aka Heapify-down
            var finalQueueIndex = node.QueueIndex;
            var childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if(childLeftIndex > m_NumNodes)
            {
                return;
            }

            // Check if the left-child is higher-priority than the current node
            var childRightIndex = childLeftIndex + 1;
            var childLeft = m_Nodes[childLeftIndex];
            if(HasHigherPriority(childLeft, node))
            {
                // Check if there is a right child. If not, swap and finish.
                if(childRightIndex > m_NumNodes)
                {
                    node.QueueIndex = childLeftIndex;
                    childLeft.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = childLeft;
                    m_Nodes[childLeftIndex] = node;
                    return;
                }
                // Check if the left-child is higher-priority than the right-child
                var childRight = m_Nodes[childRightIndex];
                if(HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if(childRightIndex > m_NumNodes)
            {
                return;
            }
            else
            {
                // Check if the right-child is higher-priority than the current node
                var childRight = m_Nodes[childRightIndex];
                if(HasHigherPriority(childRight, node))
                {
                    childRight.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else
                {
                    return;
                }
            }

            while(true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf node, we're done
                if(childLeftIndex > m_NumNodes)
                {
                    node.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = node;
                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = m_Nodes[childLeftIndex];
                if(HasHigherPriority(childLeft, node))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if(childRightIndex > m_NumNodes)
                    {
                        node.QueueIndex = childLeftIndex;
                        childLeft.QueueIndex = finalQueueIndex;
                        m_Nodes[finalQueueIndex] = childLeft;
                        m_Nodes[childLeftIndex] = node;
                        break;
                    }
                    // Check if the left-child is higher-priority than the right-child
                    var childRight = m_Nodes[childRightIndex];
                    if(HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.QueueIndex = finalQueueIndex;
                        m_Nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.QueueIndex = finalQueueIndex;
                        m_Nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if(childRightIndex > m_NumNodes)
                {
                    node.QueueIndex = finalQueueIndex;
                    m_Nodes[finalQueueIndex] = node;
                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    var childRight = m_Nodes[childRightIndex];
                    if(HasHigherPriority(childRight, node))
                    {
                        childRight.QueueIndex = finalQueueIndex;
                        m_Nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        node.QueueIndex = finalQueueIndex;
                        m_Nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(T higher, T lower)
        {
            return (higher.Priority < lower.Priority);
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherOrEqualPriority(node, node) (ie. both arguments the same node) will return true
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherOrEqualPriority(T higher, T lower)
        {
            return (higher.Priority <= lower.Priority);
        }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// If queue is empty, result is undefined
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T Dequeue()
        {
#if DEBUG
            if(m_NumNodes <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }

            if(!IsValidQueue())
            {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
            }
#endif

            var returnMe = m_Nodes[1];
            //If the node is already the last node, we can remove it immediately
            if(m_NumNodes == 1)
            {
                m_Nodes[1] = null;
                m_NumNodes = 0;
                return returnMe;
            }

            //Swap the node with the last node
            var formerLastNode = m_Nodes[m_NumNodes];
            m_Nodes[1] = formerLastNode;
            formerLastNode.QueueIndex = 1;
            m_Nodes[m_NumNodes] = null;
            m_NumNodes--;

            //Now bubble formerLastNode (which is no longer the last node) down
            CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        /// Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        /// Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        /// O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < m_NumNodes)
            {
                throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " + m_NumNodes + " nodes");
            }
#endif

            var newArray = new T[maxNodes + 1];
            var highestIndexToCopy = Math.Min(maxNodes, m_NumNodes);
            Array.Copy(m_Nodes, newArray, highestIndexToCopy + 1);
            m_Nodes = newArray;
        }

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// If the queue is empty, behavior is undefined.
        /// O(1)
        /// </summary>
        public T First
        {
            get
            {
#if DEBUG
                if(m_NumNodes <= 0)
                {
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
                }
#endif

                return m_Nodes[1];
            }
        }

        /// <summary>
        /// This method must be called on a node every time its priority changes while it is in the queue.  
        /// <b>Forgetting to call this method will result in a corrupted queue!</b>
        /// Calling this method on a node not in the queue results in undefined behavior
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(T node, float priority)
        {
#if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.Queue != null && !Equals(node.Queue))
            {
                throw new InvalidOperationException("node.UpdatePriority was called on a node from another queue");
            }
            if (!Contains(node))
            {
                throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + node);
            }
#endif

            node.Priority = priority;
            OnNodeUpdated(node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void OnNodeUpdated(T node)
        {
            //Bubble the updated node up or down as appropriate
            var parentIndex = node.QueueIndex >> 1;

            if(parentIndex > 0 && HasHigherPriority(node, m_Nodes[parentIndex]))
            {
                CascadeUp(node);
            }
            else
            {
                //Note that CascadeDown will be called if parentNode == node (that is, node is the root)
                CascadeDown(node);
            }
        }

        /// <summary>
        /// Removes a node from the queue.  The node does not need to be the head of the queue.  
        /// If the node is not in the queue, the result is undefined.  If unsure, check Contains() first
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Remove(T node)
        {
#if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.Queue != null && !Equals(node.Queue))
            {
                throw new InvalidOperationException("node.Remove was called on a node from another queue");
            }
            if (!Contains(node))
            {
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + node);
            }
#endif

            //If the node is already the last node, we can remove it immediately
            if(node.QueueIndex == m_NumNodes)
            {
                m_Nodes[m_NumNodes] = null;
                m_NumNodes--;
                return;
            }

            //Swap the node with the last node
            var formerLastNode = m_Nodes[m_NumNodes];
            m_Nodes[node.QueueIndex] = formerLastNode;
            formerLastNode.QueueIndex = node.QueueIndex;
            m_Nodes[m_NumNodes] = null;
            m_NumNodes--;

            //Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            OnNodeUpdated(formerLastNode);
        }

        /// <summary>
        /// By default, nodes that have been previously added to one queue cannot be added to another queue.
        /// If you need to do this, please call originalQueue.ResetNode(node) before attempting to add it in the new queue
        /// If the node is currently in the queue or belongs to another queue, the result is undefined
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void ResetNode(T node)
        {
#if DEBUG
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.Queue != null && !Equals(node.Queue))
            {
                throw new InvalidOperationException("node.ResetNode was called on a node from another queue");
            }
            if (Contains(node))
            {
                throw new InvalidOperationException("node.ResetNode was called on a node that is still in the queue");
            }

            node.Queue = null;
#endif

            node.QueueIndex = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
#if NET_VERSION_4_5 // ArraySegment does not implement IEnumerable before 4.5
            IEnumerable<T> e = new ArraySegment<T>(_nodes, 1, _numNodes);
            return e.GetEnumerator();
#else
            for(var i = 1; i <= m_NumNodes; i++)
                yield return m_Nodes[i];
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// <b>Should not be called in production code.</b>
        /// Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue()
        {
            for(var i = 1; i < m_Nodes.Length; i++)
            {
                if(m_Nodes[i] != null)
                {
                    var childLeftIndex = 2 * i;
                    if(childLeftIndex < m_Nodes.Length && m_Nodes[childLeftIndex] != null && HasHigherPriority(m_Nodes[childLeftIndex], m_Nodes[i]))
                        return false;

                    var childRightIndex = childLeftIndex + 1;
                    if(childRightIndex < m_Nodes.Length && m_Nodes[childRightIndex] != null && HasHigherPriority(m_Nodes[childRightIndex], m_Nodes[i]))
                        return false;
                }
            }
            return true;
        }
    }
}