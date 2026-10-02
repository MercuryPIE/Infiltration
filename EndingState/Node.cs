using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AT2.ObstacleControl;
using AT2.Utility;
using static AT2.EndingState.Node;

namespace AT2.EndingState
{
    public class PriorityQueueWithNodeID
    {
        private readonly List<Tuple<Node, int, int>> Elements = new List<Tuple<Node, int, int>>();
        public int Count => Elements.Count;

        public void Enqueue(Node Item)
        {
            int Priority = Item.GetCostData().GetCostData(CostData.Access.FCost);
            int NodeID = Item.GetNodeData().GetNodeID();

            Elements.Add(new Tuple<Node, int, int>(Item, NodeID, Priority));
            Sort();
        }
        private void Sort()
        {
            Elements.Sort((X, Y) =>
            {
                int PriorityCompare = X.Item3.CompareTo(Y.Item3);

                if (PriorityCompare == 0)
                {
                    return X.Item2.CompareTo(Y.Item2);
                }
                return PriorityCompare;
            });
        }
        public Node Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("ERROR");
            }

            Node Result = Elements[0].Item1;
            Elements.RemoveAt(0);
            return Result;
        }
    }
    public class Node
    {
        public readonly struct CostData
        {
            public enum Access
            {
                FCost, GCost, HCost
            }

            private readonly int GCost, HCost, FCost;
            private CostData(int GCost, int HCost)
            {
                this.GCost = GCost;
                this.HCost = HCost;
                FCost = GCost + HCost;
            }

            public readonly int GetCostData(Access AccessID)
            {
                switch (AccessID)
                {
                    case Access.FCost:
                        return FCost;
                    case Access.GCost:
                        return GCost;
                    case Access.HCost:
                        return HCost;
                    default:
                        return -1;
                }
            }
            public static CostData CreateCostData(int GCost, int HCost)
            {
                return new CostData(GCost, HCost);
            }

            public override string ToString()
            {
                return $"G_Cost: {GCost}, H_Cost: {HCost}, F_Cost: {FCost}";
            }
        }
        public readonly struct PathData
        {
            private readonly Vector Start, Finish;
            private PathData(Vector Start, Vector Finish)
            {
                this.Start = Start;
                this.Finish = Finish;
            }

            public readonly Vector GetStart() { return Start; }
            public readonly Vector GetFinish() { return Finish; }

            public static PathData CreatePathData(Vector Start, Vector Finish)
            {
                return new PathData(Start, Finish);
            }

            public override string ToString()
            {
                return $"Start of the Path: {Start}, End of the Path: {Finish}";
            }
        }
        public readonly struct NodeData
        {
            private static int NextNodeID = 0;
            private readonly Node? Parent;
            private readonly int NodeID;

            private NodeData(Node? Parent)
            {
                this.Parent = Parent;
                NodeID = NextNodeID++;
            }

            public readonly Node? GetParent() { return Parent; }
            public readonly int GetNodeID() { return NodeID; }

            public static NodeData CreateNodeData(Node? Parent)
            {
                return new NodeData(Parent);
            }

            public override string ToString()
            {
                return $"Parent Link:\n\n{Parent}";
            }
        }

        public readonly struct Neighbours : IEnumerable<Node>
        {
            private readonly Node North_Node, South_Node, East_Node, West_Node;

            private Neighbours(Vector CurrentLocation, Node Parent)
            {
                Vector NorthLocation = Vector.CreateVector(CurrentLocation.GetX(), CurrentLocation.GetY() - 1);
                Vector SouthLocation = Vector.CreateVector(CurrentLocation.GetX(), CurrentLocation.GetY() + 1);
                Vector EastLocation = Vector.CreateVector(CurrentLocation.GetX() + 1, CurrentLocation.GetY());
                Vector WestLocation = Vector.CreateVector(CurrentLocation.GetX() - 1, CurrentLocation.GetY());

                North_Node = CreateLinkNode(NorthLocation, Parent);
                South_Node = CreateLinkNode(SouthLocation, Parent);
                East_Node = CreateLinkNode(EastLocation, Parent);
                West_Node = CreateLinkNode(WestLocation, Parent);
            }

            public static Neighbours GenerateNeighbours(Vector Location, Node Parent)
            {
                return new Neighbours(Location, Parent);
            }
            public readonly IEnumerator<Node> GetEnumerator()
            {
                yield return North_Node;
                yield return South_Node;
                yield return East_Node;
                yield return West_Node;
            }
            readonly IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        private readonly Vector CurrentLocation;

        private readonly NodeData NodeInfo;
        private readonly PathData PathInfo;
        private readonly CostData CostInfo;

        private Node(Vector Start, Vector Finish)
        {
            CurrentLocation = Start;
            NodeInfo = NodeData.CreateNodeData(null);
            PathInfo = PathData.CreatePathData(Start, Finish);
            CostInfo = CostData.CreateCostData(0, HCostCalculate());
        }
        private Node(Vector Location, Node Parent)
        {
            CurrentLocation = Location;
            NodeInfo = NodeData.CreateNodeData(Parent);
            PathInfo = PathData.CreatePathData(Parent.PathInfo.GetStart(), Parent.PathInfo.GetFinish());
            CostInfo = CostData.CreateCostData(GCostCalculate(), HCostCalculate());
        }

        private readonly static HashSet<Node> OpenSet = new HashSet<Node>();
        private readonly static HashSet<Vector> ClosedSet = new HashSet<Vector>();

        private readonly static PriorityQueueWithNodeID Open = new PriorityQueueWithNodeID();

        private readonly static Stack<Node> Path = new Stack<Node>();

        public Vector GetNodeKey() { return CurrentLocation; }

        public NodeData GetNodeData() { return NodeInfo; }
        public CostData GetCostData() { return CostInfo; }
        public PathData GetPathData() { return PathInfo; }

        public static void FindSafePath()
        {
            while (true)
            {
                Console.WriteLine("Enter your current location (X,Y):");
                string? Input = Console.ReadLine();

                if (Vector.IsLocationValid(Input, out Vector StartLocation))
                {
                    while (true)
                    {
                        Console.WriteLine("Enter the location of the mission objective (X,Y):");
                        Input = Console.ReadLine();

                        if (Vector.IsLocationValid(Input, out Vector FinishLocation))
                        {
                            Instruction.CalculateInstructions(Boundary.Calculate(StartLocation, FinishLocation));
                            FindPath(CreateStartNode(StartLocation, FinishLocation));
                            break;
                        }
                    }
                    break;
                }
            }
        }
        private static void FindPath(Node StartNode)
        {
            Open.Enqueue(StartNode);
            OpenSet.Add(StartNode);

            while (Open.Count > 0)
            {
                Node CurrentNode = Open.Dequeue();

                OpenSet.Remove(CurrentNode);
                ClosedSet.Add(CurrentNode.GetNodeKey());

                if (Vector.CompareVectors(CurrentNode.CurrentLocation, CurrentNode.PathInfo.GetFinish(), Vector.ComOperator.Eq))
                {
                    string PathTaken = BuildPath(CurrentNode);

                    Console.WriteLine("End Found");
                    Console.WriteLine($"Path Taken:\n{PathTaken}");
                    break;
                }

                CheckNeighbours(CurrentNode, CurrentNode);
            }
        }

        private static bool IsTraversable(Node BaseNode)
        {
            Vector Start = BaseNode.PathInfo.GetStart();
            Vector Goal = BaseNode.PathInfo.GetFinish();

            if (FinalState.AccessFinalObstacles.ContainsKey(BaseNode.GetNodeKey()))
            {
                return false;
            }
            else if (!Boundary.IsInBoundary(BaseNode.CurrentLocation, out _))
            {
                return false;
            }
            else if (ClosedSet.Contains(BaseNode.GetNodeKey()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static void CheckNeighbours(Node CurrentNode, Node Parent)
        {
            Neighbours NeighbourNodes = Neighbours.GenerateNeighbours(CurrentNode.CurrentLocation, Parent);

            foreach (Node BaseNode in NeighbourNodes)
            {
                if (IsTraversable(BaseNode))
                {
                    if (CurrentNode.CostInfo.GetCostData(CostData.Access.FCost) > BaseNode.CostInfo.GetCostData(CostData.Access.GCost) || !OpenSet.Contains(BaseNode))
                    {
                        Open.Enqueue(BaseNode);
                        OpenSet.Add(BaseNode);
                    }
                }
            }
        }
        private static string BuildPath(Node FinalNode)
        {
            StringBuilder PathTaken = new StringBuilder();
            Node CurrentNode = FinalNode;

            while (CurrentNode!.NodeInfo.GetParent() != null)
            {
                Path.Push(CurrentNode);
                CurrentNode = CurrentNode.NodeInfo.GetParent()!;
            }

            foreach (Node NodesInPath in Path)
            {
                if (NodesInPath.NodeInfo.GetParent() != null)
                {
                    Vector CNodeLocation = NodesInPath.CurrentLocation;
                    Vector PNodeLocation = NodesInPath.NodeInfo.GetParent()!.CurrentLocation;

                    if (Vector.CompareVectors(CNodeLocation, PNodeLocation, Vector.ComOperator.Lt, Vector.ComDimension.X))
                    {
                        PathTaken.Append('W');
                    }
                    else if (Vector.CompareVectors(CNodeLocation, PNodeLocation, Vector.ComOperator.Gt, Vector.ComDimension.X))
                    {
                        PathTaken.Append('E');
                    }
                    else if (Vector.CompareVectors(CNodeLocation, PNodeLocation, Vector.ComOperator.Lt, Vector.ComDimension.Y))
                    {
                        PathTaken.Append('N');
                    }
                    else if (Vector.CompareVectors(CNodeLocation, PNodeLocation, Vector.ComOperator.Gt, Vector.ComDimension.Y))
                    {
                        PathTaken.Append('S');
                    }
                }
            }
            return PathTaken.ToString();
        }

        public static Node CreateLinkNode(Vector Location, Node Parent)
        {
            return new Node(Location, Parent);
        }
        public static Node CreateStartNode(Vector StartLocation, Vector FinishLocation)
        {
            return new Node(StartLocation, FinishLocation);
        }

        private int GCostCalculate()
        {
            return NodeInfo.GetParent()!.CostInfo.GetCostData(CostData.Access.GCost) + 1;
        }
        private int HCostCalculate()
        {
            return Math.Abs(CurrentLocation.GetX() - PathInfo.GetFinish().GetX()) + Math.Abs(CurrentLocation.GetY() - PathInfo.GetFinish().GetY());
        }
    }
}