using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pliant.Grammars;

namespace Pliant.Collections
{
    public class NonOverlappingIntervalSet<T> : IEnumerable<NonOverlappingIntervalSet<T>.IntervalNode>
    {
        private IntervalNode _firstNode;

        public NonOverlappingIntervalSet()
        {
            //Always sorted left to right
            _firstNode = null;
        }

        public void AddInterval(Interval interval, params T[] intervalAssociatedItems)
        {
            AddInterval(interval, intervalAssociatedItems.AsEnumerable());
        }

        /// <summary>
        /// Adds the specified interval with the specified associated items to this set - making sure that the resulting set doesn't have any overlapping intervals.
        /// If an interval would result in overlap - the interval and the target interval causing this conflict are split into one common Intersection interval and
        /// a Left and Right remainder interval - but keeping the correct associated items with each of the intervals after the split.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="intervalAssociatedItems"></param>
        public void AddInterval(Interval interval, IEnumerable<T> intervalAssociatedItems)
        {
            //Prepare the node that will be causing a split
            var splitNode = new IntervalNode(interval, intervalAssociatedItems);

            //If we don't have anything yet, just set this node as the first
            if (_firstNode == null)
            {
                _firstNode = splitNode;
                return;
            }

            //Start with the first node and keep evaluating against the "splitNode" until either there are no more interval to evaluate against
            //or there is no "right" remainder left - each "right" remainder is set as the next "splitNode" for the next iteration
            var node = _firstNode;
            while (node != null)
            {
                //Start by evaluating a split
                var split = new IntervalSplit(node, splitNode);
                if (!split.IsSplit)
                {
                    //No split was performed
                    if (node.Interval.Max > splitNode.Interval.Max)
                    {
                        //The splitNode is actually positioned to the left of the node... so we can just insert it to the left and stop!
                        node.InsertLeft(splitNode);
                        if (node == _firstNode)
                            _firstNode = splitNode;
                        return;
                    }

                    if (node.Next == null)
                    {
                        //There is nothing to the right... so just insert to the right and stop!
                        node.InsertRight(splitNode);
                        return;
                    }

                    //Second is to the right... so we must continue evaluating the split node against the "next" interval that is positioned to the right
                    node = node.Next;
                }
                else
                {
                    //A split has occured - we will now handle the resulting Intersection and Left/Right remainder Interval parts
                    var hasRemovedNode = false;

                    //Handle the left part
                    var left = split.Left;
                    if (left != null)
                    {
                        //Insert to the left
                        node.InsertLeft(left);
                        hasRemovedNode = RemoveSwapAndEnsureFirst(ref node, left, !hasRemovedNode);
                    }

                    var intersection = split.Intersection;
                    if (intersection != null)
                    {
                        node.InsertRight(intersection);
                        hasRemovedNode = RemoveSwapAndEnsureFirst(ref node, intersection, !hasRemovedNode);
                    }

                    //If we have no right split, we can stop here, there is no more Interval to evaluate/split
                    var right = split.Right;
                    if (right == null)
                        return;

                    //Set the the right as the next split node to be evaluated
                    splitNode = right;

                    //If there is no more nodes to evaluate against, jsut add to the right as the last node and stop here
                    if (node.Next == null)
                    {
                        //There is nothing to the right... so just insert
                        node.InsertRight(splitNode);
                        return;
                    }

                    //Move to the next node to be evaluated against
                    node = node.Next;
                }
            }
        }

        private bool RemoveSwapAndEnsureFirst(ref IntervalNode node, IntervalNode second, bool doRemove)
        {
            var removed = false;

            if (doRemove)
            {
                node.Remove();
                removed = true;

                if (node == _firstNode)
                {
                    _firstNode = second;
                }
            }

            node = second;

            return removed;
        }

