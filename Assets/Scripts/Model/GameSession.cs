﻿using System.Collections.Generic;
using System.Linq;
using General.Components.LevelManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Disposables;

namespace Creatures.Model.Data
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private string _defaultCheckPoint;

        public PlayerData Data => _data;
        private PlayerData _sessionSave;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        public QuickInventoryModel QuickInventory { get; private set; }

        private List<string> _checkpoints = new List<string>();


        private void Awake()
        {
            var existsSession = GetExistsSession();
            if (existsSession != null)
            {
                existsSession.StartSession(_defaultCheckPoint);
                Destroy(gameObject);
            }
            else
            {
                InitModels();
                DontDestroyOnLoad(this);
                StartSession(_defaultCheckPoint);
            }
        }


        private void StartSession(string defaultCheckPoint)
        {
            SetChecked(defaultCheckPoint);
            LoadHud();
            SpawnPlayer();
        }


        private void SpawnPlayer()
        {
            var checkpoints = FindObjectsOfType<CheckPointComponent>();
            var lastCheckPoint = _checkpoints.Last();
            foreach (var checkPoint in checkpoints)
            {
                if (checkPoint.Id == lastCheckPoint)
                {
                    checkPoint.SpawnPlayer();
                    break;
                }
            }
        }


        private void InitModels()
        {
            QuickInventory = new QuickInventoryModel(Data);
            _trash.Retain(QuickInventory);
        }


        private void LoadHud()
        {
            SceneManager.LoadScene("Hud", LoadSceneMode.Additive);
        }


        private void Start()
        {
            SaveSession();
        }


        private GameSession GetExistsSession()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var gameSession in sessions)
            {
                if (gameSession != this)
                    return gameSession;
            }
            return null;
        }


        public void SaveSession()
        {
            _sessionSave = _data.Clone();
        }


        public void LoadLastSessionSave()
        {
            _data = _sessionSave.Clone();

            _trash.Dispose();
            InitModels();
        }


        public bool IsChecked(string id)
        {
            return _checkpoints.Contains(id);
        }


        public void SetChecked(string id)
        {
            if (!_checkpoints.Contains(id))
            {
                SaveSession();
                _checkpoints.Add(id);
            }
        }


        private void OnDestroy()
        {
            _trash.Dispose();
        }


        private List<string> _removedItems = new List<string>();


        internal bool RestoreState(string itemID)
        {
            return _removedItems.Contains(itemID);
        }


        internal void StoreState(string itemID)
        {
            if (!_removedItems.Contains(itemID))
                _removedItems.Add(itemID);
        }
    }
}

