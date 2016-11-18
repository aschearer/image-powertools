namespace PowerTools.CommandLine.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpottedZebra.PowerTools.CommandLine;
    using System.IO;
    using System;

    [TestClass]
    public class TestPowerTools
    {
        [TestMethod]
        [DeploymentItem(@"Data\ComposeImages\Input\", @"Data\ComposeImages\")]
        [DeploymentItem(@"Data\ComposeImages\Expected\", @"Data\ComposeImages\Expected\")]
        public void TestComposeImages()
        {
            var args = new string[] {
                "test",
                "--tool=ComposeImages",
                @"--config=Data\ComposeImages\ComposeImages.json"
            };

            MainProgram.Main(args);

            var outputDir = "Output/ComposeImages";
            if (!Directory.Exists(outputDir))
            {
                Assert.Fail("Failed to create output directory");
                return;
            }

            var expectedImages = Directory.GetFiles("Data/ComposeImages/Expected");
            var actualImages = Directory.GetFiles(outputDir);

            if (actualImages.Length != expectedImages.Length)
            {
                Assert.Fail("Unexpected number of images generated. Expected {0}. Found: {1}", expectedImages.Length, actualImages.Length);
                return;
            }

            for (int i = 0; i < expectedImages.Length; i++)
            {
                if (!this.AreFilesEqual(expectedImages[i], actualImages[i]))
                {
                    Assert.Fail("Images did not match: {0}", Path.GetFileName(expectedImages[i]));
                    break;
                }
            }
        }

        [TestMethod]
        [DeploymentItem(@"Data\OverlayText\Input\", @"Data\OverlayText\")]
        [DeploymentItem(@"Data\OverlayText\Expected\", @"Data\OverlayText\Expected\")]
        public void TestOvelayText()
        {
            var args = new string[] {
                "test",
                "--tool=OverlayText",
                @"--config=Data\OverlayText\OverlayText.json"
            };

            MainProgram.Main(args);

            var outputDir = "Output/OverlayText";
            if (!Directory.Exists(outputDir))
            {
                Assert.Fail("Failed to create output directory");
                return;
            }

            var expectedImages = Directory.GetFiles("Data/OverlayText/Expected");
            var actualImages = Directory.GetFiles(outputDir);

            if (actualImages.Length != expectedImages.Length)
            {
                Assert.Fail("Unexpected number of images generated. Expected {0}. Found: {1}", expectedImages.Length, actualImages.Length);
                return;
            }

            for (int i = 0; i < expectedImages.Length; i++)
            {
                if (!this.AreFilesEqual(expectedImages[i], actualImages[i]))
                {
                    Assert.Fail("Images did not match: {0}", Path.GetFileName(expectedImages[i]));
                    break;
                }
            }
        }

        private bool AreFilesEqual(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }
}
