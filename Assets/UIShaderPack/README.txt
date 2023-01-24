******************
*  Introduction  *
******************
Thanks for purchasing the UI Shader Pack. Here you will find any implementation notes to help you make the post of this package.
To use these shaders, create a new material or use the supplied ones and attach to an Image or RawImage component which is the child of a Canvas element.
See the demo scene for some implementation ideas.
Please email me at cneideck@mail.com if you require further support.

******************
*    Features    *
******************
    * 15 Shaders
    * Compatible with All 3 Canvas types (Screen Space - Overlay, Screen Space - Camera, World Space)
    * Heaps of customisation through exposed properties
    * Variety of support for colour, alpha and textures from Image component
    * Compatible with LWRP

******************
*    Shaders     *
******************
    * Waves
        Procedurally creates waves of variable width, height and color
    
    * Circles
        Creates anti aliased circles that look sharp at any size. Great for UI buttons
    
    * Fire
        Simple 2D fire effect based off a noise texture
    
    * Gradient
        Linear gradient with 360 degree rotation
    
    * Rainbow
        Same as gradient but generates a rainbow color
    
    * Checkerboard
        Creates a checkerboard pattern with variable rows and columns
    
    * Additive, Scroll, Distort
        A 3-in-1 shader with additive blend, texture scroll and distortion
        
    * Rounded Rectangle
        Creates anti-aliased rounded rectangles with variable radius
        
    * Dissolve
        Dissolve an Image based off a noise texture with variable colour (gradient optional) and edge fade
    
    * Invert
        Inverts the colour of the Image texture
        
    * Desaturate
        Desaturates the Image texture (removes color/greyscale)
    
    * Sepia
        Adds a sepia effect to the Image texture
    
    * Blur
        Blurs the Image texture
    
    * Blur Behind (Uses GrabPass)
        Blurs the contents of the screen behind the Image
    
    * Distort Behind (Uses GrabPass)
        Distorts the contents of the screen behind the Image

******************
*  Usage Details *
******************

 General
---------------------
    * The smoothness or softness properties control how much the edges fade out
    * Ensure noise textures or other scrolling textures have their Wrap Mode set to Repeat in their import settings
    * BlurBehind and DistortBehind use GrabPass which is not very performant so use sparingly

 Waves
---------------------
    * Enabling secondary color creates a gradient which uses the Image component's color at the bottom and the secondary color at the top.
    * Increasing the horizontal scale brings the waves closer together
    * Increasing the verticle scale gives the waves a greater range
    * The vertical offset moves the entire wave up
    * Speed can go negative to move left to right
    
 Circles
---------------------
    * Keep Edge Softness at around 1.0 to 2.0 to have anti-aliased circles at any size
    * Rotate Speed does not go negative, to have counter-clockwise rotation, you can set the transform's negative X scale to -1

 Fire
---------------------
    * Decrease the Noise Texture's Y offset to move the entire fire up
    * Adjust the Middle Color Offset to position the center line of the gradient

 Gradient & Rainbow
---------------------
    * Set the scale to 0 and increase the speed to create a transition effect for the entire Image
    
 Checkerboard
---------------------
    * Match the aspect ratio of the Image to the rows and columns set in the material

 Additive, Scroll, Distort
----------------------------
    * Additive Blend Colour adds the color to the Image so leave black for no effect
    * Scroll can be set independantly for horizontal and vertical. Supports negative values
    * For best results, use a tilable noise texture for disort otherwise a seam may be visible

 Dissolve
---------------------
    * Increasing the Clip Threshold dissolves the Image
    * Color Size increases the size of the edge set by Edge Color
    * The Secondary Color creates a gradient that is scaled by the Color Smoothness property

 Rounded Rect
---------------------
    * For best results, set the Rect Width and Rect Height properties to match the size of the Rect Transform
    * Gradient properties here are the same as the Gradient shader
    * Avoid increasing the radius past half of the rect width or height

 Blur
---------------------
    * The regular blur shader blurs only the Image it is attached to

 Blur Behind, DistortBehind
-----------------------------
    * These shaders apply their effect to whatever is draw behind them, whether it is in 3D world space or other UI elements
    * These shaders have the same properties as their non-behind versions
    * As mentioned above, these shaders use GrabPass which is not very performant so use sparingly
    * See below if using LWRP

******************
*      LWRP      *
******************
All shaders are compatible with the Lightweight Render Pipline.
    
However, please note that the BlurBehind and DistortBehind shaders require some steps to setup
Firstly, they will render upside down so you must uncomment a line in the vertex shader. 
Look for the following:
    
// Uncomment below if using LWRP
// OUT.grabUV.y = 1 - OUT.grabUV.y;  <- Remove the // at the start of this line

Secondly, the LWRP Asset must have the Opaque Texture option ticked.
Finally, these two shaders will only work if the Canvas they are attached to is set to Screen Space - Overlay