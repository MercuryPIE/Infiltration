using System.Text;
using AT2.ObstacleControl;
using AT2.Utility;

namespace AT2.EndingState;

public class Path
{
    private readonly Vector _pathStart;
    private readonly Vector _pathFinish;
    
    private readonly static HashSet<Node> OpenSet = new();
    private readonly static HashSet<Vector> ClosedSet = new();
    private readonly static PriorityQueueWithNodeId Open = new();
    
    
    public void FindSafePath()
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
    
    private void FindPath(Node StartNode)
    {
        Open.Enqueue(StartNode);
        OpenSet.Add(StartNode);

        while (Open.Count > 0)
        {
            var currentNode = Open.Dequeue();

            OpenSet.Remove(currentNode);
            ClosedSet.Add(currentNode.GetCurrentLocation());

            if (Vector.CompareVectors(currentNode.GetCurrentLocation(), _pathFinish, Vector.ComOperator.Eq))
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
            var currentLocation = BaseNode.GetCurrentLocation();
            
            return BaseNode switch
            {
                _ when FinalState.AccessFinalObstacles.ContainsKey(currentLocation) => false,
                _ when !Boundary.IsInBoundary(currentLocation, out _) => false,
                _ when ClosedSet.Contains(currentLocation) => false,
                _ => true,
            };
        }
        private void CheckNeighbours(Node CurrentNode, Node Parent)
        {
            var neighbourNodes = GenerateAdjacentNodes(CurrentNode);

            foreach (var baseNode in neighbourNodes)
            {
                if (!IsTraversable(baseNode)) continue;
                
                if (CurrentNode.GetCostData().FCost <= baseNode.GetCostData().GCost && OpenSet.Contains(baseNode)) continue;
                
                Open.Enqueue(baseNode);
                OpenSet.Add(baseNode);
            }
        }
        private static string BuildPath(Node FinalNode)
        {
            var pathTaken = new StringBuilder();
            var currentNode = FinalNode;
            var path = new Stack<Node>();

            while (currentNode.GetNodeData().Parent != null)
            {
                path.Push(currentNode);
                currentNode = currentNode.GetNodeData().Parent!;
            }

            foreach (var nodesInPath in path)
            {
                var cNodeLocation = nodesInPath.GetCurrentLocation();
                var pNodeLocation = nodesInPath.GetNodeData().Parent!.GetCurrentLocation();
                
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
        
        private HashSet<Node> GenerateAdjacentNodes(Node CurrentNode)
        {
            HashSet<Node> neighbours = new();
            var location = CurrentNode.GetCurrentLocation();

            foreach (var direction in Vector.GetNeighbors(location))
            {
                var neighbour = new Vector(location.X + direction.X, location.Y + direction.Y);
                var neighbourNode = new Node(neighbour, CurrentNode);
                neighbours.Add(neighbourNode);
            }
            
            return neighbours;
        }
    
    
}