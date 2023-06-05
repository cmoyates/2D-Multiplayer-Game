using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST
{
    // A class representing an edge to be used in the algorithm below
    public class Edge 
    {
        public int src;
        public int dest;
        public float weight;
        public Edge(int srcParam, int destParam, float weightParam) 
        {
            src = srcParam;
            dest = destParam;
            weight = weightParam;
        }
    }

    // Calculate the Minimum Spanning Tree from the given edges and vertices using Prim's algorithm
    public static List<Edge> calculateMST(Vector2[] vertices, int[][] edges) 
    {
        // Convert the edges from the parameter into Edge objects and store them in a list
        List<Edge> actualEdges = new List<Edge>();
        for (int i = 0; i < edges.Length; i++)
        {
            int src = edges[i][0];
            int dest = edges[i][1];
            float distance = Vector2.Distance(vertices[src], vertices[dest]);
            actualEdges.Add(new Edge(src, dest, distance));
        }
        
        // Create new lists representing the vertices that are reached and unreached respectively
        List<int> reached = new List<int>();
        List<int> unreached = new List<int>();
        // Create a list to sture the edges that will be returned from this function
        List<Edge> output = new List<Edge>();

        // Consider every vertex unreached to start
        for (int i = 0; i < vertices.Length; i++)
        {
            unreached.Add(i);
        }
        // Add the first vertex to the reached list and remove it from the unreached list
        reached.Add(unreached[0]);
        unreached.RemoveAt(0);

        // While there are still vertices that haven't been reached yet
        while (unreached.Count > 0) 
        {
            // Store all the edges that have one reached vertex and one unreached vertex in a posibilities list
            List<Edge> possibilities = new List<Edge>();
            for (int i = 0; i < actualEdges.Count; i++)
            {
                if (reached.Contains(actualEdges[i].src) != reached.Contains(actualEdges[i].dest)) 
                {
                    possibilities.Add(actualEdges[i]);
                }
            }
            // Find the edge from the possibilities with the lowest weight
            Edge currentBest = possibilities[0];
            for (int i = 1; i < possibilities.Count; i++)
            {
                if (possibilities[i].weight < currentBest.weight) 
                {
                    currentBest = possibilities[i];
                }
            }
            // Add the vertex of the best edge was not in the reached list to it, and remove it from the unreached list
            if (reached.Contains(currentBest.src))
            {
                reached.Add(currentBest.dest);
                unreached.Remove(currentBest.dest);
            }
            else 
            {
                reached.Add(currentBest.src);
                unreached.Remove(currentBest.src);
            }
            // Add the best edge to the output list
            output.Add(currentBest);
        }

        // Return the output list
        return output;
    }

    public static int[] GetFarthestPoints(Edge[] mstEdges, Vector2[] points) 
    {
        // Initialize an array of lists to store the lists of edges that include the point at the index
        List<Edge>[] edgesFromPoints = new List<Edge>[points.Length];
        
        // For each point given
        for (int i = 0; i < points.Length; i++)
        {
            // Initialize a list of edges to hold the edges that include the point
            List<Edge> edgesFromPoint = new List<Edge>();
            // For each edge
            for (int j = 0; j < mstEdges.Length; j++) 
            {
                // If either the src or dest has is the same as the point index
                if (mstEdges[j].src == i || mstEdges[j].dest == i) 
                {
                    // Add it to the list
                    edgesFromPoint.Add(mstEdges[j]);
                }
            }
            // Add the list to the array of lists
            edgesFromPoints[i] = edgesFromPoint;
        }

        int[] farthestPoints = new int[2] { -1, -1 };
        float farthestDistance = 0.0f;


        for (int i = 0; i < points.Length; i++)
        {
            (int endPointIndex, float traversalDistance) = FindFarthestPoint(i, edgesFromPoints);
            if (traversalDistance > farthestDistance) 
            {
                farthestDistance = traversalDistance;
                farthestPoints[0] = i;
                farthestPoints[1] = endPointIndex;
            }
        }

        return farthestPoints;
    }

    public static (int, float) FindFarthestPoint(int startIndex, List<Edge>[] edgesArray)
    {
        int farthestIndex = startIndex;
        float farthestDistance = 0f;

        Queue<int> queue = new Queue<int>();
        queue.Enqueue(startIndex);

        Dictionary<int, float> distances = new Dictionary<int, float>();
        distances[startIndex] = 0f;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            foreach (Edge edge in edgesArray[current])
            {
                int otherPointIndex = (edge.src == current) ? edge.dest : edge.src;
                if (!distances.ContainsKey(otherPointIndex))
                {
                    float distance = distances[current] + edge.weight;
                    distances[otherPointIndex] = distance;

                    if (distance > farthestDistance)
                    {
                        farthestDistance = distance;
                        farthestIndex = otherPointIndex;
                    }

                    queue.Enqueue(otherPointIndex);
                }
            }
        }

        return (farthestIndex, farthestDistance);
    }
}
