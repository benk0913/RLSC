using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionGroupUI : MonoBehaviour
{
    public static SelectionGroupUI SelectionInControl;
    SelectionGroupUI previousSelection;

    List<SelectionGroupInstance> instances = new List<SelectionGroupInstance>();

    public SelectionGroupInstance CurrentSelected;
    public Selectable CurrentSelectedSelectable;

    private void Start()
    {
        previousSelection = SelectionInControl;
        SelectionInControl = this;

        RefreshGroup();
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

    public void RefreshGroup()
    {
        Selectable[] selectables = GetComponentsInChildren<Selectable>();
        instances.Clear();
        foreach(Selectable selectable in selectables)
        {
            if (selectable.isActiveAndEnabled)
            {
                instances.Add(new SelectionGroupInstance(selectable));
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
        }

        if (instances.Count > 0)
        {
            SelectionGroupInstance targetInst = instances.Find(x => x.CS == CurrentSelectedSelectable);
            if (targetInst != null)
            {
                Select(instances[instances.IndexOf(targetInst)]);
            }
            else
            {
                Select(instances[0]);
            }
        }
    }

    public void Select(SelectionGroupInstance target)
    {
        if(CurrentSelected != null && CurrentSelectedSelectable != null)
        {
            Image originImg = CurrentSelectedSelectable.GetComponent<Image>();
            originImg.color = new Color(originImg.color.r, originImg.color.g, originImg.color.b, 1f);
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

        StopAllCoroutines();
        StartCoroutine(StrobeSelect());
    }

    IEnumerator StrobeSelect()
    {
        Image img = CurrentSelectedSelectable.GetComponent<Image>();

        while(true)
        {
            while(img.color.a > 0f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - (Time.deltaTime * 4f));
                yield return 0;
            }

            while (img.color.a < 1f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + (Time.deltaTime * 4f));
                yield return 0;
            }
        }
    }

    private void Update()
    {
        if(SelectionInControl != this)
        {
            return;
        }
        
        if (CORE.Instance.IsTyping)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(CurrentSelected.toUp == null)
            {
                return;
            }

            Select(CurrentSelected.toUp);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (CurrentSelected.toDown == null)
            {
                return;
            }

            Select(CurrentSelected.toDown);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (CurrentSelected.toLeft == null)
            {
                return;
            }

            Select(CurrentSelected.toLeft);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CurrentSelected.toRight== null)
            {
                return;
            }

            Select(CurrentSelected.toRight);
        }

        if(Input.GetKeyDown(InputMap.Map["Interact"]) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (CurrentSelectedSelectable.GetType() == typeof(Button))
            {
                ((Button)CurrentSelectedSelectable).onClick.Invoke();
            }
            else if (CurrentSelectedSelectable.GetType() == typeof(TMP_InputField))
            {
                ((TMP_InputField)CurrentSelectedSelectable).Select();
                //TODO Implement text field selection
            }
                
            SelectionHandlerUI selectionHandler = CurrentSelectedSelectable.GetComponent<SelectionHandlerUI>();
            if (selectionHandler != null)
            {
                selectionHandler.OnSelectEvent?.Invoke();
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
