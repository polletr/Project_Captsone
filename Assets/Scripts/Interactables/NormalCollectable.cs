using UnityEngine;

public class NormalCollectable : Interactable , ICollectable
{
    public void Collect()
    {
        AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
        gameObject.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Event.OnInteractItem?.Invoke(this);
    }
}
