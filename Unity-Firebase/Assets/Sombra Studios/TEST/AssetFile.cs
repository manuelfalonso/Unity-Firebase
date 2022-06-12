using UnityEngine;

[System.Serializable]
public class AssetFile<T> where T : UnityEngine.Object
{
	[SerializeField] private string path;
	private T _asset = null;

	public T Asset
	{
		get
		{
			if (_asset == null)
			{
				_asset = Resources.Load<T>(path);
			}

			return _asset;
		}
	}
}

//public class Influencer
//{
//	[SerializeField] public AssetFile<Texture> ProfilePic;
//}
