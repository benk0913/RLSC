// *****************************************************************
// Class: AnimationHelper.cs
// Author: ngan.do	
// Date: 4/2/2019	
// Description: Use animation helper like utility for curve path animation
// *******************************************************************

using UnityEngine;
using System;
using System.Collections;

namespace VirtualKeyboard
{
    public enum AnimationCurveType
    {
        linear,
        easeInEaseOut,
        easeIn,
        easeOut,
        ring,
        bounce,
        overshoot
    }

    public class AnimationHelper : SingletonBehavior<AnimationHelper>

    {
        public AnimationCurve linear;
        public AnimationCurve easeInEaseOut;
        public AnimationCurve easeIn;
        public AnimationCurve easeOut;
        public AnimationCurve bounce;
        public AnimationCurve overshoot;
        public AnimationCurve ring;

        void Awake()
        {
            if (FindObjectsOfType(typeof(AnimationHelper)).Length > 1)
                Destroy(gameObject);
        }

        public AnimationCurve curveWithType(AnimationCurveType theType)
        {
            if (theType == AnimationCurveType.bounce) return bounce;
            if (theType == AnimationCurveType.linear) return linear;
            if (theType == AnimationCurveType.ring) return ring;
            if (theType == AnimationCurveType.easeIn) return easeIn;
            if (theType == AnimationCurveType.easeOut) return easeOut;
            if (theType == AnimationCurveType.overshoot) return overshoot;
            return easeInEaseOut; // default
        }

        public static Coroutine lerpMe(float time, Action<float> lerpAction)
        {
            return AnimationHelper.lerpMe(null, time, lerpAction, null);
        }

        public static Coroutine lerpMe(float time, Action<float> lerpAction, Action completion)
        {
            return AnimationHelper.lerpMe(null, time, lerpAction, completion);
        }

        // this how it should have looked
        public static Coroutine lerpMe(MonoBehaviour script, float time, AnimationCurve shape, Action<float> lerpAction, Action completion)
        {
            return AnimationHelper.lerpMe(script, time, lerpAction, completion, shape);
        }


        public static Coroutine lerpMe(MonoBehaviour script, float time, Action<float> lerpAction)
        {
            //if (!script.gameObject.activeInHierarchy) return;
            if (script == null || !script.isActiveAndEnabled)
            {
                return AnimationHelper.Instance.StartCoroutine(AnimationHelper.doStandardLerp(time, lerpAction, null, AnimationHelper.Instance.linear));
            }
            return script.StartCoroutine(AnimationHelper.doStandardLerp(time, lerpAction, null, AnimationHelper.Instance.linear));
        }

        public static Coroutine lerpMe(MonoBehaviour script, float time, Action<float> lerpAction, Action completion)
        {
            return AnimationHelper.lerpMe(script, time, lerpAction, completion, AnimationHelper.Instance.easeInEaseOut);
        }

        public static Coroutine lerpMe(MonoBehaviour script, float time, Action<float> lerpAction, Action completion, AnimationCurve shape)
        {
            if (script == null || !script.isActiveAndEnabled)
            {
                return AnimationHelper.Instance.StartCoroutine(AnimationHelper.doStandardLerp(time, lerpAction, completion, shape));
            }
            return script.StartCoroutine(AnimationHelper.doStandardLerp(time, lerpAction, completion, shape));
        }


        //  public static IEnumerator doStandardLerp(float time, Action<float> lerpAction,Action completion,AnimationCurve shape)
        static IEnumerator doStandardLerp(float time, Action<float> lerpAction, Action completion, AnimationCurve shape)
        {
            if (time == 0f)
            {
                lerpAction(shape.Evaluate(1.0f));
                if (completion != null) completion();
                yield break;
            }
            float fStartTime = Time.time;
            float fLerpLength = time;
            float fCurrLerp = (Time.time - fStartTime) / fLerpLength;

            while (fCurrLerp <= 1.0f)
            {
                fCurrLerp = (Time.time - fStartTime) / fLerpLength;
                lerpAction(shape.Evaluate(fCurrLerp));
                //      lerpAction(fCurrLerp);
                yield return null;
            }
            if (completion != null) completion();
        }

        public static Coroutine playOnNextFrame(MonoBehaviour script, Action completion)
        {
            if (script.gameObject.activeInHierarchy)
            {
                return script.StartCoroutine(AnimationHelper.oneFrame(completion));
            }
            return null;
        }

        public static Coroutine playAfterFrames(MonoBehaviour script, int frameCount, Action completion)
        {
            if (script.gameObject.activeInHierarchy)
            {
                return script.StartCoroutine(AnimationHelper.waitForFrames(frameCount, completion));
            }
            return null;
        }

        static IEnumerator oneFrame(Action completion)
        {
            yield return null;
            completion();
        }

        static IEnumerator waitForFrames(int frameCount, Action completion)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
            }
            completion();
        }

        public static void playThenDeactivate(ParticleSystem theSystem)
        {
            playThenDeactivate(theSystem, 0f);
        }

        public static void playThenDeactivate(ParticleSystem theSystem, float decativateDelay)
        {
            theSystem.gameObject.SetActive(true);
            theSystem.Play(true);
            AnimationHelper.playAfterDelay(AnimationHelper.Instance, theSystem.main.duration + decativateDelay, () =>
            {
                theSystem.gameObject.SetActive(false);
            });
        }

        public static Coroutine playAfterDelay(MonoBehaviour script, float delay, Action completion)
        {
            if (delay <= 0.001f)
            {
                completion();
                return null;
            }
            return script.StartCoroutine(AnimationHelper.afterDelay(delay, completion));
        }

        static IEnumerator afterDelay(float delay, Action completion)
        {
            yield return new WaitForSeconds(delay);
            completion();
        }

        public static float lerpNoClamp(float start, float end, float lerpValue)
        {
            float diff = (end - start) * lerpValue;
            return start + diff;
        }

        public static Vector3 lerpNoClamp(Vector3 start, Vector3 end, float lerpValue)
        {
            return new Vector3(AnimationHelper.lerpNoClamp(start.x, end.x, lerpValue), AnimationHelper.lerpNoClamp(start.y, end.y, lerpValue), AnimationHelper.lerpNoClamp(start.z, end.z, lerpValue));
        }


    }
}