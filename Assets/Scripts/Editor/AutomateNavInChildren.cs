using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.UI;

class AutomateNavInChildren
{
    [MenuItem("Tools/Automate UI Nav In Children")]
    public void AutoNavInChildren()
    {
        Transform origin = ((GameObject)Selection.objects[0]).transform;

        Button[] buttons = origin.GetComponentsInChildren<Button>();

        foreach(Button button in buttons)
        {
            float closestDistanceLeft = Mathf.Infinity;
            float closestDistanceRight = Mathf.Infinity;
            float closestDistanceUp = Mathf.Infinity;
            float closestDistanceDown = Mathf.Infinity;

            Button closestButtonLeft = null;
            Button closestButtonRight = null;
            Button closestButtonUp = null;
            Button closestButtonDown = null;

            foreach (Button otherBtn in buttons)
            {
                float distLeft = otherBtn.transform.position.x - button.transform.position.x;
                float distDown = otherBtn.transform.position.y - button.transform.position.y;

                if(distLeft > 0) // other Is from my right
                {
                    if(distLeft < closestDistanceRight)
                    {
                        closestDistanceRight = distLeft;
                        closestButtonRight = otherBtn;
                    }
                }
                else
                {
                    distLeft *= -1f;

                    if (distLeft < closestDistanceLeft)
                    {
                        closestDistanceLeft = distLeft;
                        closestButtonLeft = otherBtn;
                    }
                }


                if (distDown > 0) // other Is from my Up
                {
                    if (distDown < closestDistanceUp)
                    {
                        closestDistanceUp = distDown;
                        closestButtonUp = otherBtn;
                    }
                }
                else
                {
                    distDown *= -1f;

                    if (distDown < closestDistanceDown)
                    {
                        closestDistanceDown = distDown;
                        closestButtonDown = otherBtn;
                    }
                }


            }

            Navigation nav = button.navigation;

            nav.selectOnDown = closestButtonDown;
            nav.selectOnUp = closestButtonUp;
            nav.selectOnLeft = closestButtonLeft;
            nav.selectOnRight = closestButtonRight;

            button.navigation = nav;
        }
    }

}