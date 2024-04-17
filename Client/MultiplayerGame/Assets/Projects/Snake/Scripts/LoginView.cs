using TMPro;
using UnityEngine;

public class LoginView : MonoBehaviour
{
    [SerializeField] private TMP_Text _loginText;

    public void SetLoginText(string name)
    {
        _loginText.text = name;
    }
}
