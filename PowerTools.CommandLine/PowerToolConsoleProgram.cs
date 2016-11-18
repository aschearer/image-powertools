namespace SpottedZebra.PowerTools.CommandLine
{
    using Core.Data;
    using NDesk.Options;
    using Newtonsoft.Json;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using SpottedZebra.PowerTools.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// This class exposes a PowerTool to the user as a console application.
    /// </summary>
    /// <typeparam name="T">The PowerTool type.</typeparam>
    /// <typeparam name="J">The PowerTool's JobDescription type.</typeparam>
    internal class PowerToolConsoleProgram<T, J> : PowerToolConsoleProgramBase
        where T : PowerToolBase<J>, new()
        where J : IJobDescription
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Process command line arguments, deserialize job configuration data, instantiate the
        /// PowerTool, and process each job.
        /// </summary>
        public override void Run(string[] args)
        {
            string configFile = null;
            bool showHelp = false;
            bool isVerbose = false;

            var options = new OptionSet()
                .Add("v|verbose", "All logging will be written to the console.", v => isVerbose = v != null)
                .Add("h|?|help", "Prints this help message.", v => showHelp = v != null)
                .Add("config=", "{PATH} to JSON configuration file.", v => configFile = v);

            try
            {
                var extra = options.Parse(args);
            }
            catch
            {
                Console.WriteLine("Error parsing options.");
                return;
            }

            if (showHelp || string.IsNullOrEmpty(configFile))
            {
                var toolDescription = (PowerToolAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PowerToolAttribute));

                Console.WriteLine("{0}: {1}", toolDescription.Name, toolDescription.Description);
                Console.WriteLine("How to operate:");
                options.WriteOptionDescriptions(Console.Out);
            }
            else if (!File.Exists(configFile))
            {
                this.Error("Configuration file not found.");
            }
            else
            {
                if (isVerbose)
                {
                    // Enable logging to the console.
                    var logFile = new ConsoleTarget();
                    logFile.Name = "LogToConsole";
                    logFile.Layout = "${level} ${message}";

                    LoggingConfiguration config = LogManager.Configuration;
                    var rule = new LoggingRule("*", LogLevel.Info, logFile);

                    config.AddTarget(logFile);
                    config.LoggingRules.Add(rule);

                    LogManager.Configuration = config;
                }

                try
                {
                    // A batch contains common high level information for a PowerTool as well as a list of jobs.
                    var batch = JsonConvert.DeserializeObject<BatchDescription<J>>(File.ReadAllText(configFile));
                    var logger = new NLogAdapter(PowerToolConsoleProgram<T, J>.logger);

                    if (!string.IsNullOrEmpty(batch.LogFilePath) && LogManager.Configuration != null)
                    {
                        // Configure all tracing to go to the designated log file.
                        var logFile = new FileTarget();
                        logFile.Name = "LogToFile";
                        logFile.FileName = batch.LogFilePath;
                        logFile.Layout = "${level} ${message}";
                        logFile.DeleteOldFileOnStartup = true;

                        LoggingConfiguration config = LogManager.Configuration;
                        var rule = new LoggingRule("*", LogLevel.Info, logFile);

                        config.AddTarget(logFile);
                        config.LoggingRules.Add(rule);

                        LogManager.Configuration = config;
                    }

                    if (string.IsNullOrEmpty(batch.OutputFolderPath))
                    {
                        this.Error("Not output directory specified.");
                    }
                    else
                    {
                        if (!Directory.Exists(batch.OutputFolderPath))
                        {
                            this.Info("Creating output directory: {0}", batch.OutputFolderPath);
                            Directory.CreateDirectory(batch.OutputFolderPath);
                        }

                        if (batch.Jobs == null)
                        {
                            this.Warn("No jobs defined.");
                        }
                        else
                        {
                            var worker = new T();
                            worker.Setup(batch, logger);

                            this.Info("Starting {0} jobs", batch.Jobs.Length);

                            var processedJobs = new HashSet<string>();
                            int i = 0;

                            foreach (var job in batch.Jobs)
                            {
                                if (processedJobs.Contains(job.Name))
                                {
                                    this.Warn("Duplicate job name: {0}. Skipping job.", job.Name);
                                    continue;
                                }

                                worker.Process((J)job);
                                processedJobs.Add(job.Name);

                                i++;
                                this.Info("Finished job {0} of {1}", i, batch.Jobs.Length);
                            }

                            var toolDescription = (PowerToolAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PowerToolAttribute));
                            Console.WriteLine("Finished running tool: {0}.", toolDescription.Name);
                        }
                    }
                }
                catch (Exception e) when (e is JsonSerializationException || e is JsonReaderException)
                {
                    this.Info("Failed to parse JSON: {0}", e.Message);
                    this.Error("Configuration file could not be parsed.");
                }
            }
        }

        private void Info(string message, params object[] args)
        {
            PowerToolConsoleProgram<T, J>.logger.Info(message, args);
        }

        private void Warn(string message, params object[] args)
        {
            PowerToolConsoleProgram<T, J>.logger.Warn(message, args);
        }

        private void Error(string message, params object[] args)
        {
            PowerToolConsoleProgram<T, J>.logger.Error(message, args);
        }
    }
}
