﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Pihrtsoft.Snippets;
using Snippetica.CodeGeneration.Commands;

namespace Snippetica.CodeGeneration
{
    public class LanguageSnippetGenerator : SnippetGenerator
    {
        private readonly string[] _generateTypeTags;
        private readonly string[] _generateModifierTags;

        public LanguageSnippetGenerator(LanguageDefinition languageDefinition)
        {
            LanguageDefinition = languageDefinition;
            _generateTypeTags = LanguageDefinition.Types.Select(f => KnownTags.GenerateTypeTag(f.Name)).ToArray();
            _generateModifierTags = LanguageDefinition.Modifiers.Select(f => KnownTags.GenerateModifierTag(f.Name)).ToArray();
        }

        public LanguageDefinition LanguageDefinition { get; }

        protected override MultiCommandCollection CreateCommands(Snippet snippet)
        {
            var commands = new MultiCommandCollection();

            if (snippet.HasTag(KnownTags.GenerateDeclarationAndDefinition))
            {
                commands.AddMultiCommands(new Command[]
                {
                    CommandUtility.Declaration,
                    CommandUtility.Definition
                });
            }

            commands.AddMultiCommands(GetTypeCommands(snippet));

            if (snippet.HasTag(KnownTags.GenerateCollection))
                commands.AddMultiCommands(GetNonImmutableCollectionCommands(snippet));

            if (snippet.HasTag(KnownTags.GenerateImmutableCollection))
                commands.AddMultiCommands(GetImmutableCollectionCommands(snippet));

            commands.AddMultiCommands(GetAccessModifierCommands(snippet));

            if (snippet.HasTag(KnownTags.GenerateStaticModifier))
                commands.AddMultiCommand(CommandUtility.StaticCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateVirtualModifier))
                commands.AddMultiCommand(CommandUtility.VirtualCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateConstModifier))
                commands.AddMultiCommand(CommandUtility.ConstCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateConstExprModifier))
                commands.AddMultiCommand(CommandUtility.ConstExprCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateInlineModifier))
                commands.AddMultiCommand(CommandUtility.InlineCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateInitializer))
                commands.AddMultiCommand(CommandUtility.InitializerCommand, duplicateWhenEmpty: true);

            if (snippet.HasTag(KnownTags.GenerateParameters))
                commands.AddMultiCommand(CommandUtility.ParametersCommand, duplicateWhenEmpty: true);

            return commands;
        }

        protected override ExecutionContext CreateExecutionContext(Snippet snippet)
        {
            return new LanguageExecutionContext((Snippet)snippet.Clone(), LanguageDefinition);
        }

        protected virtual IEnumerable<Command> GetTypeCommands(Snippet snippet)
        {
            return CommandUtility.GetTypeCommands(snippet, LanguageDefinition);
        }

        protected virtual IEnumerable<Command> GetNonImmutableCollectionCommands(Snippet snippet)
        {
            return CommandUtility.GetNonImmutableCollectionCommands(LanguageDefinition);
        }

        protected virtual IEnumerable<Command> GetImmutableCollectionCommands(Snippet snippet)
        {
            return CommandUtility.GetImmutableCollectionCommands(LanguageDefinition);
        }

        protected virtual IEnumerable<Command> GetAccessModifierCommands(Snippet snippet)
        {
            return CommandUtility.GetAccessModifierCommands(snippet, LanguageDefinition);
        }

        protected override Snippet PostProcess(Snippet snippet)
        {
            base.PostProcess(snippet);

            if (snippet.Language == Language.VisualBasic)
                snippet.ReplaceSubOrFunctionLiteral("Function");

            snippet.Title = snippet.Title
                .Replace(Placeholders.Type, " ", true)
                .Replace(Placeholders.OfType, " ", true)
                .Replace(Placeholders.GenericType, LanguageDefinition.GetTypeParameterList("T"));

            snippet.Description = snippet.Description
                .Replace(Placeholders.Type, " ", true)
                .Replace(Placeholders.OfType, " ", true)
                .Replace(Placeholders.GenericType, LanguageDefinition.GetTypeParameterList("T"));

            snippet.RemoveTags(_generateTypeTags);
            snippet.RemoveTags(_generateModifierTags);

            snippet.RemoveTag(KnownTags.GenerateType);
            snippet.RemoveTag(KnownTags.GenerateAccessModifier);
            snippet.RemoveTag(KnownTags.GenerateInitializer);
            snippet.RemoveTag(KnownTags.GenerateParameters);
            snippet.RemoveTag(KnownTags.GenerateCollection);
            snippet.RemoveTag(KnownTags.GenerateImmutableCollection);
            snippet.RemoveTag(KnownTags.GenerateDeclarationAndDefinition);
            snippet.RemoveTag(KnownTags.Array);
            snippet.RemoveTag(KnownTags.Collection);
            snippet.RemoveTag(KnownTags.Dictionary);
            snippet.RemoveTag(KnownTags.Variable);
            snippet.RemoveTag(KnownTags.TryParse);
            snippet.RemoveTag(KnownTags.Initializer);

            return snippet;
        }
    }
}
