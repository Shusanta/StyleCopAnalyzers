﻿namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A field name in C# begins with an underscore.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a field name begins with an underscore.</para>
    ///
    /// <para>By default, StyleCop disallows the use of underscores, <c>m_</c>, etc., to mark local class fields, in
    /// favor of the <c>this.</c> prefix. The advantage of using <c>this.</c> is that it applies equally to all element
    /// types including methods, properties, etc., and not just fields, making all calls to class members instantly
    /// recognizable, regardless of which editor is being used to view the code. Another advantage is that it creates a
    /// quick, recognizable differentiation between instance members and static members, which will not be
    /// prefixed.</para>
    ///
    /// <para>If the field or variable name is intended to match the name of an item associated with Win32 or COM, and
    /// thus needs to begin with an underscore, place the field or variable within a special <c>NativeMethods</c> class.
    /// A <c>NativeMethods</c> class is any class which contains a name ending in <c>NativeMethods</c>, and is intended
    /// as a placeholder for Win32 or COM wrappers. StyleCop will ignore this violation if the item is placed within a
    /// <c>NativeMethods</c> class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1309FieldNamesMustNotBeginWithUnderscore : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1309";
        private const string Title = "Field names must not begin with underscore";
        private const string MessageFormat = "Field '{0}' must not begin with an underscore";
        private const string Category = "StyleCop.CSharp.NamingRules";
        private const string Description = "A field name in C# begins with an underscore.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1309.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleFieldDeclarationSyntax, SyntaxKind.FieldDeclaration);
        }

        private void HandleFieldDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            FieldDeclarationSyntax syntax = (FieldDeclarationSyntax)context.Node;
            if (NamedTypeHelpers.IsContainedInNativeMethodsClass(syntax))
                return;

            var variables = syntax.Declaration?.Variables;
            if (variables == null)
                return;

            foreach (VariableDeclaratorSyntax variableDeclarator in variables.Value)
            {
                if (variableDeclarator == null)
                    continue;

                var identifier = variableDeclarator.Identifier;
                if (identifier.IsMissing)
                    continue;

                if (!identifier.ValueText.StartsWith("_"))
                    continue;

                // Field '{name}' must not begin with an underscore
                string name = identifier.ValueText;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, identifier.GetLocation(), name));
            }
        }
    }
}