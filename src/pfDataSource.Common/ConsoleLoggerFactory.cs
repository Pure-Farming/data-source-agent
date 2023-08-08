using System;
using Serilog;

namespace MoA.Platform.Onboarding.Common
{
    /// <summary>
    /// LoggerFactory.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Create a Logger.
        /// </summary>
        /// <returns>A Logger.</returns>
        ILogger CreateLogger();
    }

    /// <summary>
    /// A Logger Factory for creating Console Loggers.
    /// </summary>
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Create a console Logger.
        /// </summary>
        /// <returns>A Serilog Logger that logs to the console.</returns>
        public static ILogger CreateConsoleLogger() => new LoggerConfiguration()
                .Enrich
                .FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();

        /// <summary>
        /// Create a console Logger with Debug Level.
        /// </summary>
        /// <returns>A Serilog Logger that logs to the console.</returns>
        public static ILogger CreateDebugLogger() => new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        /// <inheritdoc/>
        public ILogger CreateLogger() => CreateConsoleLogger();
    }
}

