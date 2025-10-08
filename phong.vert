#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec4 worldPos = model * vec4(aPosition, 1.0);
    FragPos = vec3(worldPos);

    // normal matrix: inverse-transpose of model for correct normal transform
    Normal = mat3(transpose(inverse(model))) * aNormal;

    TexCoord = aTexCoord;
    gl_Position = projection * view * worldPos;
}
