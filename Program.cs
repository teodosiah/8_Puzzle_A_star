using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle_A_star
{
    public class Program
    {
        //keep used states
        public static Dictionary<int[,], int> visited = new Dictionary<int[,], int>();
        //start state
        public static int[,] start = new int[3, 3] { { 6, 5, 3},
                                                     { 2, 4, 8},
                                                     { 7, 0, 1}};
        //final state
        public static int[,] end = new int[3, 3]{ { 1, 2, 3},
                                                  { 4, 5, 6},
                                                  { 7, 8, 0}};
      
        public static int size = start.GetLength(0);

        static void Main(string[] args)
        {           
           int result = AstarSearch();
           printResult(result);
        }        
        public static int AstarSearch()
        {
            List<Node> prioQueue = new List<Node>();
            prioQueue.Add(new Node() { Puzzle = start, Distance = 0 }); //add the root          
            do
            {             
                prioQueue = new List<Node>(orderByFunction(prioQueue));
                Node node = prioQueue.First();
                prioQueue.Remove(node);
                List<Node> neighbours = getMoves(node);
                neighbours = new List<Node>(orderByFunction(neighbours));
                foreach (Node neighbour in neighbours)
                {
                    if(!isElementInQueue(prioQueue, neighbour))
                    {
                        prioQueue.Add(neighbour);
                    }                                  
                }
                if (!isUsed(node))
                {
                    visited.Add(node.Puzzle, node.Distance);
                    
                    if (getManhattanDistance(node.Puzzle, end) == 0)
                    {
                        return node.Distance;
                    }
                }              
            } while (prioQueue.Any());

            return 0;
        }      
      
       //calculate the distance using the Manhattan algorithm
        public static int getManhattanDistance(int[,] parent, int[,] child)
        {
            int distance = 0;
            if(parent != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        List<int> child_indexes = getIndex(child, parent[i, j]);
                        distance += (Math.Abs(i - child_indexes[0]) + Math.Abs(j - child_indexes[1]));
                    }
                }
            }            
            return distance;
        }

        //order nodes by they're values of the sum of the manhattan distance and the distance from the root
        public static List<Node> orderByFunction(List<Node> list)
        {
            Dictionary<Node, int> compare_values = new Dictionary<Node, int>();
            int val = 0;
            foreach(Node element in list)
            {
                val = getManhattanDistance(element.Puzzle, end) + element.Distance;
                compare_values.Add(element, val);
            }
            var sortedDict = from elem in compare_values orderby elem.Value ascending select elem;
            list = new List<Node>((from elem in sortedDict select elem.Key).ToList());
            return list;
        }
        
        //get all possible moves by given
        public static List<Node> getMoves(Node node)
        {
            List<Node> moves = new List<Node>();
            Node tmp = new Node();            
            int dist = node.Distance + 1;
            
            List<int> zeroIndex = getIndex(node.Puzzle, 0);
            int new_element;
            if(zeroIndex[1] + 1 < size)  //right 
            {
                tmp = new Node();
                tmp.Distance = dist;
                new_element = node.Puzzle[zeroIndex[0], zeroIndex[1] + 1];
                tmp.Puzzle = node.Puzzle.Clone() as int[,];
                swap(tmp.Puzzle, 0, new_element);
                if (!isUsed(tmp))
                {                 
                   moves.Add(tmp);
                }               
            }
            if(zeroIndex[0] + 1 < size) //below
            {
                tmp = new Node();
                tmp.Distance = dist;
                new_element = node.Puzzle[zeroIndex[0] + 1, zeroIndex[1]];
                tmp.Puzzle = node.Puzzle.Clone() as int [, ];
                swap(tmp.Puzzle, 0, new_element);
                if (!isUsed(tmp))
                {
                    moves.Add(tmp);
                }               
            }
            if(zeroIndex[1] - 1 >= 0) //left
            {
                tmp = new Node();
                tmp.Distance = dist;
                new_element = node.Puzzle[zeroIndex[0], zeroIndex[1] - 1];
                tmp.Puzzle = node.Puzzle.Clone() as int[,];
                swap(tmp.Puzzle, 0, new_element);
                if (!isUsed(tmp))
                {
                    moves.Add(tmp);
                }                
            }
            if(zeroIndex[0] - 1 >= 0) //above
            {
                tmp = new Node();
                tmp.Distance = dist;
                new_element = node.Puzzle[zeroIndex[0] - 1, zeroIndex[1]];
                tmp.Puzzle = node.Puzzle.Clone() as int[,];
                swap(tmp.Puzzle, 0, new_element);
                if (!isUsed(tmp))
                {
                    moves.Add(tmp);
                }                
            }
            return moves;
        }

        //swap val1 and val2 in arr1
        public static void swap(int [,] arr1,int val1, int val2)
        {
            int tmp;
            List<int> val1_indexes = getIndex(arr1, val1);
            List<int> val2_indexes = getIndex(arr1, val2);

            tmp = val1;
            arr1[val1_indexes[0], val1_indexes[1]] = val2;
            arr1[val2_indexes[0], val2_indexes[1]] = tmp;
        }

        //check whether Node is already visited
        public static bool isUsed(Node node)
        {           
            return (from elem in visited where isEqualMatrix(elem.Key, node.Puzzle) select elem.Key).ToList().Count != 0;
        }
        private static bool isEqualMatrix(int [,] arr1, int [,] arr2)
        {
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (arr1[i, j] != arr2[i, j])
                        return false;
                }
            }
            return true;
        }
        private static bool isElementInQueue(List<Node> queue, Node element)
        {
            foreach(Node elem in queue)
            {
                if (isEqualMatrix(elem.Puzzle, element.Puzzle))
                    return true;
            }
            return false;
        }

        //returns the row and the column indexes of element in given array
        public static List<int> getIndex(int[,] arr, int element)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (arr[i,j] == element)
                    {
                        indexes.Add(i);
                        indexes.Add(j);
                        return indexes;
                    }
                }
            }
            return indexes;
        }

        //save the result in file Result.txt in the bin folder of the project
        private static void printResult(int res)
        {
            System.IO.File.WriteAllText("Result.txt", "The length of the shortest path from start to goal is: " + res.ToString());
            Process.Start("Result.txt");
        }

    }
    public class Node
    {
        public int[,] Puzzle { get; set; }
        public int Distance { get; set; }  //the distance from the start state
    }
}
