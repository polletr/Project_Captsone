using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxBatteryCapacity;

    private Battery battery;// on start give 1 battery to player

    private Queue<Battery> _batteryPacks = new();

    private Stack<FlashlightAbility> _collectedAbilitys = new();

    private int numAbilityCollectedperCheckpoint;

    public int BatteryCount => _batteryPacks.Count;

    private Dictionary<Door, ICollectable> keys = new();

    public GameEvent Event;


    private void Awake()
    {
        battery = Instantiate(new GameObject().AddComponent<Battery>(), transform);
        battery.Event = Event;
        AddBattery(battery);
        SendBattery();
    }

    private void OnEnable()
    {
        Event.OnInteractItem += CollectItem;
        Event.OnAskForBattery += SendBattery;
        Event.OnTryToUnlockDoor += TryToOpenDoor;
        Event.OnLevelChange += RemoveAllKeys;
        Event.OnPlayerDeath += PlayerDiedRemoveAbility;
        Event.OnLevelChange += RestAbilityCheckpointCounter;
    }

    private void OnDisable()
    {
        Event.OnInteractItem -= CollectItem;
        Event.OnAskForBattery -= SendBattery;
        Event.OnTryToUnlockDoor -= TryToOpenDoor;
        Event.OnLevelChange -= RemoveAllKeys;
        Event.OnPlayerDeath -= PlayerDiedRemoveAbility;
        Event.OnLevelChange -= RestAbilityCheckpointCounter;
    }

    private void TryToOpenDoor(Door door)
    {
        if (HasKey(door))
        {
            door.UnlockDoor();
            keys.Remove(door);
        }
        else
        {
            door.LockedDoor();
        }
    }

    public void CollectItem(ICollectable item)
    {
        switch (item)
        {
            case Battery batteryItem:
                AddBattery(batteryItem);
                break;
            case AbilityPickup abilityPickup:
                _collectedAbilitys.Push(abilityPickup.AbilityToPickup);
                numAbilityCollectedperCheckpoint++;
                Event.OnPickupAbility?.Invoke(abilityPickup.AbilityToPickup);
                abilityPickup.Collect();
                break;
            case Key key:
                Debug.Log("PICKED UP ITEM");
                keys.Add(key.doorToOpen, key);
                item.Collect();
                break;
            case FlashlightPickup flashlightPickup:
                Event.OnPickupFlashlight?.Invoke();
                flashlightPickup.Collect();
                break;
            default:
                Debug.Log($"Item not recognized wtf is this {item}");
                break;
        }

        // Display item in inventory
    }

    public void AddBattery(Battery newBattery)
    {
        if (_batteryPacks.Count < maxBatteryCapacity)
        {
            _batteryPacks.Enqueue(newBattery);
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.PickUpBatteries, transform.position);
            newBattery.Collect();
        }
        else
        {
            Debug.Log("Battery inventory is full");
        }

        Debug.Log("Battery added to inventory");
    }

    public bool CanGetBattery(out Battery newBattery)
    {
        if (_batteryPacks.Count > 0)
        {
            newBattery = _batteryPacks.Dequeue();
            Debug.Log("Battery sent from inventory");
            return true;
        }

        newBattery = null;
        return false;
    }

    public void SendBattery()
    {
        if (CanGetBattery(out Battery newBattery))
        {
            Event.OnChangeBattery?.Invoke(newBattery);
        }
        else
        {
            Debug.Log("No battery in inventory");
        }
    }

    public void RemoveAllKeys(LevelData data)
    {
        keys.Clear();
    }

    public bool HasKey(Door item) // musse wtf change this later
    {
        return keys.ContainsKey(item);
    }

    private void PlayerDiedRemoveAbility()
    {
        while (numAbilityCollectedperCheckpoint > 0 && _collectedAbilitys.Count > 0 )
        {
            FlashlightAbility ability = _collectedAbilitys.Pop();
            Event.OnRemoveAbility?.Invoke(ability);
            numAbilityCollectedperCheckpoint--;
        }
    }

    private void RestAbilityCheckpointCounter(LevelData x) => numAbilityCollectedperCheckpoint = 0;
}
