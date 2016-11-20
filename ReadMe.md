Image Power Tools
===
[![Build status](https://ci.appveyor.com/api/projects/status/i7j8axrrek8vxscr?svg=true)](https://ci.appveyor.com/project/aschearer/image-powertools)

While developing [Tumblestone][1] I needed to create a lot of images. In all there were over 500 images that varied by language and platform. Creating all the different versions once would have taken a while, but there was no way I was going to regenerate these images every time our strings or marketing images changed! I wrote this tool to automate the process.

Questions, Bug Reports or Feature Requests?
---
Do you have feature requests, questions or would you like to report a bug? Please post them on the [issue list][4].

Contributing
---
As this projected is maintained by one person, I cannot fix every bug or implement every feature on my own. So contributions are really appreciated!

A good way to get started:

1. Fork the Image Power Tools repo. 
1. Create a new branch in you current repo from the 'master' branch.
1. 'Check out' the code with Git or [GitHub Desktop](https://desktop.github.com/)
1. Push commits and create a Pull Request (PR)

License
---
Image Power Tools is open source software, licensed under the terms of MIT license. 
See [License.txt](License.txt) for details.

How to Build
---
Image Power Tools is a command line program written for Windows. It requires .NET 4.6.1. Use Visual Studio and open solution file under `Source`. You may need to restore Nuget packages on first run.

How to Run
---
As the name suggests the project contains a set of tools to help manage your images. To get started download the latest executable here: [Image Power Tools][2]. In PowerShell or Command navigate to the exe and run:

    image-powertools
    Image Power Tools: Increase your efficiency when working with images.
      -h, -?, --help             Prints this help message.
          --tool=NAME            NAME of the tool you want to run. Valid options:

                                   OverlayText: Add text to an image inside a
                                   bounding box.

                                   ComposeImages: Combine a number of images into a
                                   single image.

By default the program prints its help. Next let's specify a tool to run:

    image-powertools --tool=OverlayText
    OverlayText: Add text to an image inside a bounding box.
    How to operate:
      -v, --verbose              All logging will be written to the console.
      -h, -?, --help             Prints this help message.
          --config=PATH          PATH to JSON configuration file.

Just like before the program printed its help. But this time the help is scoped to the tool you invoked. The final piece of the puzzle is a JSON configuration file which the tool will use in order to operate.

Overlaying Text on Images
---
Imagine you need to create Steam capsule images for an upcoming sale. You've got a capsule image and some localized text to be inserted in a space in the center of each image. Using the `OverlayText` tool you can do this automatically. First let's start with your files on disk:

    C:\Example1
    C:\Example1\Capsule1.png
    C:\Example1\PrepareCapsule.json

`Capsule1.png` is your image and `PrepareCapsule.json` is the configuration file needed by the Power Tool. The configuration file will contain the text you want to add to the image as well as additional information. Open the configuration file and enter:

    {
      "OutputFolderPath": "Output", // Relative path where the generated images will be saved 
      "Debug": false, // Set to true if you want to debug information overlaid on the generated images
      "LogFilePath":  "OverlayTextLog.txt", // If you want logs use this, else delete
      // You can define one or more jobs to be executed by the tool. In this case we just have one
      "Jobs": [
        {
          "Name": "MyFirstJob", // The job's name
          "ImagePath": "Example1/Capsule1.png", // Relative path from the execution directory to the template image
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

    image-powertools --tool=OverlayText --config=Example1\PrepareCapsule.json
    Finished running tool: OverlayText.

Now you can navigate to `C:\Output` and view the four generated images.

Compose Sets of Images
---
Now imagine your launch is approaching and you need to create end slates for your launch trailer. Your game will be launching in multiple markets and on multiple platforms, so you need end slates for every combination of platform, ratings board, and language. To be specific, if you target 3 platforms, have ratings from 3 boards, and support 8 languages then you need to create 72 images! You could brew a pot of coffee and grind through them all, but, instead, just run `ComposeImages` and read reddit.

    C:\Example2
    C:\Example2\Common // Put images that will not vary per end slate here
    C:\Example2\Ratings // Put the ratings board images here
    C:\Example2\Platforms // Put the platform specific logos here
    C:\Example2\Languages // Put the language specific images here
    C:\Example2\CreateEndSlates.json

A few things to call out:

  1. You can put as many images in each directory as you want. But more images means more memory.
  1. Adding non-common images means many more generated images. Try to consolidate things where possible.
  1. All images should be the same size. For example if you want the ratings logo in the bottom right create a full sized image with the logo in the correct position.
  1. Order matters. The tool processes images in alphabetical order. Common images are processed first.
  1. You can name the directories and their images whatever you want.

Let's look at the configuration file:

    {
      "OutputFolderPath": "Output",
      "Debug": false,
      "LogFilePath":  "ComposeImagesLog.txt",
      "Jobs": [
        {
          "Name": "EndSlate",
          // Format for generated images. There should be as many placeholders as ImagesToCombinePaths
          "OutputImageNameTemplate": "EndSlate_{0}_{1}_{2}.png",
          // These are used in every image being generated
          "BaseImagesFolderPath": "Example2/Common",
          // Paths to folders containing images to be combined with the base images and each other
          "ImagesToCombinePaths": [
            "Example2/Languages",
            "Example2/Ratings",
            "Example2/Platforms"
          ]
        }
      ]
    }

Just like before, we're all set to run the tool. From the `C:\` directory run:

    image-powertools --tool=ComposeImages --config=Example2\CreateEndSlates.json
    Finished running tool: ComposeImages.

Now you can navigate to `C:\Output` and view the generated images.

[1]: http://tumblestonegame.com
[2]: https://github.com/aschearer/image-powertools/releases/latest
[3]: https://github.com/aschearer/image-powertools
[4]: https://github.com/aschearer/image-powertools/issues
