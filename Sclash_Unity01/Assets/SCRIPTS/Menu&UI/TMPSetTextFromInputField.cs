using UnityEngine;
using TMPro;
using UnityEngine.UI;



// To put on a text component, script used to fill it with a text from an input field
// OPTIMIZED
// FOOL PROOF
public class TMPSetTextFromInputField : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textToFill1 = null;
    [SerializeField] TextMeshPro textToFill2 = null;
    [SerializeField] Text textToFill3 = null;
    [SerializeField] TMP_InputField inputField1 = null;
    [SerializeField] InputField inputField2 = null;






    #region FUNCTIONS
    public void SetText()
    {
        // GET TEXT COMPONENTS IF NOT HERE
        if (textToFill1 == null)
            if (GetComponent<TextMeshProUGUI>())
                textToFill1 = GetComponent<TextMeshProUGUI>();

        if (textToFill1 == null)
            if (GetComponent<TextMeshPro>())
                textToFill2 = GetComponent<TextMeshPro>();

        if (textToFill1 == null)
            if (GetComponent<Text>())
                textToFill3 = GetComponent<Text>();




        // SET TEXTS
        if (inputField1 != null)
        {
            if (textToFill1 != null)
                textToFill1.text = inputField1.text;

            if (textToFill2 != null)
                textToFill2.text = inputField1.text;

            if (textToFill2 != null)
                textToFill2.text = inputField1.text;
        }
        else if (inputField2 != null)
        {
            if (textToFill1 != null)
                textToFill1.text = inputField2.text;

            if (textToFill2 != null)
                textToFill2.text = inputField2.text;

            if (textToFill2 != null)
                textToFill2.text = inputField2.text;
        }
    }






    // EDITOR
    private void OnDrawGizmosSelected()
    {
        if (textToFill1 == null)
            if (GetComponent<TextMeshProUGUI>())
                textToFill1 = GetComponent<TextMeshProUGUI>();

        if (textToFill1 == null)
            if (GetComponent<TextMeshPro>())
                textToFill2 = GetComponent<TextMeshPro>();

        if (textToFill1 == null)
            if (GetComponent<Text>())
                textToFill3 = GetComponent<Text>();
    }
    #endregion
}
