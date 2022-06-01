using UnityEngine;

public class Mirror : MonoBehaviour
{
    // Remark: Will do later, because kinda troublesome due to VR specifics.
    // Remark2: Maybe easier just leave On and Off option and just mirror the original hand (not 2 hand)?
    // Can be problems with some exercises. And if need to move a hand to the opposite side.
    public enum MirrorMode
    {
        None,
        Opposite,
        Both
    }

    public MirrorMode mirrorMode;

    [SerializeField]
    private Transform originTransform; // a transform of the controller in VRRig
    [SerializeField]
    private Transform mirroredTransform;
    [SerializeField]
    private GameObject originHand;
    [SerializeField]
    private GameObject mirroredHand;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform playerTransform;

    private void Start()
    {
        UpdateTargetPose();
    }

    private void UpdateTargetPose()
    {
        Vector3 newPosition = cameraTransform.position;
        Quaternion newRotation = cameraTransform.rotation;
        playerTransform.position = newPosition;
        playerTransform.rotation = newRotation;
    }

    private void Update()
    {
        //if (mirrorMode == MirrorMode.None)
        //{
        //    return;
        //}
        Vector3 newPosition = originTransform.localPosition;
        newPosition.x = -newPosition.x;
        mirroredTransform.localPosition = newPosition;
        Quaternion newRotation = originTransform.localRotation;
        mirroredTransform.localRotation = newRotation;

        //Vector3 playerToSourceHand = originTransform.position - playerTransform.position;
        //Vector3 playerToDestHand = ReflectRelativeVector(playerToSourceHand);
        //mirroredTransform.position = playerTransform.position + playerToDestHand;

        //Vector3 forwardVec = ReflectRelativeVector(originTransform.forward);
        //Vector3 upVec = ReflectRelativeVector(originTransform.up);
        //mirroredTransform.rotation = Quaternion.LookRotation(forwardVec, upVec);

    }
    Vector3 ReflectRelativeVector(Vector3 relativeVec)
    {
        // relativeVec
        //     Take the relative vector....
        // + Vector3.Dot(relativeVec, playerTransform.right)
        //     and for how far along the player's right direction it is 
        //     away from the player (may be negative),
        // * playerTransform.right
        //     move it that distance along the player's right...
        // * -2f
        //    negative two times (i.e., along the left direction 2x)
        return relativeVec
            + Vector3.Dot(relativeVec, playerTransform.right)
                * playerTransform.right
                * -2f;
    }




    public void ChangeMirrorMode(int mode)
    {
        mirrorMode = (MirrorMode)mode;
        switch (mirrorMode)
        {
            case MirrorMode.None:
                mirroredHand.SetActive(false);
                originHand.SetActive(true);
                mirroredTransform.GetChild(0).gameObject.SetActive(false);
                break;
            case MirrorMode.Opposite:
                mirroredHand.SetActive(true);
                originHand.SetActive(false);
                mirroredTransform.GetChild(0).gameObject.SetActive(true);
                UpdateTargetPose();
                break;
            case MirrorMode.Both:
                mirroredHand.SetActive(true);
                originHand.SetActive(true);
                mirroredTransform.GetChild(0).gameObject.SetActive(true);
                UpdateTargetPose();
                break;
            default:
                break;
        }
    }

}
