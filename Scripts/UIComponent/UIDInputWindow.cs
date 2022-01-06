using TMPro;
using UnityEngine;

public class UIDInputWindow : MonoBehaviour
{
    [SerializeField] private TMP_InputField uidInputField;

    public void ConfirmButtonEvent()
    {
        UserDataManager.SetNewUserData(uidInputField.text);
        gameObject.SetActive(false);
    }
}
