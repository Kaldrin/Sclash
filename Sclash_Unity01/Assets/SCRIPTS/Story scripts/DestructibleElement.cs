using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleElement : MonoBehaviour
{
    public enum e_Materials { None, Wood, Stone }

    public e_Materials m_Material;

    [SerializeField]
    private AudioClip m_DestroySound;

    void Awake()
    {
        gameObject.tag = "Destructible";
        SelectDestroySound();
    }

    public void Cut()
    {
        PlayDestroySound();
    }

    //Select sound depending of the material
    private void SelectDestroySound()
    {
        switch (m_Material)
        {
            case e_Materials.None:
                break;

            case e_Materials.Wood:
                m_DestroySound = Resources.Load<AudioClip>("Sounds/S_HeavyWood");
                break;

            case e_Materials.Stone:
                //Play Material sound
                break;
        }
    }

    private void PlayDestroySound()
    {
        DebrisSoundManager.Instance.AddSound(m_DestroySound);
    }
}
