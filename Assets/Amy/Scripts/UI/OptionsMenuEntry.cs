using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public enum OptionEntryType
	{
		InvertPitch,
		InvertYaw,
		DesiredFOV,
		LookSensitivity
	}

	public class OptionsMenuEntry : MonoBehaviour
	{

		public OptionEntryType type;
		public Text text;
		public Image checkboxImage;

		public Sprite checkedSprite;
		public Sprite uncheckedSprite;

		public bool isAvailable = true;
		public bool isChecked = false;

		public bool isSlider = false;
		public Image sliderBG;
		public Image slider;

		public float sliderMinValue = 0.0f;
		public float sliderMaxValue = 100.0f;
		public float sliderCurrentValue = 0.0f;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (isSlider)
				sliderUpdate();
			else
				checkboxUpdate();

			//setSliderRealValue(30);

			Debug.Log(getSliderRealValue());
		}

		void checkboxUpdate()
		{
			if (isChecked)
			{
				checkboxImage.sprite = checkedSprite;
			}
			else
			{
				checkboxImage.sprite = uncheckedSprite;
			}

			checkboxImage.color = text.color;
		}
		
		void sliderUpdate()
        {
			setSliderGraphicPos();
			sliderBG.color = text.color;
			slider.color = text.color;
		}

		public void setSliderGraphicPos()
		{
			float fac = sliderBG.rectTransform.rect.width * 0.45f;

			Vector3 pos = slider.transform.localPosition;

			pos.x = Mathf.Lerp(-fac, fac, sliderCurrentValue);

			slider.transform.localPosition = pos;
		}

		public void clampSlider()
        {
			sliderCurrentValue = Mathf.Clamp01(sliderCurrentValue);
        }

		public float getSliderRealValue()
        {
			return Mathf.Lerp(sliderMinValue, sliderMaxValue, sliderCurrentValue);
        }

		public void setSliderRealValue(float v)
        {
			sliderCurrentValue = Helper.remapRange(v, sliderMinValue, sliderMaxValue, 0, 1);
        }
	}
}
