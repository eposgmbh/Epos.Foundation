using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epos.CmdLine
{

    /// <summary> Command line subcommand, see <a href="/getting-started.html">Getting started</a>
    /// for an example.</summary>
    /// <typeparam name="TOptions">Options data type</typeparam>
    public sealed class CmdLineSubcommand<TOptions> : CmdLineSubcommand where TOptions : class, new()
    {
        /// <summary> Initializes an instance of the <see cref="CmdLineSubcommand{TOptions}"/> class.
        /// </summary>
        /// <param name="name">Subcommand name</param>
        /// <param name="description">Description</param>
        public CmdLineSubcommand(string name, string description) : base(name, description) { }

        /// <summary> Gets or sets the command line function. </summary>
        /// <remarks> The command line function is called with a <typeparamref name="TOptions"/>
        /// instance that is initialized with the command line arguments. The function is
        /// only called, if the command line can be parsed successfully. The function should
        /// return an error code (0 if successful). </remarks>
        public Func<TOptions, CmdLineDefinition, int> CmdLineFunc { get; set; }

        internal override int Execute(IEnumerable<CmdLineToken> argTokens, CmdLineDefinition definition) {
            var theOptions = new TOptions();
            Type theOptionsType = typeof(TOptions);

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
                                          pi.GetCustomAttribute<CmdLineParameterAttribute>().Name.ToLower() == thePropertyName
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

            return CmdLineFunc?.Invoke(theOptions, definition) ?? 0;
        }
    }

    /// <summary> Command line subcommand base class.</summary>
    public abstract class CmdLineSubcommand
    {
        /// <summary> Default name for the single subcommand, if no
        /// differentiated subcommands are to be used. </summary>
        public const string DefaultName = "default";

        /// <summary> Initializes an instance of the <see cref="CmdLineSubcommand"/> class.
        /// </summary>
        /// <param name="name">Subcommand name</param>
        /// <param name="description">Description</param>
        protected CmdLineSubcommand(string name, string description) {
            Name        = name        ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));

            Options = new List<CmdLineOption>();
            Parameters = new List<CmdLineParameter>();
        }

        /// <summary> Gets the subcommand name.</summary>
        public string Name { get; }

        /// <summary> Gets the subcommand description.</summary>
        public string Description { get; }

        /// <summary> Gets the subcommand options. </summary>
        /// <remarks> Here you can register your subcommand options. </remarks>
        public IList<CmdLineOption> Options { get; }

        /// <summary> Gets the subcommand parameters. </summary>
        /// <remarks> Here you can register your subcommand parameters. </remarks>
        public IList<CmdLineParameter> Parameters { get; }

        internal abstract int Execute(IEnumerable<CmdLineToken> argTokens, CmdLineDefinition definition);
    }
}
