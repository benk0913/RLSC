using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EdgeworldBase;

public class SelectionGroupUI : MonoBehaviour
{
    public static SelectionGroupUI SelectionInControl;
    SelectionGroupUI previousSelection;

    public List<Selectable> IgnoreSelectables= new List<Selectable>();

    List<SelectionGroupInstance> instances = new List<SelectionGroupInstance>();
    Dictionary<Selectable, SelectionGroupInstance> instancesBySelectable = new Dictionary<Selectable, SelectionGroupInstance>();

    public SelectionGroupInstance CurrentSelected;
    public Selectable CurrentSelectedSelectable;

    public Selectable DefaultSelectable;

    public bool ScrollRectSnapSupport = false;

    public bool CompensateOnEmptySelections = false;

    float joystickPressedDelay;
    const float JOYSTICK_DELAY = 0.1f;

    public bool Debug = false;

    public bool InteractingWithKeyboard;

    private void Start()
    {
        RefreshGroup(false);
    }

    private void OnEnable()
    {
        previousSelection = SelectionInControl;
        SelectionInControl = this;
    }
    
    private void OnDisable()
    {
        if(SelectionInControl == this)
        {
            if (previousSelection != null)
            {
                SelectionInControl = previousSelection;
            }
        }
    }

    private void OnDestroy()
    {
        if (SelectionInControl == this)
        {
            if (previousSelection != null)
            {
                SelectionInControl = previousSelection;
            }
        }
    }

