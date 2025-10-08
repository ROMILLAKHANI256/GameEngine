using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GameWindowSettings = OpenTK.Windowing.Desktop.GameWindowSettings;

namespace PhongOpenTK
{
    public class Game : GameWindow
    {
        private int _vao;
        private int _vbo;
        private Shader _shader;

        // camera
        private Vector3 _cameraPos = new Vector3(0, 0, 5f);
        private Vector3 _cameraFront = -Vector3.UnitZ;
        private Vector3 _cameraUp = Vector3.UnitY;
        private float _yaw = -90f, _pitch = 0f;
        private Vector2 _lastMousePos;
        private bool _firstMove = true;
        private float _sensitivity = 0.2f;
        private float _speed = 2.5f;

        // transforms
        private Matrix4 _model = Matrix4.Identity;

        // light
        private Vector3 _lightPos = new Vector3(2f, 2f, 2f);
        private Vector3 _lightColor = new Vector3(1f, 1f, 1f);
        private Vector3 _objectColor = new Vector3(0f, 0f, 1f);//(1.0f, 0.5f, 0.31f);

       
        private readonly float[] _vertices = {
            // positions        // normals         // texcoords
            
             0.5f, -0.5f, -0.5f,  1f, 0f, 0f,  1f, 0f,
             0.5f,  0.5f, -0.5f,  1f, 0f, 0f,  1f, 1f,
             0.5f,  0.5f,  0.5f,  1f, 0f, 0f,  0f, 1f,
             0.5f, -0.5f, -0.5f,  1f, 0f, 0f,  1f, 0f,
             0.5f,  0.5f,  0.5f,  1f, 0f, 0f,  0f, 1f,
             0.5f, -0.5f,  0.5f,  1f, 0f, 0f,  0f, 0f,

           
            -0.5f, -0.5f,  0.5f, -1f, 0f, 0f,  1f, 0f,
            -0.5f,  0.5f,  0.5f, -1f, 0f, 0f,  1f, 1f,
            -0.5f,  0.5f, -0.5f, -1f, 0f, 0f,  0f, 1f,
            -0.5f, -0.5f,  0.5f, -1f, 0f, 0f,  1f, 0f,
            -0.5f,  0.5f, -0.5f, -1f, 0f, 0f,  0f, 1f,
            -0.5f, -0.5f, -0.5f, -1f, 0f, 0f,  0f, 0f,

           
            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f,  0f, 1f,
             0.5f,  0.5f, -0.5f,  0f, 1f, 0f,  1f, 1f,
             0.5f,  0.5f,  0.5f,  0f, 1f, 0f,  1f, 0f,
            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f,  0f, 1f,
             0.5f,  0.5f,  0.5f,  0f, 1f, 0f,  1f, 0f,
            -0.5f,  0.5f,  0.5f,  0f, 1f, 0f,  0f, 0f,

           
            -0.5f, -0.5f,  0.5f,  0f,-1f, 0f,  0f, 1f,
             0.5f, -0.5f,  0.5f,  0f,-1f, 0f,  1f, 1f,
             0.5f, -0.5f, -0.5f,  0f,-1f, 0f,  1f, 0f,
            -0.5f, -0.5f,  0.5f,  0f,-1f, 0f,  0f, 1f,
             0.5f, -0.5f, -0.5f,  0f,-1f, 0f,  1f, 0f,
            -0.5f, -0.5f, -0.5f,  0f,-1f, 0f,  0f, 0f,

          
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  0f, 0f,
             0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  1f, 0f,
             0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  1f, 1f,
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  0f, 0f,
             0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  1f, 1f,
            -0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  0f, 1f,

           
             0.5f, -0.5f, -0.5f,  0f, 0f,-1f,  0f, 0f,
            -0.5f, -0.5f, -0.5f,  0f, 0f,-1f,  1f, 0f,
            -0.5f,  0.5f, -0.5f,  0f, 0f,-1f,  1f, 1f,
             0.5f, -0.5f, -0.5f,  0f, 0f,-1f,  0f, 0f,
            -0.5f,  0.5f, -0.5f,  0f, 0f,-1f,  1f, 1f,
             0.5f,  0.5f, -0.5f,  0f, 0f,-1f,  0f, 1f
        };

        public Game(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);

            // Build and compile shader
            _shader = new Shader("phong.vert", "phong.frag");
            _shader.Use();

            
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            int stride = 8 * sizeof(float);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

          
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));

            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));

            GL.BindVertexArray(0);

            // shader defaults
            _shader.Use();
            _shader.SetVector3("lightColor", _lightColor);
            _shader.SetVector3("objectColor", _objectColor);

            // hide cursor for mouse look
            this.CursorState = CursorState.Hidden;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(0.1f, 0.1f, 0.12f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // model, view, projection
            _model = Matrix4.CreateRotationY((float)GLFW.GetTime() * 0.2f)  ;
            //if  you want to rotate it add this and remove identity to avoid user interactions CreateRotationY((float)GLFW.GetTime() * 0.2f) * Matrix4.CreateRotationX(0.25f); since it gives gentle rotation so you can see lighting
            var view = Matrix4.LookAt(_cameraPos, _cameraPos + _cameraFront, _cameraUp);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            // set camera and light uniforms
            _shader.SetVector3("viewPos", _cameraPos);
            _shader.SetVector3("lightPos", _lightPos);
            _shader.SetFloat("ambientStrength", 0.12f);
            _shader.SetFloat("specularStrength", 0.7f);
            _shader.SetInt("shininess", 32);

            // draw cube
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
                Close();

            // camera movement
            float delta = (float)args.Time;
            if (input.IsKeyDown(Keys.W))
                _cameraPos += _cameraFront * _speed * delta;
            if (input.IsKeyDown(Keys.S))
                _cameraPos -= _cameraFront * _speed * delta;
            if (input.IsKeyDown(Keys.A))
                _cameraPos -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed * delta;
            if (input.IsKeyDown(Keys.D))
                _cameraPos += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed * delta;
            if (input.IsKeyDown(Keys.Space))
                _cameraPos += _cameraUp * _speed * delta;
            if (input.IsKeyDown(Keys.LeftControl))
                _cameraPos -= _cameraUp * _speed * delta;

            // light movement (arrow keys)
            if (input.IsKeyDown(Keys.Up))
                _lightPos.Z -= 2f * delta;
            if (input.IsKeyDown(Keys.Down))
                _lightPos.Z += 2f * delta;
            if (input.IsKeyDown(Keys.Left))
                _lightPos.X -= 2f * delta;
            if (input.IsKeyDown(Keys.Right))
                _lightPos.X += 2f * delta;

            // mouse look
            var mouse = MouseState;
            var mousePos = mouse.Position;
            if (_firstMove)
            {
                _lastMousePos = mousePos;
                _firstMove = false;
            }
            else
            {
                var deltaX = mousePos.X - _lastMousePos.X;
                var deltaY = mousePos.Y - _lastMousePos.Y;
                _lastMousePos = mousePos;

                _yaw += deltaX * _sensitivity;
                _pitch -= deltaY * _sensitivity;
                _pitch = Math.Clamp(_pitch, -89f, 89f);

                Vector3 front;
                front.X = MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));
                front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
                front.Z = MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));
                _cameraFront = Vector3.Normalize(front);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vbo);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();
        }
    }
}
