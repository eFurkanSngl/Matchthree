﻿using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;
namespace Components.UI
{
    public class PlayerScoreTMP: UITMP, ITweenContainerBind
    {
        [Inject] private GridEvents GridEvents{get;set;}
        // Zenject kullanılarak GridEvents ekleniyor Gridleri dinlemek için.
        private Tween _counterTween;
        public ITweenContainer TweenContainer{get;set;}
        private int _currCounterVal;
        private int _playerScore;
        // bu fieldlar skor animasyonu ve değerleri için kullanılacak

        private void Awake()
        {
            TweenContainer = TweenContain.Install(this);
            // TweennContainer kurulumu
        }

        protected override void RegisterEvents()
        {
            GridEvents.MatchGroupDespawn += OnMatchGroupDespawn;
            // Gridi Dinliyor
        }

        private void OnMatchGroupDespawn(int arg0)
        {
            _playerScore += arg0;

            if(_counterTween.IsActive()) _counterTween.Kill();
            
            _counterTween = DOVirtual.Int
            (
                _currCounterVal,
                _playerScore,
                1f,
                OnCounterUpdate
            );

            TweenContainer.AddTween = _counterTween;
            //bir grup eşleşme yok edildiğinde çağrılır.
            //Skoru günceller ve skoru animasyonlu bir şekilde artırmak için bir tween oluşturur.
        }

        private void OnCounterUpdate(int val)
        {
            _currCounterVal = val;
            _myTMP.text = $"Score: {_currCounterVal}";
            // Skpr anim için.
        }

        protected override void UnRegisterEvents()
        {
            GridEvents.MatchGroupDespawn -= OnMatchGroupDespawn;
            // Grid dinlemeden Çıkar
        }

        public int GetCurrentScore()
        {
            return _playerScore;
        }

        public void SetScore(int i)
        {
            _playerScore = 0;
        }
        // ♠mevcut skoru almak ve skoru sıfırlamak için kullanılır.
    }
    
}