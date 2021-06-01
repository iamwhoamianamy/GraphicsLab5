﻿using System;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLab5
{
   public class TextRenderer : IDisposable
   {
      Bitmap bmp;
      Graphics gfx;
      int texture;
      Rectangle rectGFX;
      bool disposed;
      // Конструктор нового экземпляра класса
      // width, height - ширина и высота растрового образа
      public TextRenderer(int width, int height)
      {
         if (GraphicsContext.CurrentContext == null) throw new InvalidOperationException("GraphicsContext не обнаружен");
         bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         gfx = Graphics.FromImage(bmp);

         // Используем сглаживание
         gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
         texture = GL.GenTexture();
         GL.BindTexture(TextureTarget.Texture2D, texture);

         // Свойства текстуры
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
         GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
         // Создаем пустую тектсуру, которую потом пополним растровыми данымми с текстом (см.
         GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
             width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
      }
      // Заливка образа цветом color
      public void Clear(Color color)
      {
         gfx.Clear(color);
         rectGFX = new Rectangle(0, 0, bmp.Width, bmp.Height);
      }
      // Выводит строку текта text в точке point растрового образе, используя фонт font и цвета brush
      // Начало координат растрового образа находится в его левом верхнем углу
      public void DrawString(string text, Font font, Brush brush, PointF point)
      {
         gfx.DrawString(text, font, brush, point);
      }
      // Получает обработчик texture (System.Int32) текструры, который связывается с TextureTarget.Texture2d
      // см.в OnRenderFrame: GL.BindTexture(TextureTarget.Texture2D, renderer.Texture)
      public int Texture
      {
         get
         {
            UploadBitmap();
            return texture;
         }
      }
      // Выгружеат растровые данные в текстуру OpenGL
      void UploadBitmap()
      {
         if (rectGFX != RectangleF.Empty)
         {
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(rectGFX,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            // Текстура формируется на основе растровых данных, содержащихся в data
            GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                rectGFX.X, rectGFX.Y, rectGFX.Width, rectGFX.Height,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            // Освобождаем память, занимаемую data
            bmp.UnlockBits(data);
            rectGFX = Rectangle.Empty;
         }
      }
      void Dispose(bool manual)
      {
         if (!disposed)
         {
            if (manual)
            {
               bmp.Dispose();
               gfx.Dispose();
               if (GraphicsContext.CurrentContext != null) GL.DeleteTexture(texture);
            }
            disposed = true;
         }
      }
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      ~TextRenderer()
      {
         Console.WriteLine("[Предупреждение] Есть проблеммы: {0}.", typeof(TextRenderer));
      }
   }
}
