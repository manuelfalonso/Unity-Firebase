using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterAuthController : MonoBehaviour
{
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] TMP_InputField _verifyPassowordInput;
    [SerializeField] Button _submitButton;
    [SerializeField] GameObject _popUpMessage;

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        AddUIListeners();

        FirebaseAuthController.Instance.OnRegister.AddListener(UpdatePopUpMessage);
    }

    #endregion

    #region Private Methods

    private void AddUIListeners()
    {
        _emailInput.onValueChanged.AddListener(OnValueChanged);
        _passwordInput.onValueChanged.AddListener(OnValueChanged);
        _verifyPassowordInput.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string newValue)
    {
        // Check rules to enable the submit button
        if (!string.IsNullOrEmpty(_emailInput.text)
            && !string.IsNullOrEmpty(_passwordInput.text)
            && !string.IsNullOrEmpty(_verifyPassowordInput.text)
            && _passwordInput.text == _verifyPassowordInput.text)
        {
            _submitButton.interactable = true;
        }
        else
        {
            _submitButton.interactable = false;
        }
    }

    // OnRegister listener event method
    private void UpdatePopUpMessage(string status)
    {
        _popUpMessage.SetActive(true);
        var messageText = _popUpMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = status;
    }

    #endregion

    #region Public Methods

    public void RegisterButton()
    {
        FirebaseAuthController.Instance.Register(_emailInput.text, _passwordInput.text);
    }

    #endregion
}
