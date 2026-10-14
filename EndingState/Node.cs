using System.Collections;
using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState
{
    public class PriorityQueueWithNodeId
    {
        private readonly List<Tuple<Node, int, int>> _elements = new();
        public int Count => _elements.Count;

        public void Enqueue(Node Item)
        {
            var priority = Item.GetCostData().FCost;
            var nodeId = Item.GetNodeData().NodeId;

            _elements.Add(new Tuple<Node, int, int>(Item, nodeId, priority));
            Sort();
        }
        private void Sort()
        {
            _elements.Sort((X, Y) =>
            {
                var priorityCompare = X.Item3.CompareTo(Y.Item3);

                return priorityCompare == 0 ? X.Item2.CompareTo(Y.Item2) : priorityCompare;
                     });
                 }
        
        public Node Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("ERROR");
            }
         
            var result = _elements[0].Item1;
            _elements.RemoveAt(0);
            return result;
        }

    }
    
    
    
    public class Node
    {
        public readonly struct CostData
        {
            public readonly int GCost;
            private readonly int _hCost;
            public readonly int FCost;

            public CostData(int GCost, int HCost)
            {
                this.GCost = GCost;
                _hCost = HCost;
                FCost = GCost + HCost;
            }
            
            public override string ToString()
            {
                return $"G Cost: {GCost}, H Cost: {_hCost}, F Cost: {FCost}";
            }
        }
        public readonly struct NodeData
        {
            private static int _nextNodeId;
            public readonly Node? Parent;
            public readonly int NodeId;

            public NodeData(Node? Parent)
            {
                this.Parent = Parent;
                NodeId = _nextNodeId++;
            }
            
            public override string ToString()
            {
                return $"Parent Link:\n\n{Parent}";
            }
        }
        
        private readonly Vector _currentLocation;
        private readonly Vector? _finish;

        private readonly NodeData _nodeInfo;

        private readonly CostData _costInfo;

        public Node(Vector Start, Vector Finish)
        {
            // Starting Node.
            _currentLocation = Start;
            _finish = Finish;
            _nodeInfo = new NodeData(null);
            _costInfo = new CostData(0, HCostCalculate());
        }
        public Node(Vector Location, Node Parent)
        {
            // Linking Node.
            _currentLocation = Location;
            _finish = null;
            _nodeInfo = new NodeData(Parent);
            _costInfo = new CostData(GCostCalculate(), HCostCalculate());
        }
        
        public NodeData GetNodeData() { return _nodeInfo; }
        public CostData GetCostData() { return _costInfo; }
        public Vector GetCurrentLocation() { return _currentLocation; }
        
        private int GCostCalculate()
        {
            return _nodeInfo.Parent!._costInfo.GCost + 1;
        }
        private int HCostCalculate()
        {
            var val1 = Math.Abs(_currentLocation.X - _finish!.X);
            var val2 = Math.Abs(_currentLocation.Y - _finish!.Y);
            
            return val1 + val2;
        }
    }
}