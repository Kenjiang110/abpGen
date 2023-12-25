using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    public enum AbpProjectType
    {
        Shared,
        Domain,
        Application,
        Contracts,
        HttpApi,
        Client,
        Web,
        MongoDB,
        EntityFrameworkCore
    }

    public static class AbpProjectTypeExtensions
    {
        private static readonly string[] _shortProjectNames = {
            "Domain.Shared",
            "Domain",
            "Application",
            "Application.Contracts",
            "HttpApi",
            "HttpApi.Client",
            "Web",
            "MongoDB",
            "EntityFrameworkCore"
        };

        public static string GetShortProjectName(this AbpProjectType apType)
        {
            return _shortProjectNames[(int)apType];
        }

        public static string GetFullProjectName(this AbpProjectType apType, string nsPrefix, string moduleName)
        {
            return moduleName == null 
                ? $"{nsPrefix}.{_shortProjectNames[(int)apType]}" 
                : $"{nsPrefix}.{moduleName}.{_shortProjectNames[(int)apType]}";
        }

        public static int TotalTypes => _shortProjectNames.Length;
    }
}
