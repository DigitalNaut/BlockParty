using System.Collections.Generic;
using UnityEngine;

public class ItemsHolster<T> where T : MonoBehaviour
{
  List<T> ItemsList = new List<T>();
  uint itemsAllowed = 1;
  uint minItemsLimit = 0;
  uint maxItemsLimit = 5;

  public int Count => ItemsList.Count;
  public bool CanAddItems => Count < itemsAllowed;
  public bool CanRemoveItems => Count > minItemsLimit;
  public uint ChangeMaxLimit(uint increment = 1) => (uint)Mathf.Clamp(itemsAllowed += increment, minItemsLimit, maxItemsLimit);
  public void SetMaxLimit(uint newLimit) => itemsAllowed = (uint)Mathf.Clamp(newLimit, minItemsLimit, maxItemsLimit);

  public bool Add(T item)
  {
    if (ItemsList.Count >= itemsAllowed)
      return false;

    ItemsList.Add(item);
    return true;
  }

  public void Remove(T item)
  {
    if (ItemsList.Count == 0)
      return;

    ItemsList.Remove(item);
  }

  public T GetOldest(bool isActive)
  {
    if (ItemsList.Count == 0) return null;

    var oldestBall = ItemsList[0];
    ItemsList.RemoveAt(0);

    oldestBall.gameObject.SetActive(isActive);

    return oldestBall;
  }
}
