using UnityEngine;
using TMPro;

public class UpdateUserProfileController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _displayName;
    [SerializeField] private TextMeshProUGUI _email;
    [SerializeField] private TextMeshProUGUI _photoUrl;
    [SerializeField] private TextMeshProUGUI _userProvider;

    private Firebase.Auth.FirebaseUser _user;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveButton()
    {
        FirebaseAuthController.Instance.UpdateProfile(_displayName.text, _email.text, _photoUrl.text);
    }
}
