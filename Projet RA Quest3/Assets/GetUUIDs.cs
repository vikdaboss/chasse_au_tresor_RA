using Meta.XR.BuildingBlocks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GetUUIDs : MonoBehaviour
{
    List<Guid> GetAnchorAnchorUuidFromLocalStorage()
    {
        List<Guid> _uuids = new List<Guid>();
        var playerUuidCount = 2;
        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            if (!PlayerPrefs.HasKey(uuidKey))
                continue;

            var currentUuid = PlayerPrefs.GetString(uuidKey);
            _uuids.Add(new Guid(currentUuid));
        }

        return _uuids;
    }

    public void Start()
    {
        List<Guid> liste = GetAnchorAnchorUuidFromLocalStorage();
        Debug.LogWarning(liste.Count);
        foreach (var uuid in liste)
        {
            Debug.LogWarning("GUID" + uuid);
        }
    }
}
