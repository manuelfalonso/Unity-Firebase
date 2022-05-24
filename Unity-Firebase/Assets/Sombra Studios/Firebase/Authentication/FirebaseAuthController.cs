using System;
using System.Collections;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Small String Event Class to pass status as string
/// </summary>
public class StringEvent : UnityEvent<string>
{

}

/// <summary>
/// Authentication Controller Singleton to manage the user
/// </summary>
public class FirebaseAuthController : MonoBehaviour
{
    // Event invoked when registering a new user. 
    // Sends the operation status as string
    public StringEvent OnRegister = new StringEvent();
    // Event invoked when signing in a user
    // Sends the operation status as string
    public StringEvent OnSignIn = new StringEvent();
    [HideInInspector]
    public UnityEvent OnSignInSuccesful = new UnityEvent();
    // Event invoked when updating User profile info
    // Sends the operation status as string
    public StringEvent OnUpdateUser = new StringEvent();
    [HideInInspector]
    public UnityEvent OnUpdateUserSuccesful = new UnityEvent();

    private FirebaseAuth _auth;
    public FirebaseUser User { get; private set; }

    [SerializeField]
    private TextMeshProUGUI _displayName;
    [SerializeField]
    private TextMeshProUGUI _emailAddress;
    [SerializeField]
    private Image _photo;
    [SerializeField]
    private Sprite _photoEmpty;

    private static FirebaseAuthController _instance;
    private string _uid;

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
        else
        {
            Destroy(gameObject);
        }
    }

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (_auth.CurrentUser != User)
        {
            bool signedIn = User != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && User != null)
            {
                Debug.Log("Signed out " + User.UserId);
                _displayName.text = User.DisplayName ?? "";
                _emailAddress.text = User.Email ?? "";
                _photo.sprite = _photoEmpty;
                // The user's Id, unique to the Firebase project.
                // Do NOT use this value to authenticate with your backend server, if you
                // have one; use User.TokenAsync() instead.
                _uid = User.UserId;
            }

            User = _auth.CurrentUser;

            if (signedIn)
            {
                if (User != null)
                {
                    // Obtain information from a specific provider
                    foreach (var profile in User.ProviderData)
                    {
                        // Id of the provider (ex: google.com)
                        string providerId = profile.ProviderId;

                        // UID specific to the provider
                        string uid = profile.UserId;

                        // Name, email address, and profile photo Url
                        string name = profile.DisplayName;
                        string email = profile.Email;
                        System.Uri photoUrl = profile.PhotoUrl;

                        Debug.Log("ProviderId: " + providerId + ". Signed in " + profile.UserId);
                        _displayName.text = profile.DisplayName ?? "";
                        _emailAddress.text = profile.Email ?? "";
                        //StartCoroutine(DownloadImage(_user.PhotoUrl.ToString()));
                    }
                }
            }
        }
    }

    private IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
            _photo.material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
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
        User = task.Result;
        status = String.Format("Firebase user created successfully: {0} ({1})", User.DisplayName, User.UserId);
        Debug.Log(status);
        OnRegister.Invoke(status);
    }

    private IEnumerator SignIn(string email, string password)
    {
        // Firebase Create Sign In method
        var task = _auth.SignInWithEmailAndPasswordAsync(email, password);
        // Wait until the tas is completed
        yield return new WaitUntil(() => task.IsCompleted);

        var status = string.Empty;

        if (task.IsCanceled)
        {
            status = "SignInWithEmailAndPasswordAsync was canceled.";
            Debug.LogError(status);
            OnSignIn.Invoke(status);
            yield break;
        }
        if (task.IsFaulted)
        {
            status = "SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception.Message;
            Debug.LogError(status);
            OnSignIn.Invoke(status);
            yield break;
        }
        // Firebase user has sign in correctly
        User = task.Result;
        status = string.Format("User signed in successfully: {0} ({1})",
            User.DisplayName, User.UserId);
        Debug.Log(status);
        OnSignIn.Invoke(status);
        OnSignInSuccesful.Invoke();
    }

    private void SignOutUser()
    {
        if (_auth.CurrentUser != null)
        {
            _auth.SignOut();
        }
    }

    private IEnumerator UpdateUserProfile(string name, string email, string photoUrl)
    {
        if (User != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = name,                
                PhotoUrl = photoUrl.Length == 0 ? null : new Uri(photoUrl),
            };

            // Firebase Create Sign In method
            var task = User.UpdateUserProfileAsync(profile);
            // Wait until the tas is completed
            yield return new WaitUntil(() => task.IsCompleted);

            var status = string.Empty;

            if (task.IsCanceled)
            {
                status = "UpdateUserProfileAsync was canceled.";
                Debug.LogError(status);
                OnUpdateUser.Invoke(status);
                yield break;
            }
            if (task.IsFaulted)
            {
                status = "UpdateUserProfileAsync encountered an error: " + task.Exception.Message;
                Debug.LogError(status);
                OnUpdateUser.Invoke(status);
                yield break;
            }

            status = "User profile updated successfully.";
            Debug.Log(status);
            OnUpdateUser.Invoke(status);
            OnUpdateUserSuccesful.Invoke();
        }

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

    /// <summary>
    /// Update Firebase User profile
    /// </summary>
    public void UpdateProfile(string name, string email, string photoUrl)
    {
        StartCoroutine(UpdateUserProfile(name, email, photoUrl));
    }

    /// <summary>
    /// Sign Out current User
    /// </summary>
    public void SignOut()
    {
        SignOutUser();
    }

    #endregion
}
