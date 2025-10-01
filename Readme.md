# OpenGL Setup & First Render – Assignment 2(Updated)

##  Overview
This project is a simple OpenGL setup using **C# with OpenTK**.  
It opens a blank window and renders a basic **2D rectangle** in the center of the screen.  



---

##  Library Used
- **[OpenTK](https://opentk.net/)** – a C# wrapper for OpenGL, OpenAL, and OpenCL, fully cross-platform (Windows, Linux, macOS).
 can be installed using NuGet in Visual Studio 2022.
- `Vector3`, `Matrix4`, and `Quaternion`
- Creating transformation matrices
- Rendering a rotating/scaling rectangle on screen

---

## Features
- Vector operations: addition, subtraction, dot product, cross product.
- Matrix operations: identity matrix, scaling, rotation (X-axis), and multiplication.
- Demonstration program prints all results to the console.
- Optional visualization: OpenGL window renders a rotating rectangle  using a model matrix.

-----

## Operations Implemented

### Vector Operations
- Addition: `v1 + v2`
- Subtraction: `v1 - v2`
- Dot Product: `Vector3.Dot(v1, v2)`
- Cross Product: `Vector3.Cross(v1, v2)`

###  Matrix Operations
- Identity Matrix: `Matrix4.Identity`
- Scaling: `Matrix4.CreateScale()`
- Rotation: `Matrix4.CreateRotationX()` 
- Multiplication: `combining scale * rotation to transform a point`

##  How to Run

### Prerequisites
- [.NET 7 or .NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Visual Studio 2022 (with **.NET desktop development** workload)
- Git (to clone the repository)

---

### Using Visual Studio 2022 (Recommended for Windows) 
- Clone the file using git clone or download the zip

#### Open the project:

- Launch Visual Studio 2022
- Go to File → Open → Project/Solution
- Select the .sln file inside the repo folder


#### Run the program:

- Press F5 (Debug mode) or Ctrl+F5 (Run without debugging)

- A window should appear with a green rectangle in the center rotating

- And console showing this output:
  
  === Vector Operations ===
  
  v1 + v2 = (5, 9, 13)
 
  v1 - v2 = (-3, -3, -5)
 
  Dot(v1, v2) = 58
 
  Cross(v1, v2) = (3, 7, -6)

 
  === Matrix Operations ===
 
  Identity Matrix:
 
  (1, 0, 0, 0)
 
  (0, 1, 0, 0)
 
  (0, 0, 1, 0)
 
  (0, 0, 0, 1)
 
  Transformed Point (scale + rotate): (2, 0, 0)
---
