
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Reflection;
using System.Text;
namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Serilog 日志拓展
    /// </summary>
    public static class SerilogHostingExtensions
    {
        /// <summary>
        /// 添加默认日志拓展
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="configAction"></param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseSerilogDefault(this IWebHostBuilder hostBuilder, Action<LoggerConfiguration> configAction = default)
        {
            hostBuilder.UseSerilog((context, configuration) =>
            {
                // 加载配置文件
                var config = configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();

                if (configAction != null) configAction.Invoke(config);
                else
                {
                    var hasWriteTo = context.Configuration["Serilog:WriteTo:0:Name"];
                    if (hasWriteTo == null)
                    {
                        config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                          .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "application.log"), LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, encoding: Encoding.UTF8);
                    }
                }
            });

            return hostBuilder;
        }


        /// <summary>
        /// 添加ELK日志拓展
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="configAction"></param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseSerilogElasticSearch(this IWebHostBuilder hostBuilder, Action<LoggerConfiguration> configAction = default)
        {
          
            hostBuilder.UseSerilog((context, configuration) =>
            {

                // 加载配置文件
                configuration
                       .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out LogEventPropertyValue value) && value.ToString().ToLower().Contains("nacos"))
                       .Enrich.FromLogContext()
                       .Enrich.WithExceptionDetails()
                       .Enrich.WithMachineName()
                       .Enrich.WithCorrelationId()
                       .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
                       {
                           IndexFormat = $"{Assembly.GetEntryAssembly().GetName().Name.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                           EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                           FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                           AutoRegisterTemplate = true,
                           CustomFormatter = new  ExceptionAsObjectJsonFormatter(renderMessage: true), // Better formatting for exceptions
                           AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                           ModifyConnectionSettings =
                               conn =>
                               {
                                   conn.ServerCertificateValidationCallback((source, certificate, chain, sslPolicyErrors) => true);
                                   conn.BasicAuthentication(context.Configuration["ElasticConfiguration:UserName"], context.Configuration["ElasticConfiguration:PassWord"]);

                                   return conn;
                               }
                       }).WriteTo.Console(); });
            return hostBuilder;
        }

        /// <summary>
        /// 添加默认日志拓展
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IHostBuilder UseSerilogDefault(this IHostBuilder builder, Action<LoggerConfiguration> configAction = default)
        {
            builder.UseSerilog((context, configuration) =>
            {
                // 加载配置文件
                var config = configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();

                if (configAction != null) configAction.Invoke(config);
                else
                {
                    var hasWriteTo = context.Configuration["Serilog:WriteTo:0:Name"];
                    if (hasWriteTo == null)
                    {
                        config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                          .WriteTo.File(Path.Combine("logs", "application.log"), LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null, encoding: Encoding.UTF8);
                    }
                }
            });

            return builder;
        }
    }
}
