using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    
    [Header("Sprites")]
    [SerializeField]
    internal Sprite[] myImages;  //images taken initially

    [Header("Slot Images")]
    
    [SerializeField]
    private List<SlotImage> images;     //class to store total images
    [SerializeField]
    private List<SlotImage> Tempimages;     //class to store the result matrix
    [SerializeField]
    private List<Slottext> TempText;

    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;
    [SerializeField] private GameObject MaskOverRideParrent;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Line Button Texts")]
    [SerializeField]
    private List<TMP_Text> StaticLine_Texts;

    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button Mobile_SlotStart_Button;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField] private Button P_AutoSpinStop_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    internal Button TBetPlus_Button;
    [SerializeField]
    internal Button TBetMinus_Button;
    [SerializeField] private Button Turbo_Button;
    [SerializeField] private Button Mobile_Turbo_Button;
    [SerializeField] private Button StopSpin_Button;
    [SerializeField] private Button Mobile_StopSpin_Button;
    [SerializeField] private Button M_AutoSpinStop_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    internal Sprite[] Sun_Sprite;
    [SerializeField]
    private Sprite[] FreeSpin_Sprite;
    [SerializeField]
    private Sprite[] Nine_Sprite;
    [SerializeField]
    private Sprite[] A_Sprite;
    [SerializeField]
    private Sprite[] Q_Sprite;
    [SerializeField]
    private Sprite[] k_Sprite;
    [SerializeField]
    private Sprite[] J_Sprite;
    [SerializeField]
    private Sprite[] Ten_Sprite;
    [SerializeField]
    private Sprite[] FortuneCake_Sprite;
    [SerializeField]
    private Sprite[] Dimpsum_Sprite;
    [SerializeField]
    private Sprite[] Laltern_Sprite;
    [SerializeField]
    private Sprite[] kattle_Sprite;
    

    [Header("Miscellaneous UI")]
    [SerializeField]
    internal TMP_Text Balance_text;
    [SerializeField]
    internal TMP_Text TotalBet_text;
    [SerializeField]
    internal TMP_Text LineBet_text;
    [SerializeField]
    internal TMP_Text TotalWin_text;
    [SerializeField]  internal TMP_Text[] MiniMajorMinor;

    [Header("Audio Management")]
    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private UIManager uiManager;

    [Header("BonusGame Popup")]
    [SerializeField]
    private BonusController _bonusManager;

    [Header("Free Spins Board")]
    [SerializeField]
    private GameObject FSBoard_Object;
    [SerializeField]
    private TMP_Text FSnum_text;

    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField]
    private GameObject Image_Prefab;    //icons prefab
    [SerializeField] Sprite[] TurboToggleSprites;
    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField]
    private SocketIOManager SocketManager;

    private Dictionary<Transform, (Transform parent, int siblingIndex)> originalData = new Dictionary<Transform, (Transform, int)>();
    private Coroutine AutoSpinRoutine = null;
    private Coroutine FreeSpinRoutine = null;
    private Coroutine tweenroutine;
    private Tween BalanceTween;
    internal bool IsAutoSpin = false;
    internal bool IsFreeSpin = false;
    private bool InsideFreeSpin = false;
    private bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;
    internal int BetCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    protected int Lines = 50;
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 5;          //number of columns
    private bool StopSpinToggle;
    private float SpinDelay=0.2f;
    internal bool IsTurboOn;
    internal bool WasAutoSpinOn;
    internal bool IsHoldSpin = false;
    private int priviousButtonIndex;

    private void Start()
    {
        IsAutoSpin = false;

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.onClick.RemoveAllListeners();
        if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (TBetPlus_Button) TBetPlus_Button.onClick.RemoveAllListeners();
        if (TBetPlus_Button) TBetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });

        if (TBetMinus_Button) TBetMinus_Button.onClick.RemoveAllListeners();
        if (TBetMinus_Button) TBetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if(StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
        if(StopSpin_Button) StopSpin_Button.onClick.AddListener(()=> {audioController.PlayButtonAudio(); StopSpinToggle=true; StopSpin_Button.gameObject.SetActive(false);});

        if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.onClick.RemoveAllListeners();
        if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; Mobile_StopSpin_Button.gameObject.SetActive(false); });


        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if(Turbo_Button) Turbo_Button.onClick.AddListener(TurboToggle);
        if (Mobile_Turbo_Button) Mobile_Turbo_Button.onClick.RemoveAllListeners();
        if (Mobile_Turbo_Button) Mobile_Turbo_Button.onClick.AddListener(TurboToggle);

        if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.onClick.AddListener(() => { StopAutoSpin(); WasAutoSpinOn = false; });
        if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.onClick.AddListener(() => { StopAutoSpin(); WasAutoSpinOn = false; });

        if (FSBoard_Object) FSBoard_Object.SetActive(false);

        tweenHeight = (15 * IconSizeFactor) - 280;
    }

    void TurboToggle(){
        audioController.PlayButtonAudio();
        if(IsTurboOn){
            IsTurboOn=false;
            Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Turbo_Button.image.sprite=TurboToggleSprites[0];
            Mobile_Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Mobile_Turbo_Button.image.sprite = TurboToggleSprites[0];

        }
        else{
            IsTurboOn=true;
            Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
            Mobile_Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
        }
    }

    
    #region Hold Button To Start Auto Spin
    //Start Auto Spin on Button Hold

    internal void StartSpinRoutine()
    {
        if (!IsSpinning)
        {
            IsHoldSpin = false;
            Invoke("AutoSpinHold", 1.5f);
        }

    }

    internal void StopSpinRoutine()
    {
        CancelInvoke("AutoSpinHold");
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.gameObject.SetActive(false);
            if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.gameObject.SetActive(false);
            //if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    internal void AutoSpinHold()
    {
        Debug.Log("Auto Spin Started");
        IsHoldSpin = true;
        AutoSpin();
    }
    #endregion

    #region Autospin
    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {

            IsAutoSpin = true;
            if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.gameObject.SetActive(true);
            if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

        }
    }

    private void StopAutoSpin()
    {
        audioController.PlayButtonAudio();
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
           
            if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.gameObject.SetActive(false);
            if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
        }
        WasAutoSpinOn=false;
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
       // WasAutoSpinOn = false;
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }
    #endregion

    #region FreeSpin
    internal void FreeSpin(int spins)
    {
        if (!IsFreeSpin)
        {
            if (FSBoard_Object) FSBoard_Object.SetActive(true);
            if (SocketManager.resultData.freeSpins.count >= 0) FSnum_text.text = uiManager.FreeSpinOptionButton[priviousButtonIndex].SpinNumber.ToString();
            IsFreeSpin = true;
            ToggleButtonGrp(false);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        int i = 0;
        while (SocketManager.resultData.freeSpins.count > 0)
        {
            uiManager.FreeSpins--;
            
            StartSlots();
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
            if (SocketManager.resultData.freeSpins.count>=0) FSnum_text.text = SocketManager.resultData.freeSpins.count.ToString();
            i++;
        }


        if (uiManager.FreespinWinAmount > 0)
        {
            uiManager.SetBonusWin(uiManager.FreespinWinAmount);
            yield return new WaitForSeconds(2f);
        }
        

        yield return (uiManager.SlideChangeAnimation(()=> {
            uiManager.CloseBonusWin();
            if (uiManager.FreespinBorder) uiManager.FreespinBorder.SetActive(false);
            if (uiManager.NormalBorder) uiManager.NormalBorder.SetActive(true);
            if (FSBoard_Object) FSBoard_Object.SetActive(false);

        }));
        uiManager.ClosePopup(uiManager.BonusPopup);
        IsFreeSpin = false;
        
        if (WasAutoSpinOn)
        {
            AutoSpin();
        }
        else
        {
            ToggleButtonGrp(true);
        }
    }
    #endregion

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
        }
    }

    #region LinesCalculation
    //Fetch Lines from backend
    internal void FetchLines(string LineVal, int count)
    {
        y_string.Add(count + 1, LineVal);
        StaticLine_Texts[count].text = (count + 1).ToString();
        StaticLine_Objects[count].SetActive(true);
    }

    //Generate Static Lines from button hovers
    internal void GenerateStaticLine(TMP_Text LineID_Text)
    {
        DestroyStaticLine();
        int LineID = 1;
        try
        {
            LineID = int.Parse(LineID_Text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Exception while parsing " + e.Message);
        }
        List<int> y_points = null;
        y_points = y_string[LineID]?.Split(',')?.Select(Int32.Parse)?.ToList();
        PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, true);
    }

    //Destroy Static Lines from button hovers
    internal void DestroyStaticLine()
    {
        PayCalculator.ResetStaticLine();
    }
    #endregion

    private void MaxBet()
    {
       // if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        for (int i = 0; i < MiniMajorMinor.Length; i++)
        {
            MiniMajorMinor[i].text = ((SocketManager.initialData.Bets[BetCounter] * Lines) * SocketManager.initialData.jackpotMultipliers[i]).ToString();
        }

        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            BetCounter++;
            if (BetCounter >= SocketManager.initialData.Bets.Count)
            {
                BetCounter = 0; // Loop back to the first bet
            }
        }
        else
        {
            BetCounter--;
            if (BetCounter < 0)
            {
                BetCounter = SocketManager.initialData.Bets.Count - 1; // Loop to the last bet
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        for (int i = 0; i < MiniMajorMinor.Length; i++)
        {
            MiniMajorMinor[i].text = ((SocketManager.initialData.Bets[BetCounter] * Lines) * SocketManager.initialData.jackpotMultipliers[i]).ToString();
        }
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        //CompareBalance();
    }

    #region InitialFunctions
    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 7);
                Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }

    internal void SetInitialUI()
    {
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString();
        if (TotalWin_text) TotalWin_text.text = "0.000";
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("F3");
        for (int i = 0; i < MiniMajorMinor.Length; i++)
        {
            MiniMajorMinor[i].text = ((SocketManager.initialData.Bets[BetCounter] * Lines) * SocketManager.initialData.jackpotMultipliers[i]).ToString();
        }
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        //_bonusManager.PopulateWheel(SocketManager.bonusdata);                                                               //        change here for bonusController
        CompareBalance();
        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
       
       // if (animScript == null) return;
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {
            case 0:
                for (int i = 0; i < A_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(A_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 1:
                for (int i = 0; i < k_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(k_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 2:
                for (int i = 0; i < Q_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Q_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            
            case 3:
                for (int i = 0; i < J_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(J_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 4:
                for (int i = 0; i < Ten_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ten_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 5:
                for (int i = 0; i < Nine_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Nine_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 6:
                for (int i = 0; i < Dimpsum_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Dimpsum_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 7:
                for (int i = 0; i < FortuneCake_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FortuneCake_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 8:
                for (int i = 0; i < kattle_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(kattle_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 9:
                for (int i = 0; i < kattle_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(kattle_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 10:
                for (int i = 0; i < Laltern_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Laltern_Sprite[i]);
                }
                animScript.AnimationSpeed = 28f;
                break;
            case 11:
                for (int i = 0; i < Sun_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Sun_Sprite[i]);
                }
                animScript.AnimationSpeed = 24f;
                break;
            case 12:
                for (int i = 0; i < FreeSpin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FreeSpin_Sprite[i]);
                }
                animScript.AnimationSpeed = 75f;
                break;
            
        }
    }

    #region SlotSpin
    //starts the spin process
    internal void  StartSlots(bool autoSpin = false)
    {
       
        if (audioController) audioController.PlaySpinButtonAudio();
       

        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }
       
        if (SlotAnimRoutine != null)
        {
            StopCoroutine(SlotAnimRoutine);
            SlotAnimRoutine = null;
        }
        WinningsAnim(false);
        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.interactable = false;
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
       
        PayCalculator.ResetLines();
       
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin) 
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            ToggleButtonGrp(true);
            yield break;
        }
        if (TotalWin_text) TotalWin_text.text = "0.000";
        // if (audioController) audioController.PlayWLAudio("spin");
        CheckSpinAudio = true;

        IsSpinning = true;
        RestoreObjectsToOriginalParents();
        ToggleButtonGrp(false);

        TempList.Clear();
        TempList.TrimExcess();

        if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin){
            StopSpin_Button.gameObject.SetActive(true);
            Mobile_StopSpin_Button.gameObject.SetActive(true);
        }
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }
        if (!IsFreeSpin)
        {
            BalanceDeduction();
        }

       

        SocketManager.AccumulateResult(BetCounter);
        yield return new WaitUntil(() => SocketManager.isResultdone);

        ResetSlotText();
        for (int j = 0; j < SocketManager.resultData.resultSymbols.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.resultSymbols[j];
            for (int i = 0; i < 5; i++)
            {
                if (j < 4)
                {
                    if (Tempimages[j].slotImages[i]) Tempimages[j].slotImages[i].sprite = myImages[resultnum[i]];
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
                }
            }
        }

        if(IsTurboOn ){

           
            StopSpinToggle = true;
        }
        else{
            for(int i=0;i<5;i++)
            {
                yield return new WaitForSeconds(0.1f);
                if(StopSpinToggle){
                    break;
                }
            }
            StopSpin_Button.gameObject.SetActive(false);
            Mobile_StopSpin_Button.gameObject.SetActive(false);
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
        }
        StopSpinToggle=false;

        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();
        if (SocketManager.resultData.scatterValues.Count > 0)
        {
            for (int i = 0; i < SocketManager.resultData.scatterValues.Count; i++)
            {

                MoveObjectsToNewParent(MaskOverRideParrent, Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].gameObject);
            }
        }
        if (SocketManager.playerdata.currentWining>0){
            SpinDelay=1.2f;
        }
        else{
            SpinDelay=0.2f;
        }


        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("F3");
        BalanceTween?.Kill();
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("F3");

        currentBalance = SocketManager.playerdata.Balance;


        if (IsAutoSpin)
        {
            yield return (CheckPayoutLineBackend(SocketManager.resultData.symbolsToEmit));
        }
        else
        {

            StartCoroutine( CheckPayoutLineBackend( SocketManager.resultData.symbolsToEmit));
        }

        CheckPopups = true;

       

        
        if(IsFreeSpin)
        {
            uiManager.FreespinWinAmount += SocketManager.playerdata.currentWining;
            uiManager.FreespinWinTxt.text = uiManager.FreespinWinAmount.ToString("F3");
        }
        
        if (SocketManager.resultData.bonus.isTriggered)
        {
            yield return new WaitForSeconds(1f);
            CheckBonusGame();
        }
        else
        {
            if (!IsFreeSpin)
            {
                CheckWinPopups();
            }
            else
            {
                CheckPopups = false;
            }
            
        }
       
        
        yield return new WaitUntil(() => !CheckPopups);
       
        
        
        if (SocketManager.resultData.freeSpins.isTriggered)
        {
           
            if(IsFreeSpin)
            {
                IsFreeSpin = false;
                if (FreeSpinRoutine != null)
                {
                    StopCoroutine(FreeSpinRoutine);
                    FreeSpinRoutine = null;
                }
                yield return new WaitForSeconds(2f);
                InsideFreeSpin = true;
               yield return( uiManager.ShowFreeSpinStartScreen((int)SocketManager.resultData.freeSpins.count, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer1, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer2, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer3));
              //  FreeSpinOptionSelected(priviousButtonIndex,0,0,0);                
            }
            yield return new WaitForSeconds(0.6f);
            if (IsAutoSpin)
            {
                WasAutoSpinOn = true;
            }
            if(!InsideFreeSpin) uiManager.FreeSpinProcess((int)SocketManager.resultData.freeSpins.count);
            InsideFreeSpin = false;
            if (IsAutoSpin)
            {
               
                StopAutoSpin();
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        if (!IsAutoSpin && !IsFreeSpin)
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            
            IsSpinning = false;
        }

    }

    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }
        double initAmount = balance;

        balance = balance - bet;

        BalanceTween=DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("F3");
        });
    }

    internal void CheckWinPopups()
    {
      //  Debug.Log("dev_test" + SocketManager.playerdata.currentWining+ "   0   " + currentTotalBet);
        if (SocketManager.playerdata.currentWining >= currentTotalBet * 10 )
        {
            uiManager.PopulateWin(1,SocketManager.playerdata.currentWining);
            //Debug.Log("dev_test" + "0");
        }
        //else if (SocketManager.resultData.WinAmout >= currentTotalBet * 15 &&SocketManager.playerdata.currentWining < currentTotalBet * 20)
        //{
        //    uiManager.PopulateWin(2,SocketManager.playerdata.currentWining);
        //    Debug.Log("dev_test" + "1");
        //}
        //else if (SocketManager.resultData.WinAmout >= currentTotalBet * 20)
        //{
        //    uiManager.PopulateWin(3,SocketManager.playerdata.currentWining);
        //    Debug.Log("dev_test" + "2");
        //}
        else
        {
            CheckPopups = false;
           
        }
    }

    internal void CheckBonusGame()
    {
        if (audioController) audioController.PlayWLAudio("bonusStart");
        StartCoroutine( _bonusManager.BonusStartupPage(SocketManager.resultData.bonus.spinCount));                                                         // startBonus

       
    }
    private void MoveObjectsToNewParent(GameObject newParent, GameObject obj)
    {
        if (obj != null)
        {
            Transform objTransform = obj.transform;
            originalData[objTransform] = (objTransform.parent, objTransform.GetSiblingIndex());
            //  Vector3 pos = obj.transform.position;
            objTransform.SetParent(newParent.transform, true);

        }

    }
    private void RestoreObjectsToOriginalParents()
    {
        foreach (var entry in originalData)
        {
            Transform objTransform = entry.Key;
            Transform originalParent = entry.Value.parent;
            int originalIndex = entry.Value.siblingIndex;

            if (objTransform != null && originalParent != null)
            {
                objTransform.SetParent(originalParent,true); 
                objTransform.SetSiblingIndex(originalIndex); 
            }
        }

        originalData.Clear(); 
    }
    private void ResetSlotText()
    {
        
        foreach (var temp in Tempimages)
        {
            foreach (var txt in temp.slotImages)
            {
                
                txt.transform.localScale = Vector3.one;
                txt.transform.GetChild(0).gameObject.SetActive(false);
                txt.transform.GetChild(0).localScale = Vector3.one;
            }
        }
        if (SocketManager.resultData.scatterValues.Count > 0)
        {

            for (int i = 0; i < SocketManager.resultData.scatterValues.Count; i++)
            {
               
                Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].transform.localScale *= 2;
                Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].gameObject.GetComponentInChildren<TMP_Text>().text = SocketManager.resultData.scatterValues[i].value.ToString();
                Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].transform.GetChild(0).localScale /= 1.5f;


                ImageAnimation FixedSlotAnim = Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].gameObject.GetComponent<ImageAnimation>();
                FixedSlotAnim.textureArray.Clear();
                FixedSlotAnim.textureArray.TrimExcess();
                for (int j = 0; j < Sun_Sprite.Length; j++)
                {
                    FixedSlotAnim.textureArray.Add(Sun_Sprite[j]);
                }
                FixedSlotAnim.AnimationSpeed = 40f;
                TempList.Add(FixedSlotAnim);
                FixedSlotAnim.StopAnimation();
                FixedSlotAnim.StartAnimation();

                //Tempimages[SocketManager.resultData.scatterValues[i].index[0]].slotImages[SocketManager.resultData.scatterValues[i].index[1]].material = MaskOverRide;
            }
        }
    }
    IEnumerator  CheckPayoutLineBackend( List<List<string>> points_AnimString)
    {
        
        List<int> points_anim = null;
      //  List<int> scatter_anim = null;
        
        if ( points_AnimString.Count > 0)
        {

            for (int i = 0; i < points_AnimString.Count; i++)
            {


                for (int k = 0; k < points_AnimString[i].Count; k++)

                {
                    points_anim = points_AnimString[i][k]?.Split(',')?.Select(Int32.Parse)?.ToList();


                    StartGameAnimation(Tempimages[points_anim[1]].slotImages[points_anim[0]].gameObject);

                }
                yield return new WaitForSeconds(1);
                StopGameAnimation(false);
            }
          
            WinningsAnim(true);
        }
        else
        {

            //if (audioController) audioController.PlayWLAudio("lose");
            if (audioController) audioController.StopWLAaudio();
        }
        CheckSpinAudio = false;
    }

    private Coroutine SlotAnimRoutine = null;
    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
          //  WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
           // WinTween.Kill();
           // TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    #endregion



    internal IEnumerator FreeSpinOptionSelected(int buttonIndex)                                                                  //                 freespinOptionCall
    {
        if (audioController) audioController.PlayButtonAudio();
        if (buttonIndex < 5)
        {
            SocketManager.AcumulateFreeSpin(buttonIndex);
            priviousButtonIndex = buttonIndex;
        }
        else
        {
            int x = UnityEngine.Random.Range(0, 5);
            if(buttonIndex != 8) SocketManager.AcumulateFreeSpin(x);
            priviousButtonIndex = x;
        }
        yield return (SocketManager.isResultdone);
            uiManager.StartFirstFreeSpin(uiManager.FreeSpinOptionButton[priviousButtonIndex].SpinNumber, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer1, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer2, uiManager.FreeSpinOptionButton[priviousButtonIndex].multiplyer3);
    }



    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }


    void ToggleButtonGrp(bool toggle)
    {
        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (Mobile_SlotStart_Button) Mobile_SlotStart_Button.interactable = toggle;
        //if (P_AutoSpinStop_Button) P_AutoSpinStop_Button.interactable = toggle;
        //if (Mobile_StopSpin_Button) Mobile_StopSpin_Button.interactable = toggle;
       // if (StopSpin_Button) StopSpin_Button.interactable = toggle;
        //if (M_AutoSpinStop_Button) M_AutoSpinStop_Button.interactable = toggle;




        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
        if (TBetMinus_Button) TBetMinus_Button.interactable = toggle;
        if (TBetPlus_Button) TBetPlus_Button.interactable = toggle;
        // if(Turbo_Button) Turbo_Button.interactable = toggle;
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        
        temp.StartAnimation();
        TempList.Add(temp);
    }

    //stop the icons animation
    private void StopGameAnimation(bool WithSun = true)
    {
        if (WithSun)
        {
            for (int i = 0; i < TempList.Count; i++)
            {
                TempList[i].StopAnimation();
            }
            TempList.Clear();
            TempList.TrimExcess();
        }
        else
        {
            for (int i = 0; i < TempList.Count; i++)
            {
                GameObject obj = TempList[i].gameObject;
                if(obj.transform.GetChild(0).gameObject.activeInHierarchy == false)
                {
                    TempList[i].StopAnimation();
                }               
            }
        }
        
    }


    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }



    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
    {
        alltweens[index].Kill();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, -100);
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos +250f, 0.3f).SetEase(Ease.OutQuad);
        audioController.PlayWLAudio("spinStop");
        if (!isStop){
            yield return new WaitForSeconds(0.2f);
        }
        else{
            yield return null;
        }
       
        
    }
   

    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class Slottext
{
    public List<TMP_Text> slotImages = new List<TMP_Text>(10);
}

