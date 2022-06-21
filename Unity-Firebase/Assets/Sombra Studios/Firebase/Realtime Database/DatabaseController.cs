using Firebase.Database;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    private static DatabaseController _instance;
    public static DatabaseController Instance { get => _instance; }

    public DatabaseReference DatabaseReference { get; private set; }

    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        InitializeDatabaseController();
        InitializeDatabase();
    }

    void OnDestroy()
    {

    }

    #endregion

    #region Private Methods

    // Singleton
    private void InitializeDatabaseController()
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
    private void InitializeDatabase()
    {
        Debug.Log("Setting up Firebase Database");
        // Get the root reference location of the database.
        DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    #endregion
}
