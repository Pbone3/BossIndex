using BossIndex.Common.CrossMod.Call;
using BossIndex.Core;
using BossIndex.Core.BossIndexInfoBuilding;
using log4net;
using PboneLib.CustomLoading;
using PboneLib.CustomLoading.Content;
using PboneLib.Services.CrossMod;
using PboneLib.Utils;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BossIndex
{
    // TODO: Fix all summaries, some are funky
    public class BossIndexMod : Mod
    {
        public static BossIndexMod Instance => ModContent.GetInstance<BossIndexMod>();
        public static ILog Log => Instance.Logger;

        public static IBossIndexInformationEngine InformationEngine => Instance.infoEngine;
        public static IBossIndexInformationView InformationView => Instance.infoView;
        public static BossIndexInfoFactory InformationFactory => Instance.infoFactory;
        public static CrossModManager CrossMod => Instance.crossModManager;

        private IBossIndexInformationEngine infoEngine;
        private IBossIndexInformationView infoView;
        private BossIndexInfoFactory infoFactory;
        private CrossModManager crossModManager;

        public override void Load()
        {
            base.Load();

            infoFactory = new BossIndexInfoFactory();

            crossModManager = new CrossModManager();
            crossModManager.CallManager.RegisterHandler<AddInfo>();

            // Custom Loading
            CustomModLoader loader = new(this);

            // ModConfigs
            ContentLoader cLoader = new(content => {
                // TODO Unhardcode me! Method in PConfig
                AddConfig(content.Content.GetType().Name, (ModConfig)content.Content);
            });
            cLoader.Settings.TryToLoadConditions = ContentLoader.ContentLoaderSettings.PresetTryToLoadConfigConditions;
            loader.Add(cLoader);

            // Everything else
            cLoader = new ContentLoader(this.AddContent);
            cLoader.Settings.TryToLoadConditions = ContentLoader.ContentLoaderSettings.PresetTryToLoadConfigConditions;
            loader.Add(cLoader);

            // TODO: Fix localization loading in PboneLib!
            loader.Add(new PboneLib.CustomLoading.Localization.LocalizationLoader(LocalizationLoader.AddTranslation));

            loader.Load();
        }

        public void SetInformationEngine(IBossIndexInformationEngine engine)
        {
            infoEngine?.TransferBossInfo(engine);
            infoEngine = engine;
            infoView.Engine = engine;
        }
    }
}