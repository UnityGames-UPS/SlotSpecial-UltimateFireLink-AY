using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FireResultSlot : MonoBehaviour
{
   
    [SerializeField] internal Image ShowImage;
    [SerializeField] private Image UPImage;
    [SerializeField] private Image DownImage;
    [SerializeField] internal TMP_Text NumText;
    [SerializeField] internal ImageAnimation SideburnAnimation;

    [SerializeField] internal bool isFixed = false;
    [SerializeField] internal GameObject Slot;
    [SerializeField] internal GameObject Filter;
    private Tween alltweens;

    [SerializeField] private Image[] RestOftheimages;
    private void Start()
    {
       
       // InitializeTweening(Slot.transform);
    }

    internal IEnumerator ApplyFilter(bool isTrue)
    {

        if(isTrue) Filter.SetActive(isTrue);

        else
        {
           if(alltweens !=null) yield return alltweens.WaitForCompletion();
            Filter.SetActive(isTrue);

        }
    }

    internal void ChangeData(Sprite imgToShow,Sprite imgUP,Sprite imgDown, double num =0)
    {
        ShowImage.sprite = imgToShow;
        UPImage.sprite = imgUP;
        DownImage.sprite = imgDown;

        if (num > 0)
        {
            NumText.text = num.ToString();
            ShowImage.GetComponent<ImageAnimation>().StartAnimation();
            
        }
        else NumText.text = "";

    }
    internal void isSlotFixed(bool _isFixed,double ScatterValue,Sprite[] sunSprite)
    {
        NumText.gameObject.SetActive(true);
        NumText.text = ScatterValue.ToString();
        ShowImage.transform.localScale *= 2f;

        
            ImageAnimation FixedSlotAnim = ShowImage.gameObject.GetComponent<ImageAnimation>();
            for (int i = 0; i < sunSprite.Length; i++)
            {
                FixedSlotAnim.textureArray.Add(sunSprite[i]);
            }
            FixedSlotAnim.AnimationSpeed = 60f;
        FixedSlotAnim.StartAnimation();
        
        isFixed = _isFixed;
    }

    internal void InitializeTweening(Transform slotTransform)
    {
        if (!isFixed)
        {
            slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
            Tweener tweener = slotTransform.DOLocalMoveY(-700, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.Linear);
            tweener.Play();
            alltweens = tweener;
        }
    }

    internal void SetRandomeImages(Sprite x,Sprite y,Sprite z,Sprite q,Sprite p)
    {
        RestOftheimages[0].sprite = x;
        RestOftheimages[1].sprite = y;
        RestOftheimages[2].sprite = z;
        RestOftheimages[3].sprite = q;
        RestOftheimages[4].sprite = p;
    }

    internal void StopTweening()
    {
        if (!isFixed)
        {
            alltweens.Kill();
            //int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
            Slot.transform.localPosition = new Vector2(Slot.transform.localPosition.x, 100);
            alltweens = Slot.transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.OutBack);
            // yield return null;
        }
        else
        {
            StartCoroutine(ApplyFilter(false));
        }
    }

    internal void ShowRandomeResult(Sprite upImg,Sprite downImg,Sprite showImg)
    {
        UPImage.sprite = upImg;
        DownImage.sprite = downImg;
        ShowImage.sprite = showImg;
    }

    internal void playFrameBurnAnimation()
    {
        SideburnAnimation.StartAnimation();
    }
}
