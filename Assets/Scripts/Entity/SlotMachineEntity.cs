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

    public Animator Anim;

    public List<Sprite> Options = new List<Sprite>();
    public List<Sprite> Blurs = new List<Sprite>();

    public string SpinSound;

    public string slotHitSound;

    public string SlotWinSound;

    public string SlotWinBigSound;

    [SerializeField]
    public bool Busy;

    public bool DEBUG = false;

    void Awake()
    {
        Instance = this;
        PriceField.text = "x"+ CORE.Instance.Data.content.Slots.SlotMachinePrice;
    }

    public void AttemptSlot()
    {
        if(CORE.PlayerActor.money < CORE.Instance.Data.content.Slots.SlotMachinePrice)
        {
            return;
        }

        if(SpinRoutineInstance != null)
        {
            return;
        }

        Busy = true;
        SocketHandler.Instance.SendEvent("used_slot_machine");

        if(DEBUG)
        {
            Spin((WinType)Random.Range(0,3));
        }
    }


    public void Spin(WinType type)
    {
        SpinRoutineInstance = StartCoroutine(SpinRoutine(type));
    }

    Coroutine SpinRoutineInstance;
    IEnumerator SpinRoutine(WinType type)
    {
        AudioControl.Instance.PlayInPosition(SpinSound,transform.position);
        Anim.SetTrigger("Spin");
        int SpinCount = Random.Range(3,10);
        for(int i=0;i<SpinCount;i++)
        {
            SlotA.sprite = Blurs[Random.Range(0,Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f,0.05f));
            SlotB.sprite = Blurs[Random.Range(0,Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f,0.05f));
            SlotC.sprite = Blurs[Random.Range(0,Blurs.Count)];
            yield return new WaitForSeconds(Random.Range(0f,0.05f));
        }

        if(type == WinType.Lose)
        {
            List<Sprite> trueOptions = new List<Sprite>();
            trueOptions.AddRange(Options);

            SlotA.sprite = trueOptions[Random.Range(0,trueOptions.Count)];
            trueOptions.Remove(SlotA.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.05f));

            SlotB.sprite = trueOptions[Random.Range(0,trueOptions.Count)];
            trueOptions.Remove(SlotB.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.05f));

            SlotC.sprite = trueOptions[Random.Range(0,trueOptions.Count)];
            trueOptions.Remove(SlotC.sprite);
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

        }
        else if(type == WinType.Win)
        {
            int result = Random.Range(0,Options.Count-1);

            SlotA.sprite =Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.05f));

            SlotB.sprite =Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.05f));

            SlotC.sprite = Options[result];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,00.5f));

            AudioControl.Instance.PlayInPosition(SlotWinSound,transform.position);
        }
        else if(type == WinType.WinBig)
        {
    

            SlotA.sprite =Options[Options.Count-1];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.5f));

            SlotB.sprite =Options[Options.Count-1];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.5f));

            SlotC.sprite = Options[Options.Count-1];
            AudioControl.Instance.PlayInPosition(slotHitSound,transform.position);

            yield return new WaitForSeconds(Random.Range(0f,0.5f));

            AudioControl.Instance.PlayInPosition(SlotWinBigSound,transform.position);
        }

        SpinRoutineInstance = null;
    }
    public enum WinType
    {
        WinBig, Win, Lose
    }
}

