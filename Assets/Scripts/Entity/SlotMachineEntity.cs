using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using TMPro;
using UnityEngine;

public class SlotMachineEntity : MonoBehaviour
{
    public static SlotMachineEntity Instance;

    [SerializeField]
    SpriteRenderer SlotA;
    [SerializeField]
    SpriteRenderer SlotB;
    [SerializeField]
    SpriteRenderer SlotC;

    [SerializeField]
    TextMeshProUGUI PriceField;

    [SerializeField]
    TextMeshProUGUI CurrentGold;


    public Animator Anim;

    public List<Sprite> Options = new List<Sprite>();
    public List<Sprite> Blurs = new List<Sprite>();

    public string SpinSound;

    public string slotHitSound;

    public string SlotWinSound;

    public string SlotRewardSound;

    public string SlotSlimeSound;

    public string SlotWinBigSound;

    [SerializeField]
    public bool Busy;

    public bool DEBUG = false;

    void Awake()
    {
        Instance = this;
        PriceField.text = "x" + CORE.Instance.Data.content.Slots.SlotMachinePrice;
    }

    void Update()
    {
        CurrentGold.text = System.String.Format("{0:n0}", CORE.PlayerActor.money);
    }

    public void AttemptSlot()
    {
        if (CORE.PlayerActor.money < CORE.Instance.Data.content.Slots.SlotMachinePrice)
        {
            return;
        }

        if (SpinRoutineInstance != null)
        {
            return;
        }

        Busy = true;
        SocketHandler.Instance.SendEvent("used_slot_machine");

        if (DEBUG)
        {
            Spin((WinType)Random.Range(0, 3));
        }
    }


    public void Spin(WinType type)
    {
        SpinRoutineInstance = StartCoroutine(SpinRoutine(type));
    }

    Coroutine SpinRoutineInstance;
    IEnumerator SpinRoutine(WinType type) //fornow result is hardcoded = 0-3 win, 4 - slime, 5 - reward, 6-big win, 7 lose
    {
        
            if(!CORE.IsMachinemaMode )
            {
                HitLabelEntityUI label = ResourcesLoader.Instance.GetRecycledObject("HitLabelEntityAlly").GetComponent<HitLabelEntityUI>();

                label.SetLabel("Spin: "+CORE.Instance.Data.content.Slots.SlotMachinePrice, Color.yellow);

                label.transform.position = transform.position;
                
            }

        AudioControl.Instance.PlayInPosition(SpinSound, transform.position);
        Anim.SetTrigger("Spin");
        int SpinCount = Random.Range(5, 15);
        for (int i = 0; i < SpinCount; i++)
        {
            SlotA.sprite = Blurs[Random.Range(0, Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
            SlotB.sprite = Blurs[Random.Range(0, Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
            SlotC.sprite = Blurs[Random.Range(0, Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
        }

        int result = (int)type;
        if (type == WinType.Lose)
        {
            List<Sprite> trueOptions = new List<Sprite>();
            trueOptions.AddRange(Options);

            SlotA.sprite = trueOptions[Random.Range(0, trueOptions.Count)];
            trueOptions.Remove(SlotA.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotB.sprite = trueOptions[Random.Range(0, trueOptions.Count)];
            trueOptions.Remove(SlotB.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotC.sprite = trueOptions[Random.Range(0, trueOptions.Count)];
            trueOptions.Remove(SlotC.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

        }
        else if (type == WinType.Reward)
        {
            

            SlotA.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotB.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotC.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 00.5f));

            AudioControl.Instance.PlayInPosition(SlotRewardSound, transform.position);
        }
        else if (type == WinType.Slime)
        {

            SlotA.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotB.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotC.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 00.5f));

            AudioControl.Instance.PlayInPosition(SlotSlimeSound, transform.position);
        }
        else if (type == WinType.Win1 || type == WinType.Win2 ||type == WinType.Win3 ||  type == WinType.Win4)
        {

            SlotA.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotB.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.05f));

            SlotC.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 00.5f));

            AudioControl.Instance.PlayInPosition(SlotWinSound, transform.position);
        }
        else if (type == WinType.WinBig)
        {


            SlotA.sprite = Options[5];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            SlotB.sprite = Options[5];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            SlotC.sprite = Options[5];
            AudioControl.Instance.PlayInPosition(slotHitSound, transform.position);

            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            AudioControl.Instance.PlayInPosition(SlotWinBigSound, transform.position);
        }

                
        if ((int)type < CORE.Instance.Data.content.Slots.Rewards.Count) {
            AbilityParam Reward = CORE.Instance.Data.content.Slots.Rewards[(int)type];


            if(!CORE.IsMachinemaMode)
            {
                HitLabelEntityUI label = ResourcesLoader.Instance.GetRecycledObject("HitLabelEntityAlly").GetComponent<HitLabelEntityUI>();

                if(Reward.Type == null)
                {
                    label.SetLabel("--", Color.gray);
                }
                if(Reward.Type.name == "Gain Money")
                {
                    label.SetLabel("Earned: "+Reward.Value, Color.yellow);
                }
                else if(Reward.Type.name == "Spawn")
                {
                    label.SetLabel("SLIME!", Color.red);
                }
                else
                {
                    label.SetLabel("REWARD!", Color.green);
                }

                label.transform.position = transform.position;
                
            }
            
        }

        SpinRoutineInstance = null;
    }
    public enum WinType
    {
        Win1,Win2,Win3,Win4, Slime,Reward,WinBig, Lose
    }
}

