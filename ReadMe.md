# Spotted Zebra's Image Power Tools
While developing [Tumblestone][1] we needed to create a lot of images. In all there were over 500 images that varied by language and platform. Creating all the different versions once would have taken a while, but there was no way I was going to regenerate these images every time our strings changed or our marketing images changed! So I wrote these tools to automate the process.

## Requirements
Power Tools is a command line program written for Windows. It requires .NET 4.5.2.

## How it Works
As the name suggests the project contains a set of tools to help manage your images. To get started download the latest executable here: [Power Tools Command][2]. In PowerShell or Command navigate to the exe and run:

    image-powertool

    Spotted Zebra Power Tools: Increase your efficiency when working with images.
      -h, -?, --help             Prints this help message.
          --tool=NAME            NAME of the tool you want to run. Valid options:

                                   OverlayText: Add text to an image inside a
                                   bounding box.

                                   ComposeImages: Combine a number of images into a
                                   single image.

By default the program prints its help. Next let's specify a tool to run:

    image-powertool --tool=OverlayText

    OverlayText: Add text to an image inside a bounding box.
    How to operate:
      -v, --verbose              All logging will be written to the console.
      -h, -?, --help             Prints this help message.
          --config=PATH          PATH to JSON configuration file.

Just like before the program printed it's help. But this time the help is scoped to the tool you invoked. The final piece of the puzzle is a JSON configuration file which the tool will use in order to operate.

[1]: http://tumblestonegame.com
[2]: download url