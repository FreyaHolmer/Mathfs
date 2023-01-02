# Mathfs
Freya's expanded math functionality for Unity!
- This is primarily a way for me to share the math functionality I write and use in my own personal projects
- I will recklessly edit and adapt things without too much thought into backwards compatibility
- Minimum Unity version is currently 2021.2 due to using newer C# version features. It may be possible to auto-downgrade through your IDE if necessary
- Commits with version tags should be relatively stable. Other commits may not be

## Installation instructions

There are several ways to install this library into your project:

- **Plain install**
   - Clone or [download](https://github.com/FreyaHolmer/Mathfs/archive/refs/heads/master.zip) this repository and put it somewhere in the Assets folder of your Unity project
- **Unity Package Manager (UPM)**:
   - Add either of the the following lines to *Packages/manifest.json*:
   - `"com.acegikmo.mathfs": "https://github.com/FreyaHolmer/Mathfs.git#0.1.0",` if you want to target a specific version (recommended)
   - `"com.acegikmo.mathfs": "https://github.com/FreyaHolmer/Mathfs.git",` if you want to pull the latest commit (potentially unstable)
   - More information about UPM and git [here](https://docs.unity3d.com/Manual/upm-git.html)
- **[OpenUPM](https://openupm.com)**
   - After installing [openupm-cli](https://github.com/openupm/openupm-cli), run the following command:
   - `openupm add com.acegikmo.mathfs`

After installation you will be able to access the library in scripts by including the namespace `using Freya`

## Features
 - 2D Intersection tests between all combinations of:
   - Ray
   - LineSegment
   - Line
   - Circle
 - Curves & Splines
   - Bézier (Quadratic, Cubic & Generalized)
   - Hermite
   - Catmull-Rom
   - B-Spline (Uniform Cubic & Generalized Non-Uniform)
   - NURBS (Non-Unifrom Rational B-Spline)
   - Trajectory (Cubic & Generalized)
 - Trajectory math
   - GetDisplacement (point in trajectory), given gravity, angle, speed & time
   - GetLaunchSpeed, given gravity, angle & lateral distance
   - GetLaunchAngles, given gravity, speed & lateral distance
   - GetMaxRange, given gravity & speed
   - GetHeightPotential, given gravity, current height and speed
   - GetSpeedFromHeightPotential, given gravity, current height and height potential
 - Triangle math
   - Area / SignedArea, given three points or base and height
   - Contains check, given three triangle vertices and a point to test by
   - Right-angle trig functions to calculate Opposite/Adjacent/Hypotenuse/Angle
   - Incenter / Centroid
   - Incircle / Circumcircle
   - SmallestAngle
 - Polygon math
   - Area / SignedArea
   - IsClockwise
   - WindingNumber
   - Contains
 - Circle math
   - FromTwoPoints (get smallest circle passing through both points)
   - FromThreePoints (get unique circle passing through three points)
   - RadiusToArea / AreaToRadius
   - AreaToCircumference / CircumferenceToArea
   - RadiusToCircumference / CircumferenceToRadius
 - 2D Angle helpers (AngToDir, DirToAng...)
 - 2D Vector extension methods (Rotate90CCW/CW, Rotate, RotateAround...)
 - Quadratic & Linear Root finders
 - Remap functions
 - Constants (Tau, Pi, Golden Ratio, e, sqrt2)
 - Vector extension methods (WithMagnitude, ClampMagnitude(min,max)...)
 - Expanded basic math operations to vectors (Clamp, Round, Abs...)
 - Color extensions (WithAlpha, MultiplyRGB...)
 - Smoothing functions (Smooth01, SmoothCos01...)
 - Triangle Math helpers (SignedArea, Circumcenter, Incircle...)
 - Circle Math helpers (Area, Circumference...)
 - All functions use radians
 - And more!

## Changes
Mathfs.cs **does not fully match Unity's Mathf.cs**, I've made a few changes:
 - All angles are in radians, no methods use degrees
 - Lerp and InverseLerp:
   - Unclamped by default
   - LerpClamped/InverseLerpClamped are now the special case functions/exceptions
   - Uses the more numerically stable evaluation
 - Smoothstep is removed in favor of the more explicit:
   - LerpSmooth (which is how it was implemented) and
   - InverseLerpSmooth (which is how it is implemented everywhere but Unity's Mathf.cs)
 - Min/Max functions with arbitrary inputs/array input will throw on empty instead of returning 0
