using System;
using System.Collections.Generic;

namespace AICoursework.Classes
{
    public class Cave : IComparable //Implements the interface to be able to sort the List of caves
    {
        #region PRIVATE INSTANCE VARIABLES
        private List<Cave> navigableCaves; //List of Cave objects that holds the navigable caves for each cave
        #endregion

        #region PROPERTIES
        public int CaveNum { get; set; } //Represents the place (order) in which a given cave resides in the .cav file
        public int X { get; set; }
        public int Y { get; set; } 
        public double DistanceFromStart { get; set; }
        public static Cave GoalCave { get; set; } //Shared by all caves, used to calculate heuristic
        public Cave ShortestPathCave { get; set; }
        #endregion

        #region CONSTRUCTOR
        public Cave(int caveNum, int x, int y)
        {
            CaveNum = caveNum;
            X = x;
            Y = y;
            navigableCaves = new List<Cave>();
            DistanceFromStart = double.MaxValue;
            ShortestPathCave = null;
        }
        #endregion

        #region METHODS
        public void AddNavigableCave(Cave cave) //Create tunnel between caves
        {
            this.navigableCaves.Add(cave);
        }

        public List<Cave> GetNavigableCaves() //Returns all reachable caves from this cave object
        {
            return this.navigableCaves;
        }

        public double CalculateDistance(Cave cave) //Calculates Euclidian distance between two caves
        {
            return Math.Sqrt((Math.Pow(cave.X - X, 2)) + Math.Pow(cave.Y - Y, 2));
        }

        public int CompareTo(object obj)
        {
            /*We want the list sorted in such a way that the cave with 
             * the lowest heuristic (estimated distance to goal cave + distance from start) 
             * is sorted to the top, meaning it will be explored next
             */
            Cave caveToCompare = (Cave)obj;
            double cave1F = this.DistanceFromStart + (CalculateDistance(GoalCave));
            double cave2F = caveToCompare.DistanceFromStart + (CalculateDistance(GoalCave));

            if(cave1F > cave2F)
            {
                return 1;
            }
            else if(cave1F < cave2F)
            {
                return -1;
            }
            else
            {
                return 0;
            }         
            
        }
        #endregion
    }
}
