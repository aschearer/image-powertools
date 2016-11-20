namespace SpottedZebra.PowerTools.Core.Tools
{
    using Data;
    using System;
    using System.Drawing;
    using System.IO;

    [PowerTool(typeof(OverlayText), "OverlayText", "Add text to an image inside a bounding box.")]
    internal class OverlayText : PowerToolBase<OverlayTextJob>
    {
        protected override void OnProcess(OverlayTextJob jobDescription)
        {
            this.Info("Starting job {0}. Job details:", jobDescription.Name);
            this.Info("Image path: {0}", jobDescription.ImagePath);
            this.Info("Font name: {0}", jobDescription.FontName);
            this.Info("Font color: {0}", jobDescription.FontColor);
            this.Info("Output image name format: {0}", jobDescription.OutputImageNameTemplate);
            this.Info("Bounding box: {0}, {1}, {2}, {3}", jobDescription.BoundingBox.X, jobDescription.BoundingBox.Y, jobDescription.BoundingBox.Width, jobDescription.BoundingBox.Height);
            this.Info("Anti-aliasing disabled: {0}", jobDescription.DisableAntiAliasing);

            var color = Color.Empty;
            try
            {
                color = ColorTranslator.FromHtml(jobDescription.FontColor);
            }
            catch
            {
            }

            if (Color.Empty.Equals(color))
            {
                this.Error("Color could not be parsed");
                return;
            }

            foreach (var label in jobDescription.Labels)
            {
                this.Info("Processing label: {0}", label.Name);
                bool useAntiAliasing = !jobDescription.DisableAntiAliasing &&
                                       !label.DisableAntiAliasing;

                var text = label.Value;
                var bounds = new RectangleF(
                    jobDescription.BoundingBox.X,
                    jobDescription.BoundingBox.Y,
                    jobDescription.BoundingBox.Width,
                    jobDescription.BoundingBox.Height);
                var fontSize = jobDescription.MaxFontSize;
                if (fontSize <= 0)
                {
                    this.Info("MaxFontSize not specified. Setting to 36.");
                    fontSize = 36;
                }

                var fontName = string.IsNullOrEmpty(label.FontName) ? jobDescription.FontName : label.FontName;
                Font font = null;
                try
                {
                    font = new Font(fontName, fontSize);
                }
                catch (ArgumentException)
                {
                    this.Error("Unable to create font using FontName");
                }

                if (font.Name != fontName)
                {
                    this.Error("Unable to create font using FontName");
                    font = null;
                }

                Bitmap templateImage = null;
                try
                {
                    templateImage = new Bitmap(jobDescription.ImagePath);
                }
                catch (Exception e) when (e is ArgumentException || e is FileNotFoundException)
                {
                    var message = e.Message;
                    if (e.InnerException != null)
                    {
                        message = e.InnerException.Message;
                    }

                    this.Error("Unable to open image at ImagePath");
                }

                if (font == null || templateImage == null)
                {
                    return;
                }

                var stringFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                };

                using (var resultImage = new Bitmap(templateImage, templateImage.Width, templateImage.Height))
                using (var graphics = Graphics.FromImage(resultImage))
                {
                    graphics.TextRenderingHint = useAntiAliasing
                        ? System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
                        : System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    int charactersFitted;
                    int linesFilled;
                    var size = graphics.MeasureString(
                        text,
                        font,
                        bounds.Size,
                        stringFormat,
                        out charactersFitted,
                        out linesFilled);

                    while (charactersFitted < text.Length ||
                           bounds.Width <= size.Width ||
                           bounds.Height <= size.Height)
                    {
                        fontSize = fontSize - 0.125f;
                        if (fontSize < 4)
                        {
                            this.Error("Unable to fit text in bounding box");
                            break;
                        }

                        font = new Font(font.OriginalFontName, fontSize);
                        size = graphics.MeasureString(
                            text,
                            font,
                            bounds.Size,
                            stringFormat,
                            out charactersFitted,
                            out linesFilled);
                    }

                    if (fontSize == 0)
                    {
                        continue;
                    }

                    this.Info("Found suitable font size: {0}", fontSize);
                    
                    graphics.DrawString(
                        text,
                        font,
                        new SolidBrush(color),
                        bounds,
                        stringFormat);

                    if (this.Batch.Debug)
                    {
                        graphics.DrawRectangle(
                            new Pen(new SolidBrush(Color.Red), 2),
                            bounds.X + ((bounds.Width - size.Width) / 2),
                            bounds.Y + ((bounds.Height - size.Height) / 2),
                            size.Width,
                            size.Height);
                        graphics.DrawRectangle(
                            new Pen(new SolidBrush(Color.Green), 2),
                            bounds.X, 
                            bounds.Y, 
                            bounds.Width, 
                            bounds.Height);
                        graphics.DrawString(
                            string.Format("{0:00}", fontSize),
                            new Font("Impact", 18),
                            new SolidBrush(Color.Black),
                            new Point(10, 10));
                        graphics.DrawString(
                            string.Format("{0:00}", fontSize),
                            new Font("Impact", 18),
                            new SolidBrush(Color.White),
                            new Point(8, 8));
                    }

                    try
                    {
                        var resultFile = Path.Combine(this.Batch.OutputFolderPath, string.Format(jobDescription.OutputImageNameTemplate, label.Name));
                        this.Info("Writing result to: {0}", resultFile);
                        resultImage.Save(resultFile);
                    }
                    catch (Exception e)
                    {
                        this.Info(e.Message);
                        this.Error("Failed to write result image");
                    }
                }
            }
        }
    }
}
