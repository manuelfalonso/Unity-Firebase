using Firebase.Database;
using UnityEngine;
using TMPro;

public class SaveDataController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userId;
    [SerializeField] private TMP_InputField _userUsername;
    [SerializeField] private TMP_InputField _userEmail;

    private void Start()
    {
        DatabaseController.Instance.DatabaseReference.
            Child("users").ChildChanged += HandleValueChanged;
    }

    public void SaveData()
    {
        User user = new User(_userUsername.text, _userEmail.text);
        string json = JsonUtility.ToJson(user);
        Debug.Log(JsonUtility.ToJson(user, true));
        DatabaseController.Instance.DatabaseReference.
            Child("users").Child(_userId.text).SetRawJsonValueAsync(json);
    }

    private void HandleValueChanged(object sender, ChildChangedEventArgs e)
    {
        var json = e.Snapshot.GetRawJsonValue();
        if (!string.IsNullOrEmpty(json))
        {
            var playerData = JsonUtility.FromJson<User>(json);
            _userUsername.text = playerData.username;
            _userEmail.text = playerData.email;
        }
    }

    private void OnDestroy()
    {
        DatabaseController.Instance.DatabaseReference.
            Child("users").ChildChanged -= HandleValueChanged;
    }
}