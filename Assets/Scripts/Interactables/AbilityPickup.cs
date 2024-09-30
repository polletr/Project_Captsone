using UnityEngine;

public class AbilityPickup : MonoBehaviour, IInteractable, ICollectable
{
    public GameEvent Event;

    public FlashlightAbility AbilityToPickup;

    public void Collect()
    {
        gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        Event.OnInteractItem?.Invoke(this);
    }


}
