using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public class ColorState
    {
        public static readonly Color[] Colors = new Color[4] { 5f * Color.white, 5f * Color.red, 5f * Color.green, 5f * Color.yellow };
    }

    public class ColorController : MonoBehaviour
    {
        Renderer rend;

        TileState color;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            Material mat = new Material(rend.material);
            rend.material = mat;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                SetColor(TileState.Red);
            }
        }

        public void SetColor(TileState color)
        {
            
            this.color = color;
            if (true || !PlayerManager.Instance.LocalPlayer.IsCharacter)
            {
              
                rend.material.SetColor("_BaseColor", ColorState.Colors[(int)color]);
                rend.material.SetColor("_EmissiveColor", ColorState.Colors[(int)color]);
                DynamicGI.SetEmissive(rend, ColorState.Colors[(int)color]);
                DynamicGI.UpdateEnvironment();
            }
            else
            {
                rend.material.SetColor("_BaseColor", ColorState.Colors[(int)TileState.White]);
                rend.material.SetColor("_EmissiveColor", ColorState.Colors[(int)TileState.White]);
                DynamicGI.SetEmissive(rend, ColorState.Colors[(int)TileState.White]);
                DynamicGI.UpdateEnvironment();
            }
        }

        
    }

}
