using HarmonyLib;
using MelonLoader;
using player;
using Table;

[assembly: MelonInfo(typeof(HarvestMoonML.HMStaminaMultiplier), "HM:WoA stamina mult", "1.0.0", "Greger", null)]
[assembly: MelonGame("Natsume Inc.", "Harvest Moon: The Winds of Anthos")]

namespace HarvestMoonML
{
    public class HMStaminaMultiplier : MelonMod
    {
        private MelonPreferences_Category mainPrefCategory;
        private MelonPreferences_Entry<float> prefMultiplier;
        
        public override void OnInitializeMelon()
        {
            mainPrefCategory = MelonPreferences.CreateCategory("HMStaminaMultiplier");
            prefMultiplier = mainPrefCategory.CreateEntry("Stamina damage multiplier", 0.5f);
            mainPrefCategory.SaveToFile();
            
            LoggerInstance.Msg($"Initialized with a stamina damage multiplier of {prefMultiplier.Value}");
        }
        
        [HarmonyPatch(typeof(HpCostController), "ApplyHpCostImpl")]
        public static class ApplyHpCostImplNew {
            [HarmonyPrefix]
            public static bool Prefix(PLAYERHPCOST.Defs id)
            {
                var orgDamage = TableAccessor.PlayerHpCost.get_param((int)id);
                var newDamage = (int)Math.Floor(orgDamage * Melon<HMStaminaMultiplier>.Instance.prefMultiplier.Value);
                //Melon<HMStaminaMultiplier>.Logger.Msg($"Taking {newDamage} damage. Originally {orgDamage} damage.");
                SingletonMonoBehaviour<GameInfo>.Instance.PlayerInfo.Hp -= newDamage;
                //Melon<HMStaminaMultiplier>.Logger.Msg($"Current health after changes: {SingletonMonoBehaviour<GameInfo>.Instance.PlayerInfo.Hp}");
                
                // Returning false so we skip running the original function
                return false;
            }
        }
    }
}