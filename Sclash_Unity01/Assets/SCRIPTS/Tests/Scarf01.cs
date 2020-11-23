using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script manages the scarf
[RequireComponent(typeof(Cloth))]
public class Scarf01 : MonoBehaviour
{
    [SerializeField] Cloth cloth = null;
    [SerializeField] float windFrequency = 1f;
    [SerializeField] Vector2 windBounds = new Vector2(-10, 30);
    bool windOn = false;
    float windObjective = 0;
    




    // Start is called before the first frame update
    void Start()
    {
        if (cloth == null)
            cloth = GetComponent<Cloth>();

        windObjective = windBounds.y;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (enabled && isActiveAndEnabled && cloth != null)
        {
            float wind = cloth.externalAcceleration.x;
            //wind = Mathf.Abs(Mathf.Sin(Time.time * windFrequency) * 20);


            if (Mathf.Approximately(wind, windBounds.x))
                windObjective = windBounds.y;
            else if (Mathf.Approximately(wind,windBounds.y))
                windObjective = windBounds.x;


            wind = Mathf.Lerp(wind, windObjective, 0.1f);
                

            cloth.externalAcceleration = new Vector3(wind, 0, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (cloth == null)
            cloth = GetComponent<Cloth>();
    }







    // SECONDARY
}
