using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInEmailPasswordController : MonoBehaviour
{
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] Button _submitButton;
    [SerializeField] GameObject _popUpMessage;

    [SerializeField] string _sceneToLoadSuccesfulSignIn;

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuthController.Instance.OnSignIn.AddListener(UpdatePopUpMessage);
        FirebaseAuthController.Instance.OnSignInSuccesful.AddListener(SignInSuccesfulResult);
    }

    #endregion

    #region Private Methods

    // OnSignIn listener event method
    private void UpdatePopUpMessage(string status)
    {
        _popUpMessage.SetActive(true);
        var messageText = _popUpMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = status;
    }

    // OnSignInSuccesful listener event method
    private void SignInSuccesfulResult()
    {
        var popUpButton = _popUpMessage.GetComponentInChildren<Button>();
        popUpButton.onClick.AddListener(LoadSceneByName);
    }

    private void LoadSceneByName()
    {
        SceneManager.LoadScene(_sceneToLoadSuccesfulSignIn);
    }

    #endregion

    #region Public Methods

    public void SignInButton()
    {
        FirebaseAuthController.Instance.LogIn(_emailInput.text, _passwordInput.text);
    }

    #endregion
}
