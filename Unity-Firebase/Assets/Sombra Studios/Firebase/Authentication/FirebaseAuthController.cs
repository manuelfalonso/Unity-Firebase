using System;
using System.Collections;
using UnityEngine;
using Firebase.Auth;

public class FirebaseAuthController : MonoBehaviour
{
    private FirebaseAuth _auth;
    private FirebaseUser _user;

    private string displayName;
    private string emailAddress;
    private string photoUrl;

    private static FirebaseAuthController _instance;
    public static FirebaseAuthController Instance { get { return _instance; } }

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        InitializeAuthController();
        InitializeFirebase();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
    }

    #endregion

    #region Private Methods

    private void InitializeAuthController()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && _user != null)
            {
                Debug.Log("Signed out " + _user.UserId);
            }
            _user = _auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + _user.UserId);
                displayName = _user.DisplayName ?? "";
                emailAddress = _user.Email ?? "";
                photoUrl = _user.PhotoUrl.ToString() ?? "";
            }
        }
    }

    private IEnumerator CreateUser(string email, string password)
    {
        yield return _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("4");
            // Firebase user has been created.
            _user = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                _user.DisplayName, _user.UserId);
        });
    }

    private IEnumerator SignIn(string email, string password)
    {
        yield return _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
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
    public void Register(string email, string password)
    {
        StartCoroutine(CreateUser(email, password));
    }

    /// <summary>
    /// Sign In user in Firebase with Email and Password
    /// </summary>
    public void LogIn(string email, string password)
    {
        StartCoroutine(SignIn(email, password));
    }

    #endregion
}
