using System;
using System.Collections;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.Events;

public class StringEvent : UnityEvent<string>
{

}

public class FirebaseAuthController : MonoBehaviour
{
    public StringEvent OnRegister = new StringEvent();

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
        // Firebase Create User method
        var task = _auth.CreateUserWithEmailAndPasswordAsync(email, password);
        // Wait until the tas is completed
        yield return new WaitUntil(() => task.IsCompleted);

        var status = string.Empty;

        if (task.IsCanceled)
        {
            status = "CreateUserWithEmailAndPasswordAsync was canceled.";
            Debug.LogError(status);
            OnRegister.Invoke(status);
            yield break;
        }
        if (task.IsFaulted)
        {
            status = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception.Message;
            Debug.LogError(status);
            OnRegister.Invoke(status);
            yield break;
        }
        // Firebase user has been created.
        _user = task.Result;
        status = String.Format("Firebase user created successfully: {0} ({1})", _user.DisplayName, _user.UserId);
        Debug.Log(status);
        OnRegister.Invoke(status);
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

            _user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                _user.DisplayName, _user.UserId);
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
