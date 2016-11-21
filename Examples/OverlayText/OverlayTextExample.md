Overlaying Text on Images
===
Imagine you need to create Steam capsule images for an upcoming sale. You've got a capsule image and some localized text to be inserted in a space in the center of each image. Using the `OverlayText` tool you can do this automatically. First let's start with your files on disk:

    Examples\OverlayText
    Examples\OverlayText\Capsule1.png
    Examples\OverlayText\OverlayText.json

`Capsule1.png` is your image and `OverlayText.json` is the configuration file needed by the Power Tool. The configuration file will contain the text you want to add to the image as well as additional information. Open the configuration file and enter:

    {
    "OutputFolderPath": "Output", // Relative path where the generated images will be saved 
    "Debug": false, // Set to true if you want to debug information overlaid on the generated images
    "LogFilePath":  "OverlayTextLog.txt", // If you want logs use this, else delete
    // You can define one or more jobs to be executed by the tool. In this case we just have one
    "Jobs": [
        {
        "Name": "MyFirstJob", // The job's name
        "ImagePath": "Capsule1.png", // Relative path from the execution directory to the template image
        "FontName": "Times New Roman", // Font name which is installed on the computer
        "FontColor": "#333333", // Any hex value will work. Put alpha first if you want transparency
        "MaxFontSize": 48, // The maximum size text will be rendered at -- defaults to 64
        "OutputImageNameTemplate": "Capsule1_{0}.png", // Format string for the generated images
        // This is the rectangle in which the text will be drawn
        "BoundingBox": {
            "X": 103,
            "Y": 150,
            "Width": 412,
            "Height": 53
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
        }
    ]
    }

With the configuration file defined we're all set to run the tool. From the `Examples` directory run:

    image-powertools --tool=OverlayText --config=OverlayText\OverlayText.json
    Finished running tool: OverlayText. Exited with code: 0 The program exited without errors.

Now you can navigate to `Examples\OverlayText\Output` and view the four generated images.