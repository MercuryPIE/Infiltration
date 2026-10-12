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
            public readonly int GCost, HCost, FCost;
            public CostData(int GCost, int HCost)
            {
                this.GCost = GCost;
                this.HCost = HCost;
                FCost = GCost + HCost;
            }
            
            public override string ToString()
            {
                return $"G Cost: {GCost}, H Cost: {HCost}, F Cost: {FCost}";
            }
        }
        public readonly struct PathData
        {
            public readonly Vector Start, Finish;
            public PathData(Vector Start, Vector Finish)
            {
                this.Start = Start;
                this.Finish = Finish;
            }
            
            public override string ToString()
            {
                return $"Start of the Path: {Start}, End of the Path: {Finish}";
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
        
        public readonly struct Neighbours : IEnumerable<Node>
        {
            private readonly Node _northNode, _southNode, _eastNode, _westNode;

            private Neighbours(Vector CurrentLocation, Node Parent)
            {
                var northLocation = new Vector(CurrentLocation.X, CurrentLocation.Y- 1);
                var southLocation = new Vector(CurrentLocation.X, CurrentLocation.Y + 1);
                var eastLocation = new Vector(CurrentLocation.X + 1, CurrentLocation.Y);
                var westLocation = new Vector(CurrentLocation.X - 1, CurrentLocation.Y);

                _northNode = new Node(northLocation, Parent);
                _southNode = new Node(southLocation, Parent);
                _eastNode = new Node(eastLocation, Parent);
                _westNode = new Node(westLocation, Parent);
            }

            public static Neighbours GenerateNeighbours(Vector Location, Node Parent)
            {
                return new Neighbours(Location, Parent);
            }
            
            public IEnumerator<Node> GetEnumerator()
            {
                yield return _northNode;
                yield return _southNode;
                yield return _eastNode;
                yield return _westNode;
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        private readonly Vector _currentLocation;

        private readonly NodeData _nodeInfo;
        private readonly PathData _pathInfo;
        private readonly CostData _costInfo;

        private Node(Vector Start, Vector Finish)
        {
            // Starting Node.
            _currentLocation = Start;
            _nodeInfo = new NodeData(null);
            _pathInfo = new PathData(Start, Finish);
            _costInfo = new CostData(0, HCostCalculate());
        }
        private Node(Vector Location, Node Parent)
        {
            // Linking Node.
            _currentLocation = Location;
            _nodeInfo = new NodeData(Parent);
            _pathInfo = new PathData(Parent._pathInfo.Start, Parent._pathInfo.Finish);
            _costInfo = new CostData(GCostCalculate(), HCostCalculate());
        }

        private readonly static HashSet<Node> OpenSet = new();
        private readonly static HashSet<Vector> ClosedSet = new();

        private readonly static PriorityQueueWithNodeId Open = new();
        
        

        public NodeData GetNodeData() { return _nodeInfo; }
        public CostData GetCostData() { return _costInfo; }


        public static void FindSafePath()
        {
            while (true)
            {
                Console.WriteLine("Enter your current location (X,Y):");
                var input = Console.ReadLine();

                if (!Vector.IsLocationValid(input, out var startLocation)) continue;
                
                while (true)
                {
                    Console.WriteLine("Enter the location of the mission objective (X,Y):");
                    input = Console.ReadLine();

                    if (!Vector.IsLocationValid(input, out var finishLocation)) continue;
                    
                    FindPath(new Node(startLocation, finishLocation));
                    break;
                }
                break;
            }
        }
        private static void FindPath(Node StartNode)
        {
            Open.Enqueue(StartNode);
            OpenSet.Add(StartNode);

            while (Open.Count > 0)
            {
                var currentNode = Open.Dequeue();

                OpenSet.Remove(currentNode);
                ClosedSet.Add(currentNode._currentLocation);

                if (Vector.CompareVectors(currentNode._currentLocation, currentNode._pathInfo.Finish, Vector.ComOperator.Eq))
                {
                    var pathTaken = BuildPath(currentNode);

                    Console.WriteLine("End Found");
                    Console.WriteLine($"Path Taken:\n{pathTaken}");
                    break;
                }

                CheckNeighbours(currentNode, currentNode);
            }
        }

        private static bool IsTraversable(Node BaseNode)
        {
            var currentLocation = BaseNode._currentLocation;
            
            return BaseNode switch
            {
                _ when FinalState.AccessFinalObstacles.ContainsKey(currentLocation) => false,
                _ when !Boundary.IsInBoundary(currentLocation, out _) => false,
                _ when ClosedSet.Contains(currentLocation) => false,
                _ => true,
            };
        }
        private static void CheckNeighbours(Node CurrentNode, Node Parent)
        {
            var neighbourNodes = Neighbours.GenerateNeighbours(CurrentNode._currentLocation, Parent);

            foreach (var baseNode in neighbourNodes)
            {
                if (!IsTraversable(baseNode)) continue;
                
                if (CurrentNode._costInfo.FCost <= baseNode._costInfo.GCost && OpenSet.Contains(baseNode)) continue;
                
                Open.Enqueue(baseNode);
                OpenSet.Add(baseNode);
            }
        }
        private static string BuildPath(Node FinalNode)
        {
            var pathTaken = new StringBuilder();
            var currentNode = FinalNode;
            var path = new Stack<Node>();

            while (currentNode._nodeInfo.Parent != null)
            {
                path.Push(currentNode);
                currentNode = currentNode._nodeInfo.Parent!;
            }

            foreach (var nodesInPath in path)
            {
                if (nodesInPath._nodeInfo.Parent == null) continue;
                
                var cNodeLocation = nodesInPath._currentLocation;
                var pNodeLocation = nodesInPath._nodeInfo.Parent._currentLocation;
                
                var nodeDirection = true switch
                {
                    _ when Vector.CompareVectors(cNodeLocation, pNodeLocation, Vector.VDirection.North) => 'N',
                    _ when Vector.CompareVectors(cNodeLocation, pNodeLocation, Vector.VDirection.South) => 'S',
                    _ when Vector.CompareVectors(cNodeLocation, pNodeLocation, Vector.VDirection.East) => 'E',
                    _ when Vector.CompareVectors(cNodeLocation, pNodeLocation, Vector.VDirection.West) => 'W',
                    _ => throw new Exception("Failed to determine direction")
                };
                
                pathTaken.Append(nodeDirection);
                
            }
            return pathTaken.ToString();
        }
        
        private int GCostCalculate()
        {
            return _nodeInfo.Parent!._costInfo.GCost + 1;
        }
        private int HCostCalculate()
        {
            var val1 = Math.Abs(_currentLocation.X - _pathInfo.Finish.X);
            var val2 = Math.Abs(_currentLocation.Y - _pathInfo.Finish.Y);
            
            return val1 + val2;
        }
    }
}