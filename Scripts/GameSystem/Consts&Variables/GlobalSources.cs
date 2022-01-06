using UnityEngine;

public class GlobalSources : MonoBehaviour
{
    public static GlobalSources Instance;
    
    [SerializeField] public AudioSource audioBgm;
    [SerializeField] public AudioSource audioComboSfx;
    [SerializeField] public AudioSource audioCharacterMove;

    private void Awake()
    {
        Instance = this;
    }
}