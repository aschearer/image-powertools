Compose Sets of Images
===
Imagine your launch is approaching and you need to create end slates for your launch trailer. Your game will be launching in multiple markets and on multiple platforms, so you need end slates for every combination of platform, ratings board, and language. To be specific, if you target 3 platforms, have ratings from 3 boards, and support 8 languages then you need to create 72 images! You could brew a pot of coffee and grind through them all, or you could run `ComposeImages` and catch up on reddit.

Lay of the Land
---
First off let's look at the files and directories in this example:

    Examples\ComposeImages // You are here
    Examples\ComposeImages\Input\Common // Put images that will not vary per end slate here
    Examples\ComposeImages\Input\Ratings // Put the ratings board images here
    Examples\ComposeImages\Input\Platforms // Put the platform specific logos here
    Examples\ComposeImages\Input\Languages // Put the language specific images here
    Examples\ComposeImages\ComposeImages.json // The configuration file

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
        "BaseImagesFolderPath": "Input/Common",
        // Paths to folders containing images to be combined with the base images and each other
        "ImagesToCombinePaths": [
            "Input/Languages",
            "Input/Ratings",
            "Input/Platforms"
        ]
        }
    ]
    }

Just like before, we're all set to run the tool. From the `Examples\` directory run:

    image-powertools --tool=ComposeImages --config=Examples\ComposeImages\ComposeImages.json
    Finished running tool: ComposeImages. Exited with code: 0 The program exited without errors.

Now you can navigate to `Examples\ComposeImages\Output` and view the generated images.
