namespace SpottedZebra.PowerTools.CommandLine
{
    using Core;
    using NDesk.Options;
    using NLog;
    using NLog.Config;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// This is where all the action starts with the command line program.
    /// </summary>
    internal sealed class MainProgram
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            LogManager.Configuration = new LoggingConfiguration();

            var availableTools = MainProgram.GetAttributesInAssembly(typeof(PowerToolAttribute).Assembly);
            var availableToolDescriptions = new List<string>();
            foreach (var tool in availableTools)
            {
                var toolDescription = tool;
                availableToolDescriptions.Add(string.Format("{0}: {1}", toolDescription.Name, toolDescription.Description));
            }

            string toolName = null;
            bool showHelp = false;

            var options = new OptionSet()
                .Add("h|?|help", "Prints this help message.", v => showHelp = v != null)
                .Add("tool=", "{NAME} of the tool you want to run. Valid options:\n\n" + string.Join("\n\n", availableToolDescriptions), v => toolName = v);

            try
            {
                var extra = options.Parse(args);
            }
            catch
            {
                Console.WriteLine("Error parsing options.");
                return;
            }

            PowerToolConsoleProgramBase program = null;
            if (showHelp || string.IsNullOrEmpty(toolName))
            {
                Console.WriteLine("Image Power Tools: Increase your efficiency when working with images.");
                options.WriteOptionDescriptions(Console.Out);
            }
            else
            {
                // Loop through all the PowerToolAttributes and see if any of their Names match the given
                // tool name. If so use reflection to instantiate a new PowerToolConsoleProgram for the 
                // designated PowerTool and its corresponding JobDescription.
                foreach (var tool in availableTools)
                {
                    if (tool.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            // Get the PowerToolBase generic parameter. This is the JobDescription type.
                            var jobTypes = tool.ToolType.BaseType.GetGenericArguments();
                            
                            // Prepare the generic types for the next step.
                            var typeParams = new Type[] { tool.ToolType, jobTypes[0] };

                            // Define the type we'll construct. In this case it's a PowerToolConsoleProgram<ToolName, JobType>
                            var constructedType = typeof(PowerToolConsoleProgram<,>).MakeGenericType(typeParams);

                            // Instantiate an instance of PowerToolConsoleProgram to be run further below.
                            program = (PowerToolConsoleProgramBase)Activator.CreateInstance(constructedType);
                        }
                        catch
                        {
                            Console.WriteLine("Failed to create tool. Please file a bug.");
                        }

                        break;
                    }
                }
            }

            if (program == null)
            {
                if (!string.IsNullOrEmpty(toolName))
                {
                    Console.WriteLine("Tool not found: {0}", toolName);
                }
            }
            else
            {
                // The user specified a tool name, we found a corresponding PowerTool, time to run it.
                program.Run(args);
            }
        }

        /// <summary>
        /// Loops through the assembly and returns all instances of PowerToolAttribute.
        /// </summary>
        private static IEnumerable<PowerToolAttribute> GetAttributesInAssembly(Assembly assembly)
        {
            var targetAttribute = typeof(PowerToolAttribute);
            foreach (Type type in assembly.GetTypes())
            {
                var declaredAttribute = type.GetCustomAttribute(targetAttribute);
                if (declaredAttribute != null)
                {
                    yield return (PowerToolAttribute)declaredAttribute;
                }
            }
        }
    }
}
