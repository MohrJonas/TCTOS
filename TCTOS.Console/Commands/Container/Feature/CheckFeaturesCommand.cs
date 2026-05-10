namespace TCTOS.Console.Commands.Container.Feature;

// public sealed class CheckFeaturesCommand()
//     : CommandBase("check", "Check which features are currently applicable", container,
//         arguments: [SharedArguments.ContainerNameArgument])
// {
//     protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
//     {
//         var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
//
//         var featureProvider = container.Get<IFeatureProvider>();
//         var featureRunner = container.Get<IFeatureRunner>();
//         var fileSystem = container.Get<IFileSystem>();
//         var client = container.Get<IIncusClient>();
//         var variableProvider = container.Get<IEnvironmentVariableProvider>();
//         var userInformationCollector = container.Get<IUserInformationCollector>();
//         var nonPersistentStorage = container.Get<INonPersistentStorage>();
//         var runner = container.Get<ICommandRunner>();
//         var backgroundRunner = container.Get<IBackgroundCommandRunner>();
//
//         var configuration = (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;
//
//         List<(string, bool, string)> results = [];
//         foreach (var featureName in configuration.FeatureNames)
//         {
//             var featureText = (await featureProvider.GetFeatureScriptTextAsync(featureName)).GetOrThrow();
//             var canApply = await featureRunner.CanApplyFeature(
//                 featureText,
//                 containerName,
//                 fileSystem,
//                 client,
//                 nonPersistentStorage,
//                 userInformationCollector,
//                 variableProvider,
//                 runner,
//                 backgroundRunner
//             );
//             results.Add((featureName, canApply.Data, canApply.Explanation));
//         }
//         
//         if(parseResult.GetRequiredValue(SharedOptions.PlainOption))
//             DisplayPlain([..results]);
//         else
//             DisplayPretty([..results]);
//     }
//
//     private static void DisplayPlain((string, bool, string)[] applyResults)
//     {
//         foreach (var applyResult in applyResults)
//             System.Console.WriteLine(applyResult.Item2 ? $"{applyResult.Item1}\tApplicable" : $"{applyResult.Item1}\tNot Applicable\t{applyResult.Item3}");
//     }
//     
//     private static void DisplayPretty((string, bool, string)[] applyResults)
//     {
//         var table = new Table()
//             .AddColumn("Name")
//             .AddColumn("Is Applicable")
//             .AddColumn("Reason");
//         foreach (var applyResult in applyResults)
//             table.AddRow(applyResult.Item1, applyResult.Item2.ToString(), applyResult.Item3);
//         AnsiConsole.Write(table);
//     }
// }