using TMPro;
using UnityEngine;

public class SkinSetting : MonoBehaviour
{
    public ToggleSwitch slimToggle;
    public TMP_InputField nicknameInput;
    private bool isSlim;
    string currentNickname;

    private void Start()
    {
        slimToggle.AddToggleListener(SlimToggleSwitchClicked);
        SkinManager.Instance.AddFailListener(OnFail);
        SkinManager.Instance.AddCompleteListener(OnComplte);
    }

    public void OnApplyButtonPress()
    {
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            ToastUIManager.Instance.AddToast("닉네임 입력이 없습니다.", Color.red, true);
            return;
        }

        if (ToastUIManager.Instance != null && !SkinManager.Instance.HasCachedSkin(nicknameInput.text))
            ToastUIManager.Instance.AddToast($"{nicknameInput.text} 스킨을 불러오는 중...", Color.gold);

        SkinManager.Instance.isSlim = isSlim;
        currentNickname = nicknameInput.text;
        SkinManager.Instance.LoadSkinFromNickName(nicknameInput.text);
    }

    public void SlimToggleSwitchClicked(bool toggle) => isSlim = toggle;

    public void OnFail(int errorCode)
    {
        if (ToastUIManager.Instance == null) return;
        string errMessege = "실패 : ";

        switch (errorCode)
        {
            case 0: errMessege += "Minecraft 스킨을 불러오는 데 실패했습니다."; break;
            case 1: errMessege += "Simple 스킨은 지원하지 않습니다."; break;
        }

        ToastUIManager.Instance.AddToast(errMessege, Color.red, true);
    }

    public void OnComplte()
    {
        if (string.IsNullOrEmpty(currentNickname)) return;
        if (ToastUIManager.Instance == null) return;
        ToastUIManager.Instance.AddToast($"{currentNickname} 스킨을 불러왔습니다.", Color.gold);
    }
}
