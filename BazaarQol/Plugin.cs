using BepInEx;
using RoR2;

namespace BazaarQol;

[BepInPlugin("kft.r2.bazaarqol", "Bazaar Qol", "1.0.0")]
[BepInDependency(R2API.R2API.PluginGUID)]
[R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.NoNeedForSync)]
public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        // Invulnerability in the shop to prevent loss of consumables or death
        On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
        {
            if (BazaarController.instance && self.body.teamComponent.teamIndex == TeamIndex.Player)
                damageInfo.damage = 0f;

            orig(self, damageInfo);
        };

        // Lunar seers show you which stage they go to in the text popup
        On.RoR2.SeerStationController.OnTargetSceneChanged += (orig, self, targetSceneDef) =>
        {
            orig(self, targetSceneDef);
            self.GetComponent<PurchaseInteraction>().contextToken = targetSceneDef.nameToken;
        };

        Logger.LogInfo("Setup complete!");
    }
}
