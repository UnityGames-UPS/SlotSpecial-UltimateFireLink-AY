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



    [Space]
    [Space]
    [SerializeField]
    private CanvasScaler canvasOP;

    [SerializeField]
    private UIManager Uimanager;

    [SerializeField]
    private SlotBehaviour slotManager;

    [Header("BG Change")]
    [SerializeField] private Sprite LandscapeBG;
    [SerializeField] private Sprite PotrateBG;
    [SerializeField] private Image BGObject;

    [Header("UI Elements")]
    [SerializeField] private Button m_ibutton;
    [SerializeField] private Button p_ibutton;
    [SerializeField] private Button m_MusicButton;
    [SerializeField] private Button p_MusicButton;
    [SerializeField] private Button m_SoundButton;
    [SerializeField] private Button p_SoundButton;
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



    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Calls the JavaScript function 'isMobile()' from Unity
        Debug.Log("Dev_Test:"+"DeviceCheck----------------------");
        Application.ExternalCall("isMobile");
#endif
    }
    private void Start()
    {
        if(isTesting)
        {
            if (isPC) AssignValuesForPC();
            else AssignValuesForMobile();
        }
    }

    // This method will be called from the JavaScript side
    public void OnMobileDeviceDetected(string s)
    {
        Debug.Log("Called OnMobileDeviceDetected");
        if (s == "A")
        {
            
            AssignValuesForMobile();
            Debug.Log("Dev_Test:"+"This is a mobile device.-----------------------------------------");
        }
        else
        {
           
            AssignValuesForPC();
            Debug.Log("Dev_Test:" + "This is a PC device.--------------------------------------------");
        }
    }

    public void AssignValuesForPC()
    {
        canvasOP.referenceResolution = new Vector2(2340f, 1080f);
        Uimanager.IsPcToggle(true);

        BGObject.sprite = LandscapeBG;

        Uimanager.Paytable_Button = p_ibutton;
        Uimanager.Sound_Button = p_SoundButton;
        Uimanager.Music_Button = p_MusicButton;
        Uimanager.Exit_Button = p_backButton;

       // slotManager.Balance_text = p_creditText;
       // slotManager.TotalBet_text = p_TotalbetText;
       // slotManager.TotalWin_text = p_winText;
       // slotManager.TBetPlus_Button = p_plusButton;
       // slotManager.TBetMinus_Button = p_minusButton;

    }

    public void AssignValuesForMobile()
    {
        canvasOP.referenceResolution = new Vector2(1080f, 2340f);
        Uimanager.IsPcToggle(false);

        BGObject.sprite = PotrateBG;

        Uimanager.Paytable_Button = m_ibutton;
        Uimanager.Sound_Button = m_SoundButton;
        Uimanager.Music_Button = m_MusicButton;
        Uimanager.Exit_Button = m_backButton;

        //slotManager.Balance_text = m_creditText;
        //slotManager.TotalBet_text = m_TotalbetText;
        //slotManager.TotalWin_text = m_winText;
        //slotManager.TBetPlus_Button = m_plusButton;
        //slotManager.TBetMinus_Button = m_minusButton;

    }
}