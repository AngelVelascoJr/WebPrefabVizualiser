using UnityEngine;
using UnityEngine.UI;

public class ShowInstructions : MonoBehaviour
{
    [SerializeField] private GameObject[] enableTargets;
    [SerializeField] private Scrollbar scrollbar;
    private static readonly float[] Relation = { 0f, 0.23f, 0.57f, 1f };
    private int Index;

    private void Start()
    {
        scrollbar.value = 0;
        Button_Show_Pressed_Off();
    }

    public void Button_Show_Pressed_On()
    {
        foreach (var t in enableTargets)
            if (t != null) t.SetActive(true);
    }

    public void Button_Show_Pressed_Off()
    {
        foreach (var t in enableTargets)
            if (t != null) t.SetActive(false);
    }

    public void Button_Prev_Pressed()
    {
        Index = Mathf.Clamp(Index - 1, 0, scrollbar.numberOfSteps - 1);
        ChangeSlider();
    }

    public void Button_Next_Pressed()
    {
        Index = Mathf.Clamp(Index + 1, 0, scrollbar.numberOfSteps - 1);
        ChangeSlider();
    }

    private void ChangeSlider()
    {
        scrollbar.value = Relation[Index];
    }
}
