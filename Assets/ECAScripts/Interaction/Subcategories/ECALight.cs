using EcaRules;
using ECAScripts.Utils;
using UnityEngine;

/// <summary>
/// <b>ECALight</b> is an <see cref="Interaction"/> subclass that represents a light source. It can control a light source
/// referenced inside this class.
/// </summary>
[ECARules4All("light")]
[RequireComponent(typeof(Interaction), typeof(Light))] //gerarchia 
[DisallowMultipleComponent]
public class ECALight : MonoBehaviour
{
    /// <summary>
    /// <b> Intensity </b> is the intensity of the light source.
    /// </summary>
    [StateVariable("intensity", ECARules4AllType.Float)] public float intensity = 1;
    /// <summary>
    /// <b> MaxIntensity </b> is the maximum intensity of the light source.
    /// </summary>
    [StateVariable("maxIntensity", ECARules4AllType.Float)] public float maxIntensity = 10;
    /// <summary>
    /// <b> Color </b> is the color of the light source.
    /// </summary>
    [StateVariable("color", ECARules4AllType.Color)] public Color color = new Color(1, 0.95686271f, 0.8392157f, 1);

    /// <summary>
    /// <b>On</b> is a boolean that represents the state of the light source.
    /// </summary>
    [StateVariable("on", ECARules4AllType.Boolean)]
    public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);
    /// <summary>
    /// <b>ComponentLight</b> is the GameObject's light component.
    /// </summary>
    private Light componentLight;
    
    /// <summary>
    /// <b> Turns</b> sets the light source on or off.
    /// </summary>
    /// <param name="newStatus">The boolean of the new state.</param>
    [Action(typeof(ECALight), "turns", typeof(ECABoolean))]
    public void Turns(ECABoolean newStatus)
    {
        on = newStatus;
        componentLight.enabled = on;
    }
    
    /// <summary>
    /// <b>IncreasesIntensity</b> increases the intensity of the light source by the given value.
    /// If the intensity is greater than the maximum intensity, the intensity is set to the maximum intensity.
    /// </summary>
    /// <param name="amount"> The amount to add to the current intensity. </param>
    [Action(typeof(ECALight), "increases", "intensity", "by", typeof(float))]
    public void IncreasesIntensity(float amount)
    {
        if (intensity + amount < maxIntensity)
        {
            intensity += amount;
            GetComponent<Light>().intensity = intensity;
        }
        else
        {
            intensity = maxIntensity;
            GetComponent<Light>().intensity = intensity;
        }
    }

    /// <summary>
    /// <b> DecreasesIntensity</b> decreases the intensity of the light source by the given value.
    /// If the intensity is less than 0, the intensity is set to 0.
    /// </summary>
    /// <param name="amount">The amount to subtract from the current intensity.</param>
    [Action(typeof(ECALight), "decreases","intensity", "by", typeof(float))]
    public void DecreasesIntensity(float amount)
    {
        if (intensity - amount > 0)
        {
            intensity -= amount;
            GetComponent<Light>().intensity = intensity;
        }
        else
        {
            intensity = 0;
            GetComponent<Light>().intensity = intensity;
        }
    }
    
    /// <summary>
    /// <b>SetsIntensity</b> sets the intensity of the light source to the given value.
    /// </summary>
    /// <param name="i">The new intensity value.</param>
    [Action(typeof(ECALight), "sets", "intensity", "to", typeof(float))]
    public void SetsIntensity(float i)
    {
        if (i >= maxIntensity)
        {
            GetComponent<Light>().intensity = maxIntensity;
        }
        else
        {
            GetComponent<Light>().intensity = i;
        }
    }

    /// <summary>
    /// <b>SetsColor</b> sets the color of the light source to the given value.
    /// </summary>
    /// <param name="inputColor">The new color value.</param>
    [Action(typeof(ECALight), "changes","color", "to", typeof(ECAColor))]
    public void ChangesColor(ECAColor inputColor)
    {
        color = inputColor.Color;
        componentLight.color = color;
    }

    public void Awake()
    {
        componentLight = GetComponent<Light>();
        componentLight.color = color;
        intensity = intensity > maxIntensity ? maxIntensity : intensity;
        componentLight.intensity = intensity;
        componentLight.enabled = on;
    }
}
