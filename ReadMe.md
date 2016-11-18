Image Power Tools
===
[![Build status](https://ci.appveyor.com/api/projects/status/i7j8axrrek8vxscr?svg=true)](https://ci.appveyor.com/project/aschearer/image-powertools)

While developing [Tumblestone][1] I needed to create a lot of images. In all there were over 500 images that varied by language and platform. Creating all the different versions once would have taken a while, but there was no way I was going to regenerate these images every time our strings or marketing images changed! So I wrote this tool to automate the process.

Questions, bug reports or feature requests?
---
Do you have feature requests, questions or would you like to report a bug? Please post them on the [issue list][4] and follow [these guidelines][3].

Contributing
---
As this projected is maintained by one person, I cannot fix every bug or implement every feature on my own. So contributions are really appreciated!

A good way to get started:

1. Fork the Image Power Tools repos. 
1. Create a new branch in you current repos from the 'master' branch.
1. 'Check out' the code with Git or [GitHub Desktop](https://desktop.github.com/)
1. Check [contributing][3]
1. Push commits and create a Pull Request (PR) to ImagePowerTools

License
---
Image Power Tools is open source software, licensed under the terms of MIT license. 
See [License.txt](License.txt) for details.

How to Build
---
Image Power Tools is a command line program written for Windows. It requires .NET 4.6.1. Use Visual Studio and open solution file. You may need to restore Nuget packages on first run.

How to Run
---
As the name suggests the project contains a set of tools to help manage your images. To get started download the latest executable here: [Power Tools Command][two]. In PowerShell or Command navigate to the exe and run:

    image-powertool

    Image Power Tools: Increase your efficiency when working with images.
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

Just like before the program printed its help. But this time the help is scoped to the tool you invoked. The final piece of the puzzle is a JSON configuration file which the tool will use in order to operate.

Overlaying Text on Images
---
Imagine you need to create Steam capsule images for an upcoming sale. You've got a capsule image and some localized text to be inserted in a space in the center of each image. Using the `OverlayText` tool you can do this automatically. First let's start with your files on disk:

    C:\Example
    C:\Example\Capsule1.png
    C:\Example\PrepareCapsule.json

`Capsule1.png` is your image and `PrepareCapsule.json` is the configuration file needed by the Power Tool. The configuration file will contain the text you want to add to the image as well as additional information. Open the configuration file and enter:

    {
      "OutputFolderPath": "Output", // Relative path where the generated images will be saved 
      "Debug": false, // Set to true if you want to debug information overlaid on the generated images
      "LogFilePath":  "OverlayImagesLog.txt", // If you want logs use this, else delete
      // You can define one or more jobs to be executed by the tool. In this case we just have one
      "Jobs": [
        {
          "Name": "MyFirstJob", // The job's name
          "ImagePath": "Example/Capsule1.png", // Relative path from the execution directory to the template image
          "FontName": "Times New Roman", // Font name which is installed on the computer
          "FontColor": "#333333", // Any hex value will work. Put alpha first if you want transparency
          "MaxFontSize": 48, // The maximum size text will be rendered at -- defaults to 64
          "OutputImageNameTemplate": "Capsule_{0}.png", // Format string for the generated images
          // This is the rectangle in which the text will be drawn
          "BoundingBox": {
            "X": 120,
            "Y": 82,
            "Width": 221,
            "Height": 35
          },
          // The text to be rendered. Label names will be inserted in the OutputImageNameTemplate to name the generated images
          "Labels": [
            {
              "Name": "en-US",
              "Value": "Starter Pack"
            },
            {
              "Name": "de-DE",
              "Value": "Mehrspieler-Starterpaket"
            },
            {
              "Name": "zh-CN",
              "Value": "多玩家新手包",
              "FontName": "Arial", // You can override the font for Asian languages
              "DisableAntiAliasing": true // You can disable anti-aliasing if you want, use trial and error to find the right settings
            },
            {
              "Name": "ja-JP",
              "Value": "マルチプレイスターターパック",
              "FontName": "Arial",
              "DisableAntiAliasing": true
            }
          ]
        },
      ]
    }

With the configuration file defined we're all set to run the tool. From the `C:\` directory run:

    image-powertools --tool=OverlayText --config=Example\PrepareCapsule.json
    Finished running tool: OverlayText.

Now you can navigate to `C:\Output` and view the four generated images.

TODO

1. Give example of configuration file
1. Talk through results
1. Link to detailed instructions for each tool

[1]: http://tumblestonegame.com
[two]: https://github.com/aschearer/image-powertools
[3]: https://github.com/aschearer/image-powertools
[4]: https://github.com/aschearer/image-powertools/issues
