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

    struct Edge
    {
        public Vector2 point1;
        public Vector2 point2;

        //For equation of the line making up the edge.
        public float m;
        public float c;
    }

    //The triangles made in the triangulation, the first being the super triangle.
    List<Triangle> triangles = new List<Triangle>();

    void DelenauyTriangulation(List<Vector2Int> roomPositions, int gridSize)
    {
        triangles.Clear();  
    }

    void CreateSuperTriangle(int gridSize)
    {
        //The 'verts' represent rooms which ther super traingle needs to to enclose. 
        //Create a triangle which has a point (0,0), (gridsize + 1, 1), (1/2(gridsize), gridsize + 1)

        //Calculate the vertices of the super triangle. 
        Vector2 v1 = new Vector2(0, 0);
        Vector2 v2 = new Vector2(gridSize + 1, 0);
        Vector2 v3 = new Vector2(0.5f * gridSize, gridSize + 1);

        //Create the super triangle.
        Triangle superTriangle = new Triangle()
        {
            verts = new Vector2[] { v1, v2, v3 },
            isSuperTriangle = true
        };

        //Add the super triangle to our list.
        triangles.Add(superTriangle);
    }

    Edge CalculateEdge (Vector2 vert1, Vector2 vert2) 
    {
        Edge edge = new Edge();

        //Standard Equation: y = mx + c

        //Calculate the gradient, M. Gradient equals change y / change x 
        float m = (vert1.y - vert2.y) / (vert1.x - vert2.x);
        //Calculate the y-intercept, C.
        float c = vert1.y - (m * vert1.x);

        //Assign values to the edge.
        edge.m = m;
        edge.c = c;
        edge.point1 = vert1;
        edge.point2 = vert2;
       
        return edge;
    }

    Vector2 PointOfIntersection(Edge edge1, Edge edge2)
    {
        Vector2 point = new Vector2();

        //Re-arrange into the form mx + y = c, currently y = mx + c
        //So get y - mx = c 
        //The time equation 1 

        //Times equation 1 by m2
        float m1 = edge1.m * edge2.m;
        float c1 = edge1.c * edge2.m;

        //Times equation 2 by m1
        float m2 = edge2.m * edge1.m;
        float c2 = edge2.c * edge1.m;

        //Work out how many y we have. 
        float y1Pre = edge2.m;
        float y2Pre = edge1.m;

        //Equation 3.
        float y3Pre = y1Pre - y2Pre;
        float c3 = c1 - c2;
        float m3 = m1 - m2;

        float y = (c3) / y3Pre;
        float x = (y - edge1.c) / edge1.m;

        point.x = x;
        point.y = y;  

        return point;
    }

    Vector2 CalculateCircumcentre(Triangle t)
    {
        Edge edge1 = CalculateEdge(t.verts[0], t.verts[1]);
        Edge edge2 = CalculateEdge(t.verts[1], t.verts[2]);

        #region Get the perpindicular bisector of each edge.

        //Calculate perpindicular bisector of edge1.
        float midX = (edge1.point1.x + edge1.point2.x) / 2;
        float midY = (edge1.point1.y + edge1.point2.y) / 2;
        Vector2 mid = new Vector2(midX, midY);
        float newM = -1 / edge1.m;

        Edge edge1Perp = new Edge
        {
            m = newM,
            c = edge1.c,
            point1 = edge1.point1,
            point2 = edge1.point2
        };

        //Calculate perpindicular bisector of edge2.
        float midX2 = (edge2.point1.x + edge2.point2.x) / 2;
        float midY2 = (edge2.point1.y + edge2.point2.y) / 2;
        Vector2 mid2 = new Vector2(midX2, midY2);
        float newM2 = -1 / edge2.m;

        Edge edge2Perp = new Edge
        {
            m = newM2,
            c = edge2.c,
            point1 = edge2.point1,
            point2 = edge2.point2
        };

        #endregion

        //Get the point of intersection of two lines: the circumcentre.
        Vector2 circumCentre = PointOfIntersection(edge1Perp, edge2Perp);

        return circumCentre;
    }
}
