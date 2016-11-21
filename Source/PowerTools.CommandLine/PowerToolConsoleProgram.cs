namespace SpottedZebra.PowerTools.CommandLine
{
    using Core.Data;
    using Core.Tools;
    using NDesk.Options;
    using Newtonsoft.Json;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using SpottedZebra.PowerTools.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;

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
        public override ExitCode Run(string[] args)
        {
            var toolDescription = (PowerToolAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PowerToolAttribute));
            var result = ExitCode.Success;

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
                result = ExitCode.UnexpectedOptions;
            }

            if (result != ExitCode.Success)
            {
                // No-op
            }
            else if (showHelp || string.IsNullOrEmpty(configFile))
            {
                // Print help
                Console.WriteLine("{0}: {1}", toolDescription.Name, toolDescription.Description);
                Console.WriteLine("How to operate:");
                options.WriteOptionDescriptions(Console.Out);
            }
            else if (!File.Exists(configFile))
            {
                // No configuration file
                this.Error("Configuration file not found.");
                result = ExitCode.ConfigurationFileNotFound;
            }
            else
            {
                PowerToolConsoleProgram<T, J>.ConfigureVerbosity(isVerbose);

                // A batch contains common high level information for a PowerTool as well as a list of jobs.
                BatchDescription<J> batch = null;
                try
                {
                    batch = JsonConvert.DeserializeObject<BatchDescription<J>>(File.ReadAllText(configFile));
                }
                catch (Exception e) when (e is JsonSerializationException || e is JsonReaderException)
                {
                    this.Info("Failed to parse JSON: {0}", e.Message);
                    this.Error("Configuration file could not be parsed.");
                    result = ExitCode.BadConfiguration;
                }

                if (result == ExitCode.Success)
                {
                    PowerToolConsoleProgram<T, J>.SetPathsRelativeToConfigFile(configFile, batch);
                    PowerToolConsoleProgram<T, J>.ConfigureLoggingToFile(batch.LogFilePath);
                    var logger = new NLogAdapter(PowerToolConsoleProgram<T, J>.logger);
                    if (string.IsNullOrEmpty(batch.OutputFolderPath))
                    {
                        this.Error("No output directory specified.");
                        result = ExitCode.BadConfiguration;
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

                                if (result != ExitCode.Success)
                                {
                                    break;
                                }

                                result = worker.Process(job);
                                processedJobs.Add(job.Name);

                                i++;
                                this.Info("Finished job {0} of {1}", i, batch.Jobs.Length);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(
                "Finished running tool: {0}. Exited with code: {1:N0} {2}",
                toolDescription.Name,
                (int)result,
                PowerToolConsoleProgram<T, J>.GetExitCodeDescription(result));
            return result;
        }

        /// <summary>
        /// Uses reflection to loop through the BatchDescription and JobDescriptions and update
        /// any property with a RelativePath attribute so that they start with the path from
        /// the current execution directory to the configuration directory.
        /// </summary>
        private static void SetPathsRelativeToConfigFile(string configFile, BatchDescription<J> batch)
        {
            var configPath = Path.GetDirectoryName(configFile);

            var batchType = batch.GetType();
            var batchMembers = batchType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (var member in batchMembers)
            {
                var attributes = member.GetCustomAttributes(typeof(RelativePathAttribute), false);
                if (attributes.Length > 0)
                {
                    var prop = batchType.GetProperty(member.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        var value = prop.GetValue(batch);
                        prop.SetValue(batch, Path.Combine(configPath, (string)value), null);
                    }
                }
            }

            foreach (var job in batch.Jobs)
            {
                var jobType = job.GetType();
                var jobMembers = jobType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                foreach (var member in jobMembers)
                {
                    var attributes = member.GetCustomAttributes(typeof(RelativePathAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var prop = jobType.GetProperty(member.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null && prop.CanWrite)
                        {
                            var value = prop.GetValue(job);
                            if (value is string)
                            {
                                prop.SetValue(job, Path.Combine(configPath, (string)value), null);
                            }
                            else if (value is string[])
                            {
                                var paths = (string[])value;
                                for (int i = 0; i < paths.Length; i++)
                                {
                                    paths[i] = Path.Combine(configPath, paths[i]);
                                }

                                prop.SetValue(job, paths, null);
                            }
                        }
                    }
                }
            }
        }

        private static string GetExitCodeDescription(ExitCode result)
        {
            string exitCodeDescription = "Unknown error code";
            var exitCodeType = result.GetType();
            var memberInfo = exitCodeType.GetMember(result.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            exitCodeDescription = ((DescriptionAttribute)attributes[0]).Description;
            return exitCodeDescription;
        }

        private static void ConfigureLoggingToFile(string logFilePath)
        {
            if (!string.IsNullOrEmpty(logFilePath) && LogManager.Configuration != null)
            {
                // Configure all tracing to go to the designated log file.
                var logFile = new FileTarget();
                logFile.Name = "LogToFile";
                logFile.FileName = logFilePath;
                logFile.Layout = "${level} ${message}";
                logFile.DeleteOldFileOnStartup = true;

                LoggingConfiguration config = LogManager.Configuration;
                var rule = new LoggingRule("*", LogLevel.Info, logFile);

                config.AddTarget(logFile);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }
        }

        private static void ConfigureVerbosity(bool isVerbose)
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
