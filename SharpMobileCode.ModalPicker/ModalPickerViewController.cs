/*
 * Copyright (C) 2014 
 * Author: Ruben Macias
 * http://sharpmobilecode.com @SharpMobileCode
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using UIKit;
using CoreGraphics;

namespace SharpMobileCode.ModalPicker
{
    public delegate void ModalPickerDimissedEventHandler(object sender, EventArgs e);

    public class ModalPickerViewController : UIViewController
    {
        public event ModalPickerDimissedEventHandler OnModalPickerDismissed;
        const float _headerBarHeight = 40;

        public UIColor HeaderBackgroundColor { get; set; }
        public UIColor HeaderTextColor { get; set; }
		public string HeaderText { get; set; }
		public string DoneButtonText { get; set; }
		public string CancelButtonText { get; set; }

        public UIDatePicker DatePicker { get; set; }
        public UIPickerView PickerView { get; set; }
        private ModalPickerType _pickerType;
        public ModalPickerType PickerType 
        { 
            get { return _pickerType; }
            set
            {
                switch(value)
                {
                    case ModalPickerType.Date:
						DatePicker = new UIDatePicker {
							TranslatesAutoresizingMaskIntoConstraints = false
						};
                        PickerView = null;
                        break;
                    case ModalPickerType.Custom:
                        DatePicker = null;
						PickerView = new UIPickerView {
							TranslatesAutoresizingMaskIntoConstraints = false
						};
                        break;
                    default:
                        break;
                }

                _pickerType = value;
            }
        }

        UILabel _headerLabel;
		UIButton _doneButton;
		UIButton _cancelButton;
        UIViewController _parent;
        UIView _internalView;

        public ModalPickerViewController(ModalPickerType pickerType, string headerText, UIViewController parent)
        {
            HeaderBackgroundColor = UIColor.White;
            HeaderTextColor = UIColor.Black;
            HeaderText = headerText;
            PickerType = pickerType;
			_parent = parent;
			DoneButtonText = "Done";
			CancelButtonText = "Cancel";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeControls();
        }

        void InitializeControls()
        {
            View.BackgroundColor = UIColor.Clear;
			_internalView = new UIView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			_headerLabel = new UILabel {
            	AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            	BackgroundColor = HeaderBackgroundColor,
            	TextColor = HeaderTextColor,
            	Text = HeaderText,
				TextAlignment = UITextAlignment.Center,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			_cancelButton = UIButton.FromType(UIButtonType.System);
			_cancelButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
			_cancelButton.BackgroundColor = UIColor.Clear;
			_cancelButton.SetTitle(CancelButtonText, UIControlState.Normal);
			_cancelButton.TouchUpInside += CancelButtonTapped;
			_cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;
			AddButtonSizeConstraints (_cancelButton);

            _doneButton = UIButton.FromType(UIButtonType.System);
            _doneButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
            _doneButton.BackgroundColor = UIColor.Clear;
			_doneButton.SetTitle(DoneButtonText, UIControlState.Normal);
            _doneButton.TouchUpInside += DoneButtonTapped;
			_doneButton.TranslatesAutoresizingMaskIntoConstraints = false;
			AddButtonSizeConstraints (_doneButton);

			UIView picker = null;
            switch(PickerType)
            {
                case ModalPickerType.Date:
					picker = DatePicker;
                    break;
                case ModalPickerType.Custom:
					picker = PickerView;
                    break;
                default:
                    break;
            }
			picker.BackgroundColor = UIColor.White;
			_internalView.AddSubview (picker);
            _internalView.BackgroundColor = HeaderBackgroundColor;

			_internalView.AddSubview(_headerLabel);
			_internalView.AddSubview (_cancelButton);
            _internalView.AddSubview(_doneButton);

			_internalView.AddConstraints (new[] {
				NSLayoutConstraint.Create (_cancelButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _internalView, NSLayoutAttribute.Top, 1f, 7f),
				NSLayoutConstraint.Create (_cancelButton, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _internalView, NSLayoutAttribute.Leading, 1f, 10f),
				NSLayoutConstraint.Create (_headerLabel, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, _cancelButton, NSLayoutAttribute.Baseline, 1f, 0f),
				NSLayoutConstraint.Create (_headerLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _cancelButton, NSLayoutAttribute.Trailing, 1f, 10f),
				NSLayoutConstraint.Create (_doneButton, NSLayoutAttribute.Baseline, NSLayoutRelation.Equal, _headerLabel, NSLayoutAttribute.Baseline, 1f, 0f),
				NSLayoutConstraint.Create (_doneButton, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _headerLabel, NSLayoutAttribute.Trailing, 1f, 10f),
				NSLayoutConstraint.Create (_internalView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, _doneButton, NSLayoutAttribute.Trailing, 1f, 10f),
				NSLayoutConstraint.Create (picker, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _internalView, NSLayoutAttribute.CenterX, 1f, 0f),
				NSLayoutConstraint.Create (picker, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _internalView, NSLayoutAttribute.Bottom, 1f, 0f),
				NSLayoutConstraint.Create (picker, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _cancelButton, NSLayoutAttribute.Bottom, 1f, 5f),
				NSLayoutConstraint.Create (_internalView, NSLayoutAttribute.Width, NSLayoutRelation.GreaterThanOrEqual, picker, NSLayoutAttribute.Width, 1f, 0f),
			});

            Add(_internalView);
			View.AddConstraints (new[] {
				NSLayoutConstraint.Create (_internalView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (_internalView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1f, 0f),
				NSLayoutConstraint.Create (_internalView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1f, 0f)
			});
        }

		static void AddButtonSizeConstraints (UIButton button)
		{
			button.AddConstraints (new[] {
				NSLayoutConstraint.Create (button, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1f, 71f),
				NSLayoutConstraint.Create (button, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1f, 30f)
			});
		}

        void DoneButtonTapped (object sender, EventArgs e)
        {
            DismissViewController(true, null);
            if(OnModalPickerDismissed != null)
            {
                OnModalPickerDismissed(sender, e);
            }
		}

		void CancelButtonTapped (object sender, EventArgs e)
		{
			DismissViewController(true, null);
		}
    }

    public enum ModalPickerType
    {
        Date = 0,
        Custom = 1
    }
}

