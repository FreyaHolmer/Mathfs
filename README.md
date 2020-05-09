# Mathfs
Expanded Math Functionality for Unity

## Changes
Mathf.cs has a #define to match Unity's library - **keep this defined if you are porting an existing project to use Mathfs, otherwise I recommend commenting it out**! It makes a few changes outlines in the file itself:
 - Lerp and InverseLerp
   - Now unclamped by default
   - Now uses the more numerically stable evaluation
   - LerpClamped/InverseLerpClamped are now the special case functions/exceptions
 - Smoothstep is removed in favor of the more explicit:
   - LerpSmooth (which is how it was implemented) and
   - InverseLerpSmooth (which is how it is implemented everywhere but Unity's Mathf.cs)
 - Angle functions will explicitly have a Deg-suffix to indicate degrees (this library adds radian versions as well)
 - Min/Max functions with arbitrary inputs/array input will throw instead of returning 0

## Features
 - Constants for Tau and the Golden Ratio
 - Remap functions
 - 2D Angle helpers (AngToDir, DirToAng...)
 - 2D Vector extension methods (Rotate90CCW/CW, Rotate, RotateAround...)
 - Vector extension methods (WithMagnitude, ClampMagnitude(min,max)...)
 - Color extensions (WithAlpha, MultiplyRGB...)
 - Smoothing functions (Smooth01, SmoothCos01...)
 - Triangle Math helpers (SignedArea, Circumcenter...)
 - Trajectory Math helpers (GetLaunchSpeed, GetMaxRange, TryGetLaunchAngles...)
 - Expanded basic math operations to vectors, such as clamp, round, abs, etc.
 - Added alternate angle functions to use radians instead of degrees
 - And more!
