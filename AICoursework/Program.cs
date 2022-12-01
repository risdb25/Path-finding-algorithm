using AICoursework.Classes;
using System;
using System.Collections.Generic;
using System.IO;


namespace AICoursework
{
    class Program
    {
        static void Main(string[] args)
        {

            #region .cav FILE WORK    

            List<string> dataset = new List<string>(); //List which will be used to store the values in the .cav file
            string fileName;

            fileName = args[0]; //The name of the .cav file is passed in as argument within Main method's string array parameter
            string filePath = Directory.GetCurrentDirectory() + "\\" + fileName + ".cav";
            string fileContent = File.ReadAllText(filePath); //Stores the content of the file in a variable
                
            string[] fileContentRemovedCommas = fileContent.Split(','); //File content recreated as an array with commas removed

            foreach(string value in fileContentRemovedCommas) 
            {
                dataset.Add(value); //Takes values from array and puts into a List
            }

            int numberOfCaves = Convert.ToInt32(dataset[0]); //Number of caves is the first value at index 0 of the List
            dataset.RemoveAt(0); //Don't need this value in the List anymore since it's stored in a variable
            #endregion


            #region POPULATE CAVES LIST
            List<Cave> cavesList = new List<Cave>(); //List of Cave objects

            for(int i = 0; i < numberOfCaves; i++)
            {
                //Pairs coordinates with each cave. Constructor takes in (caveNum, X, Y)
                 cavesList.Add(new Cave(i + 1, Convert.ToInt32(dataset[(2 * i)]), Convert.ToInt32(dataset[(2 * i) + 1]))); 
            }

            dataset.RemoveRange(0, numberOfCaves * 2); //No longer need the first (numberOfCaves * 2) values in the List
            #endregion


            #region ASSIGN CAVE NAVIGABILITY
            for(int i = 0; i < dataset.Count; i++)
            {
                if(dataset[i].Equals("1")) //1 indicates a navigable route
                {
                    //Create the link between the two caves. The location/index at which the 1 occurs tells which caves the link is between
                    cavesList[i % numberOfCaves].AddNavigableCave(cavesList[i / numberOfCaves]);
                }         
                
            }
            #endregion


            #region PATHFINDING - INFORMED SEARCH USING A*
            //Initialise the necessary objects for A*
            List<Cave> cavesOpenSet = new List<Cave>();
            List<Cave> cavesClosedSet = new List<Cave>();
            Cave startCave = cavesList[0]; //Start cave is always the first cave 
            Cave goalCave = cavesList[numberOfCaves - 1]; //Goal cave is always the last cave
            Cave currentCave = null;
            Cave.GoalCave = goalCave; //Static class variable to hold the goal cave, used within heuristic calculation

            startCave.DistanceFromStart = 0;
            cavesOpenSet.Add(startCave);

            while(cavesOpenSet.Count > 0) //While open set is not empty
            {
                currentCave = cavesOpenSet[0]; //Set current cave to the first cave in the sorted List

                if(currentCave.Equals(goalCave)) 
                {
                    break; //Successfully reached the goal cave
                }

                cavesOpenSet.Remove(currentCave);
                cavesClosedSet.Add(currentCave);

                foreach(Cave navigableCave in currentCave.GetNavigableCaves())
                {
                    if(cavesClosedSet.Contains(navigableCave)) 
                    {
                        continue; //Proceed past caves that have already been explored
                    }

                    double distanceToNavigableCave = currentCave.DistanceFromStart + currentCave.CalculateDistance(navigableCave); //g

                    if(!cavesOpenSet.Contains(navigableCave))
                    {
                        cavesOpenSet.Add(navigableCave); //Add cave to open set if not came across it so far                                          
                    }

                    if(cavesOpenSet.Contains(navigableCave) && distanceToNavigableCave < navigableCave.DistanceFromStart)
                    {                        
                        navigableCave.DistanceFromStart = distanceToNavigableCave; //Successfully found a shorter path for the neighbouring cave
                        navigableCave.ShortestPathCave = currentCave; //Tracks the route of caves that provide the shortest path
                    }

                }
                
                cavesOpenSet.Sort(); //Sort list based on f (which is g + h) where g = distance from start, h = straight distance to goal cave

            }
            #endregion


            #region .csn FILE WORK
            string caveNumsOfShortestPath = string.Empty; //Variable will hold canveNum of each cave that makes up the shortest path to the goal cave

            if(currentCave.Equals(goalCave)) //If a path to goal cave was found
            {
                while(currentCave.ShortestPathCave != null)
                {
                    caveNumsOfShortestPath = caveNumsOfShortestPath.Insert(0, Convert.ToString(currentCave.CaveNum) + " ");
                    currentCave = currentCave.ShortestPathCave;
                    //Proceed to the next cave that makes up the shortest path. Begins from goal cave and works back to start cave
                }

                caveNumsOfShortestPath = caveNumsOfShortestPath.Insert(0, "1 "); //Added manually because cave num of first cave not covered in above loop
            }
            else //No possible route to the goal cave found
            {
                caveNumsOfShortestPath = "0";
            }

            string saveLocation = Directory.GetCurrentDirectory() + "\\" + fileName + ".csn";
            File.WriteAllText(saveLocation, caveNumsOfShortestPath.Trim()); //Writes .csn file
            #endregion
        }

    }

}

