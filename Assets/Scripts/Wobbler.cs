using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace {
    /// <summary>
    /// Controls the "wobbling" of the space station.
    /// </summary>
    /// <remarks>
    /// "Wobbling" is composed of 2 things:
    ///     <li><b>Pitch</b>: The "magnitude" of the wobble; i.e. the difference between the "closest" and "farthest" points of the disc</li>
    ///     <li><b>Yaw</b>: Where, on the disc, the "closest" point is</li>
    /// </remarks>
    public class Wobbler : MonoBehaviour {
        /// <summary>
        /// The amount of wobble, where 0 is the minimum wobble (0) and 1 is the <see cref="MaxPitch"/> (where a player would lose)
        /// </summary>
        //[Range(0, 1)]
        public float WobbleLerpAmount;
        public  float WobbleSpeed       = 0;
        public  float WobbleSpeedChange = 0;
        private float MaxRecTime        = 2;
        private float RemainingRecTime  = 0;

        /// <summary>
        /// The maximum pitch that the station can ever wobble (i.e. <see cref="WobbleLerpAmount"/> = 1)
        /// </summary>
        /// <remarks>
        /// This would correspond to a <b>loss</b> of the game, in theory
        /// </remarks>
        //[Range(1, 180)]
        public float MaxPitch = 30;

        /// <summary>
        /// The yaw rotation speed when <see cref="WobbleLerpAmount"/> = 1, measured in degrees per second.
        /// </summary>
        public float MaxYawSpeed = 200;

        public bool CompensateYaw = true;

        private float TargetPitch => Mathf.Lerp(0, MaxPitch, WobbleLerpAmount);
        private float TargetYaw   => Time.time * YawSpeed;
        private float YawSpeed    => Mathf.Lerp(0, MaxYawSpeed, WobbleLerpAmount);

        private Quaternion TargetRotation {
            get {
                var quatToYaw   = Quaternion.AngleAxis(TargetYaw,   Vector3.up);
                var quatToPitch = Quaternion.AngleAxis(TargetPitch, Vector3.right);

                var quatRotation = quatToYaw * quatToPitch;

                if (CompensateYaw) {
                    var quatYawComp = Quaternion.AngleAxis(-TargetYaw, Vector3.up);
                    quatRotation *= quatYawComp;
                }

                return quatRotation;
            }
        }

        private void Update() {
            transform.localRotation = TargetRotation;
            if (isGameOver) {
                timeUntilReset -= Time.deltaTime;
                if (timeUntilReset <= 0) {
                    SceneManager.LoadScene("TitleScreen");
                }

                MaxPitch /= (1 - 0.5f * Time.deltaTime);
                MaxPitch =  Math.Min(MaxPitch, 180);
            }
        }

        public void Convert2Wobbling(float excessSpeed) {
            if (Math.Abs(excessSpeed) < 1e-4) {
                RemainingRecTime -= Time.deltaTime;
                if (RemainingRecTime <= 0) {
                    // once enough time has passed, the wobble speed recovers exponentially fast (v(t) = C*exp(-t))
                    // here, we compute this with implicit Euler for the ODE \dot{x} = -x
                    WobbleSpeed /= (1 + Time.deltaTime);
                }
            }
            else {
                WobbleSpeedChange =  excessSpeed;
                WobbleSpeed       += excessSpeed;
                RemainingRecTime  =  MaxRecTime;
            }

            WobbleLerpAmount = (float) (Math.Abs(2 * Math.Atan(WobbleSpeed * 5e-2) / Math.PI));

            if (WobbleLerpAmount > 0.65) {
                // End game
                isGameOver = true;
            }
        }

        public float timeUntilReset = 7f;
        public bool  isGameOver     = false;
    }
}