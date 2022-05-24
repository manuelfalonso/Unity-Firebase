using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpdateUserProfileController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _displayName;
    [SerializeField] private TMP_InputField _email;
    [SerializeField] private Toggle _emailVerified;
    [SerializeField] private TMP_InputField _photoUrl;
    [SerializeField] private TMP_InputField _userProvider;
    [SerializeField] private GameObject _popUpMessage;

    [SerializeField] private string _sceneToLoadSuccesfulSignIn;

    private Firebase.Auth.FirebaseUser _user;

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        InitializeUserProfile();

        FirebaseAuthController.Instance.OnUpdateUser.AddListener(UpdatePopUpMessage);
        FirebaseAuthController.Instance.OnUpdateUserSuccesful.AddListener(UpdateSuccesfulResult);
    }

    #endregion

    #region Private Methods

    private void InitializeUserProfile()
    {
        _user = FirebaseAuthController.Instance.User;

        _displayName.text = _user.DisplayName;
        _email.text = _user.Email;
        _emailVerified.isOn = _user.IsEmailVerified;
        if (_user.PhotoUrl != null)
            _photoUrl.text = _user.PhotoUrl.ToString();
        _userProvider.text = _user.ProviderId;
    }

    // OnSignIn listener event method
    private void UpdatePopUpMessage(string status)
    {
        _popUpMessage.SetActive(true);
        var messageText = _popUpMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = status;
    }

    // OnSignInSuccesful listener event method
    private void UpdateSuccesfulResult()
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

    /// <summary>
    /// Update user profile with the info modified on the form
    /// </summary>
    public void SaveButton()
    {
        FirebaseAuthController.Instance.UpdateProfile(_displayName.text, _email.text, _photoUrl.text);
    }

    #endregion
}
