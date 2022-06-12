using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CommunityInfo
{
    public string id;
    public string creatorName;
    public ContentType contentType;
    public Texture2D communityPicture;
    public Sprite bannerImage;
    public List<Sprite> feedImages;
}

public enum ContentType
{
    Comedy,
    Fashion,
    Lifestyle,
    Beauty,
    Music,
    Food
}
