using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epos.CmdLine
{
    public sealed class CmdLineSubcommand<TOptions> : CmdLineSubcommand where TOptions : class, new()
    {
        public CmdLineSubcommand(string name, string description) : base(name, description) { }

        public Func<TOptions, int> CmdLineFunc { get; set; }

        internal override int Execute(IEnumerable<CmdLineToken> argTokens) {
            var theOptions = new TOptions();
            var theOptionsType = typeof(TOptions);

            foreach (CmdLineToken theArgToken in argTokens) {
                if (theArgToken.Kind != CmdLineTokenKind.Subcommand) {
                    // Option oder Parameter
                    string thePropertyName = theArgToken.Name.Replace("-", string.Empty); // de-kebap-style

                    // Spezifische (Attribute) PropertyInfo finden
                    PropertyInfo thePropertyInfo;

                    if (theArgToken.Kind == CmdLineTokenKind.Option) {
                        thePropertyInfo =
                            theOptionsType
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .SingleOrDefault(
                                    pi => pi.GetCustomAttribute<CmdLineOptionAttribute>() != null &&
                                          pi.GetCustomAttribute<CmdLineOptionAttribute>().Letter.ToString() == thePropertyName
                                );
                    }
                    else {
                        // Parameter
                        thePropertyInfo =
                            theOptionsType
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .SingleOrDefault(
                                    pi => pi.GetCustomAttribute<CmdLineParameterAttribute>() != null &&
                                          pi.GetCustomAttribute<CmdLineParameterAttribute>().Name == thePropertyName
                                );
                    }

                    if (thePropertyInfo == null) {
                        // Property mit genau dem passenden Namen (case-insensitive) finden
                        thePropertyInfo = theOptionsType.GetProperty(
                            thePropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                        );
                    }

                    if (thePropertyInfo != null) {
                        // Property gefunden, Wert setzen
                        thePropertyInfo.SetMethod.Invoke(theOptions, new[] { theArgToken.Value });
                    }
                    else {
                        throw new InvalidOperationException(
                            $"No property \"{thePropertyName}\" found on type {theOptionsType.FullName}."
                        );
                    }
                }
            }

            return CmdLineFunc?.Invoke(theOptions) ?? 0;
        }
    }

    public abstract class CmdLineSubcommand
    {
        public const string DefaultName = "default";

        protected CmdLineSubcommand(string name, string description) {
            Name        = name        ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));

            Options = new List<CmdLineOption>();
            Parameters = new List<CmdLineParameter>();
        }

        public string Name { get; }

        public string Description { get; }

        public IList<CmdLineOption> Options { get; }

        public IList<CmdLineParameter> Parameters { get; }

        internal abstract int Execute(IEnumerable<CmdLineToken> argTokens);
    }
}
