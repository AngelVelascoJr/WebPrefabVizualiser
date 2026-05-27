using UnityEngine;
using UnityEngine.Rendering;

public class IsProbeTouched : MonoBehaviour
{
    [SerializeField] bool isTouched;
    [SerializeField] string ProgramingVarName;
    [SerializeField] MonoBehaviour ProbeMainScript;
    [SerializeField] ProbeBehabiour behabiour;

    private void OnTriggerEnter(Collider other)
    {
        isTouched = true;
        //behabiour.SetVar(ProgramingVarName, isTouched);
    }

    private void OnTriggerExit(Collider other)
    {
        isTouched = false;
        //behabiour.SetVar(ProgramingVarName, isTouched);
    }
}
