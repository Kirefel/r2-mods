using BepInEx;
using R2API.Utils;
using RoR2;
using RoR2.Items;
using System.Linq;

namespace LunarCoinSharedPool;

[BepInPlugin("kft.r2.lunarcoinsharedpool", "Lunar Coin Shared Pool", "1.0.0")]
[BepInDependency("com.dan8991iel.LunarCoinShareOnPickup")]
[BepInDependency("com.Varna.EphemeralCoins")]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)]
public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        EphemeralCoins.BepConfig.EnableArtifact.Value = 2;
        CostTypeCatalog.GetCostTypeDef(CostTypeIndex.LunarCoin).payCost = (costTypeDef, context) =>
        {
            foreach (var master in PlayerCharacterMasterController.instances)
            {
                master.networkUser.DeductLunarCoins((uint)context.cost);
                MultiShopCardUtils.OnNonMoneyPurchase(context);
            }
        };

        // Catch up late joiners
        On.RoR2.Run.OnUserAdded += (orig, self, user) =>
        {
            uint max = EphemeralCoins.EphemeralCoins.instance.coinCounts.Max(x => x.ephemeralCoinCount);
            EphemeralCoins.EphemeralCoins.instance.giveCoinsToUser(user, max);
        };
    }
}
