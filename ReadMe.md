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

TODO

1. Give example of configuration file
1. Talk through results
1. Link to detailed instructions for each tool

[1]: http://tumblestonegame.com
[two]: https://github.com/aschearer/image-powertools
[3]: https://github.com/aschearer/image-powertools
[4]: https://github.com/aschearer/image-powertools/issues
