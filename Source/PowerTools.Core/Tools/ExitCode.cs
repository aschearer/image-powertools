namespace SpottedZebra.PowerTools.Core.Tools
{
    using System.ComponentModel;

    internal enum ExitCode : int
    {
        [Description("The program exited without errors.")]
        Success = 0,
        [Description("The specified tool was not found.")]
        ToolNotFound = 100,
        [Description("Unexpected error processing request.")]
        UnexpectedError = 101,
        [Description("Unexpected options provided to program.")]
        UnexpectedOptions = 102,
        [Description("Configuration file not found.")]
        ConfigurationFileNotFound = 103,
        [Description("Configuration file could not be parsed.")]
        BadConfiguration = 104,
        [Description("Text color could not be parsed.")]
        OverlayText_BadColor = 200,
        [Description("Font not valid.")]
        OverlayText_BadFont = 201,
        [Description("Template image not found.")]
        OverlayText_ImageNotFound = 202,
        [Description("Could not open template image.")]
        OverlayText_BadImage = 203,
        [Description("Could not fit text in bounding box.")]
        OverlayText_TextWontFit = 204,
        [Description("Could not save generated image.")]
        OverlayText_CouldNotSaveImage = 205,
        [Description("BaseImagesFolderPath not found.")]
        ComposeImages_BaseImagesFolderNotFound = 300,
        [Description("Could not open image in BaseImagesFolderPath.")]
        ComposeImages_BadBaseImages = 301,
        [Description("Could not open folder in ImagesToCombinePaths.")]
        ComposeImages_ImageDirectoryNotFound = 302,
        [Description("Could not open image in ImagesToCombinePaths.")]
        ComposeImages_BadFile = 303,
        [Description("Could not save generated image.")]
        ComposeImages_CouldNotSaveImage = 304,
    }
}