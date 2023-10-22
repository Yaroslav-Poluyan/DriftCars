using _Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.InputManager
{
    public class TouchPad : MonoBehaviour
    {
        [SerializeField] private bool _needPedals = true;
        private Vector3 _playerDefaultPosition;
        [SerializeField] private Sprite _gasPedalSpritePressed;
        [SerializeField] private Sprite _gasPedalSpriteReleased;
        [SerializeField] private Image _leftPedalImage;
        [SerializeField] private Image _rightPedalImage;
        [SerializeField] private ScenesManager _scenesManager;
        public float Horizontal { get; private set; }
        public float Vertical { get; set; }
        private Vector3 _leftPedalDefaultPosition;
        private Vector3 _rightPedalDefaultPosition;
        private Vector2 _middleScreenPosition;

        protected void Awake()
        {
            if (_needPedals)
            {
                _leftPedalDefaultPosition = _leftPedalImage.transform.position;
                _rightPedalDefaultPosition = _rightPedalImage.transform.position;
            }

            _middleScreenPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var type = _scenesManager.GetCurrentSceneType();
            switch (type)
            {
                case ScenesManager.SceneType.Garage:
                    gameObject.SetActive(true);
                    SetPedalsActive(false);
                    break;
                case ScenesManager.SceneType.Level:
                    gameObject.SetActive(true);
                    SetPedalsActive(true);
                    break;
                case ScenesManager.SceneType.Initial:
                    break;
                case ScenesManager.SceneType.Connect:
                    break;
                case ScenesManager.SceneType.Lobby:
                    break;
                case ScenesManager.SceneType.MainMenu:
                    break;
                case ScenesManager.SceneType.Store:
                    break;
                default:
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void SetPedalsActive(bool state)
        {
            _leftPedalImage.gameObject.SetActive(state);
            _rightPedalImage.gameObject.SetActive(state);
        }
#if UNITY_EDITOR
        private Vector2? _initialClickPosition = null;
        private bool _hasMovedSinceClick = false;
#endif
        private void Update()
        {
            Vertical = 1;

#if UNITY_EDITOR
            //via keyboard
            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                Horizontal = 1;
                if (_needPedals)
                {
                    _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                    _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                    _rightPedalImage.sprite = _gasPedalSpritePressed;
                    _leftPedalImage.sprite = _gasPedalSpriteReleased;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                Horizontal = -1;
                if (_needPedals)
                {
                    _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                    _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                    _leftPedalImage.sprite = _gasPedalSpritePressed;
                    _rightPedalImage.sprite = _gasPedalSpriteReleased;
                }
            }
            else if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                Horizontal = 0;
                if (_needPedals)
                {
                    _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                    _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                    _rightPedalImage.sprite = _gasPedalSpriteReleased;
                    _leftPedalImage.sprite = _gasPedalSpriteReleased;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                Horizontal = 0;
                Vertical = -1;
                if (_needPedals)
                {
                    _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                    _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                    _leftPedalImage.sprite = _gasPedalSpritePressed;
                    _rightPedalImage.sprite = _gasPedalSpritePressed;
                }
            }
#else
            if (Input.touches.Length > 0)
            {
                foreach (var touch in Input.touches)
                {
                    if (Input.touches.Length == 1)
                    {
                        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved ||
                            touch.phase == TouchPhase.Began)
                        {
                            if (touch.position.x < _middleScreenPosition.x)
                            {
                                Horizontal = -1;
                                if (_needPedals)
                                {
                                    _leftPedalImage.transform.position = touch.position;
                                    _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                                    _leftPedalImage.sprite = _gasPedalSpritePressed;
                                    _rightPedalImage.sprite = _gasPedalSpriteReleased;
                                }
                            }
                            else if (touch.position.x > _middleScreenPosition.x)
                            {
                                Horizontal = 1;
                                if (_needPedals)
                                {
                                    _rightPedalImage.transform.position = touch.position;
                                    _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                                    _rightPedalImage.sprite = _gasPedalSpritePressed;
                                    _leftPedalImage.sprite = _gasPedalSpriteReleased;
                                }
                            }
                        }
                    }
                    else if (Input.touches.Length == 2)
                    {
                        // Проверьте, находится ли каждое касание на неиспользуемой стороне экрана
                        bool touchOneLeft = Input.touches[0].position.x < _middleScreenPosition.x;
                        bool touchTwoLeft = Input.touches[1].position.x < _middleScreenPosition.x;

                        // Если одно из касаний слева, а другое справа
                        if (touchOneLeft ^ touchTwoLeft)
                        {
                            Horizontal = 0;
                            Vertical = -1;
                            if (_needPedals)
                            {
                                _leftPedalImage.transform.position = Input.touches[0].position;
                                _rightPedalImage.transform.position = Input.touches[1].position;
                                _leftPedalImage.sprite = _gasPedalSpritePressed;
                                _rightPedalImage.sprite = _gasPedalSpritePressed;
                            }
                        }
                    }
                }
            }
            else if (Input.touches.Length == 0)
            {
                Horizontal = 0;
                Vertical = 1;
                _leftPedalImage.transform.position = _leftPedalDefaultPosition;
                _rightPedalImage.transform.position = _rightPedalDefaultPosition;
                _leftPedalImage.sprite = _gasPedalSpriteReleased;
                _rightPedalImage.sprite = _gasPedalSpriteReleased;
            }
#endif
        }

        public float GetHorizontalInput()
        {
            return Horizontal;
        }

        public float GetVerticalInput()
        {
            return Vertical;
        }
    }
}