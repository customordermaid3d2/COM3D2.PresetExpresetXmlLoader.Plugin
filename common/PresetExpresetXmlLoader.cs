using BepInEx;
using BepInEx.Logging;
using CM3D2.ExternalSaveData.Managed;
using COM3D2API;
using LillyUtill.MyMaidActive;
using LillyUtill.MyWindowRect;
using System;
using System.IO;
using UnityEngine;
//using COM3D2.LillyUtill;
using UnityEngine.SceneManagement;


namespace COM3D2.PresetExpresetXmlLoader.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "PresetExpresetXmlLoader";
        public const string PLAGIN_VERSION = "22.2.22";
        public const string PLAGIN_FULL_NAME = "COM3D2.PresetExpresetXmlLoader.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInPlugin("COM3D2.Sample.Plugin", "COM3D2.Sample.Plugin", "21.6.6")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInProcess("COM3D2x64.exe")]
    public class PresetExpresetXmlLoader : BaseUnityPlugin
    {
        internal static ManualLogSource log;

        public static WindowRectUtill myWindowRect;


        string[] type = new string[] { "one", "all" };
        internal static Maid maid;

        public static int seleted;
        private int all;

        public static System.Windows.Forms.OpenFileDialog openDialog;
        public static System.Windows.Forms.SaveFileDialog saveDialog;

        static bool isShowDialogSaveRun = false;
        static bool isShowDialogLoadRun = false;
        private bool result;


        /// <summary>
        /// 0.
        /// 플러그인 로딩시 이부분이 가장 먼저 실행됨
        /// </summary>
        public PresetExpresetXmlLoader()
        {
            log = Logger;
        }

        /// <summary>
        /// 1.
        /// 그리고 베핀 플러그인에서 이부분을 실행
        ///  게임 실행시 한번만 실행됨
        ///  유니티앤진에서 사용하는 함수라서 이와 관련된건
        ///  https://docs.unity3d.com/kr/530/Manual/ExecutionOrder.html
        ///  를 참조하면 제일 좋음
        /// </summary>
        public void Awake()
        {
            log.LogMessage("Awake");
            log.LogMessage("https://github.com/customordermaid3d2/COM3D2.PresetExpresetXmlLoader.Plugin");

            PresetExpresetXmlLoaderUtill.init();

            myWindowRect = new WindowRectUtill(WindowFunctionBody, Config, Logger, MyAttribute.PLAGIN_NAME, "PEXL");

            MaidActiveUtill.setActiveMaidNum += SetMaid;
            MaidActiveUtill.deactivateMaidNum += SetMaidd;
        }

        public static void SetMaid(int seleted)
        {
            if (PresetExpresetXmlLoader.seleted == seleted)
            {
                PresetExpresetXmlLoaderUtill.SetMaid(maid = MaidActiveUtill.GetMaid(seleted));
            }
        }

        public static void SetMaidd(int seleted)
        {
            if (PresetExpresetXmlLoader.seleted == seleted)
            {
                maid = null;
            }
        }

        /// <summary>
        /// 2. 그다음 플러그인을 켰을때 작동되는 함수
        /// 나중에 플러그인 끄고 다시 켰을때 이함수가 다시 작동되니 여기에 뭘 넣을지는 잘 생각해야함
        /// </summary>
        public void OnEnable()
        {
            log.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;


        }

        /// <summary>
        /// 3. OnEnable 실행된후에 한번만 실행
        /// OnEnable 이 몇번이나 실행되도 이건 플러그인 로딩 후에 한번만 실행되고 두번다시 실행 안됨
        /// 보통 이게 작동되는 시점은 모든 플러그인이 초기화 되고 데이터를 다 불러온 후에 작동함
        /// Awake 가 게임 킨 직후라면
        /// Start 는 데이터를 다 불러온 후
        /// 게임 실행시 한번만 실행됨
        /// </summary>
        public void Start()
        {
            log.LogMessage("Start");

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

            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , new Action(delegate ()
                { // 기어메뉴 아이콘 클릭시 작동할 기능                    
                    myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                })
                , MyAttribute.PLAGIN_NAME  // 표시될 툴팁 내용                                                                                 
                , Properties.Resources.icon);// 표시될 아이콘

        }

        public string scene_name = string.Empty;

        /// <summary>
        /// 이건 장면이 바귀었을때마다 작동할 코드 작성
        /// 보통 OnEnable 에다가 += this.OnSceneLoaded; 를 해주고
        /// OnDisable 에다가 -= this.OnSceneLoaded; 를 해줌
        /// 어지간하면 scene.buildIndex 는 쓰지 말자
        /// scene.name 를 쓰자
        /// buildIndex 는 게임 버전 바귈때마다 번호가 바귈 위험성이 매우큼
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            log.LogInfo($"OnSceneLoaded , {scene.name}, {scene.buildIndex}");
            //  scene.buildIndex 는 쓰지 말자 제발
            scene_name = scene.name;
        }

        // public void FixedUpdate()
        // {
        //
        // }

        /// <summary>
        /// 여기는 게임 로직 부분
        /// </summary>
        public void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsUp())
            //{
            //    MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
        }

        /// <summary>
        /// 여기도 마찬가지
        /// </summary>
        // public void LateUpdate()
        // {
        //
        // }


        /// <summary>
        /// 여기가 GUI 작성 부분
        /// 지금 이 플러그인은 약간 특이하게 다른족에서 GUI 불러들임
        /// 자세한건 위에 다시 설명해줌
        /// </summary>
        public void OnGUI()
        {
            myWindowRect.OnGUI();
        }

        private void WindowFunctionBody(int id)
        {

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

            // GUI.enabled = true;
            GUILayout.Label("option");

            all = GUILayout.SelectionGrid(all, type, 2);
            seleted = MaidActiveUtill.SelectionGrid(seleted);
            if (GUI.changed)
            {
                PresetExpresetXmlLoaderUtill.SetMaid(maid = MaidActiveUtill.GetMaid(seleted));
                GUI.changed = false;
            }

            //GUI.enabled = true;
            GUILayout.Label("edit");
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

        }

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
                PresetExpresetXmlLoader.log.LogMessage($"ShowDialogSaveRun {e.Message}");
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
                PresetExpresetXmlLoader.log.LogMessage($"ShowDialogLoadRun {e.Message}");
            }
            isShowDialogLoadRun = false;
        }

        /// <summary>
        /// 플러그인 껏을때 작동
        /// 게임에서 F1 누르면 나오는 베핀플러그인 메니저 띄우면
        /// 활성 비활성 시켜주는 옵션도 있는데 내가 이 플러그인은 그걸 추가 안했네..
        /// </summary>
        public void OnDisable()
        {
            log.LogMessage("OnDisable");

            SceneManager.sceneLoaded -= this.OnSceneLoaded;

        }

    }
}
