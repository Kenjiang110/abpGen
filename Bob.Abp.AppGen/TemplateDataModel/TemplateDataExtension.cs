using Bob.Abp.AppGen.Models;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    public static class TemplateExtension
    {
        public static Assembly _assembly = typeof(TemplateExtension).Assembly;

        #region Register Helper function

        private static bool _registered = false;

        static void Register()
        {
            if (!_registered)
            {
                Helpers.Register("ToCamel", ToCamel);
                Helpers.Register("ToLower", ToLower);
                Helpers.Register("ToSingle", ToSingle);
                Helpers.Register("ToSingleCamel", ToSingleCamel);
                Helpers.Register("ToDto", ToDto);
                Helpers.Register("ToModel", ToModel);
                Helpers.Register("ToPlural", ToPlural);

                _registered = true;
            }
        }

        static void ToCamel(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                context.Write(arguments[0].ToString().ToCamel());
            }
        }

        static void ToLower(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                context.Write(arguments[0].ToString().ToLower());
            }
        }

        static void ToSingle(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                string name = arguments[0].ToString();
                name = Utils.ToSingle(name);
                context.Write(name);
            }
        }

        static void ToSingleCamel(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                string name = arguments[0].ToString();
                name = Utils.ToSingle(name);

                context.Write(name.ToCamel());
            }
        }

        static void ToDto(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                string typeName = arguments[0].ToString();
                if (typeName.IsList())
                {
                    var underTypeName = typeName.GetUnderlyType();
                    if (!underTypeName.IsSimpleType())
                    {
                        typeName = typeName.Replace(underTypeName, underTypeName + "Dto");
                    }
                }
                else if (!typeName.IsSimpleType())
                {
                    typeName += "Dto";
                }
                context.Write(typeName);
            }
        }

        static void ToModel(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                string typeName = arguments[0].ToString();
                if (typeName.IsList())
                {
                    var underTypeName = typeName.GetUnderlyType();
                    if (!underTypeName.IsSimpleType())
                    {
                        typeName = typeName.Replace(underTypeName, underTypeName + "InfoModel");
                    }
                }
                else if (!typeName.IsSimpleType())
                {
                    typeName += "InfoModel";
                }
                context.Write(typeName);
            }
        }

        static void ToPlural(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                string name = arguments[0].ToString();
                name = Utils.ToPlural(name);
                context.Write(name);
            }
        }

        #endregion

        #region Template resource name

        private static string _templateResourcePath = "Bob.Abp.AppGen.Templates";

        /// <summary>
        /// Full resource name (should be [RootNamespace].[folder path].[resource name])
        /// </summary>
        /// <param name="abpFile">name of file to be generated.</param>
        /// <param name="type">template type.</param>
        /// <returns>template full resource name.</returns>
        private static string ToResourceName(this AbpEditFile abpFile, TemplateType type)
        {
            if (type == TemplateType.None)
            {
                throw new ArgumentException("There is no tempalte for type None!");
            }
            string templateName = abpFile.ToString().Replace('_', '.');
            templateName = (type == TemplateType.Main) ? templateName : $"{templateName}_{type}";
            return GetResourceName(templateName);
        }

        /// <summary>
        /// Full resource name (should be [RootNamespace].[folder path].[resource name])
        /// </summary>
        /// <param name="abpFile">name of file to be generated.</param>
        /// <returns>template full resource name.</returns>
        private static string ToResourceName(this AbpCreateFile abpFile)
        {
            string templateName = abpFile.ToString().Replace('_', '.');
            return GetResourceName(templateName);
        }

        /// <summary>
        /// Full resource name (should be [RootNamespace].[folder path].[resource name])
        /// </summary>
        /// <param name="abpFile">name of file to be generated.</param>
        /// <returns>template full resource name.</returns>
        private static string ToResourceName(this AbpMiscFile abpFile)
        {
            string templateName = abpFile.ToString().Replace('_', '.');
            return GetResourceName(templateName);
        }

        /// <summary>
        /// Full resource name (should be [RootNamespace].[folder path].[resource name])
        /// </summary>
        /// <param name="templateName">template resource name</param>
        /// <returns>template full resource name.</returns>
        private static string GetResourceName(string templateName)
        {
            return $"{_templateResourcePath}.{templateName}.vue";
        }

        #endregion

        /// <summary>
        /// Use data to build the text result from a template resource with Usings.Template
        /// </summary>
        /// <param name="abpGenFile">to determinate the template</param>
        public static string Build(this TemplateData data, AbpCreateFile abpFile)
        {
            //var test = _assembly.GetManifestResourceNames();
            var res = _assembly.GetManifestResourceStream(abpFile.ToResourceName());
            if (res != null)
            {
                var stream = new StreamReader(res);
                var template = stream.ReadToEnd();
                if (template.StartsWith("{{!#include Using.Template}}"))
                {
                    var headerRest = _assembly.GetManifestResourceStream(GetResourceName("Usings"));
                    var headerStream = new StreamReader(headerRest);
                    var headerTemplate = headerStream.ReadToEnd();
                    template = headerTemplate + Environment.NewLine + template;
                }
                Register();
                return Render.StringToString(template, data);
            }
            return null;
        }

        /// <summary>
        /// Use data to build the text result from a template resource. 
        /// </summary>
        /// <param name="templateName"></param>
        public static string Build(this TemplateData data, AbpEditFile abpFile, TemplateType type)
        {
            var res = _assembly.GetManifestResourceStream(abpFile.ToResourceName(type));
            if (res != null)
            {
                var stream = new StreamReader(res);
                var template = stream.ReadToEnd();
                Register();
                return Render.StringToString(template, data);
            }
            return null;
        }

        /// <summary>
        /// Use data to build the text array from a template resource.
        /// </summary>
        /// <param name="templateName"></param>
        public static string[] BuildUsing(this TemplateData data, AbpEditFile abpFile)
        {
            var res = _assembly.GetManifestResourceStream(abpFile.ToResourceName(TemplateType.Using));
            if (res != null)
            {
                var stream = new StreamReader(res);
                var template = stream.ReadToEnd();
                Register();
                return Render.StringToString(template, data).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }

        /// <summary>
        /// Use directly template file without data.
        /// </summary>
        public static string NotBuild(this AbpMiscFile abpFile)
        {
            var res = _assembly.GetManifestResourceStream(abpFile.ToResourceName());
            if (res != null)
            {
                var stream = new StreamReader(res);
                var template = stream.ReadToEnd();
                return template;
            }
            return null;
        }
    }
}
