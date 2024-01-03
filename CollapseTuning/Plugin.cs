using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RoR2;

namespace CollapseTuning;

[BepInPlugin("kft.r2.collapsetuning", "Collapse Tuning", "1.0.0")]
[BepInDependency(R2API.R2API.PluginGUID)]
[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
public class Plugin : BaseUnityPlugin
{
    private static ConfigEntry<float> collapseDamage;

    void Awake()
    {
        collapseDamage = this.Config.Bind("Collapse", "Damage Multiplier", 0.5f, "Affects incoming collapse damage");

        if (Chainloader.PluginInfos.TryGetValue("com.rune580.riskofoptions", out _))
            ConfigureOptionsMenu();

        On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
        {
            if (self.body.teamComponent.teamIndex == TeamIndex.Player && damageInfo.dotIndex == DotController.DotIndex.Fracture)
                damageInfo.damage *= collapseDamage.Value;
            orig(self, damageInfo);
        };

        Logger.LogInfo("Loaded successfully!");
    }

    private void ConfigureOptionsMenu()
    {
        ModSettingsManager.AddOption(new RiskOfOptions.Options.StepSliderOption(collapseDamage, new StepSliderConfig { min = 0f, max = 2f, increment = 0.1f }));
    }
}
