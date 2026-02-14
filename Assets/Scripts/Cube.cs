using UnityEngine;

public class Cube
{
    private Vector3 center;
    private Vector3 forward;
    private float width;

    Cube(Vector3 center, Vector3 forward,  float width)
    {
        this.center = center;
        this.forward = forward;
        this.width = width;
    }
}


public class Triangle
{
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
}