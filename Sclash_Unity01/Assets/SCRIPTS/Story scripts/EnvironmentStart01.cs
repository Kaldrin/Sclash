using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// HEADER
// For Sclash
// Campaign mode

/// <summary>
/// On an environment chunk, script that executes when an environment chunk loads and unloads in a campaign
/// </summary>

// Unity 2019.4.14
public class EnvironmentStart01 : MonoBehaviour
{
    [SerializeField] float lightSmoothInSpeed = 0.2f;
    [SerializeField] SpriteRenderer sky = null;


    Light[] lights = null;
    List<float> lightsIntensities = new List<float>();
    List<float> lightsBaseIntensities = new List<float>();
    bool finished = true;
    bool disable = false;
    float skyOpacityObjective = 0;
    float baseSkyOpacity = 0;








    private void Awake()
    {
        // Get sky base opacity
        if (sky)
            baseSkyOpacity = sky.color.a;
    }


    private void Update()                                                               // UPDATE
    {
        if (enabled && isActiveAndEnabled)
            if (!finished)
            {
                finished = true;
                // FADE LIGHTS
                if (lights != null && lights.Length > 0 && lightsIntensities != null)
                    for (int i = 0; i < lights.Length; i++)
                        if (lights[i] != null && lights[i].intensity != lightsIntensities[i])
                        {
                            finished = false;
                            lights[i].intensity = Mathf.Lerp(lights[i].intensity, lightsIntensities[i], Time.deltaTime * lightSmoothInSpeed);

                            // If close enough set it to the same
                            if (Mathf.Abs(lights[i].intensity - lightsIntensities[i]) < 0.1f)
                                lights[i].intensity = lightsIntensities[i];
                        }


                // Fade sky
                if (sky != null)
                    if (sky.color.a != skyOpacityObjective)
                    {
                        Color newSkyColor = sky.color;
                        newSkyColor = new Color(newSkyColor.r, newSkyColor.g, newSkyColor.b, Mathf.Lerp(newSkyColor.a, skyOpacityObjective, Time.deltaTime * lightSmoothInSpeed));
                        sky.color = newSkyColor;
                    }
                        

                if (finished)
                    if (disable)
                    {
                        disable = false;
                        gameObject.SetActive(false);
                    }
            }
    }











    public void Enable()                                                                                                                                                            // ENABLE
    {
        // If it's already enabled, don't do anything
        if (gameObject.activeInHierarchy && finished)
        { }
        else
        {
            // Was disabled before ?
            bool wasDisabled = false;
            if (!gameObject.activeInHierarchy)
            {
                wasDisabled = true;
                gameObject.SetActive(true);
            }

            // LIGHTS
            if (lights == null || lights.Length <= 0)
                GetLights();


            //lights = transform.GetComponentsInChildren<Light>(false);
            /*
            if (lightsIntensities != null)
                lightsIntensities.Clear();
                */

            if (lights != null && lights.Length > 0 && lightsIntensities != null && lightsIntensities.Count == lights.Length && lightsBaseIntensities != null && lightsBaseIntensities.Count == lights.Length)
                for (int i = 0; i < lights.Length; i++)
                {
                    lightsIntensities[i] = lightsBaseIntensities[i];

                    if (wasDisabled)
                        lights[i].intensity = 0;
                }


            // SKY
            skyOpacityObjective = baseSkyOpacity;
            if (wasDisabled)
            {
                Color newSkyColor = new Color(sky.color.r, sky.color.g, sky.color.b, 0);
                sky.color = newSkyColor;
            }


            finished = false;
            disable = false;
        }
    }


    public void Disable()                                                                                                                                                       // DISABLE
    {
        // If it's already disabled, don't do anything
        if (!gameObject.activeInHierarchy)
        { }
        else
        {
            // LIGHTS
            if (lights == null || lights.Length <= 0)
                GetLights();
            /*
            lights = transform.GetComponentsInChildren<Light>(false);
            if (lightsIntensities != null && lightsIntensities.Count > 0)
                lightsIntensities.Clear();
                */
            if (lights != null && lights.Length > 0 && lightsIntensities != null && lightsIntensities.Count == lights.Length && lightsBaseIntensities != null && lightsBaseIntensities.Count == lights.Length)
                for (int i = 0; i < lights.Length; i++)
                    lightsIntensities[i] = 0;


            // SKY
            skyOpacityObjective = 0;


            finished = false;
            disable = true;
        }
    }




    void GetLights()
    {

        lights = transform.GetComponentsInChildren<Light>(false);
        if (lightsIntensities != null)
            lightsIntensities.Clear();
        if (lightsBaseIntensities != null)
            lightsBaseIntensities.Clear();

        if (lights != null && lights.Length > 0)
            for (int i = 0; i < lights.Length; i++)
                if (lights[i] != null)
                {
                    lightsBaseIntensities.Add(lights[i].intensity);
                    lightsIntensities.Add(lights[i].intensity);
                }
    }
}
