using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using System.Threading.Tasks;
using DynamicData;
using Noggog;

namespace BookUUNPCBPCKeywords
{
    public class Program
    {

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            //Your code here!
            JObject stuff = JObject.Parse(File.ReadAllText(state.RetrieveInternalFile("bookuunp.json")));
            var ModKey = new ModKey("CBPCArmorKeywords", type: ModType.Plugin);
            var AsNakedL = new FormKey(ModKey, 0x000800);
            var AsNakedR = new FormKey(ModKey, 0x000801);
            var AsClothingL = new FormKey(ModKey, 0x000802);
            var AsLightL = new FormKey(ModKey, 0x000804);
            var AsHeavyL = new FormKey(ModKey, 0x000806);
            var NoPushUpL = new FormKey(ModKey, 0x000808);
            var AsClothingR = new FormKey(ModKey, 0x000803);
            var AsLightR = new FormKey(ModKey, 0x000805);
            var AsHeavyR = new FormKey(ModKey, 0x000807);
            var NoPushUpR = new FormKey(ModKey, 0x000809);


            foreach (var armor in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                if (armor.EditorID != null)
                {
                    bool isEnch = armor.EditorID.StartsWith("Ench");
                    bool hasHooded = armor.EditorID.Contains("Hooded");
                    bool hasRobes = armor.EditorID.Contains("Robes");
                    bool hasCuirass = armor.EditorID.Contains("Cuirass");

                    string keycheck = armor.EditorID;
                    int endpos = 999;

                    if (isEnch)
                    {
                        if (hasHooded)
                        {
                            endpos = keycheck.IndexOf("Hooded") + 6;
                        } else if (hasRobes)
                        {
                            endpos = keycheck.IndexOf("Robes") + 5;
                        } else if (hasCuirass)
                        {
                            endpos = keycheck.IndexOf("Cuirass") + 7;
                        }
                        if (endpos > 4 && endpos < 999)
                        {
                            keycheck = armor.EditorID[4..endpos]; // match enchanted armor varieties to their common substring instead of an explicit editor ID
                        }
                    }

                    if (stuff.ContainsKey(keycheck))
                    {
                        JArray? tags = stuff[keycheck] as JArray;

                        if (tags != null)
                        {
                            var armorEditObj = state.PatchMod.Armors.GetOrAddAsOverride(armor);

                            string?[] tagstext = tags.Select(c => (string?)c).ToArray();

                            foreach (var tt in tagstext)
                            {

                                switch (tt)
                                {
                                    case "CBPCAsNakedL":
                                        armorEditObj.Keywords!.Add(AsNakedL);
                                        break;
                                    case "CBPCAsNakedR":
                                        armorEditObj.Keywords!.Add(AsNakedR);
                                        break;
                                    case "CBPCAsClothingL":
                                        armorEditObj.Keywords!.Add(AsClothingL);
                                        break;
                                    case "CBPCAsClothingR":
                                        armorEditObj.Keywords!.Add(AsClothingR);
                                        break;
                                    case "CBPCAsLightL":
                                        armorEditObj.Keywords!.Add(AsLightL);
                                        break;
                                    case "CBPCAsLightR":
                                        armorEditObj.Keywords!.Add(AsLightR);
                                        break;
                                    case "CBPCAsHeavyL":
                                        armorEditObj.Keywords!.Add(AsHeavyL);
                                        break;
                                    case "CBPCAsHeavyR":
                                        armorEditObj.Keywords!.Add(AsHeavyR);
                                        break;
                                    case "CBPCNoPushUpL":
                                        armorEditObj.Keywords!.Add(NoPushUpL);
                                        break;
                                    case "CBPCNoPushUpR":
                                        armorEditObj.Keywords!.Add(NoPushUpR);
                                        break;

                                }
                            }
                        }

                    }
                }
               
            }
        }
    }
}
