using AutoMapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace SerilogSinkMemoryIssue
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var logger = CreateLogger(config);
            var mapper = CreateMapper();
            try
            {
                var mapTo = mapper.Map<MapTo>(new MapFrom());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }

            Console.ReadLine();
        }

        static ILogger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.AzureAnalytics(
                    workspaceId: configuration["workspaceId"],
                    authenticationId: configuration["authenticationId"],
                    logName: configuration["logName"])
                .CreateLogger();
        }

        static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<MapFrom, MapTo>(); }));
        }
    }

    class MapFrom
    {
        public string SomeProperty => new string[] { }.Single();
    }

    class MapTo
    {
        public string SomeProperty { get; set; }
    }
}