        public IEnumerator<IntervalNode> GetEnumerator()
        {
            return new Enumerator(_firstNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var node = _firstNode;
            while (node != null)
            {
                sb.Append(node + ", ");
                node = node.Next;
            }

            sb.Length -= 2;

            return sb.ToString();
        }


        /// <summary>
        /// Interval split that will yield non overlapping intervals given two interfals, "first" and "second"
        /// (Left,Right) if no overlap
        /// (Middle, Right) if overlap and Min is same for both first and second
        /// (Left, Middle) if overlap and Max is same for both first and second
        /// (Left, Middle, Right) if overlap and there are remainders on both sides
        /// </summary>
        private class IntervalSplit
        {
            public IntervalSplit(IntervalNode first, IntervalNode second)
            {
                First = first;
                Second = second;

                Split();
            }

            /// <summary>
            /// The first inverval that is to be used for the split
            /// </summary>
            public IntervalNode First { get; set; }

            /// <summary>
            /// The second interval that is to be used for the split
            /// </summary>
            public IntervalNode Second { get; set; }

            public bool IsSplit { get; set; }

            /// <summary>
            /// The left remainder interval after the split, keeps associated items from either first or second, but not both
            /// </summary>
            public IntervalNode Left { get; set; }

            /// <summary>
            /// The overlapping intersecting part after the split - keeps associated items from both first and second
            /// </summary>
            public IntervalNode Intersection { get; set; }

            /// <summary>
            /// The right remainder interval after the split, keeps associated items from either first or second, but not both
            /// </summary>
            public IntervalNode Right { get; set; }

            /// <summary>
            /// Splits the intervals
            /// </summary>
            private void Split()
            {
                var first = First;
                var second = Second;

                //Getthe intervals to split
                var firstInterval = first.Interval;
                var secondInterval = second.Interval;

                //Make sure there is an overlap!
                var overlaps = firstInterval.Overlaps(secondInterval);
                if (!overlaps)
                    return;

                //We have a split!
                IsSplit = true;

                //Create the intersection, and then use the "potential" left/right remainder to creating the left and right intervals
                IntervalNode leftTarget, rightTarget;
                Intersection = CreateIntersection(first, second, out leftTarget, out rightTarget);

                //Create the left remainder interval
                CreateLeftRemainderInterval(leftTarget);

                //Create the right remainder interval
                CreateRightRemainderInterval(rightTarget);
            }

            private IntervalNode CreateIntersection(IntervalNode first, IntervalNode second, out IntervalNode leftTarget, out IntervalNode rightTarget)
            {
                //Left and right remainder "targets"
                leftTarget = null;
                rightTarget = null;

                var firstInterval = first.Interval;
                var secondInterval = second.Interval;

                //Eval the min intersection point - see Interval.Split
                var intersectMin = firstInterval.Min;
                leftTarget = second;
                if (secondInterval.Min > intersectMin)
                {
                    leftTarget = first;
                    intersectMin = secondInterval.Min;
                }
                if (secondInterval.Min == firstInterval.Min)
                    leftTarget = null;

                //Eval the max intersection point - see Interval.Split
                var intersectMax = firstInterval.Max;
                rightTarget = second;
                if (secondInterval.Max < intersectMax)
                {
                    rightTarget = first;
                    intersectMax = secondInterval.Max;
                }
                if (secondInterval.Max == firstInterval.Max)
                    rightTarget = null;

                //Create the intersection
                var intersectInterval = new Interval(intersectMin, intersectMax);
                var intersection = new IntervalNode(intersectInterval);

                //Intersection always keeps associated items from both first and second interval!
                intersection.AddItems(first.AssociatedItems);
                intersection.AddItems(second.AssociatedItems);

                return intersection;
            }

            private void CreateLeftRemainderInterval(IntervalNode leftTarget)
            {
                if (leftTarget == null || leftTarget.IntervalEquals(Intersection))
                    return;

                //Create left remainder interval given the leftTarget node as the expected left one - but depending on the min/max, it could end up with the Intersection
                Left = CreateLeftRemainderInterval(leftTarget, Intersection);
                if (leftTarget.Interval.Min <= Intersection.Interval.Min)
                    Left.AddItems(leftTarget.AssociatedItems);
            }

            private void CreateRightRemainderInterval(IntervalNode rightTarget)
            {
                if (rightTarget == null || rightTarget.IntervalEquals(Intersection))
                    return;

                //Create left remainder interval given the rightTarget node as the expected right one - but depending on the min/max, it could end up with the Intersection
                Right = CreateRightRemainderInterval(Intersection, rightTarget);
                if (rightTarget.Interval.Max >= Intersection.Interval.Max)
                    Right.AddItems(rightTarget.AssociatedItems);
            }

            private IntervalNode CreateLeftRemainderInterval(IntervalNode first, IntervalNode second)
            {
                var firstInterval = first.Interval;
                var secondInterval = second.Interval;

                var localMin = firstInterval.Min;
                var localMax = secondInterval.Min;
                if (secondInterval.Min < localMin)
                {
                    localMax = localMin;
                    localMin = secondInterval.Min;
                }
                if (localMin != localMax)
                    localMax = (char)(localMax - 1);

                var remainderInterval = new Interval(localMin, localMax);
                return new IntervalNode(remainderInterval);
            }

            private IntervalNode CreateRightRemainderInterval(IntervalNode first, IntervalNode second)
            {
                var firstLocal = second.Interval;
                var secondLocal = first.Interval;

                var localMin = firstLocal.Max;
                var localMax = secondLocal.Max;
                if (secondLocal.Max < localMin)
                {
                    localMax = localMin;
                    localMin = secondLocal.Max;
                }
                if (localMin != localMax)
                    localMin = (char)(localMin + 1);

                var remainderInterval = new Interval(localMin, localMax);
                return new IntervalNode(remainderInterval);
            }
        }

        /// <summary>
        /// Internal linked list node representing an Interval. Also keeps the associated items for the Interval, making sure an item is only associated once.
        /// </summary>
        public class IntervalNode
        {
            /// <summary>
            /// The associated items
            /// </summary>
            private HashSet<T> _associatedItems;
            
            public IntervalNode(Interval interval)
                : this(interval, null)
            {
            }

            public IntervalNode(Interval interval, IEnumerable<T> associatedItems)
            {
                Interval = interval;

                _associatedItems = new HashSet<T>();

                AddItems(associatedItems);
            }

            /// <summary>
            /// The next node in the list - is NULL if last node
            /// </summary>
            public IntervalNode Next { get; private set; }

            /// <summary>
            /// The previous node in the list - is NULL if first node
            /// </summary>
            public IntervalNode Prev { get; private set; }

            /// <summary>
            /// The interval represented by this node
            /// </summary>
            public Interval Interval { get; set; }

            /// <summary>
            /// The items associated with the interval
            /// </summary>
            public IEnumerable<T> AssociatedItems { get { return _associatedItems; } }

            /// <summary>
            /// Adds an item to be associated with the interval
            /// </summary>
            /// <param name="associatedItem"></param>
            public void AddItem(T associatedItem)
            {
                _associatedItems.Add(associatedItem);
            }

            /// <summary>
            /// Adds a range of items to be associated with the interval
            /// </summary>
            /// <param name="associateItems"></param>
            public void AddItems(IEnumerable<T> associateItems)
            {
                if (associateItems == null)
                    return;

                foreach (var associateItem in associateItems)
                {
                    AddItem(associateItem);
                }
            }

            /// <summary>
            /// Inserts the specified node to the left of this one
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public void InsertLeft(IntervalNode node)
            {
                var prev = Prev;
                Prev = node;
                node.Next = this;
                node.Prev = prev;
                if (prev != null)
                {
                    prev.Next = node;
                }
            }

            /// <summary>
            /// Inserts the specified node to the right of this one
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public void InsertRight(IntervalNode node)
            {
                var next = Next;
                Next = node;
                node.Prev = this;
                node.Next = next;
                if (next != null)
                {
                    next.Prev = node;
                }
            }

            /// <summary>
            /// Removes/Unlinks this node from the list
            /// </summary>
            public void Remove()
            {
                var prev = Prev;
                var next = Next;

                if (prev != null)
                {
                    prev.Next = next;
                }
                if (next != null)
                {
                    next.Prev = prev;
                }
            }

            /// <summary>
            /// Checks if the interval represented by this node equals the interval of the other sepcified node
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool IntervalEquals(IntervalNode other)
            {
                if (other == null)
                    return false;

                return Interval.Equals(other.Interval);
            }

            public override string ToString()
            {
                return Interval.ToString();
            }
        }

        /// <summary>
        /// Enumerator for the IntervalNode linked list
        /// </summary>
        public class Enumerator : IEnumerator<IntervalNode>
        {
            private IntervalNode _firstNode;
            private IntervalNode _currentNode;

            public Enumerator(IntervalNode first)
            {
                _firstNode = first;
            }

            public IntervalNode Current { get { return _currentNode; } }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_currentNode == null)
                {
                    _currentNode = _firstNode;
                }
                else
                {
                    _currentNode = _currentNode.Next;
                }

                if (_currentNode == null)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _currentNode = null;
            }

            public void Dispose()
            {
            }
        }
    }
}
