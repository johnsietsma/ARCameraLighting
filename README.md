# AR Environmental Lighting
Captures the camera video frame and uses it for spherical environment mapping.
ARKit and ARCore supported.

Demonstrates two methods of lighting
- Realtime GI
Captures the camera feed and uses it as a spherical environment map in the skybox.
Requres a light probe in the scene.
Optionally uses relfection probes for environmental reflection

- Direct Lighting
Uses the sphereical environment map as a final color modifier in a custom shader.
