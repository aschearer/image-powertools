    namespace SpottedZebra.PowerTools.Core.Tools
{
    using Data;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    [PowerTool(typeof(ComposeImages), "ComposeImages", "Combine a number of images into a single image.")]
    internal class ComposeImages : PowerToolBase<ComposeImagesJob>
    {
        protected override void OnProcess(ComposeImagesJob jobDescription)
        {
            this.Info("Starting job {0}. Job details:", jobDescription.Name);
            this.Info("Base images folder path: {0}", jobDescription.BaseImagesFolderPath);
            this.Info("Number of folders to combine: {0}", jobDescription.ImagesToCombinePaths.Length);

            if (!Directory.Exists(jobDescription.BaseImagesFolderPath))
            {
                this.Error("BaseImagesFolderPath does not exist");
                return;
            }

            Bitmap baseImage = null;

            try
            {
                List<Bitmap> baseImages = new List<Bitmap>();
                var baseImagePaths = Directory.GetFiles(jobDescription.BaseImagesFolderPath);
                foreach (var imagePath in baseImagePaths)
                {
                    using (var image = new Bitmap(imagePath))
                    {
                        if (baseImage == null)
                        {
                            baseImage = new Bitmap(image);
                        }
                        else
                        {
                            using (var graphics = Graphics.FromImage(baseImage))
                            {
                                graphics.DrawImage(image, Point.Empty);
                            }
                        }
                    }
                }
            }
            catch
            {
                this.Error("Unable to open images at BaseImagesFolderPath");
            }

            if (baseImage == null)
            {
                return;
            }

            var componentToFiles = new Dictionary<string, string[]>();
            int totalImagesToGenerate = 0;

            foreach (var component in jobDescription.ImagesToCombinePaths)
            {
                if (!Directory.Exists(component))
                {
                    this.Error("Directory not found: {0}", component);
                    return;
                }

                componentToFiles[component] = Directory.GetFiles(component);
                if (totalImagesToGenerate == 0)
                {
                    totalImagesToGenerate = componentToFiles[component].Length;
                }
                else
                {
                    totalImagesToGenerate *= componentToFiles[component].Length;
                }
            }

            int[] imagePointers = new int[jobDescription.ImagesToCombinePaths.Length];
            for (int i = 0; i < totalImagesToGenerate; i++)
            {
                using (var resultImage = new Bitmap(baseImage))
                using (var graphics = Graphics.FromImage(resultImage))
                {
                    string[] fileNameArgs = new string[imagePointers.Length];
                    for (int j = 0; j < imagePointers.Length; j++)
                    {
                        var component = jobDescription.ImagesToCombinePaths[j];
                        var file = componentToFiles[component][imagePointers[j]];
                        var image = new Bitmap(file);
                        graphics.DrawImage(image, Point.Empty);
                        fileNameArgs[j] = Path.GetFileNameWithoutExtension(file);
                    }

                    var targetFile = string.Format(
                        Path.Combine(this.Batch.OutputFolderPath, jobDescription.OutputImageNameTemplate),
                        fileNameArgs);
                    resultImage.Save(targetFile);
                }

                imagePointers[0]++;
                int currentComponent = 0;
                while (currentComponent < imagePointers.Length &&
                       imagePointers[currentComponent] >= componentToFiles[jobDescription.ImagesToCombinePaths[currentComponent]].Length)
                {
                    imagePointers[currentComponent] = 0;
                    if (currentComponent < imagePointers.Length - 1)
                    {
                        imagePointers[currentComponent + 1]++;
                    }

                    currentComponent++;
                }
            }
        }
    }
}