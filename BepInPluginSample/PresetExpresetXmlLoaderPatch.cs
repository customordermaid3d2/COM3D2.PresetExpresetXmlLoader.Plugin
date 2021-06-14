using COM3D2.Lilly.Plugin;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    class PresetExpresetXmlLoaderPatch
    {        

        public static Maid[] maids=new Maid[18];
        public static string[] maidNames=new string[18];

        [HarmonyPatch(typeof(CharacterMgr), "SetActive")]
        [HarmonyPostfix]// CharacterMgr의 SetActive가 실행 후에 아래 메소드 작동
        public static void SetActive(Maid f_maid, int f_nActiveSlotNo, bool f_bMan)
        {
            if (!f_bMan)
            {
                maids[f_nActiveSlotNo] = f_maid;
                maidNames[f_nActiveSlotNo] = f_maid.status.fullNameEnStyle;
                
            }
            MyLog.LogMessage("CharacterMgr.SetActive", f_nActiveSlotNo, f_bMan, f_maid.status.fullNameEnStyle);
        }

        [HarmonyPatch(typeof(CharacterMgr), "Deactivate")]
        [HarmonyPrefix] // CharacterMgr의 SetActive가 실행 전에 아래 메소드 작동
        public static void Deactivate(int f_nActiveSlotNo, bool f_bMan)
        {
            if (!f_bMan)
            {
                maids[f_nActiveSlotNo] = null;
                maidNames[f_nActiveSlotNo] = string.Empty;
            }
            MyLog.LogMessage("CharacterMgr.Deactivate", f_nActiveSlotNo, f_bMan);
        }
    }
}
