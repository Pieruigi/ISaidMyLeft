using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class ColorState
    {
        public static readonly Color[] Colors = new Color[4] { Color.white, Color.red, Color.green, Color.yellow };
    }

    public class ColorController : MonoBehaviour
    {
        Renderer rend;

        TileState tileState;

        float colorTime = .25f;
        float emissionIntensity = 5f;
        
        //Color currentColor = Color.white;
        //Color targetColor;


        private void Awake()
        {
            rend = GetComponent<Renderer>();
            Material mat = new Material(rend.material);
            rend.material = mat;
           
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.V))
            {
                SetColor(TileState.Red);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pulse(3, TileState.Green);
            }
#endif
        }

        public void SetColor(TileState tileState)
        {
            Color targetColor = ColorState.Colors[(int)tileState];
            Color currentColor = rend.material.GetColor("_BaseColor");
            this.tileState = tileState;

            if (
#if UNITY_EDITOR
                true ||
#endif
            !PlayerManager.Instance.LocalPlayer.IsCharacter)
            {
                DOTween.To(() => currentColor, x => currentColor = x, targetColor, colorTime).onUpdate += () =>
                {
                    rend.material.SetColor("_BaseColor", currentColor);
                    rend.material.SetColor("_EmissiveColor", currentColor * emissionIntensity);
                };
              
                //rend.material.SetColor("_BaseColor", ColorState.Colors[(int)tileState]);
                //rend.material.SetColor("_EmissiveColor", ColorState.Colors[(int)tileState]);
                //DynamicGI.SetEmissive(rend, ColorState.Colors[(int)color]);
                //DynamicGI.UpdateEnvironment();
            }
            else
            {
                currentColor = targetColor;
                rend.material.SetColor("_BaseColor", currentColor);
                rend.material.SetColor("_EmissiveColor", currentColor * emissionIntensity);
                //DynamicGI.SetEmissive(rend, ColorState.Colors[(int)TileState.White]);
                //DynamicGI.UpdateEnvironment();
            }
        }

       

    
        public void Pulse(float pulseDuration, TileState tileState)
        {
            
            if (
#if UNITY_EDITOR
              true ||
#endif
          !PlayerManager.Instance.LocalPlayer.IsCharacter)
            {
                //pulsing = true;
                float pulseIntensity = emissionIntensity;
                Color currentColor = ColorState.Colors[(int)this.tileState];
                
                int count = 6;
                float time = pulseDuration / count;
                Sequence seq = DOTween.Sequence();
                seq.onComplete += () => { SetColor(tileState); };
                var t = DOTween.To(() => pulseIntensity, x => pulseIntensity = x, emissionIntensity * .25f, time).SetLoops(count, LoopType.Yoyo);
                t.onUpdate += () => { rend.material.SetColor("_EmissiveColor", currentColor * pulseIntensity); };
                seq.Append(t);
                seq.Play();
            }
        }
    }

}
