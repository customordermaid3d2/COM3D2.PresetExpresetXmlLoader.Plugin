using BepInEx.Configuration;
using CM3D2.ExternalSaveData.Managed;
//using COM3D2.LillyUtill;
using COM3D2API;
//using Ookii.Dialogs;
using System;
using System.IO;
using UnityEngine;
using System.Drawing;
using System.Drawing.Imaging;
using LillyUtill.MyWindowRect;
using LillyUtill.MyMaidActive;

namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    public class PresetExpresetXmlLoaderGUI : MonoBehaviour
    {
        public static PresetExpresetXmlLoaderGUI instance;

        private static ConfigFile config;

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        private static int windowId = new System.Random().Next();

        private static Vector2 scrollPosition;

        // 위치 저장용 테스트 json
        public static WindowRectUtill myWindowRect;

        public static System.Windows.Forms.OpenFileDialog openDialog;
        public static System.Windows.Forms.SaveFileDialog saveDialog;
        //VistaOpenFileDialog openDialog = new VistaOpenFileDialog();
        //VistaSaveFileDialog saveDialog = new VistaSaveFileDialog();


        public static int seleted;
        private int all;


        /// <summary>
        /// 부모 PresetExpresetXmlLoader 앤진? 에다가 PresetExpresetXmlLoaderGUI 앤진? 를 추가 시켜줌
        /// 즉 부모는 부모대로 Awake Update 같은게 돟아가고
        /// PresetExpresetXmlLoaderGUI 는 여기대로  Awake Update 가 돌아가게됨
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static PresetExpresetXmlLoaderGUI Install(GameObject parent, ConfigFile config)
        {
            PresetExpresetXmlLoaderGUI.config = config;
            instance = parent.GetComponent<PresetExpresetXmlLoaderGUI>();
            if (instance == null)
            {
                instance = parent.AddComponent<PresetExpresetXmlLoaderGUI>();
                //MyLog.LogMessage("PresetExpresetXmlLoaderGUI.Install", instance.name);
            }
            return instance;
        }

        /// <summary>
        /// 아까 부모 PresetExpresetXmlLoader 에서 봤던 로직이랑 같음
        /// </summary>
        public void Awake()
        {
            //Logger.LogMessage("PresetExpresetXmlLoaderGUI.OnEnable");

            myWindowRect = new WindowRectUtill(config, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, "PEXL");
            //IsGUIOn = config.Bind("GUI", "isGUIOn", false); // 이건 베핀 설정값 지정용
            // 이건 단축키
            ShowCounter = config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftControl));

            // 이건 기어메뉴 아이콘
            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , new Action(delegate ()
                { // 기어메뉴 아이콘 클릭시 작동할 기능
                    PresetExpresetXmlLoader.myLog.LogInfo($"SystemShortcutAPI {myWindowRect.IsGUIOn}");
                    myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                })
                , MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString() // 표시될 툴팁 내용
                                                                                 // 표시될 아이콘
                , ExtractResource(COM3D2.PresetExpresetXmlLoader.Plugin.Properties.Resources.icon));
            // 아이콘은 이렇게 추가함

            // 파일 열기창 설정 부분. 이런건 구글링 하기
            openDialog = new System.Windows.Forms.OpenFileDialog()
            {
                // 기본 확장자
                DefaultExt = "xml",
                // 기본 디렉토리
                InitialDirectory = Path.Combine(GameMain.Instance.SerializeStorageManager.StoreDirectoryPath, "preset"),
                // 선택 가능 확장자
                Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"
            };
            saveDialog = new System.Windows.Forms.SaveFileDialog()
            {
                DefaultExt = "xml",
                InitialDirectory = Path.Combine(GameMain.Instance.SerializeStorageManager.StoreDirectoryPath, "preset"),
                Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"
            };

        }

        public static byte[] ExtractResource(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public void Update()
        {
            if (ShowCounter.Value.IsUp())// 단축키가 일치할때
            {
                myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;// 보이거나 안보이게. 이런 배열이였네 지웠음
                //MyLog.LogMessage("IsUp",  ShowCounter.Value.MainKey);
            }
        }

        // 매 화면 갱신할때마다(update 말하는게 아님)
        public void OnGUI()
        {
            if (!myWindowRect.IsGUIOn)
                return;

            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        string[] type = new string[] { "one", "all" };
        Maid maid;

        // 창일 따로 뜬 부분에서 작동
        public void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
            // 라벨 추가
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!myWindowRect.IsOpen)
            {

            }
            else
            {
                // 스크롤 영역
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                // 메이드가 있을때만 여기 아래 기능들 클릭 가능
                //GUI.enabled = LillyUtill.MaidActivePatch.GetMaid(seleted) != null;
                if (GUI.enabled)
                {
                    GUILayout.Label("");
                }
                else
                {
                    GUILayout.Label("maid null");
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("load"))
                {
                    if (!isShowDialogLoadRun)
                    {
                        //Task.Factory.StartNew(ShowDialogLoadRun);
                        ShowDialogLoadRun();
                    }
                }
                if (GUILayout.Button("save"))
                {
                    if (!isShowDialogSaveRun)
                    {
                        //Task.Factory.StartNew(ShowDialogSaveRun);
                        ShowDialogSaveRun();
                    }
                }
                if (GUILayout.Button("del"))
                {
                    foreach (var itemp in PresetExpresetXmlLoaderUtill.itemps)
                        ExSaveData.Remove(maid, "CM3D2.MaidVoicePitch", itemp.name);
                }

                GUILayout.EndHorizontal();

                GUI.enabled = true;
                GUILayout.Label("option");

                all = GUILayout.SelectionGrid(all, type, 2);
                seleted = MaidActiveUtill.SelectionGrid(seleted);

                GUI.enabled = true;
                GUILayout.Label("edit");
                maid = MaidActiveUtill.GetMaid(seleted);
                if (maid != null)
                {
                    foreach (var itemp in PresetExpresetXmlLoaderUtill.itemps)
                    {
                        itemp.enable = GUILayout.Toggle(itemp.enable, itemp.name);

                        if (GUI.changed)
                        {
                            result = ExSaveData.SetBool(maid, "CM3D2.MaidVoicePitch", itemp.name, itemp.enable);
                            maid.body0.bonemorph.Blend();
                            GUI.changed = false;
                        }

                        if (itemp.enable)
                        {
                            foreach (var item in itemp.items)
                            {
                                item.value = GUILayout.HorizontalSlider(item.value, 0, 2f);
                                if (GUI.changed)
                                {
                                    result = ExSaveData.SetFloat(maid, "CM3D2.MaidVoicePitch", item.name, item.value);                                    
                                    maid.body0.bonemorph.Blend();
                                    GUI.changed = false;
                                }
                            }
                        }
                    }
                }


                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        static bool isShowDialogSaveRun = false;
        static bool isShowDialogLoadRun = false;
        private bool result;

        private void ShowDialogSaveRun()
        {
            isShowDialogSaveRun = true;
            try
            {

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (all == 0)
                    {
                        PresetExpresetXmlLoaderUtill.Save(seleted, saveDialog.FileName);
                    }
                    else
                    {
                        int s = saveDialog.FileName.LastIndexOf(".xml");
                        for (int i = 0; i < 18; i++)
                        {
                            PresetExpresetXmlLoaderUtill.Save(i, saveDialog.FileName.Insert(s, "_" + MaidActiveUtill.maidNames[i]));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                PresetExpresetXmlLoader.myLog.LogMessage($"ShowDialogSaveRun {e.Message}");
            }
            isShowDialogSaveRun = false;
        }

        /// <summary>
        /// 원래 함수를 별도로 만든게 아닌데 오류땜에 뺏다가 현상 유지됨..
        /// </summary>
        private void ShowDialogLoadRun()
        {
            isShowDialogLoadRun = true; 
            try
            {

                if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)// 오픈했을때
                {
                    if (all == 0)
                    {
                        PresetExpresetXmlLoaderUtill.Load(seleted, openDialog.FileName);
                        // 파일 읽고 메이드에게 처리해주는 기능 
                    }
                    else
                    {
                        for (int i = 0; i < 18; i++)
                            PresetExpresetXmlLoaderUtill.Load(i, openDialog.FileName);
                    }
                }
            }
            catch (Exception e)
            {
                PresetExpresetXmlLoader.myLog.LogMessage($"ShowDialogLoadRun {e.Message}");
            }
            isShowDialogLoadRun = false;
        }

        /// <summary>
        /// 게임 X 버튼 눌렀을때 반응
        /// </summary>
       //public void OnApplicationQuit()
       //{
       //    PresetExpresetXmlLoaderGUI.myWindowRect.save();
       //    //MyLog.LogMessage("OnApplicationQuit");
       //}

        /// <summary>
        /// 게임 종료시에도 호출됨
        /// </summary>
       // public void OnDisable()
       // {
       //
       //     PresetExpresetXmlLoaderGUI.myWindowRect.save();
       //     SceneManager.sceneLoaded -= this.OnSceneLoaded;
       // }

    }
}
