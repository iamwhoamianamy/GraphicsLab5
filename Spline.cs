using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLab5
{
   enum SplineType
   {
      Bezier3,
      Bezier4,
      CatmullRom,
   }

   class Spline
   {
      public List<Vector2> points;
      public bool isAnyActive = false;
      public int active = 0;
      public int resolution = 16;
      private float increment = 0.05f;

      public SplineType splineType = SplineType.Bezier3;

      public Spline()
      {
         points = new List<Vector2>();
      }

      public int FindClosest(Vector2 point)
      {
         float minDist = Single.MaxValue;
         int closestPointIndex = -1;

         for (int i = 0; i < points.Count; i++)
         {
            float dist = Vector2.Distance(points[i], point);

            if (dist < minDist)
            {
               minDist = dist;
               closestPointIndex = i;
            }
         }

         return closestPointIndex;
      }

      public void DrawSpline()
      {
         switch(splineType)
         {
            case SplineType.Bezier3: DrawBezier3(); break;
            case SplineType.Bezier4: DrawBezier4(); break;
            case SplineType.CatmullRom: DrawCatmullRom(); break;
         }
      }

      private void DrawBezier3()
      {
         if (points.Count > 2)
         {
            for (int i = 0; i < points.Count - 2; i += 2)
            {
               var p0 = points[i];
               var p1 = points[i + 1];
               var p2 = points[i + 2];

               GL.Begin(PrimitiveType.Lines);
               for (float t = 0; t <= 1.0; t += increment)
               {
                  float tx = (1 - t) * (1 - t) * p0.X + 2 * t * (1 - t) * p1.X + t * t * p2.X;
                  float ty = (1 - t) * (1 - t) * p0.Y + 2 * t * (1 - t) * p1.Y + t * t * p2.Y;

                  GL.Vertex2(tx, ty);
               }
               GL.End();
            }
         }
      }

      private void DrawBezier4()
      {
         if (points.Count > 3)
         {
            for (int i = 0; i < points.Count - 3; i += 3)
            {
               var p0 = points[i];
               var p1 = points[i + 1];
               var p2 = points[i + 2];
               var p3 = points[i + 3];

               GL.Begin(PrimitiveType.Lines);
               for (float t = 0; t <= 1.0; t += increment)
               {
                  float tx =
                     (1 - t) * (1 - t) * (1 - t) * p0.X +
                     3 * t * (1 - t) * (1 - t) * p1.X +
                     3 * t * t * (1 - t) * p2.X +
                     t * t * t  * p3.X;

                  float ty =
                     (1 - t) * (1 - t) * (1 - t) * p0.Y +
                     3 * t * (1 - t) * (1 - t) * p1.Y +
                     3 * t * t * (1 - t) * p2.Y +
                     t * t * t * p3.Y;

                  GL.Vertex2(tx, ty);
               }
               GL.End();
            }
         }
      }

      private void DrawCatmullRom()
      {
         if (points.Count > 2)
         {
            for (int i = 1; i < points.Count - 2; i++)
            {
               int p0 = i - 1;
               int p1 = i;
               int p2 = i + 1;
               int p3 = i + 2;

               float dist = Vector2.Distance(points[i], points[i + 1]);

               GL.Begin(PrimitiveType.Lines);
               //for (float t0 = 0; t0 <= 1.0; t0 += 1.0f / (resolution * dist * 0.01f))
               for (float t0 = 0; t0 <= 1.0; t0 += 0.05f)
               {
                  float t = t0 - (int)t0;
                  float tt = t * t;
                  float ttt = t * t * t;

                  float q0 = -ttt + 2 * tt - t;
                  float q1 = 3.0f * ttt - 5.0f * tt + 2.0f;
                  float q2 = -3.0f * ttt + 4.0f * tt + t;
                  float q3 = ttt - tt;

                  float tx = 0.5f * (
                     points[p0].X * q0 +
                     points[p1].X * q1 +
                     points[p2].X * q2 +
                     points[p3].X * q3);

                  float ty = 0.5f * (
                     points[p0].Y * q0 +
                     points[p1].Y * q1 +
                     points[p2].Y * q2 +
                     points[p3].Y * q3);

                  GL.Vertex2(tx, ty);
               }
               GL.End();
            }
         }
      }

      public void DrawOutline()
      {
         GL.Begin(PrimitiveType.LineStrip);

         for (int i = 0; i < points.Count; i++)
         {
            GL.Vertex2(points[i].X, points[i].Y);
         }

         GL.End();
      }

      public void DrawControlPoints()
      {

         GL.Enable(EnableCap.PointSmooth);
         GL.PointSize(10);
         GL.Begin(PrimitiveType.Points);

         foreach (var p in points)
         {
            GL.Vertex2(p.X, p.Y);
         }

         GL.End();
         GL.Disable(EnableCap.PointSmooth);

      }


      public void HighlightActive()
      {
         if (isAnyActive)
            Help.DrawRect(points[active].X, points[active].Y, 20, 20);
      }
   }
}
