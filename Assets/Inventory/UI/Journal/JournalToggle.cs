using UnityEngine;
using UnityEngine.UI;

public class JournalToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;

    private void Start()
    {
        button1.onClick.AddListener(ShowPanel1);
        button2.onClick.AddListener(ShowPanel2);
    }

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
}