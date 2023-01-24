using UnityEngine;
//#if UNITY_IPHONE
//using UnityEngine.iOS;
//#endif
using UnityEngine.UI;
using TMPro;

namespace Moonee.MoonSDK.Internal.RateUs
{
    public class RateUsView : MonoBehaviour
    {
        [SerializeField] private Image appIcon;

        [Header("GameObjects")]

        [SerializeField] private GameObject rateUsScreen;
        [SerializeField] private GameObject[] enabledStars;

        [Header("Buttons")]

        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;

        [Header("Texts")]

        [SerializeField] TextMeshProUGUI gameNameText;


        private MoonSDKSettings settings;
        private float rateValue = 3;

        void Start()
        {
            settings = MoonSDKSettings.Load();
            appIcon.sprite = settings.studioLogoSprite;

            gameNameText.text = $"Enjoying {Application.productName}";

            EnableStars(false, 5);
            EnableStars(true, 3);

            submitButton.onClick.AddListener(RateUsButtonPressed);
            cancelButton.onClick.AddListener(CloseRateUsScreen);
        }
        private void RateUsButtonPressed()
        {
            if (rateValue <= 3)
            {
                CloseRateUsScreen();
            }
            else
            {
                CloseRateUsScreen();
#if UNITY_IOS
                Application.OpenURL(settings.appStoreLink);
#elif UNITY_ANDROID

                Application.OpenURL(settings.googlePlayLink);
#endif
            }
        }
        private void EnableStars(bool isToEnable, int count)
        {
            for (int i = 0; i < count; i++)
            {
              enabledStars[i].SetActive(isToEnable);
            }
        }
      
        private void CloseRateUsScreen()
        {
            Destroy(gameObject);
        }
        public void StarButtonPressed(int index)
        {
            rateValue = index;
            EnableStars(false, 5);
            EnableStars(true, index);
        }
    }
}