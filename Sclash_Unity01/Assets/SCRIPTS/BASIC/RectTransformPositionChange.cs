using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformPositionChange : MonoBehaviour
{
    [SerializeField] RectTransform rectTransformToEdit = null;

    [SerializeField] bool
        affectX = false,
        affectY = false,
        affectZ = false;

    [SerializeField] List<Vector3> newPositionsList = new List<Vector3>();

    public void NewPosition(int newPosIndex)
    {
        if (rectTransformToEdit)
        {
            Vector3 newAnchoredPosCalculated = rectTransformToEdit.anchoredPosition3D;


            if (affectX)
                newAnchoredPosCalculated = new Vector3(newPositionsList[newPosIndex].x, newAnchoredPosCalculated.y, newAnchoredPosCalculated.z);
            if (affectY)
                newAnchoredPosCalculated = new Vector3(newAnchoredPosCalculated.x, newPositionsList[newPosIndex].y, newAnchoredPosCalculated.z);
            if (affectZ)
                newAnchoredPosCalculated = new Vector3(newAnchoredPosCalculated.x, newAnchoredPosCalculated.y, newPositionsList[newPosIndex].z);


            rectTransformToEdit.anchoredPosition3D = newAnchoredPosCalculated;
        }
    }
}
