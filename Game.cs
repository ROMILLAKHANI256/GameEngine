using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;
        private int ebo;
        private int textureHandle;


        private int modelLoc, viewLoc, projLoc;

        private float rotationAngle;
        private float scaleFactor = 1f;
        private bool scalingUp = true;


        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Size = new Vector2i(1280, 768);
            this.CenterWindow(this.Size);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(new Color4(0.5f, 0.7f, 0.8f, 1f));
            GL.Enable(EnableCap.DepthTest);


            // Define a single triangle in normalized device coordinates
            float[] vertices =
            {
                // Front face
                -0.5f, -0.5f,  0.5f,  0f, 0f,
                 0.5f, -0.5f,  0.5f,  1f, 0f,
                 0.5f,  0.5f,  0.5f,  1f, 1f,
                -0.5f,  0.5f,  0.5f,  0f, 1f,

                // Back face
                -0.5f, -0.5f, -0.5f,  1f, 0f,
                 0.5f, -0.5f, -0.5f,  0f, 0f,
                 0.5f,  0.5f, -0.5f,  0f, 1f,
                -0.5f,  0.5f, -0.5f,  1f, 1f,

                // Left face
                -0.5f, -0.5f, -0.5f,  0f, 0f,
                -0.5f, -0.5f,  0.5f,  1f, 0f,
                -0.5f,  0.5f,  0.5f,  1f, 1f,
                -0.5f,  0.5f, -0.5f,  0f, 1f,

                // Right face
                 0.5f, -0.5f, -0.5f,  1f, 0f,
                 0.5f, -0.5f,  0.5f,  0f, 0f,
                 0.5f,  0.5f,  0.5f,  0f, 1f,
                 0.5f,  0.5f, -0.5f,  1f, 1f,

                // Top face
                -0.5f,  0.5f,  0.5f,  0f, 0f,
                 0.5f,  0.5f,  0.5f,  1f, 0f,
                 0.5f,  0.5f, -0.5f,  1f, 1f,
                -0.5f,  0.5f, -0.5f,  0f, 1f,

                // Bottom face
                -0.5f, -0.5f,  0.5f,  0f, 1f,
                 0.5f, -0.5f,  0.5f,  1f, 1f,
                 0.5f, -0.5f, -0.5f,  1f, 0f,
                -0.5f, -0.5f, -0.5f,  0f, 0f,
            };
            // Adding new index array
            uint[] indices =
            {
                0,1,2, 2,3,0,
                4,5,6, 6,7,4,
                0,4,7, 7,3,0,
                1,5,6, 6,2,1,
                3,2,6, 6,7,3,
                0,1,5, 5,4,0
            };



            // Generate VBO and VAO
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);



            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);


            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);


            GL.BindVertexArray(0);

            // Vertex shader with model, view, projection matrices
            string vertexShaderCode = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoord;

                out vec2 TexCoord;

                uniform mat4 uModel;
                uniform mat4 uView;
                uniform mat4 uProj;

                void main()
                {
                    gl_Position = uProj * uView * uModel * vec4(aPos, 1.0);
                    TexCoord = aTexCoord;
                }

            ";

            string fragmentShaderCode = @"
                #version 330 core
                out vec4 FragColor;

                in vec2 TexCoord;
                uniform sampler2D texture0;

                void main()
                {
                    FragColor = texture(texture0, TexCoord);
                }

            ";

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            // Get uniform locations
            modelLoc = GL.GetUniformLocation(shaderProgramHandle, "uModel");
            viewLoc = GL.GetUniformLocation(shaderProgramHandle, "uView");
            projLoc = GL.GetUniformLocation(shaderProgramHandle, "uProj");


            // Load texture
            textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            string path = "Assets/mipmaps2.jpg";
            string fullPath = Path.GetFullPath(path);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Texture file not found: {fullPath}");

            using (Bitmap bmp = new Bitmap(fullPath))
            {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                BitmapData data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
            }


            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            rotationAngle += (float)args.Time; // rotate continuously

            if (scalingUp)
            {
                scaleFactor += (float)args.Time;
                if (scaleFactor >= 1.5f) scalingUp = false;
            }
            else
            {
                scaleFactor -= (float)args.Time;
                if (scaleFactor <= 0.5f) scalingUp = true;
            }

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgramHandle);

            // View matrix (camera looking at origin)
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);

            // Projection matrix (perspective)
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                (float)Size.X / Size.Y,
                0.1f,
                100f
            );

            // Send view and projection to shader (same for all triangles)
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.BindVertexArray(vertexArrayHandle);

            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, rotationAngle);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(rotation);
            Matrix4 scaleMatrix = Matrix4.CreateScale(scaleFactor);
            Matrix4 model = scaleMatrix * rotationMatrix;

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

            // Bind texture before drawing
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, "uTexture"), 0);


            GL.BindVertexArray(0);
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.DeleteBuffer(ebo);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }

        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
    }
}