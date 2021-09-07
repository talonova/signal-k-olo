//<<<<Customized>>>>
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;

namespace PixelCrushers
{
    /// <summary>
    /// Manages localization settings.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UILocalizationManager : MonoBehaviour
    {
        public static event System.Action<AbstractController> OnCurrentControllerChanged;

        public static event System.Action<string> OnCurrentLanguageChanged;
        private static void CallOnCurrentLanguageChanged(string language) => OnCurrentLanguageChanged(language);

        static UILocalizationManager()
        {
            OnCurrentControllerChanged += (controller) => OnCurrentLanguageChanged?.Invoke(controller.currentLanguage);
            OnCurrentLanguageChanged += (_) => UpdateUIs();
        }

        private static AbstractController _currentController;
        public static AbstractController CurrentController
        {
            get
            {
                if (_currentController != null)
                {
                    return _currentController;
                }
                else
                {
                    Debug.LogWarning("Accessing DummyController");
                    return DummyController.Instance;
                }
            }
            set
            {
                if (_currentController != null)
                {
                    _currentController.OnCurrentLanguageChanged -= CallOnCurrentLanguageChanged;
                }

                _currentController = value;

                _currentController.OnCurrentLanguageChanged += CallOnCurrentLanguageChanged;
                OnCurrentControllerChanged?.Invoke(_currentController);
            }
        }

        private static UILocalizationManager m_instance = null;

        public static AbstractController instance => CurrentController;

        /// <summary>
        /// Overrides the global text table.
        /// </summary>
        public TextTable textTable
        {
            get => CurrentController.textTable;
            set => CurrentController.textTable = value;
        }

        /// <summary>
        /// Gets or sets the current language. Setting the current language also updates localized UIs.
        /// </summary>
        public string currentLanguage
        {
            get => CurrentController.currentLanguage;
            set => CurrentController.currentLanguage = value;
        }

        /// <summary>
        /// The PlayerPrefs key to store the player's selected language code.
        /// </summary>
        public string currentLanguagePlayerPrefsKey => string.Empty;

        private void Awake()
        {
            if (m_instance == null) m_instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame(); // Wait for Text components to start.
            UpdateUIs();
        }

        /// <summary>
        /// Updates the current language and all localized UIs.
        /// </summary>
        /// <param name="language">Language code defined in your Text Table.</param>
        public void UpdateUIs(string language)
        {
            CurrentController.currentLanguage = language;
        }

        private static void UpdateUIs()
        {
            if (Application.isPlaying)
            {
                var localizeUIs = FindObjectsOfType<LocalizeUI>();
                for (int i = 0; i < localizeUIs.Length; i++)
                {
                    localizeUIs[i].UpdateText();
                }
            }
        }

        public class AbstractController
        {
            public event System.Action<string> OnCurrentLanguageChanged;
            public void CallOnCurrentLanguageChanged() => OnCurrentLanguageChanged?.Invoke(currentLanguage);

            protected virtual TextTable _textTable { get; set; }
            protected virtual string _currentLanguage { get; set; }

            public virtual TextTable textTable
            {
                get => (_textTable != null) ? _textTable : GlobalTextTable.textTable;
                set => _textTable = value;
            }

            /// <summary>
            /// Gets or sets the current language. Setting the current language also updates localized UIs.
            /// </summary>
            public virtual string currentLanguage
            {
                get => _currentLanguage ?? string.Empty;
                set
                {
                    _currentLanguage = value;
                    OnCurrentLanguageChanged?.Invoke(value);
                }
            }

            public void UpdateUIs(string language) => currentLanguage = language;
        }

        private class DummyController : AbstractController
        {
            public static DummyController Instance = new DummyController();
            protected override string _currentLanguage { get; set; } = string.Empty;
            protected override TextTable _textTable { get; set; } = null;
        }
    }
}