    public void DelayedRefreshGroup(float delay = 0.3f)
    {
        CORE.Instance.DelayedInvokation(delay,()=>RefreshGroup());
    }
    public void RefreshGroup(bool restorePlacement = true)
    {
        Selectable[] selectables = GetComponentsInChildren<Selectable>();
        instances.Clear();
        instancesBySelectable.Clear();
        foreach(Selectable selectable in selectables)
        {
            if (selectable.isActiveAndEnabled)
            {
                if (Debug)
                    CORE.Instance.LogMessage("SelectionGroup - Setting Selectable " + selectable.gameObject.name);

                if( IgnoreSelectables.Contains(selectable))
                {
                    continue;
                }

                if(selectable.GetType() == typeof(Scrollbar))
                {
                    continue;
                }

                if(selectable.GetType() == typeof(Dropdown))
                {
                    continue;
                }

                if(!selectable.interactable)
                {
                    continue;
                }


                SelectionGroupInstance instance = new SelectionGroupInstance(selectable);
                instances.Add(instance);
                instancesBySelectable.Add(selectable, instance);

                Button button = selectable.GetComponent<Button>();
                if(button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        Select(instance,true);
                    });
                }
                TMP_InputField input = selectable.GetComponent<TMP_InputField>();
                if (input != null)
                {
                    input.onSelect.RemoveAllListeners();
                    input.onSelect.AddListener((string value) => {
                        Select(instance,true);
                    });
                }
            }
        }

        foreach(SelectionGroupInstance instance in instances)
        {

            float shortestRight = Mathf.Infinity;
            float shortestLeft = Mathf.Infinity;
            float shortestUp = Mathf.Infinity;
            float shortestDown = Mathf.Infinity;

            foreach (SelectionGroupInstance otherInstance in instances)
            {
                if(otherInstance == instance)
                {
                    continue;
                }

                Vector2 otherAngel = (otherInstance.CS.transform.position - instance.CS.transform.position).normalized;
                float dist = Vector2.Distance(instance.CS.transform.position, otherInstance.CS.transform.position);

                float distToRight = Vector2.Distance(otherAngel, instance.CS.transform.right);
                float distToUp = Vector2.Distance(otherAngel, instance.CS.transform.up);
                float distToDown = Vector2.Distance(otherAngel, -instance.CS.transform.up);
                float distToLeft = Vector2.Distance(otherAngel, -instance.CS.transform.right);

                if(distToRight < distToUp && distToRight < distToDown && distToRight < distToLeft) //ON RIGHT
                {

                    if (dist < shortestRight)
                    {
                        shortestRight = dist;
                        instance.toRight = otherInstance;

                    }
                }
                else if (distToUp < distToRight && distToUp < distToDown && distToUp < distToLeft) //ON UP
                {
                    if (dist < shortestUp)
                    {
                        shortestUp = dist;
                        instance.toUp = otherInstance;
                    }
                }
                else if (distToLeft < distToUp && distToLeft < distToDown && distToLeft < distToRight) //ON LEFT
                {
                    if (dist < shortestLeft)
                    {
                        shortestLeft = dist;
                        instance.toLeft = otherInstance;
                    }
                }
                else if (distToDown < distToUp && distToDown < distToRight && distToDown < distToLeft) //ON DOWN
                {
                    if (dist < shortestDown)
                    {
                        shortestDown = dist;
                        instance.toDown = otherInstance;
                    }
                }
            }


            if (CompensateOnEmptySelections)
            {

                shortestUp = Mathf.Infinity;
                shortestDown = Mathf.Infinity;
                shortestLeft = Mathf.Infinity;
                shortestRight = Mathf.Infinity;

                foreach (SelectionGroupInstance otherInstance in instances)
                {
                    if (otherInstance == instance)
                    {
                        continue;
                    }
                    float dist = Vector2.Distance(instance.CS.transform.position, otherInstance.CS.transform.position);

                    if (otherInstance != instance.toUp && otherInstance != instance.toDown && otherInstance != instance.toLeft && otherInstance != instance.toRight)
                    {
                        if (instance.toUp == null)
                        {
                            if (otherInstance.CS.transform.position.y > instance.CS.transform.position.y && dist < shortestUp) 
                            {
                                shortestUp = dist;
                                instance.toUp = otherInstance;
                            }
                        }
                        else if (instance.toDown == null)
                        {
                            if (otherInstance.CS.transform.position.y < instance.CS.transform.position.y && dist < shortestDown)
                            {
                                shortestDown = dist;
                                instance.toDown = otherInstance;
                            }
                        }
                        else if (instance.toLeft == null)
                        {
                            if (otherInstance.CS.transform.position.x < instance.CS.transform.position.x && dist < shortestLeft)
                            {
                                shortestLeft = dist;
                                instance.toLeft = otherInstance;
                            }
                        }
                        else if (instance.toRight == null)
                        {
                            if (otherInstance.CS.transform.position.x > instance.CS.transform.position.x && dist < shortestRight)
                            {
                                shortestRight = dist;
                                instance.toRight = otherInstance;
                            }
                        }
                    }

                }

            }

            if (Debug)
                CORE.Instance.LogMessage("SelectionGroup - Setting Neighbors " + instance.CS.gameObject.name + " | U "+ instance.toUp + " | D " + instance.toDown+ " | L " + instance.toLeft + " | R " + instance.toRight);
        }

        if (instances.Count > 0)
        {
            SelectionGroupInstance targetInst = instances.Find(x => x.CS == CurrentSelectedSelectable);
            if (restorePlacement && targetInst != null)
            {
                Select(instances[instances.IndexOf(targetInst)]);
            }
            else
            {
                if(DefaultSelectable != null)
                {
                    Select(DefaultSelectable);
                }
                else
                {
                    Select(instances[0]);
                }
            }
        }
    }

    public void Select(SelectionGroupInstance target, bool withMouse = false)
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if(ScrollRectSnapSupport && !withMouse)
        {
            ScrollRect sRect = target.CS.GetComponentInParent<ScrollRect>();
            if (sRect != null)
            {
                Canvas.ForceUpdateCanvases();

                Vector2 targetPos = (Vector2)sRect.transform.InverseTransformPoint(sRect.content.position)
                    - (Vector2)sRect.transform.InverseTransformPoint(target.CS.transform.position);

                sRect.content.anchoredPosition = new Vector2(sRect.content.anchoredPosition.x, targetPos.y);
                
            }
        }

        AudioEntityUIHandle audioEntity;



        if (CurrentSelected != null && CurrentSelectedSelectable != null)
        {
            audioEntity = CurrentSelected.CS.GetComponent<AudioEntityUIHandle>();
            if (audioEntity != null)
            {
                audioEntity.PlaySound(audioEntity.PointerUnhoverSound);
            }

            CanvasGroup canvasGroup = CurrentSelectedSelectable.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
            }
            CurrentSelected = null;

            SelectionHandlerUI selectionExitHandler = CurrentSelectedSelectable.GetComponent<SelectionHandlerUI>();
            if (selectionExitHandler != null)
            {
                selectionExitHandler.OnExitEvent?.Invoke();
            }
        }

        CurrentSelected = target;
        CurrentSelectedSelectable = CurrentSelected.CS;

        SelectionHandlerUI selectionEnterHandler = CurrentSelectedSelectable.GetComponent<SelectionHandlerUI>();
        if (selectionEnterHandler != null)
        {
            selectionEnterHandler.OnEnterEvent?.Invoke();
        }

        audioEntity = CurrentSelected.CS.GetComponent<AudioEntityUIHandle>();
        if (audioEntity != null)
        {
            audioEntity.PlaySound(audioEntity.PointerHoverSound);
        }

        StopAllCoroutines();
        StartCoroutine(StrobeSelect());
    }

    public void Select(Selectable selectable)
    {
        if(!instancesBySelectable.ContainsKey(selectable))
        {
            SelectionGroupInstance inst = instances.Find(x=>x.CS == selectable);
            if(inst == null)
            {
                return;
            }

            Select(inst,false);
            return;
        }
        Select(instancesBySelectable[selectable]);
    }
    
    IEnumerator StrobeSelect()
    {
        CanvasGroup canvasGroup = CurrentSelectedSelectable.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = CurrentSelectedSelectable.gameObject.AddComponent<CanvasGroup>();   
        }

        while(true)
        {
            if(!InteractingWithKeyboard)
            {
                yield return 0;
                continue;
            }

            while (CORE.Instance.IsTyping)
            {
                canvasGroup.alpha = 1;
                yield return 0;
            }

            while(canvasGroup.alpha > 0.4f)
            {
                canvasGroup.alpha -= (Time.deltaTime * 1.5f);
                yield return 0;
            }

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += (Time.deltaTime * 1.5f);
                yield return 0;
            }
        }
    }
    
    

    private void Update()
    {

        if (SelectionInControl != this)
        {
            return;
        }
        
        if (CORE.Instance.IsTyping || CORE.Instance.IsLoading || ResourcesLoader.Instance.LoadingWindowObject.activeInHierarchy)
        {
            return;
        }

            
        if (Input.GetKeyDown(InputMap.Map["Move Up"]) || Input.GetKeyDown(InputMap.Map["Secondary Move Up"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") > 0 && joystickPressedDelay <= 0f))
        {
            if (CurrentSelected.toUp == null)
            {
                if (Debug)
                    CORE.Instance.LogMessage("SelectionGroup -" + this.gameObject.name + " No 'Above'");
                return;
            }

            InteractingWithKeyboard = true;
            Select(CurrentSelected.toUp);
            joystickPressedDelay = JOYSTICK_DELAY;
        }
        else if (Input.GetKeyDown(InputMap.Map["Move Down"]) || Input.GetKeyDown(InputMap.Map["Secondary Move Down"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") < 0 && joystickPressedDelay <= 0f))
        {
            if (CurrentSelected.toDown == null)
            {
                if (Debug)
                    CORE.Instance.LogMessage("SelectionGroup -" + this.gameObject.name + " No 'Below'");
                return;
            }

            InteractingWithKeyboard = true;
            Select(CurrentSelected.toDown);
            joystickPressedDelay = JOYSTICK_DELAY;
        }
        else if (Input.GetKeyDown(InputMap.Map["Move Left"]) || Input.GetKeyDown(InputMap.Map["Secondary Move Left"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") < 0 && joystickPressedDelay <= 0f))
        {
            if (CurrentSelected.toLeft == null)
            {
                if (Debug)
                    CORE.Instance.LogMessage("SelectionGroup -" + this.gameObject.name + " No 'To Left'");
                return;
            }

            InteractingWithKeyboard = true;
            Select(CurrentSelected.toLeft);
            joystickPressedDelay = JOYSTICK_DELAY;
        }
        else if (Input.GetKeyDown(InputMap.Map["Move Right"]) || Input.GetKeyDown(InputMap.Map["Secondary Move Right"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") > 0 && joystickPressedDelay <= 0f))
        {
            if (CurrentSelected.toRight == null)
            {
                if (Debug)
                    CORE.Instance.LogMessage("SelectionGroup -" + this.gameObject.name + " No 'To Right'");
                return;
            }
            InteractingWithKeyboard = true;
            Select(CurrentSelected.toRight);
            joystickPressedDelay = JOYSTICK_DELAY;
        }
        else if((CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f && joystickPressedDelay > 0f))
        {

            joystickPressedDelay -= Time.deltaTime;
        }

        if (Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetKeyDown(InputMap.Map["Confirm"]) || Input.GetButtonDown("Joystick 0"))
        {
            InteractingWithKeyboard = true;
            if (CurrentSelectedSelectable.interactable == false)
            {
                return;
            }

            if (CurrentSelected != null && CurrentSelected.CS != null)
            {
                AudioEntityUIHandle audioEntity = CurrentSelected.CS.GetComponent<AudioEntityUIHandle>();
                if (audioEntity != null)
                {
                    audioEntity.PlaySound(audioEntity.PointerDownSound);
                    CORE.Instance.DelayedInvokation(0.1f, ()=>audioEntity.PlaySound(audioEntity.PointerUpSound));
                }
            }

            if (CurrentSelectedSelectable.GetType() == typeof(Button))
            {
                ((Button)CurrentSelectedSelectable).onClick.Invoke();
            }
            else if (CurrentSelectedSelectable.GetType() == typeof(TMP_InputField))
            {
                if (CORE.Instance.IsUsingJoystick)
                {
                    if (!CORE.Instance.IsUsingJoystick)
                    {
                        return;
                    }
                    
                    EventSystem.current.SetSelectedGameObject(null);
                    VirtualKeyboard.VirtualKeyboard.Instance.Show((TMP_InputField)CurrentSelectedSelectable);
                    
                }
                else
                {
                    if (EventSystem.current.currentSelectedGameObject == CurrentSelectedSelectable.gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                    else
                    {
                        ((TMP_InputField)CurrentSelectedSelectable).Select();
                    }
                }
            }


            DoubleclickHandlerUI doubleclickHandler = CurrentSelectedSelectable.GetComponent<DoubleclickHandlerUI>();
            if (doubleclickHandler != null)
            {
                doubleclickHandler.OnPointerClick();
            }
        }
    }



    [Serializable]
    public class SelectionGroupInstance
    {
        public Selectable CS;

        [NonSerialized]
        public SelectionGroupInstance toRight;

        [NonSerialized]
        public SelectionGroupInstance toUp;

        [NonSerialized]
        public SelectionGroupInstance toDown;

        [NonSerialized]
        public SelectionGroupInstance toLeft;

        public SelectionGroupInstance(Selectable selectable)
        {
            this.CS = selectable;
        }
    }
}
