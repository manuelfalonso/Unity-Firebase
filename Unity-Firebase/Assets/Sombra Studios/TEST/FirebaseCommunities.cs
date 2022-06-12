using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseCommunities : MonoBehaviour
{
    private FirebaseDatabase Database;

    public async void SetCommunities(CommunitiesList communitiesList)
    {
        string json = JsonUtility.ToJson(communitiesList);
        Debug.Log("json " + json);

        await Database.RootReference.Child("Communities").SetRawJsonValueAsync(json);
    }

    public async Task<CommunitiesList> GetCommunities()
    {
        // Firebase Database call
        var dataSnapshot = await Database.GetReference("Communities").GetValueAsync();
        if (!dataSnapshot.Exists)
        {
            Debug.LogWarning("Communities values dont exist!");
            return null;
        }

        // Convert from Json
        CommunitiesList communitiesList =
            JsonUtility.FromJson<CommunitiesList>(dataSnapshot.GetRawJsonValue());
        Debug.Log("Communities: " + dataSnapshot.GetRawJsonValue());

        return communitiesList;
    }
}
