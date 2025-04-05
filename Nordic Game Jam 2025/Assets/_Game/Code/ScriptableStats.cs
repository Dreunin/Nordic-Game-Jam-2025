using UnityEngine;

    [CreateAssetMenu]
    public class PlayerStats : ScriptableObject
    {

        [Tooltip("Minimum input before running. Will sneak before."), Range(0.01f, 0.99f)]
        public float RunThreshold = 0.1f;

        [Header("MOVEMENT")]
        [Tooltip("Top horizontal movement speed")]
        public float MaxSpeed = 14;

        [Tooltip("The speed at which the player accelerates to max speed")]
        public float Acceleration = 120;
        
        [Header("JUMP")]
        [Tooltip("Power applied when jumping")]
        public float JumpPower = 36;

        [Tooltip("Terminal falling speed")]
        public float MaxFallSpeed = 40;

        [Tooltip("Gravity acceleration when falling")]
        public float FallAcceleration = 110;
        
        [Tooltip("Delay between jumps in seconds")]
        public float JumpCooldown = 0.25f;
}