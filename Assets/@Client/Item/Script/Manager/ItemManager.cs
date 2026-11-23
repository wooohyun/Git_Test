using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab = null;

    //아이템이 생성될 발판들
    [SerializeField]
    private Collider[] platforms = null;

    [SerializeField]
    private int maxItemCount = 3;
    [SerializeField]
    private float spawnInterval = 5f;
    //발판 가장자리에서 이만큼 안쪽에만 생성
    [SerializeField]
    private float edgeMargin = 1f;
    //발판 표면에서 이만큼 띄워서 생성
    [SerializeField]
    private float heightOffset = 0.5f;

    private List<Item> itemList = null;

    private void Awake()
    {
        itemList = new List<Item>();
    }

    public void Init()
    {
        StartCoroutine(CoSpawnItems());
    }

    private IEnumerator CoSpawnItems()
    {
        while (true)
        {
            if (itemList.Count < maxItemCount)
            {
                SpawnItem();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnItem()
    {
        if (TryGetRandomPosition(out Vector3 spawnPosition) == false)
        {
            return;
        }

        GameObject go = Instantiate(
            itemPrefab,
            spawnPosition,
            Quaternion.identity,
            this.transform);

        Item itemComponent = go.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.Init(RemoveItem);
            itemList.Add(itemComponent);
        }
    }

    //무작위 발판을 고르고, 그 발판 윗면의 무작위 위치를 구함
    private bool TryGetRandomPosition(out Vector3 position)
    {
        position = Vector3.zero;
        if (platforms == null || platforms.Length == 0)
        {
            return false;
        }

        Collider platform = platforms[Random.Range(0, platforms.Length)];
        Bounds bounds = platform.bounds;

        float marginX = Mathf.Min(edgeMargin, bounds.extents.x);
        float marginZ = Mathf.Min(edgeMargin, bounds.extents.z);
        float x = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
        float z = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);

        //발판 위에서 아래로 레이를 쏴서 실제 표면 높이를 찾음
        Ray ray = new Ray(new Vector3(x, bounds.max.y + 10f, z), Vector3.down);
        if (platform.Raycast(ray, out RaycastHit hit, bounds.size.y + 20f) == false)
        {
            return false;
        }

        position = hit.point + Vector3.up * heightOffset;
        return true;
    }

    private void RemoveItem(Item item)
    {
        itemList.Remove(item);
    }
}
