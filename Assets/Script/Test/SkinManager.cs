using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    [Header("Skin Sprite")]
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer arm_left;
    public SpriteRenderer arm_right;
    public SpriteRenderer leg_left;
    public SpriteRenderer leg_right;
    public SpriteRenderer head_out;
    public SpriteRenderer body_out;
    public SpriteRenderer arm_left_out;
    public SpriteRenderer arm_right_out;
    public SpriteRenderer leg_left_out;
    public SpriteRenderer leg_right_out;

    [Header("Init")]
    public bool isSlim = false;
    public string nickName;

    Vector2 centerPivot = new Vector2(0.5f, 0.5f);
    Vector2 armPivot = new Vector2(0.5f, 0.16666f);

    private Dictionary<string, Texture2D> CachedSkin = new Dictionary<string, Texture2D>();
    private Dictionary<string, int> FailedNickname = new Dictionary<string, int>();
    private System.Action<int> onFail;
    private System.Action onComplete;
    private Coroutine fetchSkin;
    private string lastNickname;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LoadSkinFromNickName(nickName);
    }

    public void LoadSkinFromNickName(string minecraftNickname)
    {
        if (fetchSkin != null) return;

        lastNickname = minecraftNickname;

        if (CachedSkin.ContainsKey(minecraftNickname))
        {
            SetSkin(CachedSkin[minecraftNickname]);
        }
        else if (FailedNickname.ContainsKey(minecraftNickname))
        {
            onFail.Invoke(FailedNickname[minecraftNickname]);
        }
        else
        {
            fetchSkin = StartCoroutine(FetchSkinRoutine(minecraftNickname));
        }
    }

    public bool HasCachedSkin(string minecraftNickname)
    {
        return CachedSkin.ContainsKey(minecraftNickname);
    }

    private IEnumerator FetchSkinRoutine(string nickname)
    {
        string url = "https://minotar.net/skin/" + nickname;

        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                onFail.Invoke(0);
                FailedNickname.Add(nickname, 0);
            }
            else
            {
                byte[] imageBytes = uwr.downloadHandler.data;

                Texture2D skinTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                skinTexture.LoadImage(imageBytes);

                skinTexture.filterMode = FilterMode.Point;
                skinTexture.wrapMode = TextureWrapMode.Clamp;

                SetSkin(skinTexture);
            }
        }
        
        fetchSkin = null;
    }

    public void SetSkin(Texture2D skinTexture)
    {
        if (IsSimpleSkin(skinTexture))
        {
            onFail.Invoke(1);
            FailedNickname.Add(lastNickname, 1);
        }
        else
        {
            SetSkin(skinTexture, isSlim);
            onComplete.Invoke();

            if (!CachedSkin.ContainsKey(lastNickname)) CachedSkin.Add(lastNickname, skinTexture);
        }
    }

    public void SetSkin(Texture2D skinTexture, bool isSlim)
    {
        Rect headRect = new Rect(8, 48, 8, 8);
        Rect bodyRect = new Rect(20, 32, 8, 12);
        Rect leftArmRect = new Rect(44, 32, isSlim ? 3 : 4, 12);
        Rect rightArmRect = new Rect(36, 0, isSlim ? 3 : 4, 12);
        Rect leftlegRect = new Rect(4, 32, 4, 12);
        Rect rightlegRect = new Rect(20, 0, 4, 12);

        Rect headOutRect = new Rect(40, 48, 8, 8);
        Rect bodyOutRect = new Rect(20, 16, 8, 12);
        Rect leftArmOutRect = new Rect(44, 16, isSlim ? 3 : 4, 12);
        Rect rightArmOutRect = new Rect(52, 0, isSlim ? 3 : 4, 12);
        Rect leftlegOutRect = new Rect(4, 16, 4, 12);
        Rect rightlegOutRect = new Rect(4, 0, 4, 12);

        head.sprite = Sprite.Create(skinTexture, headRect, centerPivot, 100f);
        body.sprite = Sprite.Create(skinTexture, bodyRect, centerPivot, 100f);
        arm_left.sprite = Sprite.Create(skinTexture, leftArmRect, armPivot, 100f);
        arm_right.sprite = Sprite.Create(skinTexture, rightArmRect, armPivot, 100f);
        leg_left.sprite = Sprite.Create(skinTexture, leftlegRect, armPivot, 100f);
        leg_right.sprite = Sprite.Create(skinTexture, rightlegRect, armPivot, 100f);

        head_out.sprite = Sprite.Create(skinTexture, headOutRect, centerPivot, 100f);
        body_out.sprite = Sprite.Create(skinTexture, bodyOutRect, centerPivot, 100f);
        arm_left_out.sprite = Sprite.Create(skinTexture, leftArmOutRect, armPivot, 100f);
        arm_right_out.sprite = Sprite.Create(skinTexture, rightArmOutRect, armPivot, 100f);
        leg_left_out.sprite = Sprite.Create(skinTexture, leftlegOutRect, armPivot, 100f);
        leg_right_out.sprite = Sprite.Create(skinTexture, rightlegOutRect, armPivot, 100f);
    }

    public bool IsSimpleSkin(Texture2D skinTexture)
    {
        if (skinTexture != null && skinTexture.height == 32)
        {
            return true;
        }

        return false;
    }

    public void ClearSkin()
    {
        head.sprite = null;
        body.sprite = null;
        arm_left.sprite = null;
        arm_right.sprite = null;
        leg_left.sprite = null;
        leg_right.sprite = null;

        head_out.sprite = null;
        body_out.sprite = null;
        arm_left_out.sprite = null;
        arm_right_out.sprite = null;
        leg_left_out.sprite = null;
        leg_right_out.sprite = null;
    }

    public void AddFailListener(System.Action<int> listener) => onFail += listener;
    public void RemoveFailListener(System.Action<int> listener) => onFail -= listener;

    public void AddCompleteListener(System.Action listener) => onComplete += listener;
    public void RemoveCompleteListener(System.Action listener) => onComplete -= listener;
}