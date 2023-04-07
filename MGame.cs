using System;
using _Components._Level;
using _Configs;
using _Enums;
using _Factories;
using _UI._Screens;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace _Managers
{
    public class MGame : MonoBehaviour
    {
        public event Action LevelLoaded = delegate { };
        public event Action LevelStarted = delegate { };
        public event Action<bool> LevelFinished = delegate(bool b) { };
        public event Action<bool> SkipBtnStateChange = delegate { };

        public Level CurrentLevel { get; private set; }
        public int CurrentBalls { get; set; }
        public GameState State { get; set; }

        private MUI          _ui;
        private MData        _data;
        private LevelFactory _levelFactory;
        private MAnalytics   _analytics;
        private MAds         _ads;
        private CoreConfig   _config;

        [Inject]
        public void Construct(MUI ui, MData data, LevelFactory levelFactory, MAnalytics analytics, MRemoteConfig abTest, 
            MAds ads, CoreConfig config)
        {
            _ui = ui;
            _data = data;
            _levelFactory = levelFactory;
            _analytics = analytics;
            _ads = ads;
            _config = config;
        }

        public void Start()
        {
            LoadLevel();
        }

        public void StartGameplay()
        {
            _data.CurrentLevelAttempt++;
            _ui.ShowScreen<GameScreen>();
            _analytics.OnLevelStart(_data.CurrentLevelNumber, _data.CurrentLevelAttempt, !_data.TutorialCompleted);
            LevelStarted.Invoke();
        }

        public void FinishLevel(bool result, int stars, bool restart = false)
        {
            if (result)
            {
                State = GameState.Win;
                _analytics.OnLevelComplete(_data.CurrentLevelNumber, _data.CurrentLevelAttempt, stars, restart, !_data.TutorialCompleted);
                _data.CurrentLevelNumber++;
                _data.CurrentLevelAttempt = 0;
            }
            else
            {
                State = GameState.Lose;
                _analytics.OnLevelFailed(_data.CurrentLevelNumber, _data.CurrentLevelAttempt, !_data.TutorialCompleted);
            }
            ShowSkipBtn(false);
            LevelFinished.Invoke(result);
        }

        public void ShowSkipBtn(bool state) => SkipBtnStateChange.Invoke(state);

        public void LoadLevel()
        {
            State = GameState.Wait;
            
            _data.BallsForRating += CurrentBalls;
            CurrentBalls = 0;

            if (CurrentLevel != null)
            {
                Destroy(CurrentLevel.gameObject);
            }

            _levelFactory.CreateLevel(out var level);
            CurrentLevel = level;

            if (!_data.TutorialCompleted && _data.CurrentLevelNumber == _config.game.tubeShowLevel - 1)
            {
                _ui.ShowScreen<TubeTutorialScreen>();
            }
            else
            {
                _ui.ShowScreen<LobbyScreen>();
            }
            _ads.ShowBanner();
        }

        public void StartAnimationFinished()
        {
            LevelLoaded.Invoke();
            State = GameState.Playing;
        }
        
        public void OnTutorialLevelCompleted()
        {
            if (_data.CurrentLevelNumber == _config.game.tutorialFinishLevel)
            {
                _data.TutorialCompleted = true;
                _data.CurrentLevelNumber = 0;
                _ui.ShowScreen<TutorialCompletedScreen>();
            }
            else
            {
                LoadLevel();
            }
        }
        
        [Button()]
        private void NextLevel()
        {
            _data.CurrentLevelNumber++;
            LoadLevel();
        }
        
        [Button()]
        private void PreviousLevel()
        {
            _data.CurrentLevelNumber--;
            LoadLevel();
        }

        [Button()]
        private void SkipTutorial()
        {
            _data.TutorialCompleted = true;
            _data.CurrentLevelNumber = 0;
            LoadLevel();
        }

        [Button()]
        private void Test()
        {
            _ui.ShowScreen<TutorialCompletedScreen>();
        }
    }
}