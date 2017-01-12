using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace LiveTileTestSimple
{
    public static class Common
    {
        private static void UnregisterBackgroundTasks()
        {
            foreach (var t in BackgroundTaskRegistration.AllTasks)
                t.Value.Unregister(true);

            BackgroundExecutionManager.RemoveAccess();
        }

        public static async Task<bool> RegisterBackgroundTasks()
        {
            var servicingTaskRegistered = false;
            var tileUpdaterTaskRegistered = false;

            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == "BgTileUpdaterTaskName")
                    tileUpdaterTaskRegistered = true;
                else if (t.Value.Name.Equals("BgServicingTaskName"))
                    servicingTaskRegistered = true;
            }


            var reqAccess = await BackgroundExecutionManager.RequestAccessAsync();
            if (reqAccess == BackgroundAccessStatus.DeniedBySystemPolicy ||
                reqAccess == BackgroundAccessStatus.DeniedByUser ||
                reqAccess == BackgroundAccessStatus.Unspecified)
                return false;

            if (!servicingTaskRegistered)
            {
                var servicingTaskBuilder = new BackgroundTaskBuilder();
                servicingTaskBuilder.Name = "BgServicingTaskName";
                servicingTaskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
                servicingTaskBuilder.AddCondition(new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh));
                servicingTaskBuilder.Register();
            }

            if (!tileUpdaterTaskRegistered)
            {
                var tileUpdaterTaskbuilder = new BackgroundTaskBuilder();
                tileUpdaterTaskbuilder.Name = "BgTileUpdaterTaskName";
                tileUpdaterTaskbuilder.SetTrigger(new TimeTrigger(15, false));
                tileUpdaterTaskbuilder.AddCondition(new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh));
                tileUpdaterTaskbuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                tileUpdaterTaskbuilder.Register();
            }

            return true;
        }

        public static async Task RefreshAllTiles()
        {
            var tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles)
            {
                var tileNotification = new TileNotification(GenerateTileContent().GetXml());
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(tileNotification);
            }
        }

        public static TileContent GenerateTileContent()
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = GenerateTileBindingMedium(),
                }
            };
        }

        private static TileBinding GenerateTileBindingMedium()
        {
            var tb = new TileBinding() { Branding = TileBranding.Name };
            var content = new TileBindingContentAdaptive();
            var group = new AdaptiveGroup();
            var sg = new AdaptiveSubgroup();
            sg.Children.Add(new AdaptiveText()
            {
                Text = DateTime.Now.Date.ToString("dd-MM-yyyy"),
                HintAlign = AdaptiveTextAlign.Right
            });
            sg.Children.Add(new AdaptiveText()
            {
                Text = $"{DateTime.Now.Hour.ToString().PadLeft(2, '0')}:{DateTime.Now.Minute.ToString().PadLeft(2, '0')}",
                HintAlign = AdaptiveTextAlign.Right,
                HintStyle = AdaptiveTextStyle.CaptionSubtle
            });
            sg.Children.Add(new AdaptiveText()
            {
                Text = $"Moon: 10 %",
                HintAlign = AdaptiveTextAlign.Right
            });
            sg.Children.Add(new AdaptiveText()
            {
                Text = $"0 °C, 40 %",
                HintAlign = AdaptiveTextAlign.Right
            });

            group.Children.Add(sg);

            content.Children.Add(group);
            tb.Content = content;

            return tb;
        }
    }
}
