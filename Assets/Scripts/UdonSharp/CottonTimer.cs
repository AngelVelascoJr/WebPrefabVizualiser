
using UnityEngine;

public class CottonTimer : MonoBehaviour
{
    private CottonBehabiour cottonBehabiour;
    [SerializeField] private float Timer = 0f;
    private bool Wet = false;

    private void Awake()
    {
        Debug.Log("CottonTimer: awake");
        cottonBehabiour = GetComponent<CottonBehabiour>();
    }

    void Start()
    {
        if (cottonBehabiour == null) cottonBehabiour = GetComponent<CottonBehabiour>();
        this.enabled = false;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= 1f)
        {
            Timer = 1f;
            Wet = true;
            cottonBehabiour.ChangeAlcohol(Timer);
            this.enabled = false;
        }
        else
            cottonBehabiour.ChangeAlcohol(Timer);
    }

    private void OnEnable()
    {
        Timer = 0f;
    }

    private void OnDisable()
    {
        Timer = 0f;
    }

}
