using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ISML.UI
{
    public class CameraFader : Singleton<CameraFader>
    {
        [SerializeField]
        Image panel;

        float smoothTime = 1f;
        int direction = 0;
        float currentSpeed = 0;
        float targetAlpha = -1f;

        protected override void Awake()
        {
            base.Awake();
            panel.color = new Color(0, 0, 0, 1);
        }

        // Start is called before the first frame update
        void Start()
        {
            //FadeIn(1f);
        }

        private void Update()
        {
            if (targetAlpha < 0)
                return;

            Color color = panel.color;
            float alpha = color.a;
            alpha = Mathf.SmoothDamp(alpha, targetAlpha, ref currentSpeed, smoothTime);
            color.a = alpha;
            panel.color = color;
            if(alpha == targetAlpha) 
                targetAlpha = -1;
        }

        public async Task FadeOut(float delay)
        {
            if(delay > 0)
                await Task.Delay(System.TimeSpan.FromSeconds(delay));

            targetAlpha = 1;

            await Task.Delay(System.TimeSpan.FromSeconds(smoothTime));

        }

        public async Task FadeIn(float delay)
        {
            if (delay > 0)
                await Task.Delay(System.TimeSpan.FromSeconds(delay));

            targetAlpha = 0;

            await Task.Delay(System.TimeSpan.FromSeconds(smoothTime));
            //Debug.Log("FadeIn");
            //Task t = new Task(() => { while (!(targetAlpha < 0)) { Debug.Log($"TargetAlpha:{targetAlpha}"); }; });
            //t.Start();
            //await t;
            Debug.Log("Completed");
        }
    }

}
