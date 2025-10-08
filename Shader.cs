using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.IO;

namespace PhongOpenTK
{
    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertPath, string fragPath)
        {
            var vertexSource = File.ReadAllText(vertPath);
            var fragmentSource = File.ReadAllText(fragPath);

            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);
            CheckShaderCompile(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);
            CheckShaderCompile(fragment);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, fragment);
            GL.LinkProgram(Handle);
            CheckProgramLink(Handle);

            GL.DetachShader(Handle, vertex);
            GL.DetachShader(Handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        private void CheckShaderCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($"Shader compilation failed: {info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"Program linking failed: {info}");
            }
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetInt(string name, int value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        public void SetFloat(string name, float value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        public void SetVector3(string name, Vector3 vec) => GL.Uniform3(GL.GetUniformLocation(Handle, name), vec);
        public void SetMatrix4(string name, Matrix4 mat) => GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref mat);

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
