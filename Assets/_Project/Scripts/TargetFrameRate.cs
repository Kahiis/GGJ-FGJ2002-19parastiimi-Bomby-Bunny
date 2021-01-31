using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{

        private void Awake()
        {
            // Make the game run as 60fps
            Application.targetFrameRate = 60;
            // Turn off v-sync
            QualitySettings.vSyncCount = 0;
    }
  
}
