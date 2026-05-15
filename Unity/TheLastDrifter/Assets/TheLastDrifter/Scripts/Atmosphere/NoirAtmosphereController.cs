using UnityEngine;

namespace TheLastDrifter.Atmosphere
{
    public sealed class NoirAtmosphereController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rain;
        [SerializeField] private AudioSource ambience;
        [SerializeField] private Object postProcessVolume;
        [SerializeField, Range(0f, 1f)] private float rainIntensity = 1f;
        [SerializeField, Range(0f, 1f)] private float alarmFlicker;
        [SerializeField] private Light[] flickerLights;

        private float[] baseIntensities;

        private void Awake()
        {
            baseIntensities = new float[flickerLights.Length];
            for (var i = 0; i < flickerLights.Length; i++)
                if (flickerLights[i] != null) baseIntensities[i] = flickerLights[i].intensity;
        }

        private void Update()
        {
            if (rain != null)
            {
                var emission = rain.emission;
                emission.rateOverTimeMultiplier = Mathf.Lerp(0f, 900f, rainIntensity);
            }

            if (ambience != null)
                ambience.volume = Mathf.Lerp(0.05f, 0.65f, rainIntensity);

            for (var i = 0; i < flickerLights.Length; i++)
            {
                if (flickerLights[i] == null) continue;
                var noise = Mathf.PerlinNoise(Time.time * 18f, i * 7.31f);
                flickerLights[i].intensity = baseIntensities[i] * Mathf.Lerp(1f, noise, alarmFlicker);
            }
        }

        public void SetRain(float intensity) => rainIntensity = Mathf.Clamp01(intensity);
        public void SetAlarmFlicker(float intensity) => alarmFlicker = Mathf.Clamp01(intensity);
        public void SetPostProcessWeight(float weight)
        {
            if (postProcessVolume == null) return;
            var property = postProcessVolume.GetType().GetProperty("weight");
            property?.SetValue(postProcessVolume, Mathf.Clamp01(weight));
        }
    }
}
