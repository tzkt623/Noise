using System;
using System.Collections;
using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public static class Shapes
    {
        static Vector2 indexToUV(int i, float resolution, float invResolution)
        {
            float v = Mathf.Floor(invResolution * i);
            float u = invResolution * (i - resolution * v);
            v = v * invResolution;
            return new Vector2(u, v);
        }

        public struct Point3
        {
            public Vector3 position;
            public Vector3 normal;
        }

        public interface IShape
        {
            Point3 getPoint(int index, float resolution, float invResolution);
        }

        public class Plane : IShape
        {
            public Point3 getPoint(int index, float resolution, float invResolution)
            {
                var plane = Shapes.indexToUV(index, resolution, invResolution);

                Point3 p;
                p.position = new Vector3(plane.x - 0.5f, 0, plane.y - 0.5f);
                p.normal = Vector3.up;

                return p;
            }
        }

        public class Sphere : IShape
        {
            public Point3 getPoint(int index, float resolution, float invResolution)
            {
                var uv = Shapes.indexToUV(index, resolution, invResolution);

                float r = 0.5f;
                float s = r * Mathf.Sin(Mathf.PI * uv.y);

                Point3 p;
                p.position.x = s * Mathf.Sin(2f * Mathf.PI * uv.x);
                p.position.y = r * Mathf.Cos(Mathf.PI * uv.y);
                p.position.z = s * Mathf.Cos(2f * Mathf.PI * uv.x);
                p.normal = p.position;

                return p;
            }
        }

        public class OctahedronSphere : IShape
        {
            public Point3 getPoint(int index, float resolution, float invResolution)
            {
                var uv = Shapes.indexToUV(index, resolution, invResolution);

                Point3 p;
                p.position.x = uv.x - 0.5f;
                p.position.y = uv.y - 0.5f;
                p.position.z = 0.5f - Mathf.Abs(p.position.x) - Mathf.Abs(p.position.y);

                float offset = Mathf.Max(-p.position.z, 0f);
                p.position.x += Tool.select(offset, -offset, p.position.x < 0f);
                p.position.y += Tool.select(offset, -offset, p.position.y < 0f);

                float scale = 0.5f * (1 / Mathf.Sqrt(
                    p.position.x * p.position.x +
                    p.position.y * p.position.y +
                    p.position.z * p.position.z));

                p.position.x *= scale;
                p.position.y *= scale;
                p.position.z *= scale;

                p.normal = p.position;

                return p;
            }
        }

        public struct Torus : IShape
        {
            public Point3 getPoint(int i, float resolution, float invResolution)
            {
                var uv = Shapes.indexToUV(i, resolution, invResolution);

                float r1 = 0.375f;
                float r2 = 0.125f;
                float s = r1 + r2 * Mathf.Cos(2f * Mathf.PI * uv.y);

                Point3 p;
                p.position.x = s * Mathf.Sin(2f * Mathf.PI * uv.x);
                p.position.y = r2 * Mathf.Sin(2f * Mathf.PI * uv.y);
                p.position.z = s * Mathf.Cos(2f * Mathf.PI * uv.x);

                p.normal = p.position;
                p.normal.x -= r1 * Mathf.Sin(2f * Mathf.PI * uv.x);
                p.normal.z -= r1 * Mathf.Cos(2f * Mathf.PI * uv.x);

                return p;
            }
        }

        public class Octahedron : IShape
        {
            public Point3 getPoint(int index, float resolution, float invResolution)
            {
                var uv = Shapes.indexToUV(index, resolution, invResolution);

                Point3 p;
                p.position.x = uv.x - 0.5f;
                p.position.y = uv.y - 0.5f;
                p.position.z = 0.5f - Mathf.Abs(p.position.x) - Mathf.Abs(p.position.y);

                float offset = Mathf.Max(-p.position.z, 0f);
                p.position.x += Tool.select(offset, -offset, p.position.x < 0f);
                p.position.y += Tool.select(offset, -offset, p.position.y < 0f);

                p.normal = p.position;

                return p;
            }
        }
    }
}