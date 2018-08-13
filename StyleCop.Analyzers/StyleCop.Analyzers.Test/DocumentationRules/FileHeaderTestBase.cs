﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Base class for file header related unit tests.
    /// </summary>
    public abstract class FileHeaderTestBase
    {
        private const string DefaultTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""Copyright (c) FooCorp. All rights reserved.""
    }
  }
}
";

        protected static DiagnosticResult[] EmptyDiagnosticResults
            => StyleCopCodeFixVerifier<FileHeaderAnalyzers, FileHeaderCodeFixProvider>.EmptyDiagnosticResults;

        /// <summary>
        /// Verifies that a file header with an autogenerated comment will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAutoGeneratedSourceFileAsync()
        {
            var testCode = @"// <auto-generated/>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header with borders will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithBordersAsync()
        {
            var testCode = @"//----------------------------------------
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary>This is a test file.</summary>
//----------------------------------------

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => StyleCopCodeFixVerifier<FileHeaderAnalyzers, FileHeaderCodeFixProvider>.Diagnostic(descriptor);

        protected virtual string GetSettings()
            => DefaultTestSettings;

        protected virtual IEnumerable<string> GetDisabledDiagnostics()
            => new[] { FileHeaderAnalyzers.SA1639Descriptor.Id };

        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, new[] { expected }, fixedSource: null, cancellationToken);

        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, expected, fixedSource: null, cancellationToken);

        protected Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        protected Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, expected, fixedSource, EmptyDiagnosticResults, cancellationToken);

        protected Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, DiagnosticResult[] remainingDiagnostics, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<FileHeaderAnalyzers, FileHeaderCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = this.GetSettings(),
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(remainingDiagnostics);
            test.DisabledDiagnostics.AddRange(this.GetDisabledDiagnostics());
            return test.RunAsync(cancellationToken);
        }
    }
}
