using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    //플레이어가 닿으면 체력을 회복시켜주는 아이템
    [SerializeField]
    private float healAmount = 30f;

    private Action<Item> onItemUsed = null;

    public void Init(Action<Item> onItemUsed)
    {
        this.onItemUsed = onItemUsed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
        {
            return;
        }

        player.Heal(healAmount);
        onItemUsed?.Invoke(this);
        Destroy(gameObject);
    }
}
