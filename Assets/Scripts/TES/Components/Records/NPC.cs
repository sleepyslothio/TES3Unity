using TES3Unity.ESM.Records;
using TES3Unity.World;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class NPC : RecordComponent
    {
        private void Start()
        {
            var NPC_ = (NPC_Record)record;

            // Add items
            var db = TES3Manager.MWDataReader.MorrowindESMFile.ObjectsByIDString;
            var nifManager = NIFManager.Instance;

            foreach (var npcItem in NPC_.Items)
            {
                if (db.ContainsKey(npcItem.Name))
                {
                    var item = db[npcItem.Name];
                    //Debug.Log($"Item {npcItem.Name} of type {item.header.name} found.");

                    var weapon = item as WEAPRecord;
                    if (weapon != null)
                    {
                        var weaponModel = nifManager.InstantiateNIF($"meshes\\{weapon.Model}", false);
                        weaponModel.transform.SetParent(transform.FindChildRecursiveExact("Right Hand"));
                        weaponModel.transform.localPosition = Vector3.zero;
                        weaponModel.transform.localRotation = Quaternion.Euler(180, 0, 180);
                    }

                    var cloth = item as CLOTRecord;
                    if (cloth != null)
                    {
                        foreach (var part in cloth.BodyPartGroup)
                        {
                            var boneName = Convert.NormalizeFromEnum(part.Index.ToString());
                            var bone = transform.FindChildRecursiveExact(boneName);
                            if (bone == null)
                            {
                                Debug.Log($"Can't find bone {boneName}");
                                continue;
                            }

                            var modelName = part.MalePartName;

                            if (NPC_.IsFemale && !string.IsNullOrEmpty(part.FemalePartName))
                            {
                                modelName = part.FemalePartName;
                            }

                            if (string.IsNullOrEmpty(modelName))
                            {
                                continue;
                            }

                            if (modelName == "imperial skirt")
                            {
                                modelName = "a\\a_imperial_skirt.nif";
                            }
                            else
                            {
                                modelName = $"c\\{modelName}.nif";
                            }

                            var renderers = bone.GetComponentsInChildren<Renderer>();
                            foreach (var renderer in renderers)
                            {
                                renderer.enabled = false;
                            }

                            NPCFactory.AddBodyPart(nifManager, $"{modelName}", bone, bone.name.ToLower().Contains("left"));
                        }
                    }

                    var armor = item as ARMORecord;
                    if (armor != null)
                    {
                        continue;
                        foreach (var part in armor.BodyPartGroup)
                        {
                            var boneName = Convert.NormalizeFromEnum(part.Index.ToString());
                            var bone = transform.FindChildRecursiveExact(boneName);
                            if (bone == null)
                            {
                                Debug.Log($"Can't find bone {boneName}");
                                continue;
                            }

                            var modelName = part.MalePartName;

                            if (NPC_.IsFemale && !string.IsNullOrEmpty(part.FemalePartName))
                            {
                                modelName = part.FemalePartName;
                            }

                            if (string.IsNullOrEmpty(modelName))
                            {
                                continue;
                            }

                            // https://github.com/philjord/esmj3dtes3/blob/master/esmj3dtes3/src/esmj3dtes3/j3d/j3drecords/type/J3dNPC_.java
                            if (modelName == "imperial cuirass" || modelName == "a_imperial_gauntlet")
                                modelName = "a\\a_imperial_skins.nif";
                            else if (modelName == "imperial boot foot")
                                modelName = "a\\a_imperial_f_shoe.nif";//(f for foot, not female)
                            else if (modelName == "imperial boot ankle")
                                modelName = "a\\a_imperial_a_boot.nif";
                            else if (modelName == "imperial helmet")
                                modelName = "a\\a_imperial_m_helmet.nif";
                            else if (modelName == "imperial ua")
                                modelName = "a\\a_imperial_ua_pauldron.nif";
                            else if (modelName == "imperial cl pauldron")
                                modelName = "a\\a_imperial_cl_pauldron.nif";
                            else if (modelName == "a_iron_cuirass")
                                modelName = "a\\a_iron_skinned.nif";
                            else if (modelName == "a_nordicfur_cuirass" || modelName == "a_nordicfur_gauntlet")
                                modelName = "a\\a_nordicfur_skinned.nif";
                            else if (modelName == "a_steel_cuirass")
                                modelName = "a\\a_steel_skin.nif";
                            else if (modelName == "a_m_chitin_chest")
                                modelName = "a\\a_m_chitin_skinned.nif";
                            else if (modelName == "a_netch_m_chest")
                                modelName = "a\\a_netch_m_cuirass2.nif";
                            else if (modelName == "a_netch_m_gauntlet")
                                modelName = "a\\a_netch_m_skinned.nif";
                            else if (modelName == "a_orcish_boot_f")
                                modelName = "a\\a_orcish_boots_f.nif";
                            else if (modelName == "a_orcish_boot_a")
                                modelName = "a\\a_orcish_boots_a.nif";
                            else if (modelName == "a_orcish_cuirass")
                                modelName = "a\\a_orcish_cuirass_c.nif";
                            else if (modelName == "c_slave_bracer")
                                modelName = "c\\c_slave_bracer.nif";
                            else if (modelName == "c_m_bracer_w_leather01")
                                modelName = "c\\c_m_bracer_w_leather01.nif";
                            else if (modelName == "c_m_bracer_w_clothwrap02")
                                modelName = "c\\c_m_bracer_w_clothwrap02.nif";
                            else if (modelName == "indoril hlelmet")
                                modelName = "a\\a_indoril_m_helmet.nif";
                            else if (modelName == "a_indoril_m_gauntlet")
                                modelName = "a\\a_indoril_m_skins.nif";
                            else if (modelName == "a_indoril_m_chest")
                                modelName = "a\\a_indoril_m_skins.nif";
                            else
                                modelName = $"a\\{modelName}.nif";

                            NPCFactory.AddBodyPart(nifManager, $"{modelName}", bone, bone.name.ToLower().Contains("left"));
                        }
                    }
                }
            }
        }
    }
}
