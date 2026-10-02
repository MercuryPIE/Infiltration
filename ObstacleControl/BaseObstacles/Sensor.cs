using AT2.EndingState;
using AT2.Utility;

namespace AT2.ObstacleControl.BaseObstacles
{
    public class Sensor : IObstacleExtension<double>
    {
        
        public Vector Location { get; set; }
        public double ExtensionData { get; set; }
        private readonly ClassId _obstacleClass = ClassId.Sensor;
        
        public Sensor(Vector Location, double Range)
        {
            this.Location = Location;
            ExtensionData = Range;
        }
        
        public void CalculateObstacle()
        {
            var range = ExtensionData;
            
            //TODO: Look at converting the vector to Double and doing calculations with it.
            var x = Convert.ToDouble(Location.X);
            var y = Convert.ToDouble(Location.Y);
            
            var xRange = x + range;
            var yRange = y + range;
            
            var xPos = Convert.ToInt32(Math.Floor(x - range));
            var yPos = Convert.ToInt32(Math.Floor(y - range));

            for (; xPos <= xRange; xPos++)
            {
                for (; yPos <= yRange; yPos++)
                {
                    var xPower = Math.Pow(xPos - x, 2);
                    var yPower = Math.Pow(yPos - y, 2);

                    var distance = Math.Sqrt(xPower + yPower);

                    var newLocation = new Vector(xPos, yPos);

                    if (distance <= range && Boundary.IsInBoundary(newLocation, out Vector checkedLocation))
                    {
                        FinalState.AddObstacle(Obstacle.Create(checkedLocation, _obstacleClass));
                    }
                }
            }
        }
        
        public double GetExtensionData()
        {
            return ExtensionData;
        }
        

        public void RemoveExtensionData()
        {
            ExtensionData = 0;
        }
    }
}
