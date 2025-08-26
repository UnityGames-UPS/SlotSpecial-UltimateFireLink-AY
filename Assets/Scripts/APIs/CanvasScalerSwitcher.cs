using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasScalerSwitcher : MonoBehaviour
{
    [Header("For Testing in unity")]
    public bool isTesting = false;
    public bool isPC = true;
    public bool isApple = false;



    [Space]
    [Space]
    [SerializeField]
    private CanvasScaler canvasOP;

    [SerializeField]
    private UIManager Uimanager;

    [SerializeField]
    private SlotBehaviour slotManager;

    [Header("UI Change For Apple")]

    [SerializeField] private GameObject MobileTopBar;
    [SerializeField] private GameObject MobileBottomBar;
    [SerializeField] private GameObject MobileMajorMiniMinor;
    [SerializeField] private GameObject BonusCountUI;

    [Header("BG Change")]
    [SerializeField] private Sprite LandscapeBG;
    [SerializeField] private Sprite PotrateBG;
    [SerializeField] private Image BGObject;
    [SerializeField] private GameObject FreeSpinCount;
    [SerializeField] private GameObject BonusSpinCount;
    [SerializeField] private Transform MobileFreeSpinCountpos;
    [SerializeField] private Transform MobileSunHitPoint;
    [SerializeField] private GameObject SunHitPoint;

    [SerializeField] private GameObject MainSlot;
    [SerializeField] private GameObject BonusSlot;

    [Header("UI Elements")]
    [SerializeField] private Button m_ibutton;
    [SerializeField] private Button p_ibutton;
    [SerializeField] private Button m_MusicButton;
    [SerializeField] private Button m_MusicOFFButton;
    [SerializeField] private Button p_MusicButton;
    [SerializeField] private Button p_MusicOFFButton;
    [SerializeField] private Button m_SoundButton;
    [SerializeField] private Button m_SoundOFFButton;
    [SerializeField] private Button p_SoundButton;
    [SerializeField] private Button p_SoundOFFButton;
    [SerializeField] private Button p_backButton;
    [SerializeField] private Button m_backButton;
    [SerializeField] private Button m_plusButton;
    [SerializeField] private Button p_plusButton;
    [SerializeField] private Button p_minusButton;
    [SerializeField] private Button m_minusButton;


    [SerializeField] private TMP_Text m_creditText;
    [SerializeField] private TMP_Text p_creditText;
    [SerializeField] private TMP_Text p_winText;
    [SerializeField] private TMP_Text m_winText;
    [SerializeField] private TMP_Text p_TotalbetText;
    [SerializeField] private TMP_Text m_TotalbetText;
    [SerializeField] private TMP_Text[] m_MiniMajorMinor;
    [SerializeField] private TMP_Text[] p_MiniMajorMinor;



    void Awake()
    {
        if (isTesting)
        {
            if (isPC) AssignValuesForPC();
            else
            {
                AssignValuesForMobile();
                if (isApple)
                {
                    AssignValuseForApple();
                }
            }
        }
        // #if UNITY_WEBGL && !UNITY_EDITOR
        //         // Calls the JavaScript function 'isMobile()' from Unity
        //         Debug.Log("Dev_Test:"+"DeviceCheck----------------------");
        //         Application.ExternalCall("isMobile");
        // #endif
    }
    private void Start()
    {

    }

    // This method will be called from the JavaScript side
    public void OnMobileDeviceDetected(string s)
    {
        Debug.Log("Called OnMobileDeviceDetected");
        if (s == "A")
        {

            AssignValuesForMobile();
            Debug.Log("Dev_Test:" + "This is a mobile device.-----------------------------------------");
        }
        else if (s == "I")
        {
            AssignValuesForMobile();
            AssignValuseForApple();
            Debug.Log("Dev_Test:" + "This is a Apple device.-----------------------------------------");
        }
        else
        {

            AssignValuesForPC();
            Debug.Log("Dev_Test:" + "This is a PC device.--------------------------------------------");
        }

    }

    private void AssignValuseForApple()
    {
        MainSlot.transform.localPosition = new Vector3(MainSlot.transform.localPosition.x, MainSlot.transform.localPosition.y + 100f, MainSlot.transform.localPosition.z);
        BonusSlot.transform.localPosition = new Vector3(BonusSlot.transform.localPosition.x, BonusSlot.transform.localPosition.y + 100f, BonusSlot.transform.localPosition.z);
        // SunHitPoint.transform.localPosition = new Vector3(SunHitPoint.transform.localPosition.x, SunHitPoint.transform.localPosition.y + 100f, SunHitPoint.transform.localPosition.z);
        MobileBottomBar.transform.localPosition = new Vector3(MobileBottomBar.transform.localPosition.x, MobileBottomBar.transform.localPosition.y + 100f, MobileBottomBar.transform.localPosition.z);


        MobileTopBar.transform.localPosition = new Vector3(MobileTopBar.transform.localPosition.x, MobileTopBar.transform.localPosition.y - 100f, MobileTopBar.transform.localPosition.z);
        MobileMajorMiniMinor.transform.localPosition = new Vector3(MobileMajorMiniMinor.transform.localPosition.x, MobileMajorMiniMinor.transform.localPosition.y - 100f, MobileMajorMiniMinor.transform.localPosition.z);
        BonusCountUI.transform.localPosition = new Vector3(BonusCountUI.transform.localPosition.x, BonusCountUI.transform.localPosition.y - 100f, BonusCountUI.transform.localPosition.z);


    }

    public void AssignValuesForPC()
    {
        canvasOP.referenceResolution = new Vector2(2340f, 1080f);
        Uimanager.IsPcToggle(true);

        BGObject.sprite = LandscapeBG;

        Uimanager.Paytable_Button = p_ibutton;
        Uimanager.Sound_Button = p_SoundButton;
        Uimanager.Music_Button = p_MusicButton;
        Uimanager.GameExit_Button = p_backButton;
        Uimanager.SoundOFF_Button = p_SoundOFFButton;
        Uimanager.MusicOFF_Button = p_MusicOFFButton;

        slotManager.Balance_text = p_creditText;
        slotManager.TotalBet_text = p_TotalbetText;
        slotManager.TotalWin_text = p_winText;
        slotManager.TBetPlus_Button = p_plusButton;
        slotManager.TBetMinus_Button = p_minusButton;

        for (int i = 0; i < p_MiniMajorMinor.Length; i++)
        {
            slotManager.MiniMajorMinor[i] = null;
            slotManager.MiniMajorMinor[i] = p_MiniMajorMinor[i];

        }
        Uimanager.UpdateThebuttons();
        slotManager.assignbuttons();

    }

    public void AssignValuesForMobile()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        canvasOP.referenceResolution = new Vector2(1080f, 2340f);
        Uimanager.IsPcToggle(false);

        BGObject.sprite = PotrateBG;

        Uimanager.Paytable_Button = m_ibutton;
        Uimanager.Sound_Button = m_SoundButton;
        Uimanager.Music_Button = m_MusicButton;
        Uimanager.GameExit_Button = m_backButton;
        Uimanager.SoundOFF_Button = m_SoundOFFButton;
        Uimanager.MusicOFF_Button = m_MusicOFFButton;


        slotManager.Balance_text = m_creditText;
        slotManager.TotalBet_text = m_TotalbetText;
        slotManager.TotalWin_text = m_winText;
        slotManager.TBetPlus_Button = m_plusButton;
        slotManager.TBetMinus_Button = m_minusButton;

        for (int i = 0; i < m_MiniMajorMinor.Length; i++)
        {
            slotManager.MiniMajorMinor[i] = null;
            slotManager.MiniMajorMinor[i] = m_MiniMajorMinor[i];

        }

        FreeSpinCount.transform.position = MobileFreeSpinCountpos.position;
        BonusSpinCount.transform.position = MobileFreeSpinCountpos.position;

        SunHitPoint.transform.position = MobileSunHitPoint.position;

        MainSlot.transform.localPosition = new Vector3(MainSlot.transform.localPosition.x, MainSlot.transform.localPosition.y + 100f, MainSlot.transform.localPosition.z);
        BonusSlot.transform.localPosition = new Vector3(BonusSlot.transform.localPosition.x, BonusSlot.transform.localPosition.y + 100f, BonusSlot.transform.localPosition.z);

        Uimanager.UpdateThebuttons();
        slotManager.assignbuttons();
    }
}