using LiveTileTestPrism.Interfaces;
using LiveTileTestPrism.Services;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveTileTestPrism
{
    sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.TryResolve<AppShell>();
            shell.SetContentFrame(rootFrame);
            return shell;
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {

        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterType<ITileUpdaterService, TileUpdaterService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILoggerService, LoggerService>(new ContainerControlledLifetimeManager());

            await base.OnInitializeAsync(args);
        }

        protected override async Task OnSuspendingApplicationAsync()
        {
            var loggerService = Container.Resolve<ILoggerService>();
            await loggerService.DumpLogsToFileAsync();
        }

        protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            var tileUpdater = Container.Resolve<ITileUpdaterService>();
            await tileUpdater.RegisterBackgroundTasks();

            NavigationService.Navigate("Main", null);
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            if (Container == null)
            {
                var logFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("info.log", CreationCollisionOption.OpenIfExists);
                await FileIO.AppendLinesAsync(logFile, new List<string> { "Container is null WTF!!" });

                deferral.Complete();
                return;
            }

            ILoggerService loggerService = Container?.Resolve<ILoggerService>();
            try
            {
                args.TaskInstance.Canceled += TaskInstance_Canceled;

                var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
                var costStr = "";
                switch (cost)
                {
                    case BackgroundWorkCostValue.Low: costStr = "Low"; break;
                    case BackgroundWorkCostValue.Medium: costStr = "Medium"; break;
                    case BackgroundWorkCostValue.High: costStr = "High"; break;
                }

                loggerService?.LogInfo($"Activated by {args.TaskInstance.Task.Name} Task -- Cost: {costStr}");

                var tileUpdater = Container.Resolve<ITileUpdaterService>();
                if (args.TaskInstance.Task.Name.Equals("BgServicingTaskName"))
                {
                    if (await tileUpdater.RegisterBackgroundTasks())
                        await tileUpdater.RefreshAllTiles();
                }
                else if (args.TaskInstance.Task.Name.Equals("BgTileUpdaterTaskName"))
                    await tileUpdater.RefreshAllTiles();
            }
            catch (Exception ex)
            {
                loggerService?.LogError((ex.InnerException ?? ex).Message);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            var reasonStr = "";
            switch (reason)
            {
                case BackgroundTaskCancellationReason.Abort: reasonStr = "Abort"; break;
                case BackgroundTaskCancellationReason.ConditionLoss: reasonStr = "ConditionLoss"; break;
                case BackgroundTaskCancellationReason.EnergySaver: reasonStr = "EnergySaver"; break;
                case BackgroundTaskCancellationReason.ExecutionTimeExceeded: reasonStr = "ExecutionTimeExceeded"; break;
                case BackgroundTaskCancellationReason.IdleTask: reasonStr = "IdleTask"; break;
                case BackgroundTaskCancellationReason.LoggingOff: reasonStr = "LoggingOff"; break;
                case BackgroundTaskCancellationReason.ResourceRevocation: reasonStr = "ResourceRevocation"; break;
                case BackgroundTaskCancellationReason.ServicingUpdate: reasonStr = "ServicingUpdate"; break;
                case BackgroundTaskCancellationReason.SystemPolicy: reasonStr = "SystemPolicy"; break;
                case BackgroundTaskCancellationReason.Terminating: reasonStr = "Terminating"; break;
                case BackgroundTaskCancellationReason.Uninstall: reasonStr = "Uninstall"; break;
            }
            var loggerService = Container?.Resolve<ILoggerService>();
            loggerService?.LogInfo($"Task '{sender.Task.Name}' cancelled: {reasonStr}");
        }
    }
}
