using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CommunitiesList", order = 1)]
public class CommunitiesList : ScriptableObject
{
    public List<CommunityInfo> communities;

    public CommunityInfo GetCommunityInfoInfo(string communityID)
    {
        CommunityInfo communityInfo = communities.Find(x => x.id.Equals(communityID));
        if (communityInfo == null)
        {
            Debug.Log($"<color=red> Couldn't Find Community Info with ID: {communityID} </color>");
        }
        return communityInfo;
    }
}
