using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInEmailPasswordController : MonoBehaviour
{
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] Button _submitButton;
    [SerializeField] GameObject _popUpMessage;

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuthController.Instance.OnSignIn.AddListener(UpdatePopUpMessage);
    }

    #endregion

    #region Private Methods

    private void UpdatePopUpMessage(string status)
    {
        _popUpMessage.SetActive(true);
        var messageText = _popUpMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = status;
    }

    #endregion

    #region Public Methods

    public void SignInButton()
    {
        FirebaseAuthController.Instance.LogIn(_emailInput.text, _passwordInput.text);
    }

    #endregion
}
