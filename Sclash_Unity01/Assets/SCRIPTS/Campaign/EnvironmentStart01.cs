using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// On an environment chunk, script that executes when an environment chunk loads and unloads in a campaign
public class EnvironmentStart01 : MonoBehaviour
{
    [SerializeField] float lightSmoothInSpeed = 0.2f;
    [SerializeField] SpriteRenderer sky = null;


    Light[] lights = null;
    List<float> lightsIntensities = new List<float>();
    bool finished = true;
    bool disable = false;









    private void Update()                                                               // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (!finished)
            {
                finished = true;


                if (lights != null && lights.Length > 0)
                    for (int i = 0; i < lights.Length; i++)
                        if (lights[i].intensity != lightsIntensities[i])
                        {
                            finished = false;
                            lights[i].intensity = Mathf.Lerp(lights[i].intensity, lightsIntensities[i], Time.deltaTime * lightSmoothInSpeed);

                            // If close enough set it to the same
                            if (Mathf.Abs(lights[i].intensity - lightsIntensities[i]) < 0.1f)
                                lights[i].intensity = lightsIntensities[i];
                        }


                if (sky != null)
                    if (sky.color.a != 0)
                    {
                        Color newSkyColor = sky.color;
                        newSkyColor = new Color(newSkyColor.r, newSkyColor.g, newSkyColor.b, Mathf.Lerp(newSkyColor.a, 0, Time.deltaTime * lightSmoothInSpeed));
                        sky.color = newSkyColor;
                    }
                        

                if (finished)
                {
                    lights = null;
                    lightsIntensities = null;
                    Debug.Log("Finished");


                    if (disable)
                    {
                        disable = false;
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }











    public void Enable()
    {
        gameObject.SetActive(true);
        finished = false;


        lights = transform.GetComponentsInChildren<Light>(false);
        lightsIntensities.Clear();


        if (lights != null && lights.Length > 0)
            for (int i = 0; i < lights.Length; i++)
            {
                if (lightsIntensities != null)
                    lightsIntensities.Add(lights[i].intensity);


                lights[i].intensity = 0;
            }
    }


    public void Disable()
    {
        lights = transform.GetComponentsInChildren<Light>(false);


        if (lightsIntensities != null && lightsIntensities.Count > 0)
            lightsIntensities.Clear();


        for (int i = 0; i < lights.Length; i++)
            if (lightsIntensities != null)
                lightsIntensities.Add(0);


        finished = false;
        disable = true;
    }
}
