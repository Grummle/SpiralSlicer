# SpiralSlicer
Slicer that does a true spiral cut of an stl

# Problem
All of the slicers that I've been able to find do vase/spiral mode by doing layer cuts then transitioning between those layers in a big x,y jump. This leaves an artifact on vase
mode prints when the line width or layer difference is great enough.
![vaseseam](https://user-images.githubusercontent.com/407186/150284916-4a314755-61eb-4bb9-a27f-19d559c72fcd.png)
![IMG_4361](https://user-images.githubusercontent.com/407186/150285011-6db856f6-a893-4bb9-8e23-d5aca644e7ee.jpg)


# Hack
I wrote (horribly) a slicer that uses a helix in the middle of an stl object to produce rays whose intersection with the walls of the stl to produce a true spiral slice.
It's got a load of cases it won't work for. It also only works for the walls. I've come up with a workflow when usin prusa slicer that lets you use PS to do the base and then
transition to the spiral. It's a pain in the arse, but it does appear to work.

# Workflow
1. Get your STL, solid stl like you'd normally use for a vase.
1. Load into Prusa slicer.
1. Set the position of the stl to x,y 0,0 exactly in prusa slicer.
1. Export the object again as stl.
1. Reload the newly export stl again. This resets the stl's origin to what prusa slicer thinks it should be, and it should be in the middle of the object now.
1. Type in x,y move that puts the object where you'd like it. 
1. Slice as you normally would. In this example vase mode, 1.8mm line width and .2mm layer height.
1. In the gcode preview choose at which layer you'd like to switch to spiral. For this example 1mm layer.
1. Open the generated gcode and find the layer switch to 1.2mm, delete all gcode after that point. Replace it with `{{replaceme}}`
1. Use this utility with the following settings.
```
helixcoordinates.exe 
-o <outputfile> 
-w 1.85 -t <gcode with {{replaceme}}>
-f
-m <path to model you saved out PS>
-x <number you moved model in x in PS>
-y <number you moved model in y in PS>
-l .2
-s 1
-r
```
1. You should now have an output file with the spiralized gcode. Currently the top will be all messed up and you'll need to cut out that gcode. I recommend looking at it in
PS gcode viewer to figure out which line you need to cut it off at.



# Details
```
  -o, --OutputPath            Required. Path to file output should be saved in

  -l, --LayerHeight           Required. Layer height vase will be printed at

  -s, --StartOffset           Amount of object that will be skipped starting at 0

  -g, --segments              (Default: 500) How many line segments should be used

  -v, --verbose               Set output to verbose messages.

  -p, --speed                 (Default: 1900) Speed in mm per min

  -w, --linewidth             (Default: 0.42) Width of the extrsion

  -t, --template              Path to file that has {{replaceme}} where the generated gcode should be placed

  -f, --FirstLayer            (Default: true) Print the first layer flat, varing the layer height

  -x, --xtransform            How many mm to transform the object on the x axis after slicing

  -y, --ytransform            How many mm to transform the object on the y axis after slicing

  -m, --modelpath             Required. Path to model to slice.

  -r, --relative-extrusion    Should extrusion lengths be relative
```



https://www.buymeacoffee.com/grummle
