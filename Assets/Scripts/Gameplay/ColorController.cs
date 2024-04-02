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

        Color currentColor = Color.white;
        Color targetColor;

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
#endif
        }

        public void SetColor(TileState tileState)
        {
            targetColor = ColorState.Colors[(int)tileState];
            currentColor = rend.material.GetColor("_BaseColor");
            this.tileState = tileState;

            if (
#if UNITY_EDITOR
                true ||
#endif
            !PlayerManager.Instance.LocalPlayer.IsCharacter)
            {
                DOTween.To(() => currentColor, x => currentColor = x, targetColor, colorTime).onUpdate += OnColorUpdate;
              
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

        private void OnColorUpdate()
        {
            Debug.Log($"Current color:{currentColor}");
            rend.material.SetColor("_BaseColor", currentColor);
            rend.material.SetColor("_EmissiveColor", currentColor * emissionIntensity);
        }
    }

}
