﻿using System;
using System.Globalization;
using Creatures.Model.Data;
using Creatures.Model.Definitions;
using Creatures.Model.Definitions.Localisation;
using Creatures.Model.Definitions.Player;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Perks.PlayerStats
{
    public class StatWidget : MonoBehaviour, IItemRenderer<StatDef>
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Text _currentValue;
        [SerializeField] private Text _increaseValue;
        [SerializeField] private ProgressBarWidget _progress;
        [SerializeField] private GameObject _selector;

        private GameSession _session;
        private StatDef _data;

        public static Action UpgradeStatsAction;


        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            UpdateView();
        }


        public void SetData(StatDef data, int index)
        {
            _data = data;
            if (_session != null)
                UpdateView();
        }


        private void UpdateView()
        {
            var statsModel = _session.StatsModel;

            _icon.sprite = _data.Icon;
            _name.text = LocalizationManager.I.Localize(_data.Name);
            var currentLevelValue = statsModel.GetValue(_data.ID);
            _currentValue.text = currentLevelValue.ToString(CultureInfo.InvariantCulture);

            var currentLevel = statsModel.GetCurrentLevel(_data.ID);
            var nextLevel = currentLevel + 1;
            var nextLevelValue = statsModel.GetValue(_data.ID, nextLevel);
            var increaseValue = nextLevelValue - currentLevelValue;
            _increaseValue.text = $" + {increaseValue}";
            _increaseValue.gameObject.SetActive(increaseValue > 0);

            var maxLevel = DefsFacade.I.Player.GetStat(_data.ID).Levels.Length - 1;
            _progress.SetProgress(currentLevel / (float)maxLevel);
            _selector.SetActive(statsModel.InterfaceSelectedStat.Value == _data.ID);
            _session.Data.Hp.Value = (int)statsModel.GetValue(StatId.Hp);
            UpgradeStatsAction?.Invoke();
        }


        public void OnSelect()
        {
            _session.StatsModel.InterfaceSelectedStat.Value = _data.ID;
        }
    }
}
