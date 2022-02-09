using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriangulationGraph
{
    #region Structs

    struct Triangle
    {
        //The array of vertices making up the triangle. 
        public Vector2[] verts;
        public bool isSuperTriangle;
        public Edge[] edges;
    }

    public struct Edge
    {
        //The two verts of the edge.
        public Vector2 point1;
        public Vector2 point2;

        //For equation of the line making up the edge. Used when calculating circum circles.
        public float a;
        public float b;
        public float c;

        //The weight assigned to the edge based on the distance of the two points.
        public float weight;
    }

    public struct NodeEdge
    {
        //The two verts of the edge.
        public int sourceNode;
        public int destNode;

        //The weight assigned to the edge based on the distance of the two points.
        public int weight;
    }

    struct Subset
    {
        public int parent;
        public int rank;
    }

    #endregion

    //The graphs which we want to create.
    public HashSet<Edge> graph = new HashSet<Edge>();
    public List<Vector2Int> rooms;
    List<NodeEdge> nodeGraph;
    int[,] graphAdjacencyTable;

    //Using the Bowyer-Watson algorithm to complete Delenauy trigulation of the points.
    public List<Edge> DelenauyTriangulation(List<Vector2Int> roomPositions, int gridSize)
    {
        if (roomPositions.Count <= 0)
        {
            return null;
        }

        rooms = roomPositions;
        triangles = new HashSet<Triangle>();
        badTriangles = new List<Triangle>();
        ////Firstly, create the super triangle which will form basis of the algorithm.
        Triangle superTriangle = CreateSuperTriangle(gridSize);
        triangles.Add(superTriangle);
        AddTriangleToGraph(superTriangle);

        foreach (Vector2Int point in rooms)
        {
            CheckVertice(point);
        }

        foreach (Triangle t in triangles)
        {
            AddTriangleToGraph(t);
        }

        List<Edge> edgeToCull = new List<Edge>();

        //Identifies any verts of the super triangle to remove.
        foreach (Edge edge in graph)
        {
            if (edge.point1 == superTriangle.verts[0] || edge.point1 == superTriangle.verts[1] || edge.point1 == superTriangle.verts[2])
            {
                edgeToCull.Add(edge);
            }
            if (edge.point2 == superTriangle.verts[0] || edge.point2 == superTriangle.verts[1] || edge.point2 == superTriangle.verts[2])
            {
                edgeToCull.Add(edge);
            }
        }

        foreach (Edge item in edgeToCull)
        {
            graph.Remove(item);
        }

        return graph.ToList();
    }

    //Create the minimum spanning tree of the graph using Kruskal's algorithm.
    public List<Edge> MinimumSpanningTree(List<Edge> graph)
    {
        //Give weightings to the edges of the graph.
        graph = MakeGraphWeighted(graph);
        nodeGraph = EdgeToNodeEdgeGraph(graph, true);

        List<Edge> mst = NodeEdgeToEdge(KruskalMST(nodeGraph));

        //remove the last edge.
        mst.RemoveAt(mst.Count - 1);

        return mst;
    }

    #region Graph Functions

    //Creates and returns a boolean adjacency table using this indexing: [From, To]
    public int[,] GraphAdjacencyTable(List<Edge> graph, List<Vector2Int> nodes, bool weighted = false)
    {
        int[,] table = new int[nodes.Count, nodes.Count];

        //Work out which edge matches who. 
        foreach (Edge edge in graph)
        {
            //Find the index of the rooms list of the rooms
            int fromIndex = nodes.IndexOf(new Vector2Int((int)edge.point1.x, ((int)edge.point1.y)));
            int toIndex = nodes.IndexOf(new Vector2Int((int)edge.point2.x, ((int)edge.point2.y)));

            if (weighted) //the int value will be the graph weight as this graph is weighted - not a 0 or 1.
            {
                //The graph is undirected thus set both to the value of the weight.
                table[fromIndex, toIndex] = (int)edge.weight;
                table[toIndex, fromIndex] = (int)edge.weight;
            }
            else // The graph is unwighted so just use 0 or 1 to represnt
            {
                //The graph is undirected thus set both to be true (represented by 1)
                table[fromIndex, toIndex] = 1;
                table[toIndex, fromIndex] = 1;
            }

        }

        graphAdjacencyTable = table;

        //Return the populated adjacency table.
        return table;
    }

    //Converts Edges (which contain two vector 2 points) to a Node Edge (which contains two nodes) for a list of Edges.
    List<NodeEdge> EdgeToNodeEdgeGraph(List<Edge> graph, bool weighted = true)
    {
        List<NodeEdge> edges = new List<NodeEdge>();

        // Work out which edge matches who. 
        foreach (Edge e in graph)
        {
            int fromIndex = rooms.IndexOf(new Vector2Int((int)e.point1.x, ((int)e.point1.y)));
            int toIndex = rooms.IndexOf(new Vector2Int((int)e.point2.x, ((int)e.point2.y)));


            if (weighted)
            {

                NodeEdge edge = new NodeEdge()
                {
                    sourceNode = fromIndex,
                    destNode = toIndex,
                    weight = (int)e.weight
                };
                edges.Add(edge);
            }
            else
            {

                NodeEdge edge = new NodeEdge()
                {
                    sourceNode = fromIndex,
                    destNode = toIndex
                };
                edges.Add(edge);
            }


        }

        return edges;
    }

    //Converts Node Edges (which contains two nodes) to Edges (which contain two vector 2 points) for a list of Edges.
    List<Edge> NodeEdgeToEdge(List<NodeEdge> graph)
    {
        List<Edge> edges = new List<Edge>();

        foreach (NodeEdge e in graph)
        {
            Edge edge = new Edge()
            {
                point1 = rooms[e.sourceNode],
                point2 = rooms[e.destNode],
                weight = e.weight
            };

            edges.Add(edge);
        }

        return edges;
    }

    //Gives a weight to each of the edges in a given graph. The eight will be the distance between the two points of the edge. 
    List<Edge> MakeGraphWeighted(List<Edge> graph)
    {
        List<Edge> weightedGraph = graph;

        for (int i = 0; i < graph.Count; i++)
        {
            Edge edge = weightedGraph[i];

            edge.weight = Vector2.Distance(weightedGraph[i].point1, weightedGraph[i].point2);

            weightedGraph[i] = edge;
        }



        return weightedGraph;
    }
    #endregion

    #region Delenauy Triangulation

    //The triangles made in the triangulation, the first being the super triangle.
    HashSet<Triangle> triangles = new HashSet<Triangle>();
    List<Triangle> badTriangles = new List<Triangle>();
    Triangle CreateSuperTriangle(int gridSize)
    {
        //The 'verts' represent rooms which ther super traingle needs to to enclose. 
        //Create a triangle which has a point (0,0), (gridsize + 1, 1), (1/2(gridsize), gridsize + 1)

        //Calculate the vertices of the super triangle. 
        Vector2 v1 = new Vector2(-gridSize * 2, -gridSize);
        Vector2 v2 = new Vector2(gridSize * 2, -gridSize * 1.5f);
        Vector2 v3 = new Vector2(0.5f * gridSize, gridSize * 4);

        //Create the super triangle.
        Triangle superTriangle = new Triangle()
        {
            verts = new Vector2[] { v1, v2, v3 },
            isSuperTriangle = true
        };

        return superTriangle;
    }

    //Caluclates the circum centre of the circum circle of a given triangle; the radius of this circle can then be found by getting the distance of any point of the triagnle and the circum centre.
    Vector2 CalculateCircumcentre(Triangle t)
    {
        Edge edge1 = CalculateEdgeFromPoints(t.verts[0], t.verts[1]);
        Edge edge2 = CalculateEdgeFromPoints(t.verts[1], t.verts[2]);

        Edge edge1Perp = PerpendicularBisectorOfEdge(edge1);
        Edge edge2Perp = PerpendicularBisectorOfEdge(edge2);

        //Get the point of intersection of two lines: the circumcentre.
        Vector2 circumCentre = PointOfIntersection(edge1Perp, edge2Perp);

        return circumCentre;
    }

    //Checks the vertice for Delanauy triangulation.
    void CheckVertice(Vector2Int point)
    {
        badTriangles = new List<Triangle>();

        //Identify any 'bad triangles' - any triangle which this point lies in the circum circle of the traingle.
        foreach (Triangle t in triangles)
        {
            //Calculate the circumcircle.
            Vector2 circumCentre = CalculateCircumcentre(t);
            //Calculate the radius of the circumcircle.
            float radius = Vector2.Distance(circumCentre, t.verts[0]);

            //Finds the room's distance from the circle centre.
            float roomDist = Vector2.Distance(point, circumCentre);

            //Checks this against the radius of the circle. > then its outside, < then its inside.
            if (roomDist < radius)
            {
                //The node is inside the circumcircle of this triangle. 
                //Not a delenauy triangle.
                badTriangles.Add(t);
            }
        }

        #region Identify boundaries of polyagonal hole.

        List<Edge> polygon = new List<Edge>();

        //For each of the verts of the bad triangles, work the edges out and add them to the list of edges.
        List<Edge> edges = new List<Edge>();
        foreach (Triangle tri in badTriangles)
        {
            edges.Add(new Edge()
            {
                point1 = tri.verts[0],
                point2 = tri.verts[1],
            });
            edges.Add(new Edge()
            {
                point1 = tri.verts[0],
                point2 = tri.verts[2],
            });
            edges.Add(new Edge()
            {
                point1 = tri.verts[1],
                point2 = tri.verts[2],
            });
        }

        var grouped = edges.GroupBy(o => o);
        var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
        polygon = boundaryEdges.ToList();

        for (int i = 0; i < badTriangles.Count; i++)
        {
           Triangle t = badTriangles[i];
            t.edges = null;
            badTriangles[i] = t;
        }

        triangles.RemoveWhere(o => badTriangles.Contains(o));

        #endregion

        //Create new trianlges between the point and the edges of the polyagonal hole.
        foreach (Edge e in polygon.Where(possEdge => possEdge.point1 != point && possEdge.point2 != point))
        {
            Triangle newTri = new Triangle
            {
                verts = new Vector2[]
                {
                    point,
                    e.point1,
                    e.point2,
                }
            };

            triangles.Add(newTri);
        }
    }

    void AddTriangleToGraph(Triangle triangle)
    {
        //Create an array of edges.
        Edge[] edges = new Edge[3];

        # region Create and add the three edges: 
        edges[0] = new Edge()
        {
            point1 = triangle.verts[0], 
            point2 = triangle.verts[1]
        };

        edges[1] = new Edge()
        {
            point1 = triangle.verts[0],
            point2 = triangle.verts[2]
        };

        edges[2] = new Edge()
        {
            point1 = triangle.verts[1],
            point2 = triangle.verts[2]
        };

        #endregion

        //Add the edges to the graph as long as that edge isn't already in the graph.
        for (int i = 0; i < 3; i++)
        {
            if (!graph.Contains(edges[i]))
            {
                graph.Add(edges[i]);
                
            }
        }
    }

    #region Maths

    Edge CalculateEdgeFromPoints (Vector2 vert1, Vector2 vert2) 
    {
        Edge edge = new Edge();

        edge.point1 = vert1;
        edge.point2 = vert2;

        edge.a = vert2.y - vert1.y;
        edge.b = vert1.x - vert2.x;
        edge.c = edge.a * (vert1.x) + edge.b * (vert1.y);
       
        return edge;
    }

    Vector2 PointOfIntersection(Edge edge1, Edge edge2)
    {

        float determinant = edge1.a * edge2.b - edge2.a * edge1.b;
        if (determinant == 0)
        {
            // The lines are parallel.
            Debug.LogError("TriangulationGraph: The lines are paralell, cannot calculate circumcentre.");
            return new Vector2();
        }
        else
        {
            float x = (edge2.b * edge1.c - edge1.b * edge2.c) / determinant;
            float y = (edge1.a * edge2.c - edge2.a * edge1.c) / determinant;
            return new Vector2(x, y);
        }
    }

    Edge PerpendicularBisectorOfEdge(Edge edge)
    {
        //Calculates the midpoint of this edge.
        Vector2 mid = new Vector2((edge.point1.x + edge.point2.x) / 2, (edge.point1.y + edge.point2.y) / 2);

        // c = -bx + ay
        edge.c = -edge.b * (mid.x) + edge.a * (mid.y);

        float temp = edge.a;
        edge.a = -edge.b;
        edge.b = temp;

        return edge;
    }

    #endregion

    #endregion

    #region Minimum Spanning Tree - Kruskal's Algorithm

    int Find(Subset[] subsets, int i)
    {
        //If the parent of this node is not this node look for nodes of the tree.
        if (subsets[i].parent != i) 
        {
            //Sets the parent of the node to the found parent node of this tree.
            subsets[i].parent = Find(subsets, subsets[i].parent);
        }

        //return the parent of the tree.
        return subsets[i].parent;
    }

    //Makes a union between two trees in the forest.
    void Union(Subset[] subsets, int x, int y)
    {
        int xroot = Find(subsets, x);
        int yroot = Find(subsets, y);

        if (subsets[xroot].rank < subsets[yroot].rank)
            subsets[xroot].parent = yroot;
        else if (subsets[xroot].rank > subsets[yroot].rank)
            subsets[yroot].parent = xroot;
        else
        {
            subsets[yroot].parent = xroot;
            ++subsets[xroot].rank;
        }
    }

    List<NodeEdge> KruskalMST(List<NodeEdge> graph)
    {
        //Number of rooms (aka nodes) on the graph.
        int nodeCount = rooms.Count;
        //An array of integers which are the room indexes of the route.
        NodeEdge[] result = new NodeEdge[nodeCount];

        int i = 0;
        int e = 0;

        //Sort the graph in order of weights - lowest to highest.
        graph.Sort(delegate (NodeEdge a, NodeEdge b)
        {
            return a.weight.CompareTo(b.weight);
        });

        //Initialize the subsets array.
        Subset[] subsets = new Subset[nodeCount];

        //Populate subsets array.
        for (int v = 0; v < nodeCount; ++v)
        {
            subsets[v].parent = v;
            subsets[v].rank = 0;
        }

        while (e < nodeCount - 1)
        {
            //Get the next edge from the graph.
            NodeEdge nextEdge = graph[i++];
            //Find the parent of the tree of the source node.
            int x = Find(subsets, nextEdge.sourceNode);
            //Find the parent of the tree of the dest node.
            int y = Find(subsets, nextEdge.destNode);

            if (x != y) //Check that the parents are not the same - would create a cycle.
            {
                //Add to the result of the mst the edge.
                result[e++] = nextEdge;
                //Make a union between the two trees.
                Union(subsets, x, y);
            }

        }

        return result.ToList();
    }

    #endregion
}
