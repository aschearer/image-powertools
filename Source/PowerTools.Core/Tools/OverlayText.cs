namespace SpottedZebra.PowerTools.Core.Tools
{
    using Data;
    using System;
    using System.Drawing;
    using System.IO;

    [PowerTool(typeof(OverlayText), "OverlayText", "Add text to an image inside a bounding box.")]
    internal class OverlayText : PowerToolBase<OverlayTextJob>
    {
        protected override ExitCode OnProcess(OverlayTextJob jobDescription)
        {
            this.Info("Starting job {0}. Job details:", jobDescription.Name);
            this.Info("Image path: {0}", jobDescription.ImagePath);
            this.Info("Font name: {0}", jobDescription.FontName);
            this.Info("Font color: {0}", jobDescription.FontColor);
            this.Info("Output image name format: {0}", jobDescription.OutputImageNameTemplate);
            this.Info("Bounding box: {0}, {1}, {2}, {3}", jobDescription.BoundingBox.X, jobDescription.BoundingBox.Y, jobDescription.BoundingBox.Width, jobDescription.BoundingBox.Height);
            this.Info("Anti-aliasing disabled: {0}", jobDescription.DisableAntiAliasing);

            var result = ExitCode.Success;

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
                result = ExitCode.OverlayText_BadColor;
            }

            foreach (var label in jobDescription.Labels)
            {
                if (result != ExitCode.Success)
                {
                    break;
                }

                result = this.ProcessLabel(jobDescription, label, color);
            }

            return result;
        }

        private ExitCode ProcessLabel(OverlayTextJob jobDescription, Label label, Color color)
        {
            ExitCode result = ExitCode.Success;
            this.Info("Processing label: {0}", label.Name);
            bool useAntiAliasing = !jobDescription.DisableAntiAliasing &&
                                   !label.DisableAntiAliasing;

            var text = label.Value;
            var bounds = new RectangleF(
                jobDescription.BoundingBox.X,
                jobDescription.BoundingBox.Y,
                jobDescription.BoundingBox.Width,
                jobDescription.BoundingBox.Height);
            if (jobDescription.MaxFontSize <= 0)
            {
                this.Info("MaxFontSize not specified. Setting to 36.");
                jobDescription.MaxFontSize = 36;
            }

            Font font = this.GetFont(jobDescription, label, jobDescription.MaxFontSize, ref result);
            Bitmap templateImage = this.GetTemplateImage(jobDescription, ref result);

            if (result != ExitCode.Success)
            {
                // Font or template image are busted
                return result;
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

                font = this.CalculateFontSize(
                    graphics,
                    font,
                    stringFormat,
                    text,
                    bounds,
                    ref result);

                if (result == ExitCode.Success)
                {
                    this.Info("Found suitable font size: {0}", font.Size);

                    graphics.DrawString(
                        text,
                        font,
                        new SolidBrush(color),
                        bounds,
                        stringFormat);

                    this.TryDrawDebugOverlay(graphics, font, stringFormat, text, bounds);

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
                        result = ExitCode.OverlayText_CouldNotSaveImage;
                    }
                }
            }

            return result;
        }

        private void TryDrawDebugOverlay(
            Graphics graphics,
            Font font,
            StringFormat stringFormat,
            string text,
            RectangleF bounds)
        {
            if (this.Batch.Debug)
            {
                int charactersFitted;
                int linesFilled;
                var size = graphics.MeasureString(
                    text,
                    font,
                    bounds.Size,
                    stringFormat,
                    out charactersFitted,
                    out linesFilled);
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
                    string.Format("{0:00}", font.Size),
                    new Font("Impact", 18),
                    new SolidBrush(Color.Black),
                    new Point(10, 10));
                graphics.DrawString(
                    string.Format("{0:00}", font.Size),
                    new Font("Impact", 18),
                    new SolidBrush(Color.White),
                    new Point(8, 8));
            }
        }

        private Bitmap GetTemplateImage(OverlayTextJob jobDescription, ref ExitCode result)
        {
            Bitmap templateImage = null;
            try
            {
                if (File.Exists(jobDescription.ImagePath))
                {
                    templateImage = new Bitmap(jobDescription.ImagePath);
                }
                else
                {
                    this.Error("ImagePath file not found");
                    result = ExitCode.OverlayText_ImageNotFound;
                }
            }
            catch (Exception e) when (e is ArgumentException || e is FileNotFoundException)
            {
                this.Error("Unable to open image at ImagePath");
                result = ExitCode.OverlayText_BadImage;
            }

            return templateImage;
        }

        private Font GetFont(OverlayTextJob jobDescription, Label label, float fontSize, ref ExitCode result)
        {
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

            if (font == null)
            {
                result = ExitCode.OverlayText_BadFont;
            }

            return font;
        }

        private Font CalculateFontSize(
            Graphics graphics,
            Font font,
            StringFormat stringFormat,
            string text,
            RectangleF bounds,
            ref ExitCode result)
        {
            int charactersFitted;
            int linesFilled;
            var size = graphics.MeasureString(
                text,
                font,
                bounds.Size,
                stringFormat,
                out charactersFitted,
                out linesFilled);

            var fontSize = font.Size;
            const float MinFontSize = 4f;
            while (charactersFitted < text.Length ||
                   bounds.Width <= size.Width ||
                   bounds.Height <= size.Height)
            {
                fontSize = font.Size - 0.125f;
                if (fontSize < MinFontSize)
                {
                    this.Error("Unable to fit text in bounding box");
                    result = ExitCode.OverlayText_TextWontFit;
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

            return fontSize >= MinFontSize ? font : null;
        }
    }
}
