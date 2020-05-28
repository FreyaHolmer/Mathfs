# Mathfs
Expanded Math Functionality for Unity

## Features
 - Quadratic & Linear Root finders
 - 2D Angle helpers (AngToDir, DirToAng...)
 - 2D Vector extension methods (Rotate90CCW/CW, Rotate, RotateAround...)
 - Trajectory Math helpers (GetLaunchSpeed, GetMaxRange, TryGetLaunchAngles...)
 - Remap functions
 - Added Tau and the Golden Ratio
 - Vector extension methods (WithMagnitude, ClampMagnitude(min,max)...)
 - Expanded basic math operations to vectors (Clamp, Round, Abs...)
 - Color extensions (WithAlpha, MultiplyRGB...)
 - Smoothing functions (Smooth01, SmoothCos01...)
 - Triangle Math helpers (SignedArea, Circumcenter...)
 - Added alternate angle functions to use radians instead of degrees
 - And more!

## Changes
Mathf.cs has a #define to match Unity's library - **keep this defined if you are porting an existing project to use Mathfs, otherwise I recommend commenting it out**! It makes a few changes:
 - Lerp and InverseLerp
   - Now unclamped by default
   - Now uses the more numerically stable evaluation
   - LerpClamped/InverseLerpClamped are now the special case functions/exceptions
 - Smoothstep is removed in favor of the more explicit:
   - LerpSmooth (which is how it was implemented) and
   - InverseLerpSmooth (which is how it is implemented everywhere but Unity's Mathf.cs)
 - Angle functions will explicitly have a Deg-suffix to indicate degrees (this library adds radian versions as well)
 - Min/Max functions with arbitrary inputs/array input will throw on empty instead of returning 0
