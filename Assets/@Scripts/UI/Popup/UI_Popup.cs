using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.UI.Popup
{
	public class UI_Popup : UI_Base
	{
		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _bgCloseButton;
		
		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			Managers.UI.SetCanvas(gameObject, true);

			if (_closeButton)
			{
				_closeButton.SafeAddButtonListener(ClosePopupUI);
			}

			if (_bgCloseButton)
			{
				_bgCloseButton.SafeAddButtonListener(ClosePopupUI);
			}
			
			return true;
		}

		public virtual void ClosePopupUI()
		{
			Managers.UI.ClosePopupUI(this);
		}
	}
}