using UnityEngine;

namespace DefaultNamespace {
    public class SunOrigin : MonoBehaviour {
        public Transform Earth;
        public Transform Sun;
        [Range(0, 360)]
        public float DegreesPerSecond;

        private void Start() {
            Sun.SetParent(null);
            transform.LookAt(Sun);
            Sun.transform.LookAt(transform);
            Sun.SetParent(transform);
        }

        private void Update() {
            transform.Rotate(Vector3.up, DegreesPerSecond * Time.deltaTime, Space.Self);
        }

        private void FixedUpdate() {
            // Transform transform1;
            // (transform1 = Sun.transform).LookAt(Earth);
            // // Sun.AddForce((transform1.position - Earth.transform.position).normalized * -1000 * Time.fixedDeltaTime);
            // transform.Rotate(transform1.up, 10 * Time.fixedDeltaTime);
        }
    }
}