# AR Environmental Lighting
Tested on Unity 2018.1.0f2

Captures the camera video frame and uses it for spherical environment mapping.
ARKit 1.5 and ARCore 1.2 supported.

The main stage are:
1. Capture the Camera Background

The camera background is blitted to a RenderTexture. This RenderTexture will be smaller then the screen size. This makes the following operations more performant and gives us some cheap filtering when it is downscaled.
The blit must be done in a CommandBuffer so that the external camera sampler is access correctly. The texture is accessed in a platform specific way. 
ARCore relies on an external sampler OpenGLES extension, so requires a GLSL shader.
ARKit uses a YCrCb image format with multiple buffers. The shaders reconstructs the RGB color.

2. Blur the Background

The lighting is an approximation, so we blur the camera blur to avoid bright, flickering highlights.

4. Lighting

The background is wrapped around the scene much like a MatCap or spherical environment map. This is not strictly accurate as the camera edges are wrapped around to the front of the object, but gives a "good enough" result in most AR scenes.

The shader calculates the reflection vector and then converts that into a UV coordinate.

There are two ways to light the scene. The first uses Unity's real-time GI system by combining a skybox with reflection and light probes in the scene. This works with any existing materials. The view direction must be corrected in the skybox shader to account for the pass for each skybox face.

The second uses the "finalcolor" function in a shader, so it requires a customer shader. This method is more performant.


## Usage
Tap on the screen to set the position of the objects in the world.

## Video

[![Robot Kyle Walking](https://img.youtube.com/vi/qcICjAeqFZE/0.jpg)](https://youtu.be/qcICjAeqFZE)
