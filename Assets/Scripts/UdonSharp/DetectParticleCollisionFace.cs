using UnityEngine;

public class DetectParticleCollisionFace : MonoBehaviour
{
    public FaceBehaviour face1;
    public FaceBehaviour face2;

    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning($"[<color=green>OnParticleCollision, GO name: </color>]{other.gameObject.name}");

        if (face1.IsReady() && face2.IsReady())
            return;

        string orientation = gameObject
            .GetComponent<OrientationChecker>()
            .checkOrientation();

        FaceBehaviour faceTarget;

        if (orientation == "Down")
        {
            faceTarget = face1;
        }
        else if (orientation == "Up")
        {
            faceTarget = face2;
        }
        else
        {
            faceTarget = null;
        }

        if (faceTarget == null)
            return;

        liquidSource(other, faceTarget);
        botellaLab(other, faceTarget);
    }

    private void liquidSource(GameObject other, FaceBehaviour faceTarget)
    {
        var liquidSource = other.GetComponentInParent<IsLiquidSource>();

        if (liquidSource != null && faceTarget.pulido.finishedPulido2)
        {
            faceTarget.pulido.waterPS = other.GetComponent<ParticleSystem>();

            Debug.LogWarning($"[<color=blue>waterPS assigned to: </color>]{faceTarget.pulido.waterPS.name}");
        }
    }

    private void botellaLab(GameObject other, FaceBehaviour faceTarget)
    {
        var botellaLab = other.GetComponentInParent<BotellaLab>();

        if (botellaLab != null)
        {
            string tipo = botellaLab.Tipo;
            checkAndSendEvent(faceTarget, tipo);
        }
    }

    private void checkAndSendEvent(FaceBehaviour face, string tipo)
    {

        if (tipo == "Nital"
            && face.pulido.finishedPulido1
            && face.pulido.finishedPulido2
            && !face.ataque.haveNital
            && face.limpieza.finishedLimpieza)
        {
            LocalUdonEventBridge.SendLocalEvent(face.ataque, "addedNital"); // VRChat: SendCustomNetworkEvent
        }

        if (tipo == "Alcohol"
            && face.pulido.finishedEnjuagado
            && !face.limpieza.haveAlcohol)
        {
            LocalUdonEventBridge.SendLocalEvent(face.limpieza, "addedAlcohol"); // VRChat: SendCustomNetworkEvent

            Debug.LogWarning($"[<color=red>Status haveAlcohol in particle collision: </color>]{face.limpieza.haveAlcohol}");
        }
    }
}
