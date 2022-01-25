using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulationGraph
{
    struct Triangle
    {
        //The array of vertices making up the triangle. 
        public Vector2[] verts;
        public bool isSuperTriangle;
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
    }

    //The triangles made in the triangulation, the first being the super triangle.
    List<Triangle> triangles = new List<Triangle>();
    //The graph which we want to create.
    HashSet<Edge> graph = new HashSet<Edge>();

    public HashSet<Edge> DelenauyTriangulation(List<Vector2Int> roomPositions, int gridSize)
    {
        triangles.Clear();

        //Firstly, create the super triangle which will form basis of the algorithm.
        Triangle superTriangle = CreateSuperTriangle(gridSize);

        //Add the super triangle to our list.
        triangles.Add(superTriangle);

        AddTriangleToGraph(superTriangle);

        CheckVertices(roomPositions);

        //RemoveTrianglesFromGraph(new List<Triangle> { superTriangle });

        HashSet<Edge> edgeToCull = new HashSet<Edge>();

        foreach (Edge edge in graph)
        {
            if (edge.point1 == superTriangle.verts[0] || edge.point1 == superTriangle.verts[1] || edge.point1 == superTriangle.verts[2])
            {
                edgeToCull.Add(edge);
            }
            if (edge.point1 == superTriangle.verts[0] || edge.point1 == superTriangle.verts[1] || edge.point1 == superTriangle.verts[2])
            {
                edgeToCull.Add(edge);
            }
        }

        foreach (Edge item in edgeToCull)
        {
            graph.Remove(item);
        }


        //select the first room:
        //Look for traingles whose circumcircles contain our point: 
        //These are not delaunay triangles.
        //Connet our the point to the vertices of that triangle to make a new set of triangles.

        //Repeat the steps for the other points. 
        //Look for traingles whose circumcircles contain our point, for each of those triangles:
        //These are not delaunay triangles.
        //Delete the overlapping triangles
        //Connect the point to dges of that triangle to make a new set of triangles. 

        //Once looped through all rooms, delete the super triangle edges.
        //The remaining edges are the final triangulation graph. 

        foreach (Edge edge in graph)
        {
            Debug.Log("Edge: Point 1: " + edge.point1 + ". Point 2: " + edge.point2);
        }

        return graph;
    }

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

    void CheckVertices(List<Vector2Int> roomsPos)
    {
        // - Look for triangles whose circum circle contain our point
        // - If inside, connect this vertex to the triangle - forming new edges - because it is NOT a delenauy triangle. 
        // - Delete triangles who's circum circle contained our point.
        // - This will create new triangles, add those to the list of triangles as these triangles need checking against in the future.

        //Counter for knowing what room we are on.
        int counter = 0;

        List<Triangle> trisToRemove = new List<Triangle>();
        List<Triangle> trisToAdd = new List<Triangle>();

        bool delaunayVirginity = true;

        foreach (Vector2Int roomPos in roomsPos)
        {
            trisToRemove.Clear();
            trisToAdd.Clear();

            foreach (Triangle t in triangles)
            {
                //Calculate the circumcircle.
                Vector2 circumCentre = CalculateCircumcentre(t);
                float radius = Vector2.Distance(circumCentre, t.verts[0]);

                //Finds the room's distance from the circle centre.
                float roomDist = Vector2.Distance(roomPos, circumCentre);

                Debug.Log("Triangle:");
                Debug.Log(t.verts[0]);
                Debug.Log(t.verts[1]);
                Debug.Log(t.verts[2]);
                Debug.Log("Circumcentre:" + circumCentre);

                //Checks this against the radius of the circle. > then its outside, < then its inside.
                if (roomDist > radius)
                {
                    //The room is outside the circle - this is a delenauy triangle.

                    //Do nothing to edges.
                }
                else
                {
                    //The room is inside this circumcircle - this is not a delenauy triangle.

                    //Create edges between this room and the vertices of the super triangle. 

                    //New triangles will be made from verts of the triangle and then our node - creating 3 new edges, 3 new triangles.
                    //Because circum circles contain our point we know each edge of the 'super t' will make a trangle

                    //Basically for each edge of the triangle, make a traingle. 

                    if (t.isSuperTriangle && delaunayVirginity)
                    {
                        #region Create the three edges of this triangle.
                        Edge edge1 = new Edge()
                        {
                            point1 = t.verts[0],
                            point2 = t.verts[1]
                        };

                        Edge edge2 = new Edge()
                        {
                            point1 = t.verts[0],
                            point2 = t.verts[2]
                        };

                        Edge edge3 = new Edge()
                        {
                            point1 = t.verts[1],
                            point2 = t.verts[2]
                        };
                        #endregion

                        //Create the three new triangles.

                        Triangle triangle1 = new Triangle();
                        Triangle triangle2 = new Triangle();
                        Triangle triangle3 = new Triangle();

                        for (int i = 0; i < 3; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    triangle1 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge1.point1, edge1.point2, roomPos }
                                    };
                                    break;
                                case 1:
                                    triangle2 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge2.point1, edge2.point2, roomPos }
                                    };
                                    break;
                                case 2:
                                    triangle3 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge3.point1, edge3.point2, roomPos }
                                    };
                                    break;
                                default:
                                    break;
                            }
                        }

                        //Now we have the three new triangles remove the old one. 
                        RemoveTrianglesFromGraph(new List<Triangle> { t });
                        trisToRemove.Add(t);

                        //Add the new triangles to the graph.
                        AddTriangleToGraph(triangle1);
                        AddTriangleToGraph(triangle2);
                        AddTriangleToGraph(triangle3);
                        trisToAdd.Add(triangle1);
                        trisToAdd.Add(triangle2);
                        trisToAdd.Add(triangle3);
                        delaunayVirginity = false;

                    }
                    else if (!t.isSuperTriangle)
                    {
                        #region Create the three edges of this triangle.
                        Edge edge1 = new Edge()
                        {
                            point1 = t.verts[0],
                            point2 = t.verts[1]
                        };

                        Edge edge2 = new Edge()
                        {
                            point1 = t.verts[0],
                            point2 = t.verts[2]
                        };

                        Edge edge3 = new Edge()
                        {
                            point1 = t.verts[1],
                            point2 = t.verts[2]
                        };
                        #endregion

                        //Create the three new triangles.

                        Triangle triangle1 = new Triangle();
                        Triangle triangle2 = new Triangle();
                        Triangle triangle3 = new Triangle();

                        for (int i = 0; i < 3; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    triangle1 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge1.point1, edge1.point2, roomPos }
                                    };
                                    break;
                                case 1:
                                    triangle2 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge2.point1, edge2.point2, roomPos }
                                    };
                                    break;
                                case 2:
                                    triangle3 = new Triangle()
                                    {
                                        verts = new Vector2[] { edge3.point1, edge3.point2, roomPos }
                                    };
                                    break;
                                default:
                                    break;
                            }
                        }

                        //Now we have the three new triangles remove the old one. 
                        RemoveTrianglesFromGraph(new List<Triangle> { t });
                        trisToRemove.Add(t);

                        //Add the new triangles to the graph.
                        AddTriangleToGraph(triangle1);
                        AddTriangleToGraph(triangle2);
                        AddTriangleToGraph(triangle3);
                        trisToAdd.Add(triangle1);
                        trisToAdd.Add(triangle2);
                        trisToAdd.Add(triangle3);

                    }
                }
            }

            foreach (Triangle item in trisToAdd)
            {
                triangles.Add(item);
            }

            foreach (Triangle item in trisToRemove)
            {
                triangles.Remove(item);
            }

            //Increment the counter.
            counter++;
        }
    }

    void RemoveTrianglesFromGraph(List<Triangle> triangles)
    {
        HashSet<Edge> edgeCull = new HashSet<Edge>();

        //For each of the triangles to remove, add the edges to a hash set - so and duped edges are attempted to be removed once.
        foreach (Triangle t in triangles)
        {
            #region Create and add the three edges to the hash set. 
            edgeCull.Add(new Edge()
            {
                point1 = t.verts[0],
                point2 = t.verts[1]
            });

            edgeCull.Add(new Edge()
            {
                point1 = t.verts[0],
                point2 = t.verts[2]
            });

            edgeCull.Add(new Edge()
            {
                point1 = t.verts[1],
                point2 = t.verts[2]
            });
            #endregion
        }

        //Remove all the edges identified from the graph.
        foreach(Edge edge in edgeCull)
        {
            graph.Remove(edge);
        }
    }

    void AddTriangleToGraph(Triangle triangle)
    {
        //Create an array of edges.
        Edge[] triangleEdges = new Edge[3];

        # region Create and add the three edges: 
        triangleEdges[0] = new Edge()
        {
            point1 = triangle.verts[0], 
            point2 = triangle.verts[1]
        };

        triangleEdges[1] = new Edge()
        {
            point1 = triangle.verts[0],
            point2 = triangle.verts[2]
        };

        triangleEdges[2] = new Edge()
        {
            point1 = triangle.verts[1],
            point2 = triangle.verts[2]
        };

        #endregion

        //Add the edges to the graph as long as that edge isn't already in the graph.
        for (int i = 0; i < 3; i++)
        {
            if (!graph.Contains(triangleEdges[i]))
            {
                graph.Add(triangleEdges[i]);
                
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
}
