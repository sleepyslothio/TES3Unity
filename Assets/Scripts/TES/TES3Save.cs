﻿using System;
using System.Collections.Generic;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity
{
    [Serializable]
    public struct PlayerData
    {
        public string Name;
        public bool Woman;
        public RaceType Race;
        public string ClassName;
        public string Faction;
    }

    [Serializable]
    public class TES3Save
    {
        public const string SaveKey = "tes3unity.autosave";

        public string CellName;
        public bool IsInterior;
        public Vector2i CellGrid;
        public Vector3 Position;
        public Quaternion Rotation;
        public PlayerData Data;
        public List<string> Items;

        public bool IsEmpty()
        {
            return CellName != null;
        }

        public void Create(PlayerData data)
        {
            Data = data;
        }

        public void Save(CELLRecord cell, Transform player)
        {
            CellName = cell.NAME.value;
            CellGrid = cell.gridCoords;
            IsInterior = cell.isInterior;
            Position = player.position;
            Rotation = player.rotation;

            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(SaveKey, json);
        }

        public static TES3Save Get()
        {
            var data = PlayerPrefs.GetString(SaveKey);

            if (!string.IsNullOrEmpty(data) && data != "{}")
            {
                return JsonUtility.FromJson<TES3Save>(data);
            }

            return new TES3Save();
        }
    }
}
