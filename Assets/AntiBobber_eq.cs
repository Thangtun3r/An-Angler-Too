using UnityEngine;

public class AntiGravityBobber_Equipment : MonoBehaviour, IEquipment
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private float liftForce = 9.81f;

    private bool isEquipped;

    private void FixedUpdate()
    {
        if (!isEquipped || targetRigidbody == null) return;

        targetRigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
        targetRigidbody.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
    }
}