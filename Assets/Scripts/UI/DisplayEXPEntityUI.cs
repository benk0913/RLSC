using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEXPEntityUI : MonoBehaviour
{
    public static DisplayEXPEntityUI Instance;

    [SerializeField]
    Image FillImage;

    [SerializeField]
    Image ConstantFillImage;

    [SerializeField]
    TextMeshProUGUI ExpValueText;

    [SerializeField]
    TextMeshProUGUI GainEXPText;

    [SerializeField]
    TextMeshProUGUI LevelLabel;

    int CurrentExp;

    [SerializeField]
    CanvasGroup CG;


    public List<DisplayExpInstance> Que = new List<DisplayExpInstance>();

    Coroutine ShowRoutineInstance;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Init()
    {
        CurrentExp = CORE.Instance.Room.PlayerActor.exp;

        int lvlExpSnapshot = getLvlExpSnapshot();
        float expPercent = (float)CurrentExp / (float)lvlExpSnapshot;

        ExpValueText.text = Mathf.RoundToInt(expPercent * 100f) + "%";
        FillImage.fillAmount = expPercent;

        LevelLabel.text = CORE.Instance.Room.PlayerActor.level.ToString();

        ConstantFillImage.fillAmount = expPercent;
    }

    private int getLvlExpSnapshot()
    {
        bool isMaxLevel = CORE.Instance.Room.PlayerActor.level >= CORE.Instance.Data.content.MaxLevel;
        
        int lvlExpSnapshot = isMaxLevel ? 1 : CORE.Instance.Data.content.ExpChart[CORE.Instance.Room.PlayerActor.level];
        return lvlExpSnapshot;
    }

    public void Show(int currentEXP)
    {
        int lvlExpSnapshot = getLvlExpSnapshot();

        DisplayExpInstance instance = new DisplayExpInstance(currentEXP, lvlExpSnapshot);

        Show(instance);

    }

    public void Show(DisplayExpInstance instance)
    {
        if(ShowRoutineInstance != null)
        {
            Que.Add(instance);
            return;
        }

        this.gameObject.SetActive(true);

        ShowRoutineInstance = StartCoroutine(ShowRoutine(instance));
    }

    void ContinueQue()
    {
        if(Que.Count == 0)
        {
            ShowRoutineInstance = null;
            Hide();
            return;
        }

        DisplayExpInstance instance = Que[0];
        Que.RemoveAt(0);

        Show(instance);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator ShowRoutine(DisplayExpInstance instance)
    {
        GainEXPText.text = "";
        

        while (CG.alpha < 1f)
        {
            CG.alpha += 1f * Time.deltaTime;
            yield return 0;
        }

        if(instance.CurrentEXP < CurrentExp)
        {
            while(FillImage.fillAmount < 1f)
            {
                FillImage.fillAmount += Time.deltaTime;
                ConstantFillImage.fillAmount += Time.deltaTime;
                yield return 0;
            }

            CurrentExp = 0;
            FillImage.fillAmount = 0f;
            ConstantFillImage.fillAmount = 0f;
            ExpValueText.text = 0 +"%";
        }
        GainEXPText.text = "+" + (instance.CurrentEXP - CurrentExp);

        yield return new WaitForSeconds(1f);

        float t = 0f;
        while(t<1f)
        {
            t += Time.deltaTime;

            CurrentExp = Mathf.RoundToInt(Mathf.Lerp(CurrentExp, instance.CurrentEXP, t));

            GainEXPText.text = "+" + (instance.CurrentEXP - CurrentExp);
            ExpValueText.text = Mathf.RoundToInt(Mathf.Min(100f, ((float)CurrentExp /(float)instance.LevelEXPSnapshot)*100f))+"%";

            FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, ((float)CurrentExp /(float)instance.LevelEXPSnapshot), t);
            ConstantFillImage.fillAmount = FillImage.fillAmount;

            yield return 0;
        }

        GainEXPText.text = "";

        CurrentExp = instance.CurrentEXP;
        LevelLabel.text = CORE.Instance.Room.PlayerActor.level.ToString();


        if (Que.Count <= 0)
        {
            yield return new WaitForSeconds(1f);

            while (CG.alpha > 0f)
            {
                CG.alpha -= 1f * Time.deltaTime;
                yield return 0;
            }
        }

        ShowRoutineInstance = null;

        GainEXPText.text = "";

        ContinueQue();
    }

    public class DisplayExpInstance
    {
        public int CurrentEXP;
        public int LevelEXPSnapshot;

        public DisplayExpInstance(int currentXP, int lvlXpSnap)
        {
            this.CurrentEXP = currentXP;
            this.LevelEXPSnapshot = lvlXpSnap;
        }
    }
}
