using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.WorldSpaceCanvasUI {
    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
    public class WorldSpaceRenderModeUIElement : MonoBehaviour {

        public Transform target;
        private Camera mainCamera;

        private void Start() {
            mainCamera = Camera.main;
        }

        private void Update() {
            if (ReferenceEquals(mainCamera, null)) {
                mainCamera = Camera.main;
            }
            transform.position = target.position;
            transform.LookAt(mainCamera.transform);
            transform.rotation = mainCamera.transform.rotation;
        }

        private void OnDestroy() {
            mainCamera = null;
        }

    }
}
