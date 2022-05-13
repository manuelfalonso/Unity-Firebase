using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FirebaseAuthController : MonoBehaviour
{
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] TMP_InputField _verifyPassowordInput;
    [SerializeField] Button _submitButton;
    [SerializeField] Image _popUpMessage;

    private Firebase.FirebaseApp app;
    private FirebaseAuth auth;
    private FirebaseUser user;

    private string displayName;
    private string emailAddress;
    private string photoUrl;

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        CheckDependencies();
        AddUIListeners();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    #endregion

    #region Private Methods

    private void CheckDependencies()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Initialize Firebase Authentication
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                photoUrl = user.PhotoUrl.ToString() ?? "";
            }
        }
    }

    private void AddUIListeners()
    {
        _emailInput.onValueChanged.AddListener(OnValueChanged);
        _passwordInput.onValueChanged.AddListener(OnValueChanged);
        _verifyPassowordInput.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string newValue)
    {
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

    private void CreateUser(string email, string password)
    {
        Debug.Log("5");
        TextMeshProUGUI infoMessage = _popUpMessage.GetComponentInChildren<TextMeshProUGUI>();

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {


            if (task.IsCanceled)
            {
        Debug.Log("2");
                _popUpMessage.enabled = true;
                infoMessage.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
        Debug.Log("3");
                _popUpMessage.enabled = true;
                infoMessage.text = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

        Debug.Log("4");
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    private void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Register user in Firebase with Email and Password
    /// </summary>
    public void Register()
    {
        Debug.Log("1");
        CreateUser(_emailInput.text, _passwordInput.text);
    }

    /// <summary>
    /// Sign In user in Firebase with Email and Password
    /// </summary>
    public void SignIn()
    {
        SignIn(_emailInput.text, _passwordInput.text);
    }

    #endregion
}
